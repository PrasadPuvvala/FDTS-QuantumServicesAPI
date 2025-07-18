using AventStack.ExtentReports;
using Grpc.Net.Client;
using QuantumServicesAPI.APIHelper;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;

namespace QuantumServicesAPI.Hooks
{
    /// <summary>
    /// Contains Reqnroll hooks for test execution lifecycle events.
    /// Manages Extent Report nodes for features, environments, regions, and scenarios.
    /// Loads configuration files and attaches them to the scenario context.
    /// </summary>
    [Binding]
    public sealed class ReqnrollHooks
    {
        // For additional details on Reqnroll hooks see https://go.reqnroll.net/doc-hooks

        /// <summary>
        /// Singleton instance of the ExtentReportManager.
        /// </summary>
        private static ExtentReportManager? _reportManager;

        /// <summary>
        /// The current ExtentTest node for the running scenario.
        /// </summary>
        private static ExtentTest? _currentTest;

        /// <summary>
        /// Hierarchy for organizing ExtentTest nodes: Feature → Environment → Region → ExtentTest Node.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, Dictionary<string, ExtentTest>>> _featureHierarchy = new();

        /// <summary>
        /// Initializes the ExtentReportManager before any tests run.
        /// </summary>
        [BeforeTestRun]
        public static void Setup()
        {
            _reportManager = ExtentReportManager.GetInstance();
        }

        /// <summary>
        /// Creates a feature node in the Extent Report before each feature.
        /// </summary>
        /// <param name="featureContext">The context of the current feature.</param>
        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            if (_reportManager == null)
            {
                throw new InvalidOperationException("ExtentReportManager instance is not initialized. Ensure Setup() is called before using the report manager.");
            }

            var featureTitle = featureContext.FeatureInfo.Title;
            if (featureTitle.Contains("Success", StringComparison.OrdinalIgnoreCase) || featureTitle.Contains("Fail", StringComparison.OrdinalIgnoreCase))
            {
                // Use switch for faster branching and avoid repeated string comparisons
                switch (featureTitle)
                {
                    case string s when s.Contains("Success", StringComparison.OrdinalIgnoreCase):
                        SocketHelperClass.SuccessSocketCommands();
                        break;
                    case string s when s.Contains("Fail", StringComparison.OrdinalIgnoreCase):
                        SocketHelperClass.FailureSocketCommands();
                        break;
                    default:
                        // For GRPCService, we assume success commands are needed.
                        SocketHelperClass.SuccessSocketCommands();
                        break;
                }
                // Only launch the gRPC process if not already running
                if (GRPCAPIHelperClass.GrpcProcess == null || GRPCAPIHelperClass.GrpcProcess.HasExited)
                {
                    const string exePath = @"C:\Program Files\WindowsApps\Avalon.Dooku3.gRPCService_5.3.1.0_x86__ab7apr970t1ng\Avalon.Dooku3.gRPCService.exe";
                    GRPCAPIHelperClass.LaunchGrpcLocalPort(exePath);
                }
            }
            var styledFeatureTitle = $"<span font-size:18px; font-weight:bold;'>{featureTitle.ToUpper()} : FEATURE</span>";
            var featureTest = _reportManager.CreateFeature(styledFeatureTitle);
            featureContext.Set(featureTest, "FeatureTest");
            if (!_featureHierarchy.ContainsKey(featureTitle))
            {
                _featureHierarchy[featureTitle] = new Dictionary<string, Dictionary<string, ExtentTest>>();
            }
        }

        /// <summary>
        /// Runs before each scenario. Loads configuration files and creates environment/region/scenario nodes in the Extent Report.
        /// </summary>
        /// <param name="scenarioContext">The context of the current scenario.</param>
        /// <param name="featureContext">The context of the current feature.</param>
        [BeforeScenario]
        public void FirstBeforeScenario(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            // Example of ordering the execution of hooks
            // See https://go.reqnroll.net/doc-hooks#hook-execution-order

            // Load configuration files and add them to the scenario context
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectRootDirectory = Directory.GetParent(baseDirectory)!.Parent!.Parent!.Parent!.FullName;
            string APIEndpointsFilesDirectory = Path.Combine(projectRootDirectory, "JsonFiles");

            // Find the first JSON file in the folder
            var apiEndpointsConfig = Directory.GetFiles(APIEndpointsFilesDirectory, "APIEndpoints.json").FirstOrDefault();
            if (File.Exists(apiEndpointsConfig))
            {
                string json = File.ReadAllText(apiEndpointsConfig);
                var apiendpointssettings = Newtonsoft.Json.JsonConvert.DeserializeObject<APIEndpointsDTO>(json);
                scenarioContext.Add("apiendpoints", apiendpointssettings);
            }
            else
            {
                throw new FileNotFoundException("Configuration file not found at path: " + apiEndpointsConfig);
            }

            var mfgDataServiceAPIkeyConfig = Directory.GetFiles(APIEndpointsFilesDirectory, "MFGDataServiceAPIkeys.json").FirstOrDefault();
            if (File.Exists(mfgDataServiceAPIkeyConfig))
            {
                string mfgjson = File.ReadAllText(mfgDataServiceAPIkeyConfig);
                var mfgdataserviceapikeyssettings = Newtonsoft.Json.JsonConvert.DeserializeObject<MFGDataServiceAPIKeysDTO>(mfgjson);
                scenarioContext.Add("mfgdataserviceapikeys", mfgdataserviceapikeyssettings);
            }
            else
            {
                throw new FileNotFoundException("Configuration file not found at path: " + mfgDataServiceAPIkeyConfig);
            }

            var processControlServiceAPIkeyConfig = Directory.GetFiles(APIEndpointsFilesDirectory, "ProcessControlServiceAPIKeys.json").FirstOrDefault();
            if (File.Exists(processControlServiceAPIkeyConfig))
            {
                string processControljson = File.ReadAllText(processControlServiceAPIkeyConfig);
                var processControlserviceapikeyssettings = Newtonsoft.Json.JsonConvert.DeserializeObject<ProcessControlServiceAPIKeysDTO>(processControljson);
                scenarioContext.Add("processcontrolserviceapikeys", processControlserviceapikeyssettings);
            }
            else
            {
                throw new FileNotFoundException("Configuration file not found at path: " + processControlServiceAPIkeyConfig);
            }

            // Create environment, region, and scenario nodes in the Extent Report
            var featureTitle = featureContext.FeatureInfo.Title;
            if (featureTitle.Contains("Success", StringComparison.OrdinalIgnoreCase) || featureTitle.Contains("Fail", StringComparison.OrdinalIgnoreCase))
            {
                var url = GRPCAPIHelperClass.Url;
                var sharedChannel = GrpcChannel.ForAddress(url);

                var hearingHelper = new HearingInstrumentPage(sharedChannel);
                var productIdentificationHelper = new ProductIdentificationPage(sharedChannel);

                scenarioContext["GrpcUrl"] = url;
                scenarioContext["GrpcHearingInstrument"] = hearingHelper;
                scenarioContext["GrpcProductIdentification"] = productIdentificationHelper; 
            }
            var environment = scenarioContext.ScenarioInfo.Arguments["Environment"]?.ToString() ?? "PC Programming Prototype";
            var region = scenarioContext.ScenarioInfo.Arguments["Region"]?.ToString() ?? $"{featureTitle}";
            // Retrieve the feature node and create a scenario node under it
            var featureTest = featureContext.Get<ExtentTest>("FeatureTest");
            // Ensure environment node
            if (!_featureHierarchy[featureTitle].ContainsKey(environment))
            {
                var envLabel = $"<span style='color:skyblue; font-weight:bold;'>{environment.ToUpper()} </span>";
                //  var envLabel = $"<span style='color:skyblue; font-weight:bold;'>{environment.ToUpper()} : ENVIRONMENT</span>";
                _featureHierarchy[featureTitle][environment] = new Dictionary<string, ExtentTest>();
                _featureHierarchy[featureTitle][environment]["__envRoot"] = _reportManager!.CreateEnvironment(featureTest, envLabel);
            }
            // Retrieve or create the environment node
            var envNode = _featureHierarchy[featureTitle][environment]["__envRoot"];

            // Ensure region node under environment
            if (!_featureHierarchy[featureTitle][environment].ContainsKey(region))
            {
                var regionLabel = $"<span style='color:skyblue; font-weight:bold;'>{region.ToUpper()} </span>";
                //  var regionLabel = $"<span style='color:skyblue; font-weight:bold;'>{region.ToUpper()} : REGION</span>";
                _featureHierarchy[featureTitle][environment][region] = _reportManager!.CreateRegion(envNode, regionLabel);
            }

            var regionNode = _featureHierarchy[featureTitle][environment][region];

            // Create scenario node
            var scenarioLabel = $"<span style='color:skyblue; font-weight:bold;'>{scenarioContext.ScenarioInfo.Title.ToUpper()}</span>";
            _currentTest = _reportManager!.CreateScenario(regionNode, scenarioLabel);
            scenarioContext.Set(_currentTest, "CurrentTest");
        }

        /// <summary>
        /// Runs after each scenario. Place logic here to execute after scenario execution.
        /// </summary>
        [AfterScenario]
        public void AfterScenario()
        {
            //TODO: implement logic that has to run after executing each scenario
        }

        /// <summary>
        /// Runs after each feature has completed execution.
        /// Handles any necessary cleanup, such as terminating sockets or processes
        /// that were started for the feature.
        /// </summary>
        /// <param name="featureContext">The context of the feature that has just finished.</param>
        [AfterFeature]
        public static void AfterFeature(FeatureContext featureContext)
        {
            SocketHelperClass.HandleProcessExit();
            // Fix for CS1061: Check if GRPCAPIHelperClass has a method or property to determine if the gRPC service is running.
            // Since 'IsGrpcServiceRunning' does not exist in ExtentReportManager, we should use GRPCAPIHelperClass directly.
            if (GRPCAPIHelperClass.GrpcProcess != null && !GRPCAPIHelperClass.GrpcProcess.HasExited)
            {
                GRPCAPIHelperClass.StopGrpcLocalPort();
            }
        }

        /// <summary>
        /// Flushes the Extent Report after all tests have run.
        /// </summary>
        [AfterTestRun]
        public static void TearDown()
        {
            _reportManager!.FlushReport();
        }
    }
}
using AventStack.ExtentReports;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using Reqnroll;

namespace QuantumServicesAPI.Hooks
{
    [Binding]
    public sealed class ReqnrollHooks
    {
        // For additional details on Reqnroll hooks see https://go.reqnroll.net/doc-hooks

        private static ExtentReportManager? _reportManager;
        private static ExtentTest? _currentTest;
        // Feature → Environment → Region → ExtentTest Node
        private static readonly Dictionary<string, Dictionary<string, Dictionary<string, ExtentTest>>> _featureHierarchy = new();

        [BeforeTestRun]
        public static void Setup()
        {
            _reportManager = ExtentReportManager.GetInstance();
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            var featureTitle = featureContext.FeatureInfo.Title;
            var styledFeatureTitle = $"<span font-size:18px; font-weight:bold;'>{featureTitle.ToUpper()} : FEATURE</span>"; 
            var featureTest = _reportManager.CreateFeature(styledFeatureTitle);
            featureContext.Set(featureTest, "FeatureTest");
            if (!_featureHierarchy.ContainsKey(featureTitle))
            {
                _featureHierarchy[featureTitle] = new Dictionary<string, Dictionary<string, ExtentTest>>();
            }
        }

        [BeforeScenario]
        public void FirstBeforeScenario(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            // Example of ordering the execution of hooks
            // See https://go.reqnroll.net/doc-hooks#hook-execution-order

            //TODO: implement logic that has to run before executing each scenario

            string apiEndpointsConfig = @"../../../APIEndpoints.json";
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
            var featureTitle = featureContext.FeatureInfo.Title;
            var environment = scenarioContext.ScenarioInfo.Arguments["Environment"]?.ToString() ?? "UnknownEnv";
            var region = scenarioContext.ScenarioInfo.Arguments["Region"]?.ToString() ?? "UnknownRegion";
            // Retrieve the feature node and create a scenario node under it
            var featureTest = featureContext.Get<ExtentTest>("FeatureTest");
            // Ensure environment node
            if (!_featureHierarchy[featureTitle].ContainsKey(environment))
            {
                var envLabel = $"<span style='color:skyblue; font-weight:bold;'>{environment.ToUpper()} : ENVIRONMENT</span>"; 
                _featureHierarchy[featureTitle][environment] = new Dictionary<string, ExtentTest>();
                _featureHierarchy[featureTitle][environment]["__envRoot"] = _reportManager.CreateEnvironment(featureTest, envLabel);
            }

            var envNode = _featureHierarchy[featureTitle][environment]["__envRoot"];

            // Ensure region node under environment
            if (!_featureHierarchy[featureTitle][environment].ContainsKey(region))
            {
                var regionLabel = $"<span style='color:skyblue; font-weight:bold;'>{region.ToUpper()} : REGION</span>"; 
                _featureHierarchy[featureTitle][environment][region] = _reportManager.CreateRegion(envNode, regionLabel);
            }

            var regionNode = _featureHierarchy[featureTitle][environment][region];

            // Create scenario node
            var scenarioLabel = $"<span style='color:skyblue; font-weight:bold;'>{scenarioContext.ScenarioInfo.Title.ToUpper()}</span>";
            _currentTest = _reportManager.CreateScenario(regionNode, scenarioLabel);
            scenarioContext.Set(_currentTest, "CurrentTest");
        }

        [AfterScenario]
        public void AfterScenario()
        {
            //TODO: implement logic that has to run after executing each scenario
        }

        [AfterTestRun]
        public static void TearDown()
        {
            _reportManager.FlushReport();
        }
    }
}
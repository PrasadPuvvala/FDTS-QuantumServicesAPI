using AventStack.ExtentReports;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using Reqnroll;

namespace QuantumServicesAPI.Images.Hooks
{
    [Binding]
    public sealed class ReqnrollHooks
    {
        // For additional details on Reqnroll hooks see https://go.reqnroll.net/doc-hooks

        private static ExtentReportManager? _reportManager;
        private static ExtentTest? _currentTest;

        [BeforeTestRun]
        public static void Setup()
        {
            _reportManager = ExtentReportManager.GetInstance();
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            var featureTitle = featureContext.FeatureInfo.Title;
            var featureTest = _reportManager.CreateTest(featureTitle);
            featureContext.Set(featureTest, "FeatureTest");
        }

        //[BeforeScenario("@tag1")]
        //public void BeforeScenarioWithTag()
        //{
        //    // Example of filtering hooks using tags. (in this case, this 'before scenario' hook will execute if the feature/scenario contains the tag '@tag1')
        //    // See https://go.reqnroll.net/doc-hooks#tag-scoping

        //    //TODO: implement logic that has to run before executing each scenario
        //}

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

            // Retrieve the feature node and create a scenario node under it
            var featureTest = featureContext.Get<ExtentTest>("FeatureTest");
            _currentTest = _reportManager.CreateTestStep(featureTest, scenarioContext.ScenarioInfo.Title);
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
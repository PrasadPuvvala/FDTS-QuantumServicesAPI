using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.MarkupUtils;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.ExtentReport
{
    public class ExtentReportManager
    {
        private readonly ExtentReports _extent;
        private readonly ExtentSparkReporter _htmlReporter;
        private static ExtentReportManager? _instance;

        // Constructor to initialize the report
        private ExtentReportManager()
        {
            // report path: one HTML per env+region
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var reportDir = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
            Directory.CreateDirectory(reportDir);
            var reportFile = Path.Combine(reportDir, $"APIAutomationReport_{timestamp}.html");
            _htmlReporter = new ExtentSparkReporter(reportFile) { Config = { Theme = Theme.Dark, ReportName = "API Regression Test", DocumentTitle = "API Automation Report" } };
            _extent = new ExtentReports();
            _extent.AttachReporter(_htmlReporter);
            _extent.AddSystemInfo("Environment", "QA");
            _extent.AddSystemInfo("Executed By", Environment.UserName);
        }

        // Singleton instance to ensure only one report instance
        public static ExtentReportManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ExtentReportManager();
            }
            return _instance;
        }

        // Create a new test case in the report
        public ExtentTest CreateFeature(string featureTitle)
        {
            return _extent.CreateTest(featureTitle);
        }
        public ExtentTest CreateTestStep(ExtentTest test, string stepName)
        {
            return test.CreateNode(stepName);
        }
        public ExtentTest CreateEnvironment(ExtentTest test, string environment)
        {
            return test.CreateNode(environment);
        }
        public ExtentTest CreateRegion(ExtentTest test, string region)
        {
            return test.CreateNode(region);
        }
        public ExtentTest CreateScenario(ExtentTest test, string scenario)
        {
            return test.CreateNode(scenario);
        }

        // Log messages to the report
        public void LogToReport(ExtentTest test, Status status, string message)
        {
            var logMessage = $"<span style='color:lightgreen;'>{message}</span>";
            test?.Log(status, logMessage);
        }
        public void LogJson(ExtentTest test, Status status, string title, string json)
        {
            try
            {
                var prettyJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented);
                var formattedJson = $"<pre><span style='color:lightgreen;'>{prettyJson}</span></pre>";
                test?.Log(status, $"{title}:<br>{formattedJson}");
            }
            catch (Exception ex)
            {
                var fallback = $"<span style='color:red;'>Failed to format JSON: {ex.Message}</span><br><pre>{json}</pre>";
                test?.Log(Status.Warning, fallback);
            }
        }

        public void LogStatusCode(ExtentTest test, Status status, string message)
        {
            var color = status == Status.Pass ? "lightgreen" : "red";
            var styled = $"<span style='color:{color};'>{message}</span>";
            test?.Log(status, styled);
        }

        public void LogError(ExtentTest test, Status status, string message)
        {
            var errorMessage = $"<span style='color:red;'>{message}</span>";
            test?.Log(Status.Fail, errorMessage);
        }

        // Flush the report to save changes
        public void FlushReport()
        {
            _extent.Flush();
        }
    }
}

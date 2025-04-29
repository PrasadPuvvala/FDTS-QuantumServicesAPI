using AventStack.ExtentReports;
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
            string reportPath = Path.Combine(Directory.GetCurrentDirectory(), "APIAutomationReport.html");
            _htmlReporter = new ExtentSparkReporter(reportPath) { Config = { Theme = Theme.Dark, ReportName = "API Regression Test", DocumentTitle = "API Automation Report" } };
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
        public ExtentTest CreateTest(string testName)
        {
            return _extent.CreateTest(testName);
        }

        public ExtentTest CreateTestStep(ExtentTest test, string stepName)
        {
            return test.CreateNode(stepName);
        }

        // Log messages to the report
        public void LogToReport(ExtentTest test, Status status, string message)
        {
            test?.Log(status, message);
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
            var errorHtml = $"<span style='color:red;'>Error Message: {message}</span>";
            test?.Log(Status.Fail, errorHtml);
        }

        // Flush the report to save changes
        public void FlushReport()
        {
            _extent.Flush();
        }
    }
}

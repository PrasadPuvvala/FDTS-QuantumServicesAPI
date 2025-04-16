using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // Flush the report to save changes
        public void FlushReport()
        {
            _extent.Flush();
        }
    }
}

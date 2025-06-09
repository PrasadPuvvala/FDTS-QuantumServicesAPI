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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.ExtentReport
{
    /// <summary>
    /// Manages the Extent Report generation for API automation tests.
    /// This class implements a singleton pattern to ensure only one instance of the report is created.
    /// It provides methods to create tests, log messages, and flush the report.
    /// </summary>
    public class ExtentReportManager
    {
        private readonly ExtentReports _extent;
        private readonly ExtentSparkReporter _htmlReporter;
        private static ExtentReportManager? _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtentReportManager"/> class.
        /// This constructor is private to enforce the singleton pattern.
        /// It sets up the report file, configures the HTML reporter, and adds system information.
        /// </summary>
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
            _extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
            _extent.AddSystemInfo("User Name", Environment.UserName);
            _extent.AddSystemInfo("Machine Name", Environment.MachineName);
            _extent.AddSystemInfo("Assembly Version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="ExtentReportManager"/> class.
        /// </summary>
        /// <returns>The singleton instance of the <see cref="ExtentReportManager"/> class.</returns>
        public static ExtentReportManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ExtentReportManager();
            }
            return _instance;
        }

        /// <summary>
        /// Creates a new feature in the Extent Report.
        /// </summary>
        /// <param name="featureTitle">The title of the feature.</param>
        /// <returns>The created ExtentTest object representing the feature.</returns>
        public ExtentTest CreateFeature(string featureTitle)
        {
            return _extent.CreateTest(featureTitle);
        }

        /// <summary>
        /// Creates a new test step (node) under the given test in the Extent Report.
        /// </summary>
        /// <param name="test">The parent test to which the step will be added.</param>
        /// <param name="stepName">The name of the test step.</param>
        /// <returns>The created ExtentTest object representing the test step.</returns>
        public ExtentTest CreateTestStep(ExtentTest test, string stepName)
        {
            return test.CreateNode(stepName);
        }

        /// <summary>
        /// Creates a new environment node under the given test in the Extent Report.
        /// </summary>
        /// <param name="test">The parent test to which the environment will be added.</param>
        /// <param name="environment">The name of the environment.</param>
        /// <returns>The created ExtentTest object representing the environment.</returns>
        public ExtentTest CreateEnvironment(ExtentTest test, string environment)
        {
            return test.CreateNode(environment);
        }

        /// <summary>
        /// Creates a new region node under the given test in the Extent Report.
        /// </summary>
        /// <param name="test">The parent test to which the region will be added.</param>
        /// <param name="region">The name of the region.</param>
        /// <returns>The created ExtentTest object representing the region.</returns>
        public ExtentTest CreateRegion(ExtentTest test, string region)
        {
            return test.CreateNode(region);
        }

        /// <summary>
        /// Creates a new scenario node under the given test in the Extent Report.
        /// </summary>
        /// <param name="test">The parent test to which the scenario will be added.</param>
        /// <param name="scenario">The name of the scenario.</param>
        /// <returns>The created ExtentTest object representing the scenario.</returns>
        public ExtentTest CreateScenario(ExtentTest test, string scenario)
        {
            return test.CreateNode(scenario);
        }

        /// <summary>
        /// Logs a message to the Extent Report with the specified status.
        /// </summary>
        /// <param name="test">The test to which the message will be logged.</param>
        /// <param name="status">The status of the message (e.g., Pass, Fail, Info).</param>
        /// <param name="message">The message to log.</param>
        public void LogToReport(ExtentTest test, Status status, string message)
        {
            var logMessage = $"<span style='color:lightgreen;'>{message}</span>";
            test?.Log(status, logMessage);
        }

        /// <summary>
        /// Logs a JSON message to the Extent Report with the specified status and title.
        /// </summary>
        /// <param name="test">The test to which the JSON will be logged.</param>
        /// <param name="status">The status of the message.</param>
        /// <param name="title">The title of the JSON message.</param>
        /// <param name="json">The JSON string to log.</param>
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

        /// <summary>
        /// Logs a status code to the Extent Report with the specified status.
        /// </summary>
        /// <param name="test">The test to which the status code will be logged.</param>
        /// <param name="status">The status of the status code (e.g., Pass, Fail).</param>
        /// <param name="message">The status code message to log.</param>
        public void LogStatusCode(ExtentTest test, Status status, string message)
        {
            var color = status == Status.Pass ? "lightgreen" : "red";
            var styled = $"<span style='color:{color};'>{message}</span>";
            test?.Log(status, styled);
        }

        /// <summary>
        /// Logs an error message to the Extent Report.
        /// </summary>
        /// <param name="test">The test to which the error will be logged.</param>
        /// <param name="status">The status of the error (typically Fail).</param>
        /// <param name="message">The error message to log.</param>
        public void LogError(ExtentTest test, Status status, string message)
        {
            var errorMessage = $"<span style='color:red;'>{message}</span>";
            test?.Log(Status.Fail, errorMessage);
        }

        /// <summary>
        /// Flushes the Extent Report to persist any changes to the report file.
        /// </summary>
        /// <remarks>This method must be called to ensure that all logs are written to the report.</remarks>
        public void FlushReport()
        {
            _extent.Flush();
        }
    }
}

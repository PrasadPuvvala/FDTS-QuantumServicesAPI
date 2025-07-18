﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using AventStack.ExtentReports.MarkupUtils;
using Newtonsoft.Json;
using NUnit.Framework;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using RestSharp;

namespace QuantumServicesAPI.StepDefinitions
{
    [Binding]
    public class OCRServiceStepDefinitions
    {
        private readonly OCRServicePage _OCRServicePage;
        private RestResponse? _response;
        private readonly ScenarioContext _scenarioContext;
        public OCRServiceStepDefinitions(ScenarioContext scenarioContext)
        {
            _OCRServicePage = new OCRServicePage();
            _scenarioContext = scenarioContext;
        }


        [When("OCR service is deployed to the WestEurope cloud region")]
        [When("OCR service is deployed to the SouthEast Asia cloud region")]
        [When("OCR service is deployed to the East US cloud region")]
        [When("Send the request with a correct APIkey as input")]
        [When("Send the request with a correct image as input")]
        [When("Send the request with a blurry image as input")]
        [When("Send the request with an invalid image \\(no characters)")]
        public async Task WhenSendTheRequestWithACorrectImageAsInput(DataTable table)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in table.Rows)
            {
                string image = row["ImageFormat"];
                string env = row["Env"];
                string region = row["Region"];
                string apikey = row["APIkey"];
                _response = await _OCRServicePage.PostImageAnalyzeAsync(step, apiEndpoint, image, env, region, apikey);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
            }
        }

        [When("OCR service should be operational in the WestEurope cloud region")]
        [When("OCR service should be operational in the SouthEast Asia cloud region")]
        [When("OCR service should be operational in the East US cloud region")]
        [When("Verify the response when correct APIkey is inputted")]
        [When("Verify the response when correct image is inputted")]
        public void WhenVerifyTheResponseWhenCorrectImageIsInputted()
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.OK, _response?.StatusCode, "Expected 200 OK status code");
                var responseContent = _response.Content;
                ExtentReportManager.GetInstance().LogStatusCode(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
                ExtentReportManager.GetInstance().LogJson(step, Status.Info, "Response Body", _response.Content);
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"<span style='color:red;'>Error Message: {ex.Message}</span>");
            }
        }

        [Then("The response must contain a list of all the identified character strings.")]
        public void ThenTheResponseMustContainAListOfAllTheIdentifiedCharacterStrings_()
        {

            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            try
            {
                var result = JsonConvert.DeserializeObject<List<dynamic>>(_response.Content);
                Assert.NotNull(result, "Deserialized response should not be null");
                ExtentReportManager.GetInstance().LogJson(step, Status.Pass, "Image Analysis Data", _response.Content);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to deserialize JSON response: " + ex.Message);
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"Image Analysis Data is Not found : {ex.Message}");
            }
        }

        [When("Send a request with input as an image in PNG format with size more than {int}kb")]
        public async Task WhenSendARequestWithInputAsAnImageInPNGFormatWithSizeMoreThanKbAsync(int p0, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in dataTable.Rows)
            {
                string image = row["ImageFormat"];
                string env = row["Env"];
                string region = row["Region"];
                string apikey = row["APIkey"];
                _response = await _OCRServicePage.PostImageAnalyzeAsync(step, apiEndpoint, image, env, region, apikey);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
            }
        }

        [Then("Verify the response when image size is more than {int}kb")]
        public void ThenVerifyTheResponseWhenImageSizeIsMoreThanKb(int p0)
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.BadRequest, _response?.StatusCode, "Expected 400 BadRequest status code");
                ExtentReportManager.GetInstance().LogStatusCode(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
                ExtentReportManager.GetInstance().LogJson(step, Status.Pass, "Image Analysis Data", _response.Content);
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"Error Message : {ex.Message}");
            }
        }

        [When("Send a request with input as an image in PNG format with size less than {int}kb")]
        public async Task WhenSendARequestWithInputAsAnImageInPNGFormatWithSizeLessThanKbAsync(int p0, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in dataTable.Rows)
            {
                string image = row["ImageFormat"];
                string env = row["Env"];
                string region = row["Region"];
                string apikey = row["APIkey"];
                _response = await _OCRServicePage.PostImageAnalyzeAsync(step, apiEndpoint, image, env, region, apikey);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
            }
        }

        [Then("Verify the response when image size is less than {int}kb")]
        public void ThenVerifyTheResponseWhenImageSizeIsLessThanKb(int p0)
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.OK, _response?.StatusCode, "Expected 200 OK status code");
                ExtentReportManager.GetInstance().LogStatusCode(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"Error Message : {ex.Message}");
            }
        }

        [Then("Verify the response when the inputted image is blurry")]
        [Then("Verify the response when the inputted image is an invalid image \\(no characters)")]
        public void ThenVerifyTheResponseWhenTheInputtedImageIsBlurry()
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.OK, _response?.StatusCode, "Expected 200 OK status code");
                ExtentReportManager.GetInstance().LogStatusCode(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"Error Message : {ex.Message}");
            }
        }

        [Then("The response must contain an empty list.")]
        public void ThenTheResponseMustContainAnEmptyList_()
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            var result = JsonConvert.DeserializeObject<List<dynamic>>(_response.Content);

            if (result?.Count == 0)
            {
                ExtentReportManager.GetInstance().LogJson(step, Status.Pass, $"Image Analysis Data :", _response.Content);
            }
            else
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"Image Analysis Data is Not null : {_response.Content}");
            }
        }

        [When("Send a request to the South-East Asia region without an API key")]
        [When("Send a request to the WestEurope region without an API key")]
        [When("Send a request to the SouthEastAsia region without an API key")]
        [When("Send a request to the EastUS region without an API key")]
        [When("Send a request to the WestEurope region using an invalid API key")]
        [When("Send a request to the EastUS region using an invalid API key")]
        [When("Send a request to the South-East Asia region using an invalid API key")]
        public async Task WhenSendARequestToTheSouth_EastAsiaRegionUsingAnInvalidAPIKeyAsync(DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in dataTable.Rows)
            {
                string image = row["ImageFormat"];
                string env = row["Env"];
                string region = row["Region"];
                string apikey = row["InvalidAPIkey"];
                _response = await _OCRServicePage.PostImageAnalyzeAsync(step, apiEndpoint, image, env, region, apikey);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
            }
        }

        [Then("The request is rejected and returns a {int} Unauthorized error")]
        public void ThenTheRequestIsRejectedAndReturnsAUnauthorizedError(int p0)
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                if (_response == null)
                {
                    throw new Exception("Response should not be null");
                }

                var result = JsonConvert.DeserializeObject(_response.Content);

                if (_response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ExtentReportManager.GetInstance().LogStatusCode(step, Status.Pass, $"Status code is 401 Unauthorized as expected.");
                    ExtentReportManager.GetInstance().LogJson(step, Status.Pass, "Response Body :", $"{_response.Content}");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"Expected status code 401 Unauthorized but got {_response.StatusCode}.");
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"{_response.Content}");
                }
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"Error Message: {ex.Message}");
            }
        }
        [When("Send a request with input as an image in a supported format \\(JPEG, PNG, BMP, PDF, TIFF) and verify the response and list of all the identified character strings")]
        public async Task WhenSendARequestWithInputAsAnImageInASupportedFormatJPEGPNGBMPPDFTIFFAndVerifyTheResponseAndListOfAllTheIdentifiedCharacterStringsAsync(DataTable table)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in table.Rows)
            {
                string image = row["ImageFormat"];
                string env = row["Env"];
                string region = row["Region"];
                string apikey = row["APIkey"];
                _response = await _OCRServicePage.PostImageAnalyzeAsync(step, apiEndpoint, image, env, region, apikey);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"POST request failed: Response is null for {image} format");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"POST request Success for {image} format");
                }
                try
                {
                    Assert.NotNull(_response, "Response should not be null");
                    Assert.AreEqual(HttpStatusCode.OK, _response?.StatusCode, "Expected 200 OK status code");
                    ExtentReportManager.GetInstance().LogStatusCode(step, Status.Pass, $"Statuscode : {_response?.StatusCode} for {image} format");
                }
                catch (Exception ex)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"Error Message : {ex.Message}");
                }
                try
                {
                    var result = JsonConvert.DeserializeObject<List<dynamic>>(_response.Content);
                    Assert.NotNull(result, "Deserialized response should not be null");
                    ExtentReportManager.GetInstance().LogJson(step, Status.Pass, $"Identified characters from {image} format :", _response.Content);
                }
                catch (Exception ex)
                {
                    Assert.Fail("Failed to deserialize JSON response: " + ex.Message);
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"Image Analysis Data is Not found : {ex.Message}");
                }
            }
        }

        [When("Send a request with input as an image in an unsupported format \\(Ex: GIF, WEBP, SVG, etc.) and verify the {int} error returned")]
        public async Task WhenSendARequestWithInputAsAnImageInAnUnsupportedFormatExGIFWEBPSVGEtc_AndVerifyTheErrorReturnedAsync(int p0, DataTable table)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in table.Rows)
            {
                string image = row["ImageFormat"];
                string env = row["Env"];
                string region = row["Region"];
                string apikey = row["APIkey"];
                _response = await _OCRServicePage.PostImageAnalyzeAsync(step, apiEndpoint, image, env, region, apikey);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"POST request failed: Response is null for {image} format");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"POST request Success for {image} format");
                }
                try
                {
                    Assert.NotNull(_response, "Response should not be null");
                    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, _response?.StatusCode, "Expected Method Not Allowed status code");
                    ExtentReportManager.GetInstance().LogStatusCode(step, Status.Pass, $"Statuscode : {_response?.StatusCode} for {image} format");
                }
                catch (Exception ex)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"Error Message : {ex.Message}");
                }
            }
        }

        [When("Send a request to the OCR service under normal system load and verify the median response time")]
        public async Task WhenSendARequestToTheOCRServiceUnderNormalSystemLoadAndVerifyTheMedianResponseTimeAsync(DataTable table)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in table.Rows)
            {
                string image = row["ImageFormat"];
                string env = row["Env"];
                string region = row["Region"];
                string apikey = row["APIkey"];
                _response = await _OCRServicePage.PostImageMedianResponseTime(step, apiEndpoint, image, env, region, apikey);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
            }
        }
    }
}

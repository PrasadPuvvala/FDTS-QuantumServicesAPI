@DataSource:mfg-environments.json @DataSet:MfgDataService
Feature: MFG Data Service

A short summary of the feature

@mfgdataservice
Scenario: 01Test Case ID 1784675: Verify that the MFG Data Service processes valid compressed JSON test data within the allowed size

	When Send a request to MFG data service with a compressed JSON below 256 kb using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
	Then Verify the response with the compressed data

@mfgdataservice
Scenario: 02Test Case ID 1785191: Verify that the MFG Data Service rejects uncompressed JSON test data

	When Send a request to MFG data service with an uncompressed JSON using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
		| MFGDataFile     |
		| json_valid.json |
	Then Verify the response with the uncompressed data

@mfgdataservice
Scenario: 03Test Case ID 1784417: Verify that the MFG Data Service rejects requests without/invalid API key

	When Send a request to the MFG Data Service using an invalid API key "<ApiKey>" with baseUrl "<BaseUrl>"
	Then Verify the API response
	When Send a request to the MFG Data Service without API key "<ApiKey>" with baseUrl "<BaseUrl>"
	Then Verify the API response

@mfgdataservice
Scenario: 04Test Case ID 1785192: Verify that the API rejects unsupported formats and size Constraints

	When Send a request to MFG data service with an unsupported format using baseUrl "<BaseUrl>" and apiKey "<ApiKey>" and verify the response with the unsupported format
		| MFGDataFile  |
		| validxml.zip |
		| validtxt.zip |
	And Send a request to MFG data service with a compressed JSON below 256 kb using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
	Then Verify the response with the compressed JSON below 256 kb
	When Send a request to MFG data service with a compressed JSON more 256 kb using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
		| MFGDataFile         |
		| large_json_file.zip |
	Then Verify the response with the compressed JSON more 256 kb

@mfgdataservice
Scenario: 05Test Case ID 1784415: Verify that the MFG Data Service authenticates requests with a valid API key per cloud region

	When Send a request to MFG data service using baseUrl "<BaseUrl>" and a valid API Key "<ApiKey>"
	Then Verify the response

@mfgdataservice
Scenario: 06Test Case ID 1784139: Verify that the MFG Data Service is accessible in all specified environments

	When Send a request to the MFG Data Service "<Environment>" Environment and "<Region>" Region using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
	Then Verify the response

@mfgdataservice
Scenario: 07Test Case ID 1789180: Verify MFG Data Service Deployment in Each Cloud Region

	When Send a request to the MFG Data Service "<Environment>" Environment and "<Region>" cloud Region using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
	Then Verify the response

@mfgdataservice
Scenario: 08Test Case ID 1784416: Verify that the MFG Data Service API rejects requests using an API key from a different cloud region

	When Send a request to the MFG Data Service "<Environment>" Environment and "<Region>" cloud Region using baseUrl "<BaseUrl>" and apiKey "<ApiKey>" from a different cloud region and verify the response

@mfgdataservice
Scenario: 09Test Case ID 1784230: Verify the Response Time for MFG Data Service Under Normal Conditions

    When Send a request to the MFG data service under normal system load using baseUrl "<BaseUrl>" and apiKey "<ApiKey>" and verify the median response time
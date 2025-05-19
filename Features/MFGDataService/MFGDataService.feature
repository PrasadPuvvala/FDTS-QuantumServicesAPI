@DataSource:mfg-environments.json @DataSet:MfgDataService
Feature: MFG Data Service

A short summary of the feature

@mfgdataservice
Scenario: 01Test Case ID 1784675: Verify that the MFG Data Service processes valid compressed JSON test data within the allowed size

	When Send a request to MFG data service with a compressed JSON below 256 kb using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
		| MFGDataFile |
		| file_16.zip |
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
		| MFGDataFile |
		| file_16.zip |
	Then Verify the API response
	When Send a request to the MFG Data Service without API key "<ApiKey>" with baseUrl "<BaseUrl>"
		| MFGDataFile |
		| file_16.zip |
	Then Verify the API response

@DataSource:mfg-environments.json @DataSet:MfgDataService
Feature: MFG Data Service

A short summary of the feature

@mfgdataservice
Scenario: 01Test Case ID 1784675: Verify that the MFG Data Service processes valid compressed JSON test data within the allowed size

	When Send a request to MFG data service with a compressed JSON below 256 kb
		| MFGDataFile |
		| file.zip    |
	Then Verify the response with the compressed data

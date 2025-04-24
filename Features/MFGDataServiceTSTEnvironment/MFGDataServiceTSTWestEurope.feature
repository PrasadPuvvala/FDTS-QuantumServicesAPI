Feature: MFG Data Service - TST Environment (West Europe Region)

A short summary of the feature

@1784675 @TST @WestEurope
Scenario: 01Test Case ID 1784675: Verify that the MFG Data Service processes valid compressed JSON test data within the allowed size

	When Send a request to MFG data service with a compressed JSON below 256 kb
		| MFGDataFile | Env | Region | APIkey                           |
		| file.zip    | tst | europe | 4038ebf7e3ea45c29795b494d02fb117 |
	Then Verify the response with the compressed data

@1785191 @TST @WestEurope
Scenario: 02Test Case ID 1785191: Verify that the MFG Data Service rejects uncompressed JSON test data

	When Send a request to MFG data service with an uncompressed JSON
		| MFGDataFile     | Env | Region | APIkey                           |
		| json_valid.json | tst | europe | 4038ebf7e3ea45c29795b494d02fb117 |
	Then Verify the response with the uncompressed data

@1784417 @TST @WestEurope
Scenario: 03Test Case ID 1784417: Verify that the MFG Data Service rejects requests without/invalid API key

	When Send a request to the MFG Data Service invalid API key
		| MFGDataFile | Env | Region | APIkey     |
		| file.zip    | tst | europe | 1234567890 |
	Then Verify the API response
	When Send a request to the MFG Data Service without API key
		| MFGDataFile | Env | Region | APIkey |
		| file.zip    | tst | europe |        |
	Then Verify the API response

@1784416 @TST @WestEurope
Scenario: 04Test Case ID 1784416: Verify that the MFG Data Service API rejects requests using an API key from a different cloud region

	When Send a request to the MFG Data Service using an API key from a different cloud region (Ex: use an EastUS API key for WestEurope)
		| MFGDataFile | Env | Region | APIkey                           |
		| file.zip    | tst | europe | d71a0c7fc0e74b058bd66488e2ac9210 |
	Then Verify the API response


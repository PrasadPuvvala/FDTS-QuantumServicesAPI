Feature: ProductionTestData Success

A short summary of the feature

@ProductionTestData
Scenario: 01Test Case ID 1810727: [Avalon Service] Verify ProductionTestData API Successfully Reads Test Metadata From Hearing Instrument

	When Send a request to the ProductionTestData API to read test date, site, station, TPI release code, and verification flags from the device
		| SerialNumber |
		| 2400801504   |
	Then API returns all stored production test metadata correctly from the hearing instrument

@ProductionTestData
Scenario: 02Test Case ID 1810726: [Avalon Service] Verify ProductionTestData API Successfully Writes Test Metadata to Hearing Instrument

	When Send a request to the ProductionTestData API to write test date, site, station, TPI release code, and verification flags to the device
		| TestSite | TestStation | TPIReleaseCode | Year | Month | Day | Hour | Minute | Second | ModelVerificationId |
		| 98       | GN-PF5FJ67N | 1234-5678-9ABC | 2025 | 08    | 06  | 14   | 38     | 45     | 1                   |
	Then API writes all provided test metadata values successfully to the hearing instrument
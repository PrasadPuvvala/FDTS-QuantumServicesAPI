@DataSource:processcontrol-environments.json @DataSet:ProcessControlService
Feature: Process Control Service

A short summary of the feature

@processcontrolservice
Scenario: 01Test Case ID 1784405: Verify that the Process Control Service API authenticates requests with a valid API key per cloud region

	When Send a request to the Process Control Service using a valid APIkey "<ApiKey>" and baseUrl "<BaseUrl>" for its respective cloud region
		| MetaData       |
		| EventData.json |
	Then Verify the API response for each cloud region

@processcontrolservice
Scenario: 02Test Case ID 1782446: Verify that the Process Control Service API returns an error when required metadata fields are missing

	When Send a request to the API with missing required metadata fileds (e.g., without EventDateTime, EventName, Status, SiteId, or WorkstationId) using a valid APIkey "<ApiKey>" and baseUrl "<BaseUrl>" for its respective cloud region
		| MetaData             |
		| MissingMetaData.json |
	Then Verify the API response is missing metadata for each cloud region

@processcontrolservice
Scenario: 03Test Case ID 1783278: Verify that the Process Control Service API returns an error when invalid metadata is provided

	When Send a request with a Invalid metadata fields such as Incorrect TimestampUtc/Invalid Status/Non-unique SiteId or WorkstationId using a valid APIkey "<ApiKey>" and baseUrl "<BaseUrl>" for its respective cloud region
		| MetaData             |
		| InvalidMetaData.json |
	Then Verify the API response is invalid metadata for each cloud region

@processcontrolservice
Scenario: 04Test Case ID 1783279: Verify that the Process Control Service API correctly processes optional metadata fields

	When Send a request to the API with only the required metadata fields, excluding all optional fields (Description, OrderNumber, ProductName, ProcessName, ProcessStepName, Tags, Data) using a valid APIkey "<ApiKey>" and baseUrl "<BaseUrl>" for its respective cloud region
		| MetaData             |
		| ExcludeMetaData.json |
	Then Verify the API response excludes metadata fields for each cloud region

@processcontrolservice
Scenario: 05Test Case ID 1782454: Verify that the Process Control Service is accessible in all specified environments

	When Send a request to the Process Control Service "<Environment>" Environment and "<Region>" Region using a valid APIkey "<ApiKey>" and baseUrl "<BaseUrl>"
		| MetaData       |
		| EventData.json |
	Then Verify the API response for specified environment

@processcontrolservice
Scenario: 06Test Case ID 1789179: Verify Process Control Service Deployment in Each Cloud Region

	When Send a request to the Process Control Service "<Environment>" Environment and "<Region>" Region using a valid APIkey "<ApiKey>" and baseUrl "<BaseUrl>"
		| MetaData       |
		| EventData.json |
	Then Verify the API response for each cloud region

@processcontrolservice
Scenario: 07Test Case ID 1784405: Verify that the Process Control Service API authenticates requests with a valid API key per cloud region

	When Send a request to the Process Control Service using a valid APIkey "<ApiKey>" and baseUrl "<BaseUrl>" for its respective cloud region (Ex: EastUS, WestEurope, SouthEastAsia)
		| MetaData       |
		| EventData.json |
	Then Verify the API response for each cloud region

@processcontrolservice
Scenario: 08Test Case ID 1784407: Verify that the Process Control Service API rejects requests using an API key from a different cloud region

	When Send a request to the Process Control Service "<Environment>" Environment and "<Region>" cloud Region using baseUrl "<BaseUrl>" and apiKey "<ApiKey>" from a different cloud region and verify the response
		| MetaData       |
		| EventData.json |

@processcontrolservice
Scenario: 09Test Case ID 1784409: Verify that the Process Control Service API rejects requests without/invalid API key

	When Send a request to the Process Control Service using an invalid API key "<ApiKey>" with baseUrl "<BaseUrl>"
		| MetaData       |
		| EventData.json |
	Then Verify the API response using invalid API key
	When Send a request to the Process Control Service without API key "<ApiKey>" with baseUrl "<BaseUrl>"
		| MetaData       |
		| EventData.json |
	Then Verify the API response using without API key

@processcontrolservice
Scenario: 10Test Case ID 1782464: Verify the Response Time for Process Control Service Under Normal Conditions

	When Send a request to the process control service under normal system load using baseUrl "<BaseUrl>" and apiKey "<ApiKey>" and verify the median response time
		| MetaData       |
		| EventData.json |
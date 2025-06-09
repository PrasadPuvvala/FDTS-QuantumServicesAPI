# Documentation for ProcessControlService.feature
#
# This feature file defines BDD scenarios for validating the Process Control Service API.
# The API is responsible for managing and tracking process events across multiple cloud regions and environments.
# The scenarios cover authentication, metadata validation, error handling, deployment accessibility, and performance.
#
# Scenario Overview:
# 01: Validates authentication with a valid API key per cloud region.
# 02: Checks error response when required metadata fields are missing.
# 03: Checks error response for invalid metadata.
# 04: Validates correct processing of optional metadata fields.
# 05: Ensures service accessibility in all specified environments.
# 06: Verifies deployment in each cloud region.
# 07: Additional authentication validation for specific regions.
# 08: Ensures API rejects requests with an API key from a different region.
# 09: Ensures API rejects requests with invalid or missing API key.
# 10: Measures response time under normal conditions.
#
# Data sources and datasets are referenced at the top of the file.
# Each scenario uses parameterized values for API keys, base URLs, environments, and regions.
# Metadata for requests is provided via JSON files referenced in the scenario tables.

@DataSource:processcontrol-environments.json @DataSet:ProcessControlService
Feature: Process Control Service

A short summary of the feature:
The Process Control Service API provides endpoints for managing and tracking process events across multiple cloud regions and environments. This feature validates authentication, metadata handling, error responses, and performance for the API, ensuring correct behavior under various scenarios including valid and invalid API keys, required and optional metadata, and deployment accessibility.

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
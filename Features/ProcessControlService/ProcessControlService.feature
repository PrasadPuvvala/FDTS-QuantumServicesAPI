@DataSource:processcontrol-environments.json @DataSet:ProcessControlService
Feature: Process Control Service

A short summary of the feature

@processcontrolservice
Scenario: 01Test Case ID 1784405: Verify that the Process Control Service API authenticates requests with a valid API key per cloud region

	When Send a request to the Process Control Service using a valid APIkey "<ApiKey>" and baseUrl "<BaseUrl>" for its respective cloud region
	Then Verify the API response for each cloud region

Feature: Process Control Service - DEV Environment (East US Region)

A short summary of the feature

@1784405 @DEV @EastUS
Scenario: 01Test Case ID 1784405: Verify that the Process Control Service API authenticates requests with a valid API key per cloud region

	When Send a request to the Process Control Service using a valid API key for its respective cloud region
		| APIkey                           | Env | Region |
		| dab1453076f24424881b96d408fcd504 | tst | us     |

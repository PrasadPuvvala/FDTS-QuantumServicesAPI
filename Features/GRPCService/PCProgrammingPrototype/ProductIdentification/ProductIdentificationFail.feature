Feature: ProductIdentification Fail

A short summary of the feature

@ProductIdentification
Scenario: 01Test Case ID 1810698: [HI Automation] VerifyProductIdentity API Skips Brand Validation for Non-Zero Private Label Code
	Given Send a request with valid BleId, Brand, and private label code as Non-Zero (Ex: '1')
	| SerialNumber |BleId      | Brand   | PrivateLabelCode |FittingSide |
	| 2400801519   |1093078272 | ReSound | 1                |Right       |
	Then API skips Brand verification

#@ProductIdentification
#Scenario: 02Test Case 1810699: [Avalon Service] Verify ProductIdentity API Skips Brand Validation When IsGenericFaceplate Is True
#
#	When Send a request with valid BleId, Brand, private label code as '0', and isGenericFaceplate as 'true'
#		| SerialNumber | BleId      | Brand   | PrivateLabelCode | FittingSide | IsGenericFaceplate |
#		| 2400801519   | 1093078272 | ReSound | 0                | Right       |       true         |
#	Then API skips the Brand verification
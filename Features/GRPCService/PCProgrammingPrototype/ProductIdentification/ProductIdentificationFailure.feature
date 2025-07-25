Feature: ProductIdentification Failure

A short summary of the feature

@ProductIdentification
Scenario: 01Test Case ID 1810698: [HI Automation] VerifyProductIdentity API Skips Brand Validation for Non-Zero Private Label Code

	When Send a request with valid BleId, Brand, and private label code as Non-Zero
	 | SerialNumber |bleId  | brand | privateLabelCode |
	 | 2400801519   | 1093078272 |ReSound  | 1 |
	Then API skips Brand verification

@ProductIdentification
Scenario: 02Test Case ID 1810699: [HI Automation] VerifyProductIdentity API Fails for Invalid BleId
	When Send a request with valid BleId, Brand, private label code as zero, and isGenericFaceplate as true.
		| bleId      | brand   | privateLabelCode |isGenericFaceplate  |
		| 1093078272 | ReSound | 0                | true               |
	Then API returns an error indicating that the BleId is invalid


Feature: OCR Service in TSTEnv(WestEurope)

A short summary of the feature

@1769058 @TST @WestEurope
Scenario: 01Test Case ID 1769058: Verify that the OCR service returns a list of all identified character strings from the image provided
	
	When Send the request with a correct image as input
		| ImageFormat    | Env | Region | APIkey                           |
		| 2400811734.png | tst | europe | 781b802322d248079ba7cce2abc2531a |
	And Verify the response when correct image is inputted
	Then The response must contain a list of all the identified character strings.

@1769060 @TST @WestEurope
Scenario: 02Test Case ID 1769060: Verify that the OCR service does not accept image with size more than 256kb

	When Send a request with input as an image in PNG format with size more than 256kb
		| ImageFormat    | Env | Region | APIkey                           |
		| 2400811734.png | tst | europe | 781b802322d248079ba7cce2abc2531a |
	Then Verify the response when image size is more than 256kb
	When Send a request with input as an image in PNG format with size less than 256kb
		| ImageFormat    | Env | Region | APIkey                           |
		| 2400811734.png | tst | europe | 781b802322d248079ba7cce2abc2531a |
	Then Verify the response when image size is less than 256kb
	And The response must contain a list of all the identified character strings.

@1769076 @TST @WestEurope
Scenario: 03Test Case ID 1769076: Verify that the OCR service returns an empty list as response when a blurry image is passed as an input.

	When Send the request with a blurry image as input
		| ImageFormat    | Env | Region | APIkey                           |
		| 2400811734.png | tst | europe | 781b802322d248079ba7cce2abc2531a |
	Then Verify the response when the inputted image is blurry
	And The response must contain an empty list.
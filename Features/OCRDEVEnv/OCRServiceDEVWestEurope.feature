Feature: OCR Service in DEVEnv(WestEurope)

A short summary of the feature

@1769058 @DEV @WestEurope
Scenario: 01Test Case ID 1769058: Verify that the OCR service returns a list of all identified character strings from the image provided
	
	When Send the request with a correct image as input
		| ImageFormat  | Env | Region | APIkey                           |
		| PNGImage.png | dev | europe | 39731117349c436792eca8513c7d2eb6 |
	And Verify the response when correct image is inputted
	Then The response must contain a list of all the identified character strings.

@1769060 @DEV @WestEurope
Scenario: 02Test Case ID 1769060: Verify that the OCR service does not accept image with size more than 256kb

	When Send a request with input as an image in PNG format with size more than 256kb
		| ImageFormat        | Env | Region | APIkey                           |
		| More256KBImage.jpg | dev | europe | 39731117349c436792eca8513c7d2eb6 |
	Then Verify the response when image size is more than 256kb
	When Send a request with input as an image in PNG format with size less than 256kb
		| ImageFormat        | Env | Region | APIkey                           |
		| Less256KBImage.png | dev | europe | 39731117349c436792eca8513c7d2eb6 |
	Then Verify the response when image size is less than 256kb
	And The response must contain a list of all the identified character strings.

@1769076 @DEV @WestEurope
Scenario: 03Test Case ID 1769076: Verify that the OCR service returns an empty list as response when a blurry image is passed as an input.

	When Send the request with a blurry image as input
		| ImageFormat        | Env | Region | APIkey                           |
		| BlurryPNGImage.png | dev | europe | 39731117349c436792eca8513c7d2eb6 |
	Then Verify the response when the inputted image is blurry
	And The response must contain an empty list.

@1776266 @DEV @WestEurope
Scenario: 04Test Case ID 1776266: Verify that the OCR service returns an empty list as response when an invalid image is passed as an input.

	When Send the request with an invalid image (no characters)
		| ImageFormat      | Env | Region | APIkey                           |
		| NoCharacters.png | dev | europe   | 39731117349c436792eca8513c7d2eb6 |
	Then Verify the response when the inputted image is an invalid image (no characters)
	And The response must contain an empty list.

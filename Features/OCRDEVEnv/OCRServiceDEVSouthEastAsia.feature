Feature: OCR Service in DEVEnv(SouthEastAsia)

A short summary of the feature

@1769058 @DEV @SouthEastAsia
Scenario: 01Test Case ID 1769058: Verify that the OCR service returns a list of all identified character strings from the image provided
	
	When Send the request with a correct image as input
		| ImageFormat  | Env | Region | APIkey                           |
		| PNGImage.png | dev | asia   | 69df563662e74ba8811e47a9a61ccf7b |
	And Verify the response when correct image is inputted
	Then The response must contain a list of all the identified character strings.

@1769060 @DEV @SouthEastAsia
Scenario: 02Test Case ID 1769060: Verify that the OCR service does not accept image with size more than 256kb

	When Send a request with input as an image in PNG format with size more than 256kb
		| ImageFormat        | Env | Region | APIkey                           |
		| More256KBImage.jpg | dev | asia   | 69df563662e74ba8811e47a9a61ccf7b |
	Then Verify the response when image size is more than 256kb
	When Send a request with input as an image in PNG format with size less than 256kb
		| ImageFormat        | Env | Region | APIkey                           |
		| Less256KBImage.png | dev | asia   | 69df563662e74ba8811e47a9a61ccf7b |
	Then Verify the response when image size is less than 256kb
	And The response must contain a list of all the identified character strings.

@1769076 @DEV @SouthEastAsia
Scenario: 03Test Case ID 1769076: Verify that the OCR service returns an empty list as response when a blurry image is passed as an input.

	When Send the request with a blurry image as input
		| ImageFormat        | Env | Region | APIkey                           |
		| BlurryPNGImage.png | dev | asia   | 69df563662e74ba8811e47a9a61ccf7b |
	Then Verify the response when the inputted image is blurry
	And The response must contain an empty list.

@1776266 @DEV @SouthEastAsia
Scenario: 04Test Case ID 1776266: Verify that the OCR service returns an empty list as response when an invalid image is passed as an input.

	When Send the request with an invalid image (no characters)
		| ImageFormat      | Env | Region | APIkey                           |
		| NoCharacters.png | dev | asia   | 69df563662e74ba8811e47a9a61ccf7b |
	Then Verify the response when the inputted image is an invalid image (no characters)
	And The response must contain an empty list.
Feature: OCR Service – TST Environment (Southeast Asia Region)

A short summary of the feature

@1769058 @TST @SouthEastAsia
Scenario: 01Test Case ID 1769058: Verify that the OCR service returns a list of all identified character strings from the image provided
	
	When Send the request with a correct image as input
		| ImageFormat  | Env | Region | APIkey                           |
		| PNGImage.png | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
	And Verify the response when correct image is inputted
	Then The response must contain a list of all the identified character strings.

@1769060 @TST @SouthEastAsia
Scenario: 02Test Case ID 1769060: Verify that the OCR service does not accept image with size more than 256kb

	When Send a request with input as an image in PNG format with size more than 256kb
		| ImageFormat        | Env | Region | APIkey                           |
		| More256KBImage.jpg | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
	Then Verify the response when image size is more than 256kb
	When Send a request with input as an image in PNG format with size less than 256kb
		| ImageFormat        | Env | Region | APIkey                           |
		| Less256KBImage.png | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
	Then Verify the response when image size is less than 256kb
	And The response must contain a list of all the identified character strings.

@1769076 @TST @SouthEastAsia
Scenario: 03Test Case ID 1769076: Verify that the OCR service returns an empty list as response when a blurry image is passed as an input.

	When Send the request with a blurry image as input
		| ImageFormat        | Env | Region | APIkey                           |
		| BlurryPNGImage.png | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
	Then Verify the response when the inputted image is blurry
	And The response must contain an empty list.

@1776266 @TST @SouthEastAsia
Scenario: 04Test Case ID 1776266: Verify that the OCR service returns an empty list as response when an invalid image is passed as an input.

	When Send the request with an invalid image (no characters)
		| ImageFormat      | Env | Region | APIkey                           |
		| NoCharacters.png | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
	Then Verify the response when the inputted image is an invalid image (no characters)
	And The response must contain an empty list.

@1780460 @TST @SouthEastAsia
Scenario: 05Test Case ID 1780460: Verify that requests sent to the OCR Service with an invalid API key are rejected

	When Send a request to the South-East Asia region using an invalid API key
		| ImageFormat      | Env | Region | InvalidAPIkey |
		| NoCharacters.png | tst | asia   | 1234567890    |
	Then The request is rejected and returns a 401 Unauthorized error

@1780459 @TST @SouthEastAsia
Scenario: 06Test Case ID 1780459: Verify that requests without an API key for OCR Service are rejected

	When Send a request to the South-East Asia region without an API key
		| ImageFormat  | Env | Region | InvalidAPIkey |
		| PNGImage.png | tst | asia   |               |
	Then The request is rejected and returns a 401 Unauthorized error


@1780458 @TST @SouthEastAsia
Scenario: 07Test Case ID 1780458: Verify that requests with a valid API key for OCR Service are authenticated successfully
    
	When Send the request with a correct APIkey as input
		| ImageFormat  | Env | Region | APIkey                           |
		| PNGImage.png | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
	And Verify the response when correct APIkey is inputted

@1769059 @TST @SouthEastAsia
Scenario: 08Test Case ID 1769059: Verify that the OCR service only accepts supported image formats (JPEG, PNG, BMP, PDF, and TIFF)

	When Send a request with input as an image in a supported format (JPEG, PNG, BMP, PDF, TIFF) and verify the response and list of all the identified character strings
		| ImageFormat    | Env | Region | APIkey                           |
		| JPEGImage.jpeg | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
		| PNGImage.png   | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
		| BMPImage.bmp   | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
		| PDFImage.pdf   | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
		| TIFFImage.tiff | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
	And Send a request with input as an image in an unsupported format (Ex: GIF, WEBP, SVG, etc.) and verify the 400 error returned
		| ImageFormat    | Env | Region | APIkey                           |
		| GIFImage.gif   | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
		| WEBPImage.webp | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
		| SVGImage.svg   | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |

@1780485 @TST @SouthEastAsia
Scenario: 09Test Case ID 1780485: Verify the Response Time for OCR Service Under Normal Conditions

	When Send a request to the OCR service under normal system load and verify the median response time
		| ImageFormat  | Env | Region | APIkey                           |
		| PNGImage.png | tst | asia   | 891b8ea3a52d4cce94d436d633eb1b07 |
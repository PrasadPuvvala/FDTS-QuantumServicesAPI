@DataSource:ocr-environments.json @DataSet:OcrService
Feature: OCR Service

A short summary of the feature
@ocrservice
Scenario: 01Test Case ID 1769058: Verify that the OCR service returns a list of all identified character strings from the image provided

	When Send the request with a correct image as input using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
		| ImageFormat  |
		| PNGImage.png |
	And Verify the response when correct image is inputted
	Then The response must contain a list of all the identified character strings.

@ocrservice
Scenario: 02Test Case ID 1769060: Verify that the OCR service does not accept image with size more than 256kb

	When Send a request with input as an image in PNG format with size more than 256kb using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
		| ImageFormat        |
		| More256KBImage.jpg |
	Then Verify the response when image size is more than 256kb
	When Send a request with input as an image in PNG format with size less than 256kb using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
		| ImageFormat        |
		| Less256KBImage.png |
	Then Verify the response when image size is less than 256kb
	And The response must contain a list of all the identified character strings.

@ocrservice
Scenario: 03Test Case ID 1769076: Verify that the OCR service returns an empty list as response when a blurry image is passed as an input.

	When Send the request with a blurry image as input using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
		| ImageFormat        |
		| BlurryPNGImage.png |
	Then Verify the response when the inputted image is blurry
	And The response must contain an empty list.

@ocrservice
Scenario: 04Test Case ID 1776266: Verify that the OCR service returns an empty list as response when an invalid image is passed as an input.

	When Send the request with an invalid image (no characters) using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
		| ImageFormat      |
		| NoCharacters.png |
	Then Verify the response when the inputted image is an invalid image (no characters)
	And The response must contain an empty list.

@ocrservice
Scenario: 05Test Case ID 1780460: Verify that requests sent to the OCR Service with an invalid API key are rejected

	When Send a request using an invalid API key "<ApiKey>" with baseUrl "<BaseUrl>"
		| ImageFormat      | 
		| NoCharacters.png | 
	Then The request is rejected and returns a 401 Unauthorized error

@ocrservice
Scenario: 06Test Case ID 1780459: Verify that requests without an API key for OCR Service are rejected

	When Send a request without API key "<ApiKey>" with baseUrl "<BaseUrl>"
		| ImageFormat  | 
		| PNGImage.png | 
	Then The request is rejected and returns a 401 Unauthorized error

@ocrservice
Scenario: 07Test Case ID 1780458: Verify that requests with a valid API key for OCR Service are authenticated successfully
    
	When Send the request with baseUrl "<BaseUrl>" and correct API key "<ApiKey>" as input
		| ImageFormat  |
		| PNGImage.png |
	And Verify the response when correct APIkey is inputted

@ocrservice
Scenario: 08Test Case ID 1769059: Verify that the OCR service only accepts supported image formats (JPEG, PNG, BMP, PDF, and TIFF)

	When Send a request with input as an image in a supported format (JPEG, PNG, BMP, PDF, TIFF) using baseUrl "<BaseUrl>" and apiKey "<ApiKey>" and verify the response and list of all the identified character strings
		| ImageFormat    |
		| JPEGImage.jpeg | 
		| PNGImage.png   | 
		| BMPImage.bmp   | 
		| PDFImage.pdf   | 
		| TIFFImage.tiff |
	And Send a request with input as an image in an unsupported format (Ex: GIF, WEBP, SVG, etc.) using baseUrl "<BaseUrl>" and apiKey "<ApiKey>" and verify the 400 error returned
		| ImageFormat    |
		| GIFImage.gif   | 
		| WEBPImage.webp | 
		| SVGImage.svg   | 

@ocrservice
Scenario: 09Test Case ID 1780485: Verify the Response Time for OCR Service Under Normal Conditions

	When Send a request to the OCR service under normal system load using baseUrl "<BaseUrl>" and apiKey "<ApiKey>" and verify the median response time
		| ImageFormat  | 
		| PNGImage.png | 

@ocrservice
Scenario: 10Test Case ID 1780470: Verify OCR Service Deployment in Each Cloud Region
	
	When OCR service is deployed to all the cloud regions using baseUrl "<BaseUrl>" and apiKey "<ApiKey>"
		| ImageFormat  | 
		| PNGImage.png | 
	And OCR service should be operational in all the cloud region
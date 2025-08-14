Feature: DeviceImage Success

A short summary of the feature

@DeviceImage
Scenario: 01Test Case ID 1810722: [Avalon Service] Verify UpdateHDI API Updates HDI Version When Image Has Higher Version and Flash Write Protect Is Not LockedPermanent

	When Load a DFU image with a higher HDI version than the device, ensure Flash Write Protect is not set to "LockedPermanent", and send a request to the UpdateHDI API
		| SerialNumber |
		| 2400809347   |
	Then API updates the HDI in the device before writing the image

@DeviceImage
Scenario: 02Test Case ID 1810723: [Avalon Service] Verify UpdateHDI API Skips HDI Update When Flash Write Protect Is LockedPermanent

	When Load a DFU image with a higher HDI version than the device, and set device Flash Write Protect status to "LockedPermanent", and send a request to the UpdateHDI API
	Then API does not update the HDI in the device

@DeviceImage
Scenario: 03Test Case ID 1810711: [Avalon Service] Verify CheckDFUCompatibility API Returns True When Image HDI Version Is Equal or Higher Than Device HDI Version

	When Load a DFU image with an equal or higher HDI version than the one on the device and send a compatibility check request
	Then API returns "True" indicating the DFU image is compatible

@DeviceImage
Scenario: 04Test Case ID 1810710: [Avalon Service] Verify CheckDFUCompatibility API Returns False When Image HDI Version Is Lower Than Device HDI Version

	When Load a DFU image with a lower HDI version than the one on the device and send a compatibility check request
	Then API returns "False" indicating the DFU image is not compatible

@DeviceImage
Scenario: 05Test Case ID 1810717: [Avalon Service] Verify WriteFDI API Successfully Writes Image to Device When Optimized Programming Is False

	When Load a valid DFU image, set isOptimizedProgramming to "false", and send a request to WriteFDI API
	Then API writes the image to the device successfully

@DeviceImage
Scenario: 06Test Case 1810718: [Avalon Service] Verify WriteFDI API does not Write Image When Optimized Programming Is Set to True

	When Load a valid DFU image, set isOptimizedProgramming to "true", and send a request to WriteFDI API
	Then API does not write the image to the device
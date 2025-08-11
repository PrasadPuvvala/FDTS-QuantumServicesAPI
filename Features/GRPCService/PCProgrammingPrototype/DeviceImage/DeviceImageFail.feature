Feature: DeviceImage Fail

A short summary of the feature

@DeviceImage
Scenario: 01Test Case ID 1810723: [Avalon Service] Verify UpdateHDI API Skips HDI Update When Flash Write Protect Is LockedPermanent
	When Load a DFU image with a higher HDI version than the device, and set device Flash Write Protect status to "LockedPermanent", and send a request to the UpdateHDI API 
	|     SerialNumber       |
	|     2400801508         |
	Then API does not update the HDI in the device 

@DeviceImage
Scenario: 02Test Case ID 1810718: [Avalon Service] Verify WriteFDI API does not Write Image When Optimized Programming Is Set to True

	When Load a valid DFU image, set isOptimizedProgramming to true, and send a request to WriteFDI API
		| SerialNumber |
		| 2400801508   |
	Then API does not write the image to the device 

@DeviceImage
Scenario: 03Test Case ID 1810715: [Avalon Service] Verify WriteFDI API Returns Failed Precondition When DFU Compatibility has not been Checked

	When Send a request to the WriteFDI API without performing DFU compatibility check
		| SerialNumber |
		| 2400801508   |   
	Then API throws an exception with status "Failed Precondition"

@DeviceImage
Scenario: 04Test Case ID 1810716: [Avalon Service] Verify WriteFDI API Returns Failed Precondition When DFU Image Is Not Loaded

	When Send a request to the WriteFDI API without loading a DFU image
		| SerialNumber |
		| 2400801508   |
	Then API throws an exception with status the "Failed Precondition"



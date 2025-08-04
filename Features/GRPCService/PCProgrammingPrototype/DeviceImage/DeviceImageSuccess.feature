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
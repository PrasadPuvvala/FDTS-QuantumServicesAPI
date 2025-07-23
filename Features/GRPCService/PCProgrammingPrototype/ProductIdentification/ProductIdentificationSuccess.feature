Feature: ProductIdentification Success

A short summary of the feature

@ProductIdentification
Scenario: 01Test Case ID 1809234: [Avalon Service] Verify PCBAPartNumber API Returns the PCBA Part Number When Requested

	When Send a request to the PCBAPartNumber with connected device
		| SerialNumber |
		| 2400801519   |
	Then API returns the PCBA part number of the device

@ProductIdentification
Scenario: 02Test Case ID 1809245: [Avalon Service] Verify PlatformName API Returns Platform Name Starting with 'C' for Coyote Chip

	When Send a request to the PlatformName API with a connected device
	Then API returns the hardware platform name, which starts with "C" for a Coyote chip

@ProductIdentification
Scenario: 03Test Case ID 1809252: [Avalon Service] Verify SerialNumber API Returns the Device Serial Number on Valid Request

	When Send a request to the SerialNumber API to read the current serial number
	Then API returns the serial number of the connected device

@ProductIdentification
Scenario: 04Test Case ID 1809250: [Avalon Service] Verify SerialNumber API Successfully Writes Serial Number to the Device

	When Send a request to the SerialNumber API with a valid serial number
		| SerialNumber |
		| 2400801519   |
	Then API writes the serial number to the device and return status as "Success"
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

@ProductIdentification
Scenario: 05Test Case ID 1809265: [Avalon Service] Verify FittingSide API Returns the Current Fitting Side of the Device

	When Send a request to the FittingSide API to read the current fitting side from the device.
	Then API returns the fitting side of the connected device (Ex: Left or Right).

@ProductIdentification
Scenario: 06Test Case ID 1809262: [Avalon Service] Verify FittingSide API Successfully Writes the Fitting Side to the Device

	When Send a request to the FittingSide API with a valid fitting side (Ex: Left or Right) 
		| FittingSide |
		| Left        |
	Then API writes the fitting side to the device and returns status as "success"

@ProductIdentification
Scenario: 07Test Case ID 1809275: [Avalon Service] Verify ProximityNetworkAddress API Successfully Returns the Network Address from the Device

	When Send a request to the ProximityNetworkAddress API to read the current network address from the device
	Then API returns the network address of the device 

@ProductIdentification
Scenario: 08Test Case ID 1810697: [Avalon Service] Verify ProductIdentity API Successfully Verifies Product Identity With Valid Inputs

	When Send a request with valid BleId, correct Brand, and private label code.
	     | bleId      | brand |privateLabelCode|
	     | 1093078272 |ReSound| 0             |
	Then API verifies that the input values match those in the device

@ProductIdentification
Scenario: 09Test Case ID 1810702: [Avalon Service] Verify API Successfully Writes MFI Brand, Family, Model, and GAP Device Name to GATT Database

	When Send a request with valid values for MFI brand, MFI family, MFI model, and GAP device name
		| MFIBrand | MFIModel | MFIFamily | GapDeviceName        |
		| ReSound  | NX962-DRW| Dooku3    | Users Hearing Aid    |
	Then API writes the values to the GATT database successfully

@ProductIdentification
Scenario: 10Test Case ID 1809246: [Avalon Service] Verify CloudIdentityData API Returns HIID and Bluetooth Address on Valid Request

	When Send a request to the GNOSRegistrationData API to retrieve GNOS registration data from the hearing instrument
	Then API returns the GNOS registration data in valid XML format

@ProductIdentification
Scenario: 11Test Case ID 1809277: [Avalon Service] Verify DateModified API Successfully Returns the Modified Date from the Device

	When Send a request to the DateModified API to read the modified date from the device
	Then API returns the modified date of the device 
	
@ProductIdentification
Scenario: 12Test Case 1809278: [Avalon Service] Verify ResetDateModified API Resets the Date Modified to Default Value

	When Send a request to the ResetDateModified API to reset the date modified
	Then API resets the date modified to "1970-01-01 00:00:00"

@ProductIdentification
Scenario: 13Test Case 1809280: [Avalon Service] Verify OptionsForDevice API Returns Current Device Options Successfully

	When Send a request to the OptionsForDevice API to retrieve current device options
	Then API returns the device options of the device

@ProductIdentification
Scenario: 14Test Case ID 1809279: [Avalon Service] Verify OptionsForDevice API Writes Device Options Successfully

	When Send a request to the OptionForDevice API with a valid integer related to device options
	      | optionsForDevice |
	      |       0          |
	Then API writes the device options to the device successfully

@ProductIdentification
Scenario: 15Test Case 1809282: [Avalon Service] Verify PrivateLabelCode API Returns Current Private Label Code Successfully
	When Send a request to the PrivateLabelCode API to retrieve the private label code 
	Then API returns the private label code of the device 

@ProductIdentification
Scenario: 16Test Case 1809281: [Avalon Service] Verify PrivateLabelCode API Writes Private Label Code Successfully

	When Send a request to the PrivateLabelCode API with a valid private label code
		| privateLabelCode |
		|      0           |
	Then API writes the private label code to the device successfully



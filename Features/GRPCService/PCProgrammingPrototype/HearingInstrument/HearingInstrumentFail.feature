Feature: HearingInstrument Fail

A short summary of the feature

@HearingInstrument
Scenario: 01Test Case ID 1809102: [Avalon Service] Verify DetectBySerialNumber API Returns 'DeviceNotFound' When No Matching Serial Number Exists

	When Send a request to the DetectBySerialNumber API with a valid serial number that does not match any device.
		| SerialNumber |
		| 2400801543   |
	Then API returns null for device node data and status "DeviceNotfound"

@HearingInstrument
Scenario: 02Test Case ID 1807118: [Avalon Service] Verify DetectClosest API Returns 'DeviceNotFound' When No Devices Are Nearby

	When Send a request to the DetectClosest API when no devices are nearby
	Then DetectClosest API returns null for device node data and status "DeviceNotFound"

@HearingInstrument
Scenario: 03Test Case ID 1809107: [Avalon Service] Verify DetectWired API Returns 'DeviceNotFound' When No Device Is Found for Monaural Side

	When Send a request to the DetectWired API with a valid monoaural side (e.g., "Left/Right") when no device is connected.
	    | SerialNumber |
		| 2400801543   |
	Then API returns null for device node data and status for DetectWired "DeviceNotFound"

@HearingInstrument
Scenario: 04Test Case ID 1809120: [Avalon Service] Verify DeviceNodeData API Returns 'Null' When No Device Is Connected

	When Ensure that no device is connected Send a request to the DeviceNodeData API 
	Then API returns null for device node data

@HearingInstrument
Scenario: 05Test Case ID 1809116: [Avalon Service] Verify ConnectToDevice API Returns 'ConnectFailed' When Device Connection Fails
  
    When Simulate where a valid device node is detected but the connection to the device and Send a request to the ConnectToDevice API
	Then API returns status "ConnectFailed"

@HearingInstrument
Scenario: 06Test Case ID 1809119: [Avalon Service] Verify ConnectToDevice API Returns 'AuthenticationFailed' When Device Has Been Powered for More Than 3 Minutes

	When Select a device that has been powered on for more than 3 minutes, then attempt to connect using the ConnectToDevice API
	Then API returns a status "AuthenticationFailed"

@HearingInstrument
Scenario: 07Test Case ID 1809118: [Avalon Service] Verify ConnectToDevice API Returns 'HearingInstrumentCreationFailed' When Device Model Is Not Found in Product Configuration Database

	When Send a request to the ConnectToDevice API with a valid node data where the device model not listed in the product configuration database
	         | SerialNumber  |
	         | 1949192119    |
	Then API returns the status "HearingInstrumentCreationFailed"

@HearingInstrument
Scenario: 08Test Case ID 1810796: [Avalon Service] Verify BootDevice API Boots Device with selected Boot mode without reconnecting when reconnect flag is False

	When Send a request to the BootDevice API with any boot type when reconnect flag is set to False and verify the response
		| BootType    |
		| ServiceMode |
		| DfuMode     |
		| DspRunning  |



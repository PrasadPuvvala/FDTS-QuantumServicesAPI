Feature: HearingInstrument Fail

A short summary of the feature

@tag1
Scenario: 01Test Case ID 1809102: [Avalon Service] Verify DetectBySerialNumber API Returns 'DeviceNotFound' When No Matching Serial Number Exists

	When Send a request to the DetectBySerialNumber API with a valid serial number that does not match any device.
		| SerialNumber |
		| 2400801520   |
	Then API returns null for device node data and status "DeviceNotfound"

Scenario: 02Test Case ID 1807118: [Avalon Service] Verify DetectClosest API Returns 'DeviceNotFound' When No Devices Are Nearby

	When Send a request to the DetectClosest API when no devices are nearby
	Then DetectClosest API returns null for device node data and status "DeviceNotFound"

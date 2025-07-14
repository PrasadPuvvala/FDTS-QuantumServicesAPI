Feature: HearingInstrument Success

A short summary of the feature

@HearingInstrument
Scenario: 01Test Case ID 1809101: [Avalon Service] Verify DetectBySerialNumber API Returns 'Success' When Valid Serial Number Matches a Device
	
	When Send a request to the DetectBySerialNumber API with a valid serial number that matches an existing device
		| SerialNumber |
		| 2400801520   |
	Then API returns device node data and AvalonStatus "Success"

@HearingInstrument
Scenario: 02Test Case ID 1807206: [Avalon Service] Verify DetectClosest API Returns Correct Device Node Data for One or Multiple Devices

	When Send a request to the DetectClosest API when one device is found
	Then API returns the correct device node data and AvalonStatus "Success"

@HearingInstrument
Scenario: 03Test Case ID 1809265: [Avalon Service] Verify FittingSide API Returns the Current Fitting Side of the Device

	When Send a request to the FittingSide API to read the current fitting side from the device
	Then API returns the fitting side of the connected device (Ex: Left or Right)

@HearingInstrument
Scenario: 04Test Case ID 1809122: [Avalon Service] Verify DeviceNodeData API Returns Device Node Details When Device Is Connected

	When Connect a supported device Send a request to the DeviceNodeData API
	Then API returns complete device node data

@HearingInstrument
Scenario: 05Test Case ID 1809114: [Avalon Service] Verify ConnectToDevice API Returns 'Success' When Valid Device Node Data Is Provided

	When Send a request to the ConnectToDevice API with valid and detected device node data
	Then API successfully connects to the device and returns status "Success"

@HearingInstrument
Scenario: 06Test Case ID 1809127: [Avalon Service] Verify CheckBootMode API Returns Boot Mode of the Connected Device

	When Connect a supported device Send a request to the CheckBootMode API 
	Then API returns the current boot mode of the connected device

@HearingInstrument
Scenario: 07Test Case ID 1810795: [Avalon Service] Verify BootDevice API Boots Device with selected Boot mode and reconnects when reconnect flag is True

	When Send a request to the BootDevice API with any boot type(Ex: DspRunning, DfuMode, ServiceMode) "ServiceMode" and reconnect flag set to True
	Then Device boots and API reconnects to the device successfully.

@HearingInstrument
Scenario: 08Test Case ID 1810811: [Avalon Service] Verify FlashWriteProtect API returns current Flash Write Protect status on read request

	When Send a request to the FlashWriteProtect API to read current status 
	Then API returns one of the valid states "NotLocked" , "Locked" & "LockedPermanent"  
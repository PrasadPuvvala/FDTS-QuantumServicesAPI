Feature: HearingInstrument Success

A short summary of the feature

@HearingInstrument
Scenario: 01Test Case ID 1809101: [Avalon Service] Verify DetectBySerialNumber API Returns 'Success' When Valid Serial Number Matches a Device
	
	When Send a request to the DetectBySerialNumber API with a valid serial number that matches an existing device
		| SerialNumber |
		| 2400801519   |
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

	When Send a request to the BootDevice API with any boot type when reconnect flag is set to True and verify the response
		| BootType    |
		| ServiceMode |
		| DfuMode     |
		| DspRunning  |

@HearingInstrument
Scenario: 08Test Case ID 1810811: [Avalon Service] Verify FlashWriteProtect API returns current Flash Write Protect status on read request

	When Send a request to the FlashWriteProtect API to read current status
	Then API returns one of the valid states "NotLocked" , "Locked" & "LockedPermanent"

@HearingInstrument
Scenario: 09Test Case ID 1810812: [Avalon Service] Verify FlashWriteProtect API sets Flash Write Protect state to 'Lock' and returns updated status

	When Send a request to the FlashWriteProtect API with state as "Lock"
	Then API returns status as "Lock"

@HearingInstrument
Scenario: 10Test Case ID 1810813: [Avalon Service] Verify FlashWriteProtect API sets Flash Write Protect state to 'UnLock' and returns updated status

	When Send a request to the FlashWriteProtect API with state as "UnLock"
	Then API returns status as "UnLock"

@HearingInstrument
Scenario: 11Test Case ID 1810815: [Avalon Service] Verify FlashWriteProtect API sets Flash Write Protect state to 'LockedPermanent' and returns updated status

	When Send a request to the FlashWriteProtect API with state set as "LockPermanent"
	Then API returns status as "LockPermanent"

@HearingInstrument
Scenario: 12Test ID Case 1840256: [HI Automation] Verify FlashWriteProtect API does not allow changing state from 'LockedPermanent' to 'Lock'

	When Send a request to FlashWriteProtect API with state "Lock"
	Then API returns status as "LockPermanent" when state is set to Lock

@HearingInstrument
Scenario: 13Test Case ID 1810947: [HI Automation] Verify FlashWriteProtect API does not allow changing state from 'LockedPermanent' to 'UnLock'

	When Send a request to FlashWriteProtect API with state "UnLock" 
	Then API returns status as "LockPermanent" when state is set to UnLock

@HearingInstrument
Scenario: 14Test Case ID 1809214: [Avalon Service] Verify RHI Status API Returns 'False' for Non-Rechargeable Devices

	When Connect a non-rechargeable device and send a request to the RHI Status API
	Then API returns "False" for the RHI status

@HearingInstrument
Scenario: 15Test Case ID 1809213: [Avalon Service] Verify RHI Status API Returns 'True' for Rechargeable Devices

	When Connect a rechargeable RHI device and send a request to the RHI Status API
	Then API returns "True" for the RHI status

@HearingInstrument
Scenario: 16Test Case ID 1809130: [Avalon Service] Verify RHIBatteryLevel API Returns Battery Level of the Connected Device

	When Connect a supported RHI device Send a request to the RHIBatteryLevel API
	Then API returns the battery level of the device values between 0 to 10

@HearingInstrument
Scenario: 17Test Case ID 1809222: [Avalon Service] Verify MFI Chip Health API Returns 'False' When MFI Chip Is Unhealthy

	When Send a request to the MFI Chip Health API with a connected device having unhealthy MFI chip
	Then API returns "False" for the MFI chip health status

@HearingInstrument
Scenario: 18Test Case ID 1809221: [Avalon Service] Verify MFI Chip Health API Returns 'True' When MFI Chip Is Healthy

	When Send a request to the MFI Chip Health API with a connected device having healthy MFI chip
	Then API returns "True" for the MFI chip health status

@HearingInstrument
Scenario: 19Test Case ID 1809218: [Avalon Service] Verify RHI Battery Type API Reads Battery Type from Device Successfully

	When Send a request to the RHI Battery Type API to read the battery type from the connected RHI device
	Then API returns the correct battery type from the device

@HearingInstrument
Scenario: 20Test Case ID 1809217: [Avalon Service] Verify RHI Battery Type API Writes Battery Type to Device Successfully

	When Send a request to the RHI Battery Type API with a valid battery type to write to the connected RHI device
		| BatteryType    |
		| Mic-Power 9440 |
	Then Battery type is successfully written to the device

@HearingInstrument
Scenario: 21Test Case ID 1810951: [Avalon Service] Verify ReadBatteryVoltage API returns battery voltage values for a RHI device

	When Send a request to the ReadBatteryVoltage API on a RHI device
	Then API returns valid values for Voltage,MinimumVoltage,MaximumVoltage

@HearingInstrument
Scenario: 22Test Case ID 1809226: [Avalon Service] Verify DeviceFunctional API Makes Device Functional When 'Enable Functionality' Is Set to True

	When Send a request to the DeviceFunctional API with enable functionality is "true"
	Then Verify the response when enable functionality input is set to true

@HearingInstrument
Scenario: 23Test Case ID 1809227: [Avalon Service] Verify DeviceFunctional API Makes Device Functional When 'Enable Functionality' Is Set to False

	When Send a request to the DeviceFunctional API with enable functionality is "false"
	Then Verify the response when enable functionality input is set to false

@HearingInstrument
Scenario: 24Test Case ID 1809231: [Avalon Service] Verify RHIPowerOff API Powers Off the Device When Requested

	When Send a request to the RHIPowerOff API to power off the connected RHI device
	Then Verify the response when RHIPowerOff API is called
Feature: GRPCService

A short summary of the feature

@GRPCService
Scenario: 01 GRPC Services for Initialize method
	When I call the Initialize method on the GRPC service
	Then Verify the response for the Initialize method on the GRPC service

@GRPCService
Scenario: 02 GRPC Service for ConfigureProduct
	When I call the ConfigureProduct on the GRPC service with folder path "C:\\ProgramData\\GN GOP\\Configuration\\FDTS"
	Then Verify the response for the ConfigureProduct on the GRPC service

@GRPCService
Scenario: 03 GRPC Service for DetectBySerialNumber
	When I call the DetectBySerialNumber on the GRPC service with serialnumber "2400800242"
	Then Verify the response for the DetectBySerialNumber on the GRPC service

@GRPCService
Scenario: 04 GRPC Service for DetectClosest
	When I call the DetectClosest on the GRPC service 
	Then Verify the response for the DetectClosest on the GRPC service

@GRPCService
Scenario: 05 GRPC Service for DetectOnSide
	When I call the DetectOnSide on the GRPC service 
	Then Verify the response for the DetectOnSide on the GRPC service

@GRPCService
Scenario: 06 GRPC Service for EnableMasterConnect
	When I call the EnableMasterConnect on the GRPC service 
	Then Verify the response for the EnableMasterConnect on the GRPC service

@GRPCService
Scenario: 07 GRPC Service for EnableFittingMode
	When I call the EnableFittingMode on the GRPC service 
	Then Verify the response for the EnableFittingMode on the GRPC service

@GRPCService
Scenario: 08 GRPC Service for GetDeviceNode
	When I call the GetDeviceNode on the GRPC service 
	Then Verify the response for the GetDeviceNode on the GRPC service

@GRPCService
Scenario: 09 GRPC Service for Connect
	When I call the Connect on the GRPC service 
	Then Verify the response for the Connect on the GRPC service

@GRPCService
Scenario: 10 GRPC Service for GetBootMode
	When I call the GetBootMode on the GRPC service 
	Then Verify the response for the GetBootMode on the GRPC service

@GRPCService
Scenario: 11 GRPC Service for Boot
	When I call the Boot on the GRPC service 
	Then Verify the response for the Boot on the GRPC service

@GRPCService
Scenario: 12 GRPC Service for GetFlashWriteProtectStatus
	When I call the GetFlashWriteProtectStatus on the GRPC service 
	Then Verify the response for the GetFlashWriteProtectStatus on the GRPC service

@GRPCService
Scenario: 13 GRPC Service for SetFlashWriteProtectState
	When I call the SetFlashWriteProtectState on the GRPC service 
	Then Verify the response for the SetFlashWriteProtectState on the GRPC service

@GRPCService
Scenario: 14 GRPC Service for IsRechargeable
	When I call the IsRechargeable on the GRPC service 
	Then Verify the response for the IsRechargeable on the GRPC service

@GRPCService
Scenario: 15 GRPC Service for GetBatteryLevel
	When I call the GetBatteryLevel on the GRPC service 
	Then Verify the response for the GetBatteryLevel on the GRPC service

@GRPCService
Scenario: 16 GRPC Service for ShouldVerifyMfiChip
	When I call the ShouldVerifyMfiChip on the GRPC service 
	Then Verify the response for the ShouldVerifyMfiChip on the GRPC service

@GRPCService
Scenario: 17 GRPC Service for VerifyMfiChipIsHealthy
	When I call the VerifyMfiChipIsHealthy on the GRPC service 
	Then Verify the response for the VerifyMfiChipIsHealthy on the GRPC service

@GRPCService
Scenario: 18 GRPC Service for GetBatteryType
	When I call the GetBatteryType on the GRPC service 
	Then Verify the response for the GetBatteryType on the GRPC service

@GRPCService
Scenario: 19 GRPC Service for SetBatteryType
	When I call the SetBatteryType on the GRPC service 
	Then Verify the response for the SetBatteryType on the GRPC service

@GRPCService
Scenario: 20 GRPC Service for GetBatteryVoltage
	When I call the GetBatteryVoltage on the GRPC service 
	Then Verify the response for the GetBatteryVoltage on the GRPC service

@GRPCService
Scenario: 21 GRPC Service for MakeDeviceFunctional
	When I call the MakeDeviceFunctional on the GRPC service 
	Then Verify the response for the MakeDeviceFunctional on the GRPC service

@GRPCService
Scenario: 22 GRPC Service for SetPowerOff
	When I call the SetPowerOff on the GRPC service 
	Then Verify the response for the SetPowerOff on the GRPC service
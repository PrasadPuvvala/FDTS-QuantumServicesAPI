Feature: GRPCService

A short summary of the feature

@GRPCService
Scenario: 01GRPC Services for Initialize method
	When I call the Initialize method on the GRPC service
	Then Verify the response for the Initialize method on the GRPC service

@GRPCService
Scenario: 02GRPC Service for ConfigureProduct
	When I call the ConfigureProduct on the GRPC service with folder path "C:\\ProgramData\\GN GOP\\Configuration\\FDTS"
	Then Verify the response for the ConfigureProduct on the GRPC service

@GRPCService
Scenario: 03GRPC Service for DetectBySerialNumber
	When I call the DetectBySerialNumber on the GRPC service with serialnumber "2400800242"
	Then Verify the response for the DetectBySerialNumber on the GRPC service

@GRPCService
Scenario: 04GRPC Service for DetectClosest
	When I call the DetectClosest on the GRPC service 
	Then Verify the response for the DetectClosest on the GRPC service

@GRPCService
Scenario: 05GRPC Service for DetectOnSide
	When I call the DetectOnSide on the GRPC service 
	Then Verify the response for the DetectOnSide on the GRPC service

@GRPCService
Scenario: 06GRPC Service for EnableMasterConnect
	When I call the EnableMasterConnect on the GRPC service 
	Then Verify the response for the EnableMasterConnect on the GRPC service

@GRPCService
Scenario: 07GRPC Service for EnableFittingMode
	When I call the EnableFittingMode on the GRPC service 
	Then Verify the response for the EnableFittingMode on the GRPC service

@GRPCService
Scenario: 08GRPC Service for GetDeviceNode
	When I call the GetDeviceNode on the GRPC service 
	Then Verify the response for the GetDeviceNode on the GRPC service
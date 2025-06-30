Feature: HearingInstrument Success

A short summary of the feature

@HearingInstrument
Scenario: 01 GRPC Services for Initialize method
	When I call the Initialize method on the GRPC service
	Then Verify the response for the Initialize method on the GRPC service

@HearingInstrument
Scenario: 02 GRPC Service for ConfigureProduct
	When I call the ConfigureProduct on the GRPC service with folder path "C:\\ProgramData\\GN GOP\\Configuration\\FDTS"
	Then Verify the response for the ConfigureProduct on the GRPC service

@HearingInstrument
Scenario: 03Test Case ID 1809101: [Avalon Service] Verify DetectBySerialNumber API Returns 'Success' When Valid Serial Number Matches a Device
	
	When Send a request to the DetectBySerialNumber API with a valid serial number that matches an existing device
		| SerialNumber |
		| 2400800242   |
	Then API returns device node data and status "Success"

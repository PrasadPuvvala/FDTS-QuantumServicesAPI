Feature: Communication Success

A short summary of the feature

@Communication
Scenario: 01Test Case ID 682208: [Avalon Service] Verify CommunicationDevice API initializes HIPRO2 device when firmware is available

    When Connect HIPRO2 with available/test firmware and send a request to the InitializeCommunication API
    Then API initializes the device successfully with supported firmware

@Communication
Scenario: 02Test Case ID 682212: [Avalon Service] Verify CommunicationDevices API initializes NoahLink Wireless device with supported firmware

    When Connect NoahLink Wireless (firmware 2.19) and send a request to the InitializeCommunicationDevice API
    Then API initializes the device successfully

@Communication
Scenario: 03Test Case ID 682209: [HI Automation] Verify CommunicationDevice API initializes SpeedLink device with supported firmware

     When Connect SpeedLink (firmware 3.0.22) and send a request to the InitializeCommunicationDevice API
     Then API initializes the device successfully 
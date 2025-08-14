Feature: SecurityCertificates Fail

A short summary of the feature

@SecurityCertificates
Scenario: 01Test Case ID 1810747: [Avalon Service] Verify ModelInPricePointCertificate API Returns ValidCertModelMismatch When BleId Does Not Match
	When Load a valid image with a BleId that differs from the device’s BleId, and send the request to the ModelInPricePointCertificate API
       | SerialNumber |
	   | 2400801508   |
	Then API returns 'ValidCertModelMismatch'

@SecurityCertificates
Scenario: 02Test Case 1810755: [Avalon Service] Verify DeviceFamilyCertificateValidity API Returns False When Certificate in Device Is Invalid
	When Send a request to the DeviceFamilyCertificateValidity API when the device contains an invalid Device Family Certificate.
	| SerialNumber |
	| 2400801508   |
	Then API returns status for DeviceFamilyCertificateValidity 'False'


@SecurityCertificates
Scenario: 03Test Case 1810751: [Avalon Service] Verify PricePointCertificateValidity API Returns False When Certificate in Device Is Invalid

	When Send a request to the PricePointCertificateValidity API when the device has an invalid Price Point Certificate
	| SerialNumber |
	| 2400801508   |
	Then API returns status for PricePointCertificateValidity 'False' 
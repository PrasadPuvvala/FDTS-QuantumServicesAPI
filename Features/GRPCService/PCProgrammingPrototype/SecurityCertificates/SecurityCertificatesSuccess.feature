Feature: Security Certificates Success

A short summary of the feature

@SecurityCertificates
Scenario: 01Test Case ID 1810730: [Avalon Service] Verify InProductionCertificate API Successfully Writes Certificate to Hearing Instrument

	When Send a request to the InProductionCertificate API with a valid certificate to be written to the device
		| SerialNumber |
		| 2400801504   |
	Then API writes the certificate successfully to the hearing instrument

@SecurityCertificates
Scenario: 02Test Case ID 1810746: [Avalon Service] Verify ModelInPricePointCertificate API Returns ValidCertModelMatch When BleId Matches Between Image and Device

	When Load a valid image and ensure the BleId in the image matches the device’s BleId, and send request to the ModelInPricePointCertificate API
	Then API returns "ValidCertModelMatch"

@SecurityCertificates
Scenario: 03Test Case ID 1810754: [Avalon Service] Verify DeviceFamilyCertificateValidity API Returns True When Certificate in Device Is Valid

	When Send a request to the DeviceFamilyCertificateValidity API when the device contains a valid Device Family Certificate
	Then API returns "True" for DeviceFamilyCertificateValidity

@SecurityCertificates
Scenario: 04Test Case ID 1810750: [Avalon Service] Verify PricePointCertificateValidity API Returns True When Certificate in Device Is Valid

	When Send a request to the PricePointCertificateValidity API when the device has a valid Price Point Certificate
	Then API returns "True" for PricePointCertificateValidity

@SecurityCertificates
Scenario: 05Test Case ID 1810734: [Avalon Service] Verify PricePointCertificate API Successfully Writes Certificate to Hearing Instrument

	When Send a request to the PricePointCertificate API with a valid Price Point Certificate
	Then API writes the PricePointCertificate successfully to the hearing instrument
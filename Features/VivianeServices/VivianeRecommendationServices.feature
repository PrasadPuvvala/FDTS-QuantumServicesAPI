@DataSource:Viviane-Recommandation-Services.json @DataSet:VivianeRecommendationService
Feature: Viviane Recommendation Services

A short summary of the feature

@VivianeRecommendationService
Scenario: 01Test Case ID 2414375: [Recommendations Service] Verify PostCorrectiveAction API works as expected for Europe, Asia, and US regions

	When Send POST request to the "<Region>" endpoint with valid headers and body "<BaseUrl>" and "<ApiKey>" with endpoint PostCorrectiveAction
		| PostCorrectiveActionData  |
		| PostCorrectiveAction.json |
	Then verify the API response of the Viviane Recommendation Service

@VivianeRecommendationService
Scenario: 02Test Case ID 2414381: [Recommendations Service] Verify PostFdtsDataForRecommendation API works as expected for Europe, Asia, and US regions

	When Send POST request to the "<Region>" endpoint with valid FDTS metadata and payload "<BaseUrl>" and "<ApiKey>" with endpoint PostFdtsDataForRecommendation
		| PostFdtsDataForRecommendationData  |
		| PostFdtsDataForRecommendation.json |
	Then verify the API response of the Viviane Recommendation Service

@VivianeRecommendationService
Scenario: 03Test Case ID 2414385: [Recommendations Service] Verify PostRecommendationFeedback API works as expected for Europe, Asia, and US regions

	When Send POST request to the "<Region>" endpoint with valid feedback "<BaseUrl>" and "<ApiKey>" with endpoint PostRecommendationFeedback
		| PostRecommendationFeedbackData  |
		| PostRecommendationFeedback.json |
	Then verify the API response of the Viviane Recommendation Service
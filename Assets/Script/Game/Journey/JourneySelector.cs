// JourneySelector.cs
// Handles the Financial Journey selection screen (Savings, Budgeting, Investing).
// Saves the player's chosen journey to PlayFab and navigates to HomeScreen.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JourneySelector : MonoBehaviour
{
    [Header("Journey Buttons")]
    public Button savingsButton;
    public Button budgetingButton;
    public Button investingButton;

    [Header("Navigation Buttons")]
    public Button confirmButton;
    public Button backButton;

    private string selectedJourney = null;

    private void Start()
    {
        // Confirm button is not interactable until a journey is selected
        confirmButton.interactable = false;

        // Add listeners for journey selection
        savingsButton.onClick.AddListener(() => SelectJourney("Savings"));
        budgetingButton.onClick.AddListener(() => SelectJourney("Budgeting"));
        investingButton.onClick.AddListener(() => SelectJourney("Investing"));

        // Navigation buttons
        confirmButton.onClick.AddListener(OnConfirmJourney);
        backButton.onClick.AddListener(OnBackPressed);
    }

    private void SelectJourney(string journey)
    {
        selectedJourney = journey;

        // Enable confirm button
        confirmButton.interactable = true;

        // Highlight selected button
        Color selectedColor = Color.yellow;
        Color defaultColor = Color.white;

        savingsButton.image.color = (journey == "Savings") ? selectedColor : defaultColor;
        budgetingButton.image.color = (journey == "Budgeting") ? selectedColor : defaultColor;
        investingButton.image.color = (journey == "Investing") ? selectedColor : defaultColor;
    }

    private void OnConfirmJourney()
    {
        if (string.IsNullOrEmpty(selectedJourney))
        {
            Debug.LogWarning("⚠️ No journey selected!");
            return;
        }

        // Create JourneyData object
        JourneyData journeyData = new JourneyData(selectedJourney, true);

        // Save the journey to PlayFab using PlayFabDataManager
        PlayFabDataManager.Instance.SaveJourney(journeyData, () =>
        {
            Debug.Log("✅ Journey saved to PlayFab!");
            SceneManager.LoadScene("HomeScreen");
        });
    }

    private void OnBackPressed()
    {
        SceneManager.LoadScene("MenuScreen");
    }
}

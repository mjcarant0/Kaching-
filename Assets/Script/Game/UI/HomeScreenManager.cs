// HomeScreenManager.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class HomeScreenManager : MonoBehaviour
{
    [Header("Journey Backgrounds")]
    public GameObject savingsBackground;
    public GameObject budgetingBackground;
    public GameObject investingBackground;

    [Header("Journey Chapter Texts (Images)")]
    public GameObject savingsChapterText;
    public GameObject budgetingChapterText;
    public GameObject investingChapterText;

    [Header("Journey Selector")]
    public Button journeyDownButton; // The main button showing current journey
    public TMP_Text journeyDownText; // TMP Text for current journey
    public GameObject journeyUpPanel; // Panel showing unselected journeys
    public Button journeyUpSelectedButton; // JourneyUp button (current journey)
    public TMP_Text journeyUpSelectedText; // TMP Text showing current journey in JourneyUp
    public Button[] unselectedJourneyButtons; // Array of buttons inside JourneyUp
    public TMP_Text[] unselectedJourneyTexts; // TMP Texts inside JourneyUp buttons

    [Header("Mission Buttons")]
    public Button mission1Button;
    public Button mission2Button;
    public Button mission3Button;
    public Button mission4Button;
    public Button mission5Button;

    [Header("Popup")]
    public GameObject popupPanel;
    public TMP_Text popupText;

    [Header("Coins Display")]
    public TMP_Text goldCoinText; // TMP text for gold
    public TMP_Text silverCoinText; // TMP text for silver

    private string currentJourney;
    private int goldCoins = 0; // Gold coin count
    private int silverCoins = 0; // Silver coin count

    void Start()
    {
        LoadCurrentJourney(); // Load current journey from PlayFab or default
        UpdateJourneyUI(); // Update journey UI elements

        journeyDownButton.onClick.AddListener(ToggleJourneyUpPanel); // Toggle JourneyUp panel
        journeyUpSelectedButton.onClick.AddListener(() => journeyUpPanel.SetActive(false)); // JourneyUp selected button click: just close panel

        // Add listener to each unselected journey button
        for (int i = 0; i < unselectedJourneyButtons.Length; i++)
        {
            int index = i; // local copy for closure
            unselectedJourneyButtons[i].onClick.AddListener(() => TrySelectJourney(index));
        }

        // Mission buttons
        mission1Button.onClick.AddListener(() => GoToMission(1));
        mission2Button.onClick.AddListener(() => GoToMission(2));
        mission3Button.onClick.AddListener(() => GoToMission(3));
        mission4Button.onClick.AddListener(() => GoToMission(4));
        mission5Button.onClick.AddListener(() => GoToMission(5));

        // Disable locked missions (for now only mission 1 is interactable)
        mission1Button.interactable = true;
        mission2Button.interactable = false;
        mission3Button.interactable = false;
        mission4Button.interactable = false;
        mission5Button.interactable = false;

        popupPanel.SetActive(false); // Hide popup panel initially
        journeyUpPanel.SetActive(false); // Hide JourneyUp panel initially

        LoadPlayerCoins(); // Load coins from PlayFab
    }

    void LoadCurrentJourney()
    {
        if (PlayFabDataManager.Instance != null && PlayFabDataManager.Instance.HasJourney())
        {
            currentJourney = PlayFabDataManager.Instance.GetJourney();
        }
        else
        {
            currentJourney = "Savings"; // default
        }
    }

    void UpdateJourneyUI()
    {
        // ✅ Update background
        savingsBackground.SetActive(currentJourney == "Savings");
        budgetingBackground.SetActive(currentJourney == "Budgeting");
        investingBackground.SetActive(currentJourney == "Investing");

        // ✅ Update chapter texts
        savingsChapterText.SetActive(currentJourney == "Savings");
        budgetingChapterText.SetActive(currentJourney == "Budgeting");
        investingChapterText.SetActive(currentJourney == "Investing");

        // ✅ Update texts
        journeyDownText.text = currentJourney;
        journeyUpSelectedText.text = currentJourney;
    }

    void ToggleJourneyUpPanel()
    {
        journeyUpPanel.SetActive(!journeyUpPanel.activeSelf);

        if (journeyUpPanel.activeSelf)
        {
            // ✅ Show current journey in JourneyUp
            journeyUpSelectedText.text = currentJourney;

            // ✅ Update unselected journeys correctly
            string[] allJourneys = { "Savings", "Budgeting", "Investing" };
            int unselectedIndex = 0;

            for (int i = 0; i < allJourneys.Length; i++)
            {
                if (allJourneys[i] == currentJourney)
                    continue; // skip current journey

                if (unselectedIndex < unselectedJourneyTexts.Length)
                {
                    unselectedJourneyTexts[unselectedIndex].text = allJourneys[i];
                    // Only allow interactable if unlocked (for now always locked)
                    unselectedJourneyButtons[unselectedIndex].interactable = false; 
                    unselectedIndex++;
                }
            }
        }
    }

    string GetJourneyNameByIndex(int index)
    {
        switch (index)
        {
            case 0: return "Savings";
            case 1: return "Budgeting";
            case 2: return "Investing";
            default: return "";
        }
    }

    void TrySelectJourney(int index)
    {
        string selected = GetJourneyNameByIndex(index);

        if (selected == currentJourney)
        {
            journeyUpPanel.SetActive(false);
            return;
        }

        // For now, always locked
        ShowPopup("This journey is locked! Finish the first mission to unlock.");

        // Close panel after click
        journeyUpPanel.SetActive(false);
    }

    void GoToMission(int missionNumber)
    {
        Button selectedButton = null;
        switch (missionNumber)
        {
            case 1: selectedButton = mission1Button; break;
            case 2: selectedButton = mission2Button; break;
            case 3: selectedButton = mission3Button; break;
            case 4: selectedButton = mission4Button; break;
            case 5: selectedButton = mission5Button; break;
        }

        // Only Savings chapters load
        if (currentJourney != "Savings")
        {
            ShowPopup("Coming soon!");
            return;
        }

        if (selectedButton != null && selectedButton.interactable)
        {
            // NEW: Mission 1 first goes to StorySetupScene
            if (missionNumber == 1)
            {
                SceneManager.LoadScene("StorySetupScene"); // Load story setup first
            }
            else
            {
                string sceneName = $"SavingsC1M{missionNumber}";
                SceneManager.LoadScene(sceneName);
            }
        }
        else
        {
            ShowPopup("Complete the previous mission first!");
        }
    }

    void ShowPopup(string message)
    {
        popupPanel.SetActive(true);
        popupText.text = message;
        CancelInvoke(nameof(HidePopup));
        Invoke(nameof(HidePopup), 2f);
    }

    void HidePopup()
    {
        popupPanel.SetActive(false);
    }

    // COINS SYSTEM

    void LoadPlayerCoins()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            bool isNew = false;

            // Load gold coins
            if (result.Data.ContainsKey("GoldCoins"))
                goldCoins = int.Parse(result.Data["GoldCoins"].Value);
            else
            {
                goldCoins = 50; // Initial value for new player
                isNew = true;
            }

            // Load silver coins
            if (result.Data.ContainsKey("SilverCoins"))
                silverCoins = int.Parse(result.Data["SilverCoins"].Value);
            else
            {
                silverCoins = 100; // Initial value for new player
                isNew = true;
            }

            UpdateCoinUI(); // Update coin texts

            if (isNew)
                SavePlayerCoins(); // Save initial coins to PlayFab
        }, error =>
        {
            Debug.LogError("Failed to load coins: " + error.GenerateErrorReport());
        });
    }

    void UpdateCoinUI()
    {
        if (goldCoinText != null) goldCoinText.text = goldCoins.ToString();
        if (silverCoinText != null) silverCoinText.text = silverCoins.ToString();
    }

    public void AddCoins(int goldAmount, int silverAmount)
    {
        goldCoins += goldAmount;
        silverCoins += silverAmount;
        UpdateCoinUI();
        SavePlayerCoins();
    }

    void SavePlayerCoins()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { "GoldCoins", goldCoins.ToString() },
                { "SilverCoins", silverCoins.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log("✅ Coins saved successfully!");
        }, error =>
        {
            Debug.LogError("❌ Failed to save coins: " + error.GenerateErrorReport());
        });
    }
}

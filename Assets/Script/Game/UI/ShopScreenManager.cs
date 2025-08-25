// ShopScreenManager.cs

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class ShopScreenManager : MonoBehaviour
{
    [Header("Character Display")]
    public CharacterDisplay display;

    [Header("Coins Display")]
    public TMP_Text goldCoinText;
    public TMP_Text silverCoinText;

    [Header("Category Panels")]
    public GameObject femaleCategoryPanel; // Parent for all female category buttons
    public GameObject maleCategoryPanel;   // Parent for all male category buttons

    [Header("Shop Holders (Panels for items)")]
    public GameObject hairHolder;
    public GameObject eyesHolder;
    public GameObject pantsHolder;
    public GameObject topHolder;

    [Header("Category Buttons - Female")]
    public Button femaleHairButton;
    public Button femaleEyesButton;
    public Button femaleTopButton;
    public Button femalePantsButton;

    [Header("Category Buttons - Male")]
    public Button maleHairButton;
    public Button maleEyesButton;
    public Button maleTopButton;
    public Button malePantsButton;

    private int goldCoins;
    private int silverCoins;

    private void OnEnable()
    {
        LoadCharacterDisplay();
        LoadCoinsFromPlayFab();
        SetupCategoryButtons();

        // Always open Hair Shop by default
        ShowShopPanel(hairHolder);
    }

    private void LoadCharacterDisplay()
    {
        CharacterManager.Instance.LoadCharacterDataFromPlayFab(() =>
        {
            if (display != null)
                display.UpdateDisplay(CharacterManager.Instance.GetCharacter());

            UpdateCategoryButtons(); // Activate correct gender category buttons
        });
    }

    private void UpdateCategoryButtons()
    {
        if (CharacterManager.Instance == null) return;

        var character = CharacterManager.Instance.GetCharacter();
        if (character == null) return;

        // Show the correct category panel based on saved gender
        if (character.gender == CharacterData.Gender.Female)
        {
            if (femaleCategoryPanel != null) femaleCategoryPanel.SetActive(true);
            if (maleCategoryPanel != null) maleCategoryPanel.SetActive(false);
        }
        else
        {
            if (femaleCategoryPanel != null) femaleCategoryPanel.SetActive(false);
            if (maleCategoryPanel != null) maleCategoryPanel.SetActive(true);
        }
    }

    private void LoadCoinsFromPlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null)
            {
                goldCoins = result.Data.ContainsKey("GoldCoins") ? int.Parse(result.Data["GoldCoins"].Value) : 0;
                silverCoins = result.Data.ContainsKey("SilverCoins") ? int.Parse(result.Data["SilverCoins"].Value) : 0;
            }
            else
            {
                goldCoins = 0;
                silverCoins = 0;
            }

            UpdateCoinsUI();
        },
        error =>
        {
            Debug.LogWarning("Failed to load coins from PlayFab: " + error.GenerateErrorReport());
            goldCoins = 0;
            silverCoins = 0;
            UpdateCoinsUI();
        });
    }

    private void UpdateCoinsUI()
    {
        if (goldCoinText != null) goldCoinText.text = goldCoins.ToString();
        if (silverCoinText != null) silverCoinText.text = silverCoins.ToString();
    }

    public void SaveCoinsToPlayFab()
    {
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { "GoldCoins", goldCoins.ToString() },
                { "SilverCoins", silverCoins.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("Coins saved to PlayFab."),
            error => Debug.LogWarning("Failed to save coins to PlayFab: " + error.GenerateErrorReport())
        );
    }

    public void AddCoins(int goldToAdd, int silverToAdd)
    {
        goldCoins += goldToAdd;
        silverCoins += silverToAdd;
        UpdateCoinsUI();
        SaveCoinsToPlayFab();
    }

    public bool SpendCoins(int goldToSpend, int silverToSpend)
    {
        if (goldCoins >= goldToSpend && silverCoins >= silverToSpend)
        {
            goldCoins -= goldToSpend;
            silverCoins -= silverToSpend;
            UpdateCoinsUI();
            SaveCoinsToPlayFab();
            return true;
        }
        return false;
    }

    // CATEGORY BUTTON LOGIC 
    private void SetupCategoryButtons()
    {
        // Female buttons
        if (femaleHairButton != null) femaleHairButton.onClick.AddListener(() => ShowShopPanel(hairHolder));
        if (femaleEyesButton != null) femaleEyesButton.onClick.AddListener(() => ShowShopPanel(eyesHolder));
        if (femaleTopButton != null) femaleTopButton.onClick.AddListener(() => ShowShopPanel(topHolder));
        if (femalePantsButton != null) femalePantsButton.onClick.AddListener(() => ShowShopPanel(pantsHolder));

        // Male buttons
        if (maleHairButton != null) maleHairButton.onClick.AddListener(() => ShowShopPanel(hairHolder));
        if (maleEyesButton != null) maleEyesButton.onClick.AddListener(() => ShowShopPanel(eyesHolder));
        if (maleTopButton != null) maleTopButton.onClick.AddListener(() => ShowShopPanel(topHolder));
        if (malePantsButton != null) malePantsButton.onClick.AddListener(() => ShowShopPanel(pantsHolder));
    }

    private void ShowShopPanel(GameObject panelToShow)
    {
        if (hairHolder != null) hairHolder.SetActive(false);
        if (eyesHolder != null) eyesHolder.SetActive(false);
        if (pantsHolder != null) pantsHolder.SetActive(false);
        if (topHolder != null) topHolder.SetActive(false);

        if (panelToShow != null) panelToShow.SetActive(true);
    }
}

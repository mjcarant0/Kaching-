using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System;

public class AccountCharacterScreen : MonoBehaviour
{
    [Header("Character Display")]
    public CharacterDisplay display;

    [Header("User Display UI")]
    public GameObject userDisplay;
    public TMP_Text nameText;
    public TMP_Text usernameText;

    [Header("Category Buttons")]
    public GameObject femaleCategoryPanel;
    public GameObject maleCategoryPanel;

    [Header("Inventory Item Holders")]
    public GameObject hairHolder;
    public GameObject eyesHolder;
    public GameObject topHolder;
    public GameObject pantsHolder;

    [Header("Save Button")]
    public Button saveButton;

    [Header("Item Sprites (Female)")]
    public GameObject femaleHairSprite;
    public GameObject femaleEyesSprite;
    public GameObject femaleTopSprite;
    public GameObject femalePantsSprite;

    [Header("Item Sprites (Male)")]
    public GameObject maleHairSprite;
    public GameObject maleEyesSprite;
    public GameObject maleTopSprite;
    public GameObject malePantsSprite;

    private string currentCategory = "Hair";
    private InventoryManager inventoryManager;

    private void OnEnable()
    {
        if (saveButton != null)
            saveButton.interactable = false;

        CharacterManager.Instance.LoadCharacterDataFromPlayFab(() =>
        {
            var character = CharacterManager.Instance.GetCharacter();

            // Ensure default items reflect stored default colors
            if (character.hair == "DefaultHair" && character.ownedHair.Count > 0)
                character.hair = character.ownedHair[0];
            if (character.eyes == "DefaultEyes" && character.ownedEyes.Count > 0)
                character.eyes = character.ownedEyes[0];
            if (character.top == "DefaultTop" && character.ownedTop.Count > 0)
                character.top = character.ownedTop[0];
            if (character.pants == "DefaultPants" && character.ownedPants.Count > 0)
                character.pants = character.ownedPants[0];

            if (display != null)
                display.UpdateDisplay(character);

            UpdateCategoryButtons();

            inventoryManager = GetComponent<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.SetCharacter(character);

                // ✅ Safe event subscription (if implemented in InventoryManager)
                inventoryManager.onItemChanged += OnItemChanged;
            }

            // ✅ Activate the correct item sprites for Button1 children
            UpdateItemSprites(character.gender);
        });

        LoadUserDisplay();
        ShowCategory("Hair"); // load hair first
    }

    private void OnDisable()
    {
        if (inventoryManager != null)
            inventoryManager.onItemChanged -= OnItemChanged;
    }

    private void OnItemChanged()
    {
        if (saveButton != null)
            saveButton.interactable = true;

        if (display != null)
            display.UpdateDisplay(CharacterManager.Instance.GetCharacter());
    }

    public void OnSaveButtonClicked()
    {
        if (CharacterManager.Instance != null)
        {
            CharacterManager.Instance.SaveCharacterDataToPlayFab(() =>
            {
                Debug.Log("Character saved!");
                if (saveButton != null)
                    saveButton.interactable = false;
            });
        }

        if (display != null)
            display.UpdateDisplay(CharacterManager.Instance.GetCharacter());
    }

    private void LoadUserDisplay()
    {
        if (userDisplay != null)
            userDisplay.SetActive(true);

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
        {
            var accountInfo = result.AccountInfo;

            if (nameText != null)
                nameText.text = !string.IsNullOrEmpty(accountInfo.TitleInfo.DisplayName)
                                ? accountInfo.TitleInfo.DisplayName
                                : "Player";

            if (usernameText != null)
                usernameText.text = !string.IsNullOrEmpty(accountInfo.Username)
                                    ? accountInfo.Username
                                    : "User";

        }, error =>
        {
            Debug.LogWarning("Failed to load account info: " + error.GenerateErrorReport());

            if (nameText != null)
                nameText.text = "Player";

            if (usernameText != null)
                usernameText.text = "User";
        });
    }

    private void UpdateCategoryButtons()
    {
        if (CharacterManager.Instance == null) return;

        var character = CharacterManager.Instance.GetCharacter();
        if (character == null) return;

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

    public void ShowCategory(string category)
    {
        currentCategory = category;

        if (hairHolder != null) hairHolder.SetActive(category == "Hair");
        if (eyesHolder != null) eyesHolder.SetActive(category == "Eyes");
        if (topHolder != null) topHolder.SetActive(category == "Top");
        if (pantsHolder != null) pantsHolder.SetActive(category == "Pants");

        if (saveButton != null)
            saveButton.interactable = true;

        if (inventoryManager != null)
        {
            inventoryManager.PopulateInventoryUI(category);
        }
    }

    // ✅ Activate correct child item sprites based on gender
    private void UpdateItemSprites(CharacterData.Gender gender)
    {
        bool isFemale = gender == CharacterData.Gender.Female;

        if (femaleHairSprite != null) femaleHairSprite.SetActive(isFemale);
        if (femaleEyesSprite != null) femaleEyesSprite.SetActive(isFemale);
        if (femaleTopSprite != null) femaleTopSprite.SetActive(isFemale);
        if (femalePantsSprite != null) femalePantsSprite.SetActive(isFemale);

        if (maleHairSprite != null) maleHairSprite.SetActive(!isFemale);
        if (maleEyesSprite != null) maleEyesSprite.SetActive(!isFemale);
        if (maleTopSprite != null) maleTopSprite.SetActive(!isFemale);
        if (malePantsSprite != null) malePantsSprite.SetActive(!isFemale);
    }

    public void OnHairCategoryButtonClicked() => ShowCategory("Hair");
    public void OnEyesCategoryButtonClicked() => ShowCategory("Eyes");
    public void OnTopCategoryButtonClicked() => ShowCategory("Top");
    public void OnPantsCategoryButtonClicked() => ShowCategory("Pants");
}

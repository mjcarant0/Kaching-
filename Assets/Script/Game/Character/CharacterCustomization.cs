// CharacterCustomization.cs
// Controls the Character Customization screen UI (gender, skin, hair, eyes, top, pants).
// Applies selected customization to preview sprite and saves choice to PlayFab.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class CharacterCustomization : MonoBehaviour
{
    [Header("Gender Sprites")]
    public GameObject femaleSpriteRoot;
    public GameObject maleSpriteRoot;

    [Header("Sprite Renderers (Female)")]
    public SpriteRenderer femaleSkin;
    public SpriteRenderer femaleHair;
    public SpriteRenderer femaleEyes;
    public SpriteRenderer femaleTop;
    public SpriteRenderer femalePants;

    [Header("Sprite Renderers (Male)")]
    public SpriteRenderer maleSkin;
    public SpriteRenderer maleHair;
    public SpriteRenderer maleEyes;
    public SpriteRenderer maleTop;
    public SpriteRenderer malePants;

    [Header("Gender Buttons")]
    public Button femaleButton;
    public Button maleButton;

    [System.Serializable]
    public class ColorButton
    {
        public Button button;
        public Color assignedColor;
    }

    [Header("Skin Color Buttons")]
    public ColorButton[] skinColorButtons;
    [Header("Hair Color Buttons")]
    public ColorButton[] hairColorButtons;
    [Header("Eye Color Buttons")]
    public ColorButton[] eyeColorButtons;
    [Header("Top Color Buttons")]
    public ColorButton[] topColorButtons;
    [Header("Pants Color Buttons")]
    public ColorButton[] pantsColorButtons;

    [Header("Navigation Buttons")]
    public Button confirmButton;
    public Button backButton;

    private CharacterData tempData;
    private CharacterData originalData;

    private void Start()
    {
        // Create default character
        tempData = new CharacterData
        {
            gender = CharacterData.Gender.Female,
            skinColor = HexToColor("E5CAB4"),
            hairColor = HexToColor("393B39"),
            eyeColor = HexToColor("474646"),
            topColor = HexToColor("FFFFFF"),
            pantsColor = HexToColor("3F3F3F")
        };

        originalData = new CharacterData(tempData);

        // Gender buttons
        femaleButton.onClick.AddListener(() => SwitchGender(CharacterData.Gender.Female));
        maleButton.onClick.AddListener(() => SwitchGender(CharacterData.Gender.Male));

        // Assign color buttons
        AssignColorButtons(skinColorButtons, "skin");
        AssignColorButtons(hairColorButtons, "hair");
        AssignColorButtons(eyeColorButtons, "eye");
        AssignColorButtons(topColorButtons, "top");
        AssignColorButtons(pantsColorButtons, "pants");

        confirmButton.onClick.AddListener(ConfirmSelection);
        backButton.onClick.AddListener(BackToMenu);

        ApplyPreview();
    }

    private void AssignColorButtons(ColorButton[] buttons, string part)
    {
        foreach (var cb in buttons)
        {
            Color btnColor = cb.assignedColor;
            btnColor.a = 1f;
            cb.button.onClick.AddListener(() => ChangeColor(part, btnColor));
        }
    }

    private void SwitchGender(CharacterData.Gender gender)
    {
        tempData.gender = gender;
        ApplyPreview();
    }

    public void ApplyPreview()
    {
        // Toggle gender roots
        femaleSpriteRoot.SetActive(tempData.gender == CharacterData.Gender.Female);
        maleSpriteRoot.SetActive(tempData.gender == CharacterData.Gender.Male);

        // Both genders share colors
        femaleSkin.color = tempData.skinColor;
        femaleHair.color = tempData.hairColor;
        femaleEyes.color = tempData.eyeColor;
        femaleTop.color = tempData.topColor;
        femalePants.color = tempData.pantsColor;

        maleSkin.color = tempData.skinColor;
        maleHair.color = tempData.hairColor;
        maleEyes.color = tempData.eyeColor;
        maleTop.color = tempData.topColor;
        malePants.color = tempData.pantsColor;
    }

    private void ChangeColor(string part, Color newColor)
    {
        newColor.a = 1f;

        switch (part)
        {
            case "skin": tempData.skinColor = newColor; break;
            case "hair": tempData.hairColor = newColor; break;
            case "eye": tempData.eyeColor = newColor; break;
            case "top": tempData.topColor = newColor; break;
            case "pants": tempData.pantsColor = newColor; break;
        }
        ApplyPreview();
    }

    private void ConfirmSelection()
    {
        CharacterManager.Instance.SetCharacter(tempData);

        // 1️⃣ Save main equipped items
        var mainItems = new Dictionary<string, string>()
        {
            { "Gender", tempData.gender.ToString() },
            { "Skin", tempData.skin },
            { "Hair", tempData.hair },
            { "Eyes", tempData.eyes },
            { "Top", tempData.top },
            { "Pants", tempData.pants }
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest { Data = mainItems },
            result1 =>
            {
                Debug.Log("✅ Equipped items saved (main).");

                // 2️⃣ Save default colors & tint colors
                var colors = new Dictionary<string, string>()
                {
                    { "DefaultSkinColor", ColorUtility.ToHtmlStringRGB(tempData.skinColor) },
                    { "DefaultHairColor", ColorUtility.ToHtmlStringRGB(tempData.hairColor) },
                    { "DefaultEyesColor", ColorUtility.ToHtmlStringRGB(tempData.eyeColor) },
                    { "DefaultTopColor", ColorUtility.ToHtmlStringRGB(tempData.topColor) },
                    { "DefaultPantsColor", ColorUtility.ToHtmlStringRGB(tempData.pantsColor) },
                    { "StoredHairColor", "FFFFFF" },
                    { "StoredEyesColor", "FFFFFF" },
                    { "StoredTopColor", "FFFFFF" },
                    { "StoredPantsColor", "FFFFFF" }
                };

                PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest { Data = colors },
                    result2 =>
                    {
                        Debug.Log("✅ Colors saved.");

                        // 3️⃣ Save inventory items
                        var inventory = new Dictionary<string, string>()
                        {
                            { "InventoryHair", tempData.ownedHair != null && tempData.ownedHair.Count > 0 ? string.Join(",", tempData.ownedHair) : "DefaultHair" },
                            { "InventoryEyes", tempData.ownedEyes != null && tempData.ownedEyes.Count > 0 ? string.Join(",", tempData.ownedEyes) : "DefaultEyes" },
                            { "InventoryTop", tempData.ownedTop != null && tempData.ownedTop.Count > 0 ? string.Join(",", tempData.ownedTop) : "DefaultTop" },
                            { "InventoryPants", tempData.ownedPants != null && tempData.ownedPants.Count > 0 ? string.Join(",", tempData.ownedPants) : "DefaultPants" }
                        };

                        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest { Data = inventory },
                            result3 =>
                            {
                                Debug.Log("✅ Inventory items saved.");
                                SceneManager.LoadScene("FinancialJourneyScreen");
                            },
                            error3 => Debug.LogError("❌ Error saving inventory: " + error3.GenerateErrorReport())
                        );
                    },
                    error2 => Debug.LogError("❌ Error saving colors: " + error2.GenerateErrorReport())
                );
            },
            error1 => Debug.LogError("❌ Error saving main items: " + error1.GenerateErrorReport())
        );
    }

    private void BackToMenu()
    {
        Debug.Log("Back pressed - no changes saved");
        SceneManager.LoadScene("MenuScreen");
    }

    private Color HexToColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color col);
        return col;
    }

    // NEW: allow AccountCharacterScreen to set tempData
    public void SetTempData(CharacterData data)
    {
        tempData = new CharacterData
        {
            gender = data.gender,
            skinColor = data.skinColor,
            hairColor = data.hairColor,
            eyeColor = data.eyeColor,
            topColor = data.topColor,
            pantsColor = data.pantsColor
        };
        originalData = new CharacterData(tempData);
    }
}

// CharacterManager.cs
// Manages the player’s character across all scenes.
// Ensures persistence (DontDestroyOnLoad) and provides access to current character data.

using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }
    public CharacterData characterData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (characterData == null) characterData = new CharacterData();
        }
        else Destroy(gameObject);
    }

    #region SAVE & LOAD WITH PLAYFAB

    public void SaveCharacterDataToPlayFab(System.Action onSuccess = null)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Gender", characterData.gender.ToString() },
                { "Skin", characterData.skin },
                { "Hair", characterData.hair },
                { "Eyes", characterData.eyes },
                { "Top", characterData.top },
                { "Pants", characterData.pants },
                { "SkinColor", ColorUtility.ToHtmlStringRGBA(characterData.skinColor) },
                { "HairColor", ColorUtility.ToHtmlStringRGBA(characterData.hairColor) },
                { "EyeColor", ColorUtility.ToHtmlStringRGBA(characterData.eyeColor) },
                { "TopColor", ColorUtility.ToHtmlStringRGBA(characterData.topColor) },
                { "PantsColor", ColorUtility.ToHtmlStringRGBA(characterData.pantsColor) },
                { "DefaultHair", characterData.ownedHair.Count > 0 ? characterData.ownedHair[0] : "DefaultHair" },
                { "DefaultEyes", characterData.ownedEyes.Count > 0 ? characterData.ownedEyes[0] : "DefaultEyes" },
                { "DefaultTop", characterData.ownedTop.Count > 0 ? characterData.ownedTop[0] : "DefaultTop" },
                { "DefaultPants", characterData.ownedPants.Count > 0 ? characterData.ownedPants[0] : "DefaultPants" },
                { "Name", characterData.name },           
                { "Username", characterData.username }    
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log("✅ Character data saved to PlayFab!");
                onSuccess?.Invoke(); // ✅ Call the callback if provided
            },
            error => Debug.LogError("❌ Error saving character data: " + error.GenerateErrorReport())
        );
    }

    public void LoadCharacterDataFromPlayFab(System.Action onLoaded = null)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                var data = result.Data;

                if (data != null && data.Count > 0)
                {
                    characterData.gender = data.ContainsKey("Gender") 
                        ? (CharacterData.Gender)System.Enum.Parse(typeof(CharacterData.Gender), data["Gender"].Value)
                        : CharacterData.Gender.Female;

                    characterData.skin = data.ContainsKey("Skin") ? data["Skin"].Value : "Default";
                    characterData.hair = data.ContainsKey("Hair") ? data["Hair"].Value : "Default";
                    characterData.eyes = data.ContainsKey("Eyes") ? data["Eyes"].Value : "Default";
                    characterData.top = data.ContainsKey("Top") ? data["Top"].Value : "Default";
                    characterData.pants = data.ContainsKey("Pants") ? data["Pants"].Value : "Default";

                    // Load equipped colors
                    characterData.skinColor = data.ContainsKey("SkinColor") ? ParseColor(data["SkinColor"].Value) : characterData.skinColor;
                    characterData.hairColor = data.ContainsKey("HairColor") ? ParseColor(data["HairColor"].Value) : characterData.hairColor;
                    characterData.eyeColor = data.ContainsKey("EyeColor") ? ParseColor(data["EyeColor"].Value) : characterData.eyeColor;
                    characterData.topColor = data.ContainsKey("TopColor") ? ParseColor(data["TopColor"].Value) : characterData.topColor;
                    characterData.pantsColor = data.ContainsKey("PantsColor") ? ParseColor(data["PantsColor"].Value) : characterData.pantsColor;

                    // --- Load default items from PlayFab ---
                    if (data.ContainsKey("DefaultHair") && characterData.ownedHair.Count > 0)
                        characterData.ownedHair[0] = data["DefaultHair"].Value;
                    if (data.ContainsKey("DefaultEyes") && characterData.ownedEyes.Count > 0)
                        characterData.ownedEyes[0] = data["DefaultEyes"].Value;
                    if (data.ContainsKey("DefaultTop") && characterData.ownedTop.Count > 0)
                        characterData.ownedTop[0] = data["DefaultTop"].Value;
                    if (data.ContainsKey("DefaultPants") && characterData.ownedPants.Count > 0)
                        characterData.ownedPants[0] = data["DefaultPants"].Value;

                    // --- Apply stored default colors if equipped item is default ---
                    if (characterData.skin == "DefaultSkin")
                        characterData.skinColor = data.ContainsKey("DefaultSkinColor") ? ParseColor(data["DefaultSkinColor"].Value) : characterData.skinColor;

                    if (characterData.hair == characterData.ownedHair[0] && characterData.hair.StartsWith("Default"))
                        characterData.hairColor = data.ContainsKey("DefaultHairColor") ? ParseColor(data["DefaultHairColor"].Value) : characterData.hairColor;

                    if (characterData.eyes == characterData.ownedEyes[0] && characterData.eyes.StartsWith("Default"))
                        characterData.eyeColor = data.ContainsKey("DefaultEyesColor") ? ParseColor(data["DefaultEyesColor"].Value) : characterData.eyeColor;

                    if (characterData.top == characterData.ownedTop[0] && characterData.top.StartsWith("Default"))
                        characterData.topColor = data.ContainsKey("DefaultTopColor") ? ParseColor(data["DefaultTopColor"].Value) : characterData.topColor;

                    if (characterData.pants == characterData.ownedPants[0] && characterData.pants.StartsWith("Default"))
                        characterData.pantsColor = data.ContainsKey("DefaultPantsColor") ? ParseColor(data["DefaultPantsColor"].Value) : characterData.pantsColor;
                    // --------------------------------------------------------------

                    characterData.name = data.ContainsKey("Name") ? data["Name"].Value : "Player";
                    characterData.username = data.ContainsKey("Username") ? data["Username"].Value : "User";

                    Debug.Log("✅ Character data loaded from PlayFab!");
                }
                else
                {
                    Debug.Log("ℹ️ No saved character data found. Using defaults.");
                    if (characterData == null) characterData = new CharacterData();
                }

                onLoaded?.Invoke();
            },
            error =>
            {
                Debug.LogError("❌ Error loading character data: " + error.GenerateErrorReport());
                if (characterData == null) characterData = new CharacterData();
                onLoaded?.Invoke();
            });
    }

    #endregion

    // Getter and setter for CharacterData
    public CharacterData GetCharacter() => characterData;

    public void SetCharacter(CharacterData data)
    {
        characterData = new CharacterData
        {
            gender = data.gender,
            skinColor = data.skinColor,
            hairColor = data.hairColor,
            eyeColor = data.eyeColor,
            topColor = data.topColor,
            pantsColor = data.pantsColor,
            name = data.name,
            username = data.username,
            ownedHair = data.ownedHair,
            ownedEyes = data.ownedEyes,
            ownedTop = data.ownedTop,
            ownedPants = data.ownedPants
        };
    }

    // Helper method to parse HTML color string safely
    private Color ParseColor(string html)
    {
        if (ColorUtility.TryParseHtmlString("#" + html, out var c))
            return c;
        return Color.white;
    }

    // -----------------------------
    // NEW: Notify subscribers when character updates
    // -----------------------------
    public void NotifyCharacterUpdated()
    {
        characterData?.TriggerUpdate();
    }
}

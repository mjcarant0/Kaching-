// PlayFabDataManager.cs
// Central wrapper for saving and retrieving player data from PlayFab.
// Handles both Character and Journey data persistence.

using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

public class PlayFabDataManager : MonoBehaviour
{
    public static PlayFabDataManager Instance;

    private CharacterData character;
    private JourneyData journey;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ----- CHARACTER ----- //
    public bool HasCharacter() => character != null;

    public void SaveCharacter(CharacterData data, Action onSuccess = null)
    {
        character = data;

        // ✅ Equipped items
        var equippedItems = new Dictionary<string, string>
        {
            { "Skin", string.IsNullOrEmpty(data.skin) ? "" : data.skin },
            { "Hair", string.IsNullOrEmpty(data.hair) ? "" : data.hair },
            { "Eyes", string.IsNullOrEmpty(data.eyes) ? "" : data.eyes },
            { "Top", string.IsNullOrEmpty(data.top) ? "" : data.top },
            { "Pants", string.IsNullOrEmpty(data.pants) ? "" : data.pants }
        };

        // ✅ Default colors
        equippedItems["DefaultSkinColor"] = string.IsNullOrEmpty(data.skin) ? "" : ColorUtility.ToHtmlStringRGBA(data.skinColor);
        equippedItems["DefaultHairColor"] = string.IsNullOrEmpty(data.hair) ? "" : ColorUtility.ToHtmlStringRGBA(data.hairColor);
        equippedItems["DefaultEyesColor"] = string.IsNullOrEmpty(data.eyes) ? "" : ColorUtility.ToHtmlStringRGBA(data.eyeColor);
        equippedItems["DefaultTopColor"] = string.IsNullOrEmpty(data.top) ? "" : ColorUtility.ToHtmlStringRGBA(data.topColor);
        equippedItems["DefaultPantsColor"] = string.IsNullOrEmpty(data.pants) ? "" : ColorUtility.ToHtmlStringRGBA(data.pantsColor);

        // ✅ Stored shop item tints (white by default)
        equippedItems["StoredHairColor"] = "FFFFFF";
        equippedItems["StoredEyesColor"] = "FFFFFF";
        equippedItems["StoredTopColor"] = "FFFFFF";
        equippedItems["StoredPantsColor"] = "FFFFFF";

        // ✅ Inventory items
        equippedItems["InventoryHair"] = data.ownedHair != null && data.ownedHair.Count > 0
            ? string.Join(",", data.ownedHair)
            : "";

        equippedItems["InventoryEyes"] = data.ownedEyes != null && data.ownedEyes.Count > 0
            ? string.Join(",", data.ownedEyes)
            : "";

        equippedItems["InventoryTop"] = data.ownedTop != null && data.ownedTop.Count > 0
            ? string.Join(",", data.ownedTop)
            : "";

        equippedItems["InventoryPants"] = data.ownedPants != null && data.ownedPants.Count > 0
            ? string.Join(",", data.ownedPants)
            : "";

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = equippedItems
        },
        result =>
        {
            Debug.Log("✅ Character saved to PlayFab with custom keys.");
            onSuccess?.Invoke();
        },
        error => Debug.LogError("❌ Failed to save character: " + error.GenerateErrorReport()));
    }

    public void LoadCharacter(Action<CharacterData> onLoaded = null)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null && result.Data.Count > 0)
            {
                // Only create character if any key is non-empty
                bool hasAnyCharacterData = false;
                foreach (var key in new string[] { "Skin", "Hair", "Eyes", "Top", "Pants" })
                {
                    if (result.Data.ContainsKey(key) && !string.IsNullOrEmpty(result.Data[key].Value))
                    {
                        hasAnyCharacterData = true;
                        break;
                    }
                }

                if (!hasAnyCharacterData)
                {
                    Debug.Log("⚠️ No character found in PlayFab (empty keys).");
                    character = null;
                    onLoaded?.Invoke(null);
                    return;
                }

                character = new CharacterData
                {
                    gender = result.Data.ContainsKey("Gender") && !string.IsNullOrEmpty(result.Data["Gender"].Value)
                        ? (CharacterData.Gender)Enum.Parse(typeof(CharacterData.Gender), result.Data["Gender"].Value)
                        : CharacterData.Gender.Female,

                    skin = result.Data.ContainsKey("Skin") ? result.Data["Skin"].Value : "DefaultSkin",
                    hair = result.Data.ContainsKey("Hair") ? result.Data["Hair"].Value : "DefaultHair",
                    eyes = result.Data.ContainsKey("Eyes") ? result.Data["Eyes"].Value : "DefaultEyes",
                    top = result.Data.ContainsKey("Top") ? result.Data["Top"].Value : "DefaultTop",
                    pants = result.Data.ContainsKey("Pants") ? result.Data["Pants"].Value : "DefaultPants",

                    // Default colors
                    skinColor = result.Data.ContainsKey("DefaultSkinColor") && !string.IsNullOrEmpty(result.Data["DefaultSkinColor"].Value)
                        ? ParseColor(result.Data["DefaultSkinColor"].Value)
                        : Color.white,
                    hairColor = result.Data.ContainsKey("DefaultHairColor") && !string.IsNullOrEmpty(result.Data["DefaultHairColor"].Value)
                        ? ParseColor(result.Data["DefaultHairColor"].Value)
                        : Color.white,
                    eyeColor = result.Data.ContainsKey("DefaultEyesColor") && !string.IsNullOrEmpty(result.Data["DefaultEyesColor"].Value)
                        ? ParseColor(result.Data["DefaultEyesColor"].Value)
                        : Color.white,
                    topColor = result.Data.ContainsKey("DefaultTopColor") && !string.IsNullOrEmpty(result.Data["DefaultTopColor"].Value)
                        ? ParseColor(result.Data["DefaultTopColor"].Value)
                        : Color.white,
                    pantsColor = result.Data.ContainsKey("DefaultPantsColor") && !string.IsNullOrEmpty(result.Data["DefaultPantsColor"].Value)
                        ? ParseColor(result.Data["DefaultPantsColor"].Value)
                        : Color.white
                };

                Debug.Log("✅ Character loaded from PlayFab with custom keys.");
                onLoaded?.Invoke(character);
            }
            else
            {
                Debug.Log("⚠️ No character found in PlayFab. Using defaults.");
                character = null;
                onLoaded?.Invoke(null);
            }
        },
        error =>
        {
            Debug.LogError("❌ Failed to load character: " + error.GenerateErrorReport());
            character = null;
            onLoaded?.Invoke(null);
        });
    }

    // JOURNEY
    public bool HasJourney() => journey != null && journey.hasJourney;

    public void SaveJourney(JourneyData data, Action onSuccess = null)
    {
        journey = data;

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Journey", data.selectedJourney },
                { "HasJourney", data.hasJourney.ToString() }
            }
        },
        result =>
        {
            Debug.Log("✅ Journey saved to PlayFab.");
            onSuccess?.Invoke();
        },
        error => Debug.LogError("❌ Failed to save journey: " + error.GenerateErrorReport()));
    }

    public void LoadJourney(Action<JourneyData> onLoaded = null)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("Journey"))
            {
                journey = new JourneyData
                {
                    selectedJourney = result.Data["Journey"].Value,
                    hasJourney = result.Data.ContainsKey("HasJourney") && bool.Parse(result.Data["HasJourney"].Value)
                };

                Debug.Log("✅ Journey loaded from PlayFab.");
                onLoaded?.Invoke(journey);
            }
            else
            {
                Debug.Log("⚠️ No journey found in PlayFab.");
                journey = null;
                onLoaded?.Invoke(null);
            }
        },
        error =>
        {
            Debug.LogError("❌ Failed to load journey: " + error.GenerateErrorReport());
            onLoaded?.Invoke(null);
        });
    }

    // Direct accessor for current journey
    public string GetJourney()
    {
        return journey != null ? journey.selectedJourney : null;
    }

    // Utility 
    private Color ParseColor(string html)
    {
        if (ColorUtility.TryParseHtmlString("#" + html, out var c))
            return c;
        return Color.white;
    }
}

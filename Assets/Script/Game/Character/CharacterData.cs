// CharacterData.cs
// Data container for character properties (gender, selected colors, equipped items).
// Used to transfer data between scenes and for saving/loading from PlayFab.

using UnityEngine;
using System;

[System.Serializable]
public class CharacterData
{
    public enum Gender { Female, Male }
    public Gender gender = Gender.Female; // default gender

    // Equipped items (by name, e.g., "Default", "RedShirt", "BlueJeans")
    private string _skin = "DefaultSkin";
    public string skin
    {
        get => _skin;
        set { _skin = value; /* removed OnCharacterUpdated?.Invoke(); */ }
    }

    private string _hair = "DefaultHair";
    public string hair
    {
        get => _hair;
        set { _hair = value; /* removed OnCharacterUpdated?.Invoke(); */ }
    }

    private string _eyes = "DefaultEyes";
    public string eyes
    {
        get => _eyes;
        set { _eyes = value; /* removed OnCharacterUpdated?.Invoke(); */ }
    }

    private string _top = "DefaultTop";
    public string top
    {
        get => _top;
        set { _top = value; /* removed OnCharacterUpdated?.Invoke(); */ }
    }

    private string _pants = "DefaultPants";
    public string pants
    {
        get => _pants;
        set { _pants = value; /* removed OnCharacterUpdated?.Invoke(); */ }
    }

    // Customizable colors (only used if item == "Default")
    private Color _skinColor;
    public Color skinColor
    {
        get => _skinColor;
        set { _skinColor = value; /* removed OnCharacterUpdated?.Invoke(); */ }
    }

    private Color _hairColor;
    public Color hairColor
    {
        get => _hairColor;
        set { _hairColor = value; /* removed OnCharacterUpdated?.Invoke(); */ }
    }

    private Color _eyeColor;
    public Color eyeColor
    {
        get => _eyeColor;
        set { _eyeColor = value; /* removed OnCharacterUpdated?.Invoke(); */ }
    }

    private Color _topColor;
    public Color topColor
    {
        get => _topColor;
        set { _topColor = value; /* removed OnCharacterUpdated?.Invoke(); */ }
    }

    private Color _pantsColor;
    public Color pantsColor
    {
        get => _pantsColor;
        set { _pantsColor = value; /* removed OnCharacterUpdated?.Invoke(); */ }
    }

    // NEW: User display
    public string name = "Player";
    public string username = "User";

    // ----- NEW: Lists to track all owned items (defaults + shop) -----
    public System.Collections.Generic.List<string> ownedHair = new System.Collections.Generic.List<string> { "DefaultHair" };
    public System.Collections.Generic.List<string> ownedEyes = new System.Collections.Generic.List<string> { "DefaultEyes" };
    public System.Collections.Generic.List<string> ownedTop = new System.Collections.Generic.List<string> { "DefaultTop" };
    public System.Collections.Generic.List<string> ownedPants = new System.Collections.Generic.List<string> { "DefaultPants" };

    // Event to notify when character data changes
    public event Action OnCharacterUpdated;

    // Default constructor
    public CharacterData()
    {
        // Updated default colors
        skinColor = HexToColor("E5CAB4");   // new skin color
        hairColor = HexToColor("393B39");   // new hair color
        eyeColor = HexToColor("474646");    // new eye color
        topColor = HexToColor("FFFFFF");    // top color
        pantsColor = HexToColor("3F3F3F");  // pants color
    }

    // Helper method to convert hex to Color
    private Color HexToColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color col);
        return col;
    }

    // COPY constructor
    public CharacterData(CharacterData other)
    {
        gender = other.gender;

        skin = other.skin;
        hair = other.hair;
        eyes = other.eyes;
        top = other.top;
        pants = other.pants;

        skinColor = other.skinColor;
        hairColor = other.hairColor;
        eyeColor = other.eyeColor;
        topColor = other.topColor;
        pantsColor = other.pantsColor;

        // Copy new user fields
        name = other.name;
        username = other.username;

        // Copy owned items
        ownedHair = new System.Collections.Generic.List<string>(other.ownedHair);
        ownedEyes = new System.Collections.Generic.List<string>(other.ownedEyes);
        ownedTop = new System.Collections.Generic.List<string>(other.ownedTop);
        ownedPants = new System.Collections.Generic.List<string>(other.ownedPants);
    }

    // Call this to safely notify subscribers
    public void TriggerUpdate()
    {
        OnCharacterUpdated?.Invoke();
    }

    // Added method to fix CS0070
    public void NotifyUpdated()
    {
        OnCharacterUpdated?.Invoke();
    }
}

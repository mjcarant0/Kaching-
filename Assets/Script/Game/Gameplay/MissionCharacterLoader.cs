using UnityEngine;

public class MissionCharacterLoader : MonoBehaviour
{
    [Header("Character Display in this scene")]
    public CharacterDisplay characterDisplay;

    private void Start()
    {
        // Ensure CharacterManager exists and has data
        if (CharacterManager.Instance != null && CharacterManager.Instance.characterData != null)
        {
            characterDisplay.UpdateDisplay(CharacterManager.Instance.characterData);
            Debug.Log("✅ Character loaded into mission scene.");
        }
        else
        {
            Debug.LogWarning("⚠️ No character data found. Default character will be shown.");
        }
    }
}

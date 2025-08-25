using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private CharacterData character;

    public event Action onItemChanged;

    [Header("Inventory UI Holders")]
    public GameObject hairHolder;
    public GameObject eyesHolder;
    public GameObject topHolder;
    public GameObject pantsHolder;

    public void SetCharacter(CharacterData data)
    {
        character = data;
    }

    // Call this to refresh the UI
    public void PopulateInventoryUI(string category = "Hair")
    {
        if (hairHolder != null) hairHolder.SetActive(category == "Hair");
        if (eyesHolder != null) eyesHolder.SetActive(category == "Eyes");
        if (topHolder != null) topHolder.SetActive(category == "Top");
        if (pantsHolder != null) pantsHolder.SetActive(category == "Pants");

        // Make sure children buttons + gender sprites activate properly
        ActivateChildrenForCategory(category);

        onItemChanged?.Invoke();
    }

    private void ActivateChildrenForCategory(string category)
    {
        GameObject activeHolder = null;

        switch (category)
        {
            case "Hair": activeHolder = hairHolder; break;
            case "Eyes": activeHolder = eyesHolder; break;
            case "Top": activeHolder = topHolder; break;
            case "Pants": activeHolder = pantsHolder; break;
        }

        if (activeHolder == null || character == null) return;

        // Loop through all buttons under this holder
        foreach (Transform button in activeHolder.transform)
        {
            button.gameObject.SetActive(true); // activate button itself

            // Now check children of button (MaleItemSprite, FemaleItemSprite)
            Transform male = button.Find("MaleItemSprite");
            Transform female = button.Find("FemaleItemSprite");

            if (male != null)   male.gameObject.SetActive(character.gender == CharacterData.Gender.Male);
            if (female != null) female.gameObject.SetActive(character.gender == CharacterData.Gender.Female);
        }
    }

    public void EquipItem(string category, string itemName)
    {
        if (character == null) return;

        switch (category)
        {
            case "Hair":  character.hair = itemName; break;
            case "Eyes":  character.eyes = itemName; break;
            case "Top":   character.top = itemName; break;
            case "Pants": character.pants = itemName; break;
        }

        onItemChanged?.Invoke();
    }
}

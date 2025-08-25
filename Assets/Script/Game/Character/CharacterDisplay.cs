using UnityEngine;

public class CharacterDisplay : MonoBehaviour
{
    [Header("Female Character Parts")]
    public SpriteRenderer femaleSkin;
    public SpriteRenderer femaleHair;
    public SpriteRenderer femaleEyes;
    public SpriteRenderer femaleTop;
    public SpriteRenderer femalePants;
    public GameObject femaleCharacter;

    [Header("Male Character Parts")]
    public SpriteRenderer maleSkin;
    public SpriteRenderer maleHair;
    public SpriteRenderer maleEyes;
    public SpriteRenderer maleTop;
    public SpriteRenderer malePants;
    public GameObject maleCharacter;

    // Called to update character display from data
    public void UpdateDisplay(CharacterData data)
    {
        if (data == null)
        {
            Debug.LogWarning("CharacterData is null!");
            return;
        }

        bool isFemale = (data.gender == CharacterData.Gender.Female);

        // Ensure correct parent object is active
        if (femaleCharacter != null) femaleCharacter.SetActive(isFemale);
        if (maleCharacter != null) maleCharacter.SetActive(!isFemale);

        // Apply colors or equipped item sprites
        if (isFemale)
        {
            if (femaleSkin != null) femaleSkin.color = data.skinColor;

            if (femaleHair != null)
            {
                // Use color only if default; otherwise white
                if (data.hair.StartsWith("DefaultHair"))
                    femaleHair.color = data.hairColor;
                else
                    femaleHair.color = Color.white;
            }

            if (femaleEyes != null)
            {
                if (data.eyes.StartsWith("DefaultEyes"))
                    femaleEyes.color = data.eyeColor;
                else
                    femaleEyes.color = Color.white;
            }

            if (femaleTop != null)
            {
                if (data.top.StartsWith("DefaultTop"))
                    femaleTop.color = data.topColor;
                else
                    femaleTop.color = Color.white;
            }

            if (femalePants != null)
            {
                if (data.pants.StartsWith("DefaultPants"))
                    femalePants.color = data.pantsColor;
                else
                    femalePants.color = Color.white;
            }
        }
        else
        {
            if (maleSkin != null) maleSkin.color = data.skinColor;

            if (maleHair != null)
            {
                if (data.hair.StartsWith("DefaultHair"))
                    maleHair.color = data.hairColor;
                else
                    maleHair.color = Color.white;
            }

            if (maleEyes != null)
            {
                if (data.eyes.StartsWith("DefaultEyes"))
                    maleEyes.color = data.eyeColor;
                else
                    maleEyes.color = Color.white;
            }

            if (maleTop != null)
            {
                if (data.top.StartsWith("DefaultTop"))
                    maleTop.color = data.topColor;
                else
                    maleTop.color = Color.white;
            }

            if (malePants != null)
            {
                if (data.pants.StartsWith("DefaultPants"))
                    malePants.color = data.pantsColor;
                else
                    malePants.color = Color.white;
            }
        }
    }
}

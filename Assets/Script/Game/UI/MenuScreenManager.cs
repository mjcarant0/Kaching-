// MenuScreenManager.cs
// Controls the Menu Screen after login or signup.
// Start Button checks character/journey status and routes to the correct screen.

using UnityEngine;

public class MenuScreenManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        Debug.Log("🔄 Start button clicked → Checking PlayFab data...");

        // First load character from PlayFab
        PlayFabDataManager.Instance.LoadCharacter(character =>
        {
            if (character == null)
            {
                Debug.Log("➡️ No character found → Going to CharacterCustomScreen");
                GameManager.Instance.GoToScene("CharacterCustomScreen");
                return;
            }

            // Then check journey
            PlayFabDataManager.Instance.LoadJourney(journey =>
            {
                if (journey == null || !journey.hasJourney)
                {
                    Debug.Log("➡️ Character found, but no journey → Going to FinancialJourneyScreen");
                    GameManager.Instance.GoToScene("FinancialJourneyScreen");
                }
                else
                {
                    Debug.Log("➡️ Character + Journey found → Going to HomeScreen");
                    GameManager.Instance.GoToScene("HomeScreen");
                }
            });
        });
    }
}

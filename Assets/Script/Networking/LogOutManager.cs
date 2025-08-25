// Logs the player out by clearing PlayFab session and returning to login

using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;

public class LogoutManager : MonoBehaviour
{
    public void Logout()
    {
        // Clear PlayFab session
        PlayFabClientAPI.ForgetAllCredentials();

        // Go back to Start/Login screen
        SceneManager.LoadScene("StartScreen");
    }
}

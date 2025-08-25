//PlayFabLogin.cs
// Handles user login using PlayFab (username and password).
// Loads the MenuScreen if login is successful.

using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;


public class PlayFabLogin : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;

    public void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Please fill in all fields.";
            return;
        }

        var request = new LoginWithPlayFabRequest
        {
            Username = username,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        feedbackText.text = "Login successful!";
        SceneManager.LoadScene("MenuScreen"); // Go to next screen
    }

    private void OnLoginError(PlayFabError error)
    {
        feedbackText.text = "Login failed: " + error.ErrorMessage;
    }
}

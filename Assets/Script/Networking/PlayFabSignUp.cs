// PlayFabSignUp.cs
// Handles new user registration with PlayFab.
// Creates a username, password, and display name, then loads the MenuScreen.

using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;


public class PlayFabSignUp : MonoBehaviour
{
    public TMP_InputField usernameInput; // Unique login username
    public TMP_InputField passwordInput; // Password
    public TMP_InputField nameInput;     // Display name
    public TextMeshProUGUI feedbackText;

    public void OnSignUpButtonClicked()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();
        string displayName = nameInput.text.Trim();

        // Check for empty fields
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(displayName))
        {
            feedbackText.text = "Please fill in all fields.";
            return;
        }

        // Username length check
        if (username.Length > 10)
        {
            feedbackText.text = "Username must be 10 characters or fewer.";
            return;
        }

        // Display name length check
        if (displayName.Length > 15)
        {
            feedbackText.text = "Name must be 15 characters or fewer.";
            return;
        }

        // Alphabet-only check for display name
        if (!Regex.IsMatch(displayName, @"^[a-zA-Z]+$"))
        {
            feedbackText.text = "Name must contain letters only.";
            return;
        }

        // Register with PlayFab
        var request = new RegisterPlayFabUserRequest
        {
            Username = username,
            Password = password,
            DisplayName = displayName, // Store name in PlayFab
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterError);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        feedbackText.text = "Signup successful!";
        SceneManager.LoadScene("MenuScreen");
    }

    private void OnRegisterError(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.UsernameNotAvailable)
        {
            feedbackText.text = "Username is already taken.";
        }
        else
        {
            feedbackText.text = "Signup failed: " + error.ErrorMessage;
        }
    }
}

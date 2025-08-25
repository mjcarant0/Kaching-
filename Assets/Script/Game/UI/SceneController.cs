using UnityEngine;
using UnityEngine.SceneManagement;

// Switches between scenes or pages when the buttons are clicked

public class SceneController : MonoBehaviour
{
    // Loads the Info Screen when the Continue Button is clicked

    public void LoadInfoScreen()
    {
        SceneManager.LoadScene("InfoScreen");
    }

    // Loads the Start Screen when the Confirm Button in Info Screen is clicked
    public void LoadStartScreen()
    {
        SceneManager.LoadScene("StartScreen");
    }

    // Loads the Log In screen when the Log In button is clicked
    public void LoadLogInScreen()
    {
        SceneManager.LoadScene("LogInScreen");
    }

    // Loads the Sign Up screen when the Sign Up button is clicked
    public void LoadSignUpScreen()
    {
        SceneManager.LoadScene("SignUpScreen");
    }

    // Loads the Credits screen when the Credits button is clicked
    public void LoadCreditsScreen()
    {
        SceneManager.LoadScene("CreditsScreen");
    }

    // Loads the Menu Screen when the Log in or Sign up is clicked

    public void LoadMenuScreen()
    {
        SceneManager.LoadScene("MenuScreen");
    }

    // Loads the Home screen when the Home icon is clicked

    public void LoadHome()
    {
        SceneManager.LoadScene("HomeScreen");
    }

    // Loads the Finni Ai screen when the Finni Icon is clicked

    public void LoadFinni()
    {
        SceneManager.LoadScene("FinniScreen");
    }

    // Loads the Event screen when the Event icon is clicked

    public void LoadEvent()
    {
        SceneManager.LoadScene("EventScreen");
    }

    // Loads the Store screen when the Store icon is clicked

    public void LoadShop()
    {
        SceneManager.LoadScene("ShopScreen");
    }

    public void LoadAccount()
    {
        SceneManager.LoadScene("AccountScreen");
    }
}
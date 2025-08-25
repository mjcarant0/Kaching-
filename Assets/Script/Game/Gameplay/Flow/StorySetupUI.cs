using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class StorySetupUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text storyText;
    public Button continueButton;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f; // Adjust typing speed

    private string fullStory = "You arrive in Savings Forest with 500 Gold Coins and a rustic cottage. " +
                               "The village Elder, Sage Prudence, greets you at the town square beneath " +
                               "the majestic Great Tree of Stability—but something's wrong. The tree's " +
                               "leaves are yellowing, and the local merchant Rico Spendwell is in distress.";

    private void Start()
    {
        // Hide continue button initially
        continueButton.gameObject.SetActive(false);

        // Start typing the story
        StartCoroutine(TypeStory(fullStory));

        // Hook up button listener
        continueButton.onClick.AddListener(OnContinue);
    }

    private IEnumerator TypeStory(string text)
    {
        storyText.text = "";
        foreach (char c in text)
        {
            storyText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Typing finished, show continue button
        continueButton.gameObject.SetActive(true);
    }

    private void OnContinue()
    {
        // Load your first mission or cutscene
        SceneManager.LoadScene("OpeningCutScene");
    }
}

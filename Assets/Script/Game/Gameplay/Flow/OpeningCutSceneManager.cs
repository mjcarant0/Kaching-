using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // New Input System

public class OpeningCutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    public Image nameHolder;           // background for name
    public Image dialogueHolder;       // background for dialogue
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public GameObject choicePanel;
    public Button choiceYesButton;
    public Button choiceNoButton;
    public GameObject toBeContinuedPanel; // Pop-up panel
    public TMP_Text toBeContinuedText;    // TMP_Text inside the panel (leave empty in Inspector)
    public float toBeContinuedDelay = 2f;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;

    [Header("Character Images")]
    public Image sageImage;
    public Image ricoShadowImage;
    public Image ricoFullImage;

    private List<DialogueLine> dialogueLines = new List<DialogueLine>();
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool skipTyping = false;

    private void Start()
    {
        // Setup dialogue lines
        dialogueLines.Add(new DialogueLine("SAGE PRUDENCE",
            "[Standing beneath the Great Tree, stroking her silver beard]\n" +
            "\"Welcome, young traveler. I sense great potential in you, but look...\" " +
            "[gestures to the tree] \"The Great Tree of Stability grows weaker each day. " +
            "It feeds on the financial wisdom of our villagers.\"", sageImage));

        dialogueLines.Add(new DialogueLine("????",
            "[Running up frantically, coins spilling from his pockets]", ricoShadowImage));

        dialogueLines.Add(new DialogueLine("????",
            "Sage Prudence! My merchant stand is failing! I earn 200 gold per week, " +
            "but somehow I'm always broke! My beautiful garden behind the shop is wilting " +
            "because I can't afford the special fertilizer!", ricoFullImage));

        dialogueLines.Add(new DialogueLine("SAGE PRUDENCE",
            "[Turning to you with knowing eyes]\n" +
            "\"Ah, young one. Rico suffers from the Curse of the Vanishing Coins—his money flows out " +
            "faster than water through a broken dam. Perhaps you could help him... and in doing so, " +
            "learn the ancient art of Money Tracking?\"", sageImage));

        // Start everything hidden
        dialogueHolder.gameObject.SetActive(false);
        nameHolder.gameObject.SetActive(false);
        choicePanel.SetActive(false);
        toBeContinuedPanel.SetActive(false);
        sageImage.gameObject.SetActive(false);
        ricoFullImage.gameObject.SetActive(false);
        ricoShadowImage.gameObject.SetActive(false);

        ShowNextLine();

        // Hook up choice buttons
        choiceYesButton.onClick.AddListener(OnChoiceYes);
        choiceNoButton.onClick.AddListener(OnChoiceNo);
    }

    private void Update()
    {
        // Using the New Input System for mouse click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isTyping)
                skipTyping = true;
            else
                ShowNextLine();
        }
    }

    void ShowNextLine()
    {
        if (currentLineIndex >= dialogueLines.Count)
        {
            // Activate choice panel when all dialogues finished
            choicePanel.SetActive(true);
            return;
        }

        DialogueLine line = dialogueLines[currentLineIndex];

        // Activate dialogue and name holders
        dialogueHolder.gameObject.SetActive(true);
        nameHolder.gameObject.SetActive(true);

        nameText.text = line.speakerName;

        // Show/hide character portraits dynamically
        sageImage.gameObject.SetActive(line.characterImage == sageImage);
        ricoFullImage.gameObject.SetActive(line.characterImage == ricoFullImage);
        ricoShadowImage.gameObject.SetActive(line.characterImage == ricoShadowImage);

        StopAllCoroutines();
        StartCoroutine(TypeDialogue(line.dialogueText));

        currentLineIndex++;
    }

    IEnumerator TypeDialogue(string text)
    {
        dialogueText.text = "";
        isTyping = true;
        skipTyping = false;

        foreach (char c in text)
        {
            if (skipTyping)
            {
                dialogueText.text = text;
                break;
            }
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        skipTyping = false;
    }

    void OnChoiceYes()
    {
        choicePanel.SetActive(false);
        StartCoroutine(ShowToBeContinued());
    }

    void OnChoiceNo()
    {
        // Directly go to HomeScreen without showing "To Be Continued..."
        SceneManager.LoadScene("HomeScreen");
    }

    IEnumerator ShowToBeContinued()
    {
        if (toBeContinuedText != null)
            toBeContinuedText.text = "To Be Continued...";

        toBeContinuedPanel.SetActive(true);
        yield return new WaitForSeconds(toBeContinuedDelay);
        SceneManager.LoadScene("HomeScreen");
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public string dialogueText;
        public Image characterImage;

        public DialogueLine(string speakerName, string dialogueText, Image characterImage)
        {
            this.speakerName = speakerName;
            this.dialogueText = dialogueText;
            this.characterImage = characterImage;
        }
    }
}

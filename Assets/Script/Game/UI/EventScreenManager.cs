// EventScreenManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public Button kachingXBPIEventButton;
    public Button[] otherEventButtons;

    [Header("Popup")]
    public GameObject popupPanel;          // The popup panel
    public TMP_Text popupText;             // The popup text

    void Start()
    {
        // Hide popup at start
        popupPanel.SetActive(false);

        // Assign listeners
        if (kachingXBPIEventButton != null)
            kachingXBPIEventButton.onClick.AddListener(() => ShowPopup("Coming Soon!"));

        if (otherEventButtons != null)
        {
            foreach (Button btn in otherEventButtons)
            {
                if (btn != null)
                    btn.onClick.AddListener(() => ShowPopup("There is no event yet."));
            }
        }
    }

    void ShowPopup(string message)
    {
        popupPanel.SetActive(true);
        popupText.text = message;
        CancelInvoke(nameof(HidePopup));
        Invoke(nameof(HidePopup), 2f); // Auto-close after 2s
    }

    void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}

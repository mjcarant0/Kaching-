using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Panels (inside ItemHolder)")]
    public GameObject hairShop;
    public GameObject eyesShop;
    public GameObject topShop;
    public GameObject pantsShop;

    private GameObject currentActivePanel;

    private void Start()
    {
        // Start with HairShop open (or none, if you prefer)
        ShowPanel(hairShop);
    }

    public void ShowPanel(GameObject panelToShow)
    {
        // Hide current
        if (currentActivePanel != null)
            currentActivePanel.SetActive(false);

        // Show new one
        panelToShow.SetActive(true);
        currentActivePanel = panelToShow;
    }
}

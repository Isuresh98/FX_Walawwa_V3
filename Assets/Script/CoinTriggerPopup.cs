using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinTriggerPopup : MonoBehaviour
{
    public GameObject popupPanel;
    public TMP_Text popupText;

    private bool hasShownPopup = false;

    private void Start()
    {
        // Auto-find if not set manually
        if (popupPanel == null)
            popupPanel = GameObject.Find("popupPanelCoin"); // Replace with your panel name

        if (popupText == null)
            popupText = GameObject.Find("PopupText").GetComponent<TMP_Text>(); // Replace with your text name

        popupPanel.SetActive(false); // Hide at start
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasShownPopup)
        {
            hasShownPopup = true;
            ShowPopup();
        }
    }

    void ShowPopup()
    {
        popupPanel.SetActive(true);
        popupText.text = "ñks;a;= 5la i|yd N+; i;=rd úkdY lsÍug fuu ldish tl;= lrkakæ ";
       // Invoke("HidePopup", 3f);
    }

  public  void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}
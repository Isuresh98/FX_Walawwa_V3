using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ToolHintPopup : MonoBehaviour
{
    public TMP_Text hintText;             // Assign in Inspector
    public GameObject popupPanel;         // Assign in Inspector
    public float hintDuration = 3f;       // Duration to show popup
    public Image ItemImage;               // Assign in Inspector

    private HashSet<string> shownHints = new HashSet<string>();
    public ItemsDatabase itemsDatabase;   // Assign in Inspector

    void Start()
    {
        popupPanel.SetActive(false);
    }

    /// <summary>
    /// Call this when a tool is collected.
    /// </summary>
    /// <param name="toolName">Tool name, e.g. "Hammer"</param>
    public void ShowHintOnce(string toolName)
    {
        if (shownHints.Contains(toolName))
            return;

        shownHints.Add(toolName);

        string message = GetHintMessage(toolName);
        Sprite itemIcon = GetItemIcon(toolName);

        if (!string.IsNullOrEmpty(message))
        {
            ShowHint(message, itemIcon);
        }
    }

    private void ShowHint(string message, Sprite icon)
    {
        popupPanel.SetActive(true);
        hintText.text = message;

        if (ItemImage != null && icon != null)
        {
            ItemImage.sprite = icon;
            ItemImage.enabled = true;
        }

        CancelInvoke(nameof(HideHint));
        Invoke(nameof(HideHint), hintDuration);
    }

    private void HideHint()
    {
        popupPanel.SetActive(false);
    }

    private string GetHintMessage(string toolName)
    {
        switch (toolName)
        {
            case "Hammer":
                return "You collected a Hammer! Use it to break objects.";
            case "Screwdriver":
                return "You collected a Screwdriver! Use it to unlock screws.";
            case "Coin":
                return "You collected a Coin! Destroy the ghost enemy for 5 minutes!";
            case "HeadTorch":
                return "You collected a Head Torch! Use it to see in dark areas.";
            default:
                return "";
        }
    }

    private Sprite GetItemIcon(string toolName)
    {
        if (itemsDatabase == null) return null;

        foreach (var item in itemsDatabase.Items)
        {
            if (item.name == toolName)
            {
                return item.m_itemIcon;
            }
        }

        return null;
    }
}
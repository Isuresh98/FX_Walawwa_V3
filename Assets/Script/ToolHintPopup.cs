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
       // Invoke(nameof(HideHint), hintDuration);
    }

    public void HideHint()
    {
        popupPanel.SetActive(false);
    }

    private string GetHintMessage(string toolName)
    {
        switch (toolName)
        {
            case "Hammer":
                return "Tng ñáhla tl;= lr .; hq;=hsæ jia;+ka leãug th Ndú;d lrkak'";
            case "Screwdriver":
                return "Tng bial=remamq kshkla tl;= lr .; hq;=hsæ bial=remamq w.=¿ weÍug th Ndú;d lrkak'";
            case "Coin":
                return "Tng ldishla tl;= lsÍug wjYHhsæ ñks;a;= 5 la i|yd N+; i;=rd úkdY lrkakæ";
            case "HeadTorch":
                return "Tng fyâ fgdaÉ tlla tl;= lr .ekSug wjYHhsæ w÷re m%foaYj, ne,Sug th Ndú;d lrkak'";
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
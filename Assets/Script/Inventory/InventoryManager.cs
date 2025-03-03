using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<string> inventory = new List<string>();

    public void AddItem(string itemName)
    {
        inventory.Add(itemName);
        Debug.Log(itemName + " added to inventory.");
    }

    public void DisplayInventory()
    {
        Debug.Log("Inventory:");
        foreach (string item in inventory)
        {
            Debug.Log(item);
        }
    }
}
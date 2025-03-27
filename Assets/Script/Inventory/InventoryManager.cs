using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<string> inventory = new List<string>();

    public void AddItem(string itemName)
    {
        inventory.Add(itemName);
#if UNITY_EDITOR

        Debug.Log(itemName + " added to inventory.");

#endif
    }

    public void DisplayInventory()
    {
#if UNITY_EDITOR

        Debug.Log("Inventory:");

#endif
        foreach (string item in inventory)
        {
#if UNITY_EDITOR

            Debug.Log(item);

#endif
        }
    }
}
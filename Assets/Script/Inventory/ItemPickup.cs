using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryManager inventory = FindObjectOfType<InventoryManager>();
            inventory.AddItem(itemName);
            Destroy(gameObject); // Remove the item from the scene
        }
    }
}
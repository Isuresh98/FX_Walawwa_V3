using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;


public class Inventory : MonoBehaviour {

    [Header("Inventory Settings")]
    private GameControll m_gameControll;
    [HideInInspector]
    public ItemsDatabase m_itemsDatabase;
    public Transform m_slotsContent;
    public GameObject m_slotPrefab;
    public List<Slot> m_slots = new List<Slot>();

    [Header("UI Settings")]
    public Sprite m_emptySprite;

    Interact InteractScript;

    // Define the maximum stack limit per item (adjust as needed)
    public int maxSlots = 1;

    private void Start()
    {
        InteractScript = FindObjectOfType<Interact>();
    }



    private void Awake()
    {
        m_gameControll = GetComponent<GameControll>();
        m_itemsDatabase = GetComponent<ItemsDatabase>();
       
    }
    public void DropItem(int itemID)
    {
        print("Drop Item.." + itemID);
        int slotIndex = GetSlotWithSameItem(itemID);

        if (slotIndex != -1)
        {
            Slot slot = m_slots[slotIndex];

            if (slot.m_itemCount > 0)
            {
                int dbID = m_itemsDatabase.GetItemInDatabaseByID(itemID);

                if (dbID != -1)
                {
                    Database itemData = m_itemsDatabase.Items[dbID]; // Correctly reference Database class

                    if (itemData.m_itemPrefab != null) // Ensure the item has a prefab to spawn
                    {
                        // Get drop position in front of the player
                        Vector3 dropPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;

                        // Instantiate the dropped item at the calculated position
                        GameObject droppedItem = Instantiate(itemData.m_itemPrefab, dropPosition, Quaternion.identity);
                        Rigidbody rb = droppedItem.transform.gameObject.GetComponent<Rigidbody>(); // Freezes all movement and rotation
                        rb.constraints = RigidbodyConstraints.None;
                        Debug.Log($"Dropped: {itemData.name}");
                    }
                }

                // Remove one item from the slot
                RemoveItem(itemID, 1);
            }
        }
    }

    public void AddItem (int id, int cnt)
    {
        if (id != 0)
        {

            int same = GetSlotWithSameItem(id);

            if (same != -1)
            {

                m_slots[same].m_itemCount += cnt;
                PrepareSlot(m_slots[same]);

            }
            else
            {

                // Check if the inventory has reached the maximum slot limit
                if (m_slots.Count >= maxSlots)
                {
                
                    Debug.Log("Cannot add item: Inventory is full! No free slots available.");
                    return; // Stop adding
                }
              
                GameObject slt = Instantiate(m_slotPrefab, m_slotsContent);
                Slot newSlot = slt.GetComponent<Slot>();
                newSlot.m_itemID = id;
                newSlot.m_itemCount = cnt;
                m_slots.Add(newSlot);
                PrepareSlot(newSlot);

                if (id == 0) /// if item id == 0 (eyePills id)
                {
                    m_gameControll.AddEyePills(1);
                }

            }
        }else
        {
            if (id == 0) /// if item id == 0 (eyePills id)
            {
                m_gameControll.AddEyePills(1);
            }
        }
    }
    public bool IsInventoryFull()
    {
        return m_slots.Count >= maxSlots;
    }


    public void RemoveItem(int itemID, int removeCount)
    {
        int same = GetSlotWithSameItem(itemID);

        if(same != -1)
        {
            m_slots[same].m_itemCount -= removeCount;
            m_gameControll.ShowTip(itemID,4);

            if(m_slots[same].m_itemCount <= 0)
            {
                Destroy(m_slots[same].gameObject);
                m_slots.RemoveAt(same);
                InteractScript.InteractItemBehavior(0);
            }
            else
            {
                PrepareSlot(m_slots[same]);
            }
        }


    }



    private void PrepareSlot(Slot slot)
    {

        if (slot.m_itemID != -1)
        {
            int dbID = m_itemsDatabase.GetItemInDatabaseByID(slot.m_itemID);
            if (dbID != -1)
            {
                slot.m_icon.sprite = m_itemsDatabase.Items[dbID].m_itemIcon;
                slot.m_countText.text = slot.m_itemCount.ToString();

            }
        }else
        {
            slot.m_itemCount = 0;
            slot.m_countText.text = "";
            slot.m_icon.sprite = m_emptySprite;
        }
    }


    int GetFreeSlot()
    {
        for (int i = 0; i < m_slots.Count; i++)
        {
            if (m_slots[i].m_itemID == -1)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetSlotWithSameItem(int id)
    {
        for (int i = 0; i < m_slots.Count; i++)
        {
            if(m_slots[i].m_itemID == id)
            {
                return i;
            }
        }

        return -1;
    }


}

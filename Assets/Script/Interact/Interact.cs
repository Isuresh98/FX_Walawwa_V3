using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
#if UNITY_EDITOR
using UnityEditorInternal.Profiling.Memory.Experimental; // Only in Editor
#endif

public class Interact : MonoBehaviour {


    [Header("Interact Settings")]
    [HideInInspector]
    public GameControll m_gameController;
    [Tooltip("Distance of ray to interact")]
    public float rayDistance;
    [Tooltip("Layers to interact (default as obstacle)")]
    public LayerMask interactLayers;

    [Tooltip("Tags for interact")]
    public string interactTag;
    public string interactCoinTag;
   

    [Header("UI Settings")]
    private PlayerController player;

    [Header("Pictures Paper Settings")]
    public Transform m_examineTransform;
    public Transform m_paperScreenTransform;
    public Transform m_itemScreenTransform;
    public float m_moveSpeed;
    public float m_paperShowTime;
    public float m_itemShowTime;
    bool m_readPaper;
    bool m_takeItem;
    PicturePaper m_paper;
    Item m_item;
    int m_readState;
    int m_itemTakeState;

    [Header("New UI Settings")]
    public TextMeshProUGUI itemNameText;
    public Button HabdBT;
    private float displayDuration = 0.5f;
    public float timer;
    public Image targetIconhighlighter;

    [Header("UI Item Settings")]
    public Button ItemInteractdBT;
    public Button ItemDropBT;
    public int ItemID;
    private ItemsDatabase m_itemsDatabase;
    private Inventory GetInventory;
    [SerializeField]
  //  private float rayDistance = 3f;

    public LayerMask detectableLayer;

    // Store raycast hits to avoid memory allocation
    private RaycastHit[] hits = new RaycastHit[1];

    // Update rate for mobile optimization
    private float checkInterval = 0.05f;
    private float checkTimer;


    [Header("Door Settings")]
    public string interactDorTag;
    public string interactKeyTag;
    public string interactKeyholTag;
    public string interactScruKeyTag;
    public string interactHammerKeyTag;
    [Header("Timer Use Button Settings")]
    public Button DoorInteractdBT;
    private bool isHolding = false;
    public float holdTime = 0f;
    public float requiredHoldTime = 2f; // Time in seconds to trigger hold action
    public LayerMask interactDoorLayers;
    public Slider TimerForCollect;
    public Sprite[] sprites;
    [Header("Popup Hint Settings")]
   [SerializeField] ToolHintPopup toolHintPopup;
    private void Start()
    {
        m_gameController = FindObjectOfType<GameControll>();
        m_itemsDatabase = FindObjectOfType<ItemsDatabase>();
        GetInventory= FindObjectOfType<Inventory>();
        InteractItemBehavior(0);

        player = m_gameController.player;

        HabdBT.onClick.AddListener(OnHandButtonClicked); // Add this line
        ItemInteractdBT.onClick.AddListener(InteractBTUse); // Add this line
        ItemDropBT.onClick.AddListener(DropItem); // Add this line
        targetIconhighlighter.gameObject.SetActive(false);


    }
    private void OnHandButtonClicked()
    {
        RaycastHit hot;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
#if UNITY_EDITOR

        // Draw the ray for debugging
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

#endif

        if (Physics.Raycast(ray, out hot, rayDistance, interactLayers))
        {
            if (hot.transform.gameObject.tag == interactTag)
            {


#if UNITY_EDITOR
                print("Interactive Object Name: " + hot.transform.name);
#endif


                Rigidbody rb= hot.transform.gameObject.GetComponent<Rigidbody>(); // Freezes all movement and rotation
                rb.constraints = RigidbodyConstraints.FreezeAll;
                CheckRaycastedObject(hot.transform.gameObject, -1);
            }
            else if (hot.transform.gameObject.tag == interactCoinTag)
            {
#if UNITY_EDITOR
                print("Interactive Object Name: " + hot.transform.name);
#endif
                SphereCollider sphereCollider = hot.transform.gameObject.GetComponent<SphereCollider>();
                if (sphereCollider != null)
                {
                    sphereCollider.radius = 0.5f; // Set your desired radius
                }

            }
            else if (hot.transform.gameObject.tag == interactKeyTag)
            {
#if UNITY_EDITOR

                print("Interactive Dor Object Name: " + hot.transform.name);

#endif
                Rigidbody rb = hot.transform.gameObject.GetComponent<Rigidbody>(); // Freezes all movement and rotation
                rb.constraints = RigidbodyConstraints.FreezeAll;
                KeyItem keyItem = hot.transform.gameObject.GetComponent<KeyItem>();
                keyItem.CollectKey();
                CheckRaycastedObject(hot.transform.gameObject, -1);

            }
            else if (hot.transform.gameObject.tag == interactKeyholTag)
            {
#if UNITY_EDITOR

                print("Interactive Dor Hol Object Name: " + hot.transform.name);

#endif
                Keyhole keyhol = hot.transform.gameObject.GetComponent<Keyhole>();

                if (keyhol.locket)
                {
                   

                    keyhol.TryUnlockDoor();

                    itemNameText.gameObject.SetActive(true);
                    itemNameText.text = keyhol.TryUnlockDoor();
                    timer = displayDuration;
                }

            }
          
            


        }
       if (Physics.Raycast(ray, out hot, rayDistance, interactDoorLayers))
        {
            if (hot.transform.gameObject.tag == interactDorTag)
            {
#if UNITY_EDITOR

                print("Interactive Dor Object Name: " + hot.transform.name);

#endif
                DoorSystem doorSystem = hot.transform.gameObject.GetComponent<DoorSystem>();
                doorSystem.ToggleDoor();


            }

        }
    }



    public void OnHandButtonDoorClicked()
    {
#if UNITY_EDITOR

        Debug.Log(" Button Clicked!");

#endif
        isHolding = true;
            holdTime = 0f;
        
      
          
     
    }

    public void OnHandButtonDoorReleased()
    {
       
            if (isHolding)
            {
                if (holdTime < requiredHoldTime)
            {
#if UNITY_EDITOR

                Debug.Log("Hold Canceled. Button Released Too Early!");

#endif
            }

            isHolding = false; // Stop the hold process
                holdTime = 0f; // Reset the timer
                TimerForCollect.value = 0f; // Reset slider when released
             }
        
       
    }

    private void HoldAction()
    {
#if UNITY_EDITOR

        Debug.Log(" Hold Successful! Action triggered.");

#endif
        TimerForCollect.gameObject.SetActive(false);
        HabdBT.gameObject.SetActive(false);
#if UNITY_EDITOR

        Debug.Log(" Helth Up");

#endif
        RaycastHit hot;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out hot, rayDistance, interactLayers))
        {
            Item item = hot.transform.gameObject.GetComponent<Item>();

            if (item != null && ItemID == item.itemID)
            {
#if UNITY_EDITOR

                print("Interactive Dor Hol Object Name: " + hot.transform.name);

#endif
                CheckRaycastedObject(hot.transform.gameObject, -1);
                        
                
                // Add action when hold is completed
                isHolding = false; // Stop the hold process
                holdTime = 0f; // Reset the timer
            }
            else if (item != null && ItemID == item.itemID)
            {
#if UNITY_EDITOR

                print("Interactive Dor Hol Object Name: " + hot.transform.name);

#endif
                CheckRaycastedObject(hot.transform.gameObject, -1);


                // Add action when hold is completed
                isHolding = false; // Stop the hold process
                holdTime = 0f; // Reset the timer
            }
            else
            {
#if UNITY_EDITOR

                Debug.Log("Unlock Up");


#endif
                if (Physics.Raycast(ray, out hot, rayDistance, interactLayers))
                {

                    if (hot.transform.gameObject.tag == interactKeyholTag)
                    {
#if UNITY_EDITOR

                        print("Interactive Dor Hol Object Name: " + hot.transform.name);

#endif
                        Keyhole keyhol = hot.transform.gameObject.GetComponent<Keyhole>();

                        if (keyhol.locket)
                        {
#if UNITY_EDITOR

                            Debug.Log("Try Unlocket");

#endif
                            keyhol.TryUnlockDoor();

                            itemNameText.gameObject.SetActive(true);
                            itemNameText.text = keyhol.TryUnlockDoor();
                            timer = displayDuration;
                        }

                    }
                    if (hot.transform.gameObject.tag == interactScruKeyTag)
                    {
                       
#if UNITY_EDITOR

                        print("Interactive Dor Hol Object Name: " + hot.transform.name);
                        print("Interactive Dor Hol Scruw Work ");
#endif
                        Transform hitTransform = hot.transform;

                        ScrowHoll scrowHoll = hitTransform.GetComponent<ScrowHoll>();
                        if (scrowHoll != null)
                        {
                            scrowHoll.ActiveScruwHoll();
                        }
                        else
                        {
                            Debug.LogError("ScrowHoll component not found on object: " + hot.transform.gameObject.name);
                        }
                        itemNameText.gameObject.SetActive(true);
                        timer = displayDuration;
                        //this use scru hol active setup



                    }
                    if (hot.transform.gameObject.tag == interactHammerKeyTag)
                    {

#if UNITY_EDITOR

                        print("Interactive Dor Hol Object Name: " + hot.transform.name);
                        print("Interactive Dor Hol Scruw Work ");
#endif
                        Transform hitTransform = hot.transform;

                        KickTrigger KickTrigger = hitTransform.GetComponent<KickTrigger>();

                      
                            if (KickTrigger != null)
                            {
                                KickTrigger.kikdoor();
                            }
                            else
                            {
                                Debug.LogError("ScrowHoll component not found on object: " + hot.transform.gameObject.name);
                            }
                            itemNameText.gameObject.SetActive(true);
                        itemNameText.text = "Unlock";
                            timer = displayDuration;
                        
                        


                       
                        //this use scru hol active setup



                    }


                }
                // Add action when hold is completed
                isHolding = false; // Stop the hold process
                holdTime = 0f; // Reset the timer
            }




        }




    }
    private void TimerRay()
    {
        // Hide the text after the timer runs out
        if (itemNameText.gameObject.activeSelf)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                itemNameText.gameObject.SetActive(false);
                HabdBT.gameObject.SetActive(false);
                DoorInteractdBT.gameObject.SetActive(false);
                TimerForCollect.gameObject.SetActive(false);
            }
        }
    }
    // button reaction in dor
    private void Update()
    {

        if (isHolding)
        {
            holdTime += Time.deltaTime;
            TimerForCollect.value = holdTime / requiredHoldTime; // Update slider progress
            if (holdTime >= requiredHoldTime)
            {
                HoldAction();
                isHolding = false; // Stop further execution
                holdTime = 0f; // Reset hold time
                TimerForCollect.value = 0f; // Reset slider
            }
        }



        if (m_readPaper)
        {
            ReadPaper();
        }

        if (m_takeItem)
        {
            TakingItem();
        }
        TimerRay();
         //Timer for checking interval
         checkTimer += Time.deltaTime;
        if (checkTimer < checkInterval) return;
        checkTimer = 0f;

        // Create a ray going straight forward from the camera
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
#if UNITY_EDITOR

        // Draw the ray in the Scene view for debugging
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

#endif

        // Perform the raycast using RaycastNonAlloc for better performance on mobile
        int hitCount = Physics.RaycastNonAlloc(ray, hits, rayDistance, detectableLayer);
          if (!player.locked)
        {
            if (m_gameController.m_mobileTouchInput)
            {
        if (hitCount > 0)
        {
            RaycastHit hit = hits[0];
            GameObject hitObject = hit.collider.gameObject;
                   

                    // Check if the hit object has an interactable tag
                    if (hitObject.CompareTag(interactTag) || hitObject.CompareTag(interactKeyTag))
                    {
                        bool inventoryFull = m_gameController.inventory.IsInventoryFull();
                        string itemName = "";
                        int Id = -1;

                        //corsor icon hiliter
                        targetIconhighlighter.gameObject.SetActive(true);

                        if (hitObject.CompareTag(interactTag)) // Normal Item Pickup
                        {
                            // need this add UI Hiliter

                            Item item = hitObject.GetComponent<Item>();
                            int hitItemID = item.itemID;
                            itemName = item.itemName;
                            Id = hitItemID;

                           






                            bool itemExists = m_gameController.inventory.m_slots.Any(slot => slot.m_itemID == hitItemID);
                            // this cheak item bag full or not
                            if (itemExists || !inventoryFull)
                            {
                                // Update UI for pickup items
                                if (itemName == "Helth")
                                {
                                    DoorInteractdBT.gameObject.SetActive(true);
                                    Image buttonImage = DoorInteractdBT.GetComponent<Image>();
                                    buttonImage.gameObject.SetActive(true);
                                    buttonImage.sprite = sprites[0];
                                    ItemID = Id;
                                    TimerForCollect.gameObject.SetActive(true);
                                    HabdBT.gameObject.SetActive(false);
                                }
                               else
                                {
                                    ItemID = Id;
                                    HabdBT.gameObject.SetActive(true);

                                    //popup hitnt tool massage
                                    if (itemName == "Coin")
                                    {
                                        toolHintPopup.ShowHintOnce("Coin");
                                    }
                                    else if (itemName == "HeadTorch")
                                    {
                                        toolHintPopup.ShowHintOnce("HeadTorch");
                                    }

                                }
                            }
                            else
                            {
                                itemName = "Hand bags Item Full";
                            }
                        }


                        if (hitObject.CompareTag(interactKeyTag)) // Key Item Pickup
                        {
                            if (!inventoryFull)
                            {
                                KeyItem keyItem = hitObject.GetComponent<KeyItem>();
                                itemName = "Pickup " + keyItem.keyID;
                                HabdBT.gameObject.SetActive(true);
                                ItemID = Id;
                            }
                            else
                            {
                                itemName = "Hand bags Item Full";
                            }
                        }

                        // Update UI
                        itemNameText.text = itemName;
                        itemNameText.gameObject.SetActive(true);
                       
                        timer = displayDuration;
                    }

                    
                    if (hitObject.CompareTag(interactHammerKeyTag))
                    {
                        //corsor icon hiliter
                        targetIconhighlighter.gameObject.SetActive(true);
                        toolHintPopup.ShowHintOnce("Hammer");
                        if (InteractID == 1)
                        {//hammer
                         //popup hitnt tool massage
                          
                             
                            
                            KickTrigger kickTrigger = hitObject.gameObject.GetComponent<KickTrigger>();

                            if (kickTrigger.isUnlock)
                            {
                                if (!kickTrigger.isUnlockend)
                                {
                                    itemNameText.gameObject.SetActive(true);
                                    itemNameText.text = "UNLOCKED";
                                    timer = displayDuration;
                                    DoorInteractdBT.gameObject.SetActive(true);
                                    //enable evettrigger
                                    EventTrigger trigger = DoorInteractdBT.GetComponent<EventTrigger>();
                                    if (trigger != null)
                                    {
                                        trigger.enabled = true;
                                    }
                                    Image buttonImage = DoorInteractdBT.GetComponent<Image>();
                                    buttonImage.gameObject.SetActive(true);
                                    buttonImage.sprite = sprites[3];
                                    TimerForCollect.gameObject.SetActive(true);
                                    HabdBT.gameObject.SetActive(false);
                                }
                                else
                                {
                                   
                                         itemNameText.gameObject.SetActive(true);
                                    itemNameText.text = " Iron Grill Door";
                                    timer = displayDuration;
                                }
                               
                            }
                            else
                            {
                                itemNameText.gameObject.SetActive(true);
                                itemNameText.text = "You need Unlock plank";
                                timer = displayDuration;
                                DoorInteractdBT.gameObject.SetActive(true);
                                // Disable EventTrigger
                                EventTrigger trigger = DoorInteractdBT.GetComponent<EventTrigger>();
                                if (trigger != null)
                                {
                                    trigger.enabled = false;
                                }
                                Image buttonImage = DoorInteractdBT.GetComponent<Image>();
                                buttonImage.gameObject.SetActive(true);
                                buttonImage.sprite = sprites[2];
                            }

                           
                        }
                        else
                        {
                            itemNameText.gameObject.SetActive(true);
                            itemNameText.text = "This door is LOCKED.Use the  Hammer to unlock";
                            timer = displayDuration;
                            DoorInteractdBT.gameObject.SetActive(true);
                            // Disable EventTrigger
                            EventTrigger trigger = DoorInteractdBT.GetComponent<EventTrigger>();
                            if (trigger != null)
                            {
                                trigger.enabled = false;
                            }
                            Image buttonImage = DoorInteractdBT.GetComponent<Image>();
                            buttonImage.gameObject.SetActive(true);
                            buttonImage.sprite = sprites[2];

                        }



                    }


                    if (hitObject.CompareTag(interactScruKeyTag))
                    {
                        //corsor icon hiliter
                        targetIconhighlighter.gameObject.SetActive(true);
                        toolHintPopup.ShowHintOnce("Screwdriver");
                        if (InteractID == 2)
                        {
                            itemNameText.gameObject.SetActive(true);
                            itemNameText.text = "UNLOCKED";
                            timer = displayDuration;
                            DoorInteractdBT.gameObject.SetActive(true);
                            //enable evettrigger
                            EventTrigger trigger = DoorInteractdBT.GetComponent<EventTrigger>();
                            if (trigger != null)
                            {
                                trigger.enabled = true;
                            }
                            Image buttonImage = DoorInteractdBT.GetComponent<Image>();
                            buttonImage.gameObject.SetActive(true);
                            buttonImage.sprite = sprites[4];
                            TimerForCollect.gameObject.SetActive(true);
                            HabdBT.gameObject.SetActive(false);
                        }
                        else
                        {
                            itemNameText.gameObject.SetActive(true);
                            itemNameText.text = "This door is LOCKED.Use the  Scruwdrive to unlock";
                            timer = displayDuration;
                            DoorInteractdBT.gameObject.SetActive(true);
                            // Disable EventTrigger
                            EventTrigger trigger = DoorInteractdBT.GetComponent<EventTrigger>();
                            if (trigger != null)
                            {
                                trigger.enabled = false;
                            }
                            Image buttonImage = DoorInteractdBT.GetComponent<Image>();
                            buttonImage.gameObject.SetActive(true);
                            buttonImage.sprite = sprites[2];
                           
                        }


                    }




                    if (hitObject.CompareTag(interactDorTag))
                    {
                        //corsor icon hiliter
                        targetIconhighlighter.gameObject.SetActive(true);
                        DoorSystem doorSystem = hitObject.gameObject.GetComponent<DoorSystem>();

                        if (doorSystem.isLocked)
                        {
                            itemNameText.gameObject.SetActive(true);
                            itemNameText.text = "This door is LOCKED.Use the " + doorSystem.requiredKey + " to unlock";
                            timer = displayDuration;


                        }
                        else
                        {
                            itemNameText.gameObject.SetActive(true);
                            itemNameText.text = "Open " + doorSystem.doorID + " Door";
                           
                            HabdBT.gameObject.SetActive(true);
                            TimerForCollect.gameObject.SetActive(false);
                            timer = displayDuration;
                        }
                      
                    }

                 
                
                    if (hitObject.CompareTag(interactKeyholTag))
                    {
                        //corsor icon hiliter
                        targetIconhighlighter.gameObject.SetActive(true);

                        Keyhole keyhol = hitObject.GetComponent<Keyhole>();
                        int keyvaluwe = PlayerPrefs.GetInt(keyhol.requiredKey);
                       
                        if (keyvaluwe == 1&& keyhol.locket)// Player has the key
                        {
                            Image buttonImage = DoorInteractdBT.GetComponent<Image>(); // Get the Image component of the Button

                            TimerForCollect.gameObject.SetActive(true);
                            itemNameText.gameObject.SetActive(true);
                            itemNameText.text = "Unlock Use " + keyhol.requiredKey;
                            DoorInteractdBT.gameObject.SetActive(true);
                            buttonImage.sprite = sprites[1]; // Set the sprite
                            timer = displayDuration;
                            ItemID = 0;
                        }
                       else if(!(keyvaluwe == 1))
                        {
                            
                            itemNameText.text = "Need " + keyhol.requiredKey;
                            itemNameText.gameObject.SetActive(true);
                            DoorInteractdBT.gameObject.SetActive(false);
                            TimerForCollect.gameObject.SetActive(false);
                            timer = displayDuration;

                        }
                        else
                        {
                            itemNameText.gameObject.SetActive(false);
                            HabdBT.gameObject.SetActive(false);
                            DoorInteractdBT.gameObject.SetActive(false);
                            TimerForCollect.gameObject.SetActive(false);
                        }
                       


                    }
                    //else
                    //{
                    //    // If the object has a different tag, hide the text
                    //    itemNameText.gameObject.SetActive(false);
                    //    HabdBT.gameObject.SetActive(false);
                    //}
                }
             else
                 {
                      // If nothing was hit, hide the text
                   itemNameText.gameObject.SetActive(false);
                   HabdBT.gameObject.SetActive(false);
                     DoorInteractdBT.gameObject.SetActive(false);
                    TimerForCollect.gameObject.SetActive(false);
                    ItemInteractdBT.gameObject.SetActive(false);
                    targetIconhighlighter.gameObject.SetActive(false);

                    //enable evettrigger
                    EventTrigger trigger = DoorInteractdBT.GetComponent<EventTrigger>();
                    if (trigger != null)
                    {
                        trigger.enabled = true;
                    }
                }


      

        }
        }

      
    }
    int InteractID;
    private void InteractBTUse()
    {
        Image buttonImage = ItemInteractdBT.GetComponent<Image>(); // Get the Image component of the Button

        if (InteractID == 0)
        {
            buttonImage.gameObject.SetActive(false); // Hide the button when ID is 0
            ItemDropBT.gameObject.SetActive(false);
        }
        else if (InteractID == 6)
        {
            m_gameController.OpenBook();
        }
        else if (InteractID == 1)
        {
            //use hammer for animate
            itemNameText.text = "Use Item Animation";
          
        }
        else
        {
            itemNameText.gameObject.SetActive(true);
            itemNameText.text = "Use Item Animation";
#if UNITY_EDITOR

            print("Use Animation");

#endif
        }
    }

    private void DropItem()
    {
        GetInventory.DropItem(InteractID);
        DoorInteractdBT.gameObject.SetActive(true);
        Image buttonImage = DoorInteractdBT.GetComponent<Image>();
        buttonImage.gameObject.SetActive(true);
        buttonImage.sprite = sprites[2];
        InteractID = 0;
    }

    public void InteractItemBehavior(int ID)
    {
        Image buttonImage = ItemInteractdBT.GetComponent<Image>(); // Get the Image component of the Button

        if (buttonImage == null)
        {
#if UNITY_EDITOR

            Debug.LogError("Button does not have an Image component!");

#endif
            return;
        }

        if (ID == 0)
        {
            buttonImage.gameObject.SetActive(false); // Hide the button when ID is 0
            ItemDropBT.gameObject.SetActive(false);
            InteractID = ID;
        }
        else if (ID > 0 && ID < m_itemsDatabase.Items.Count)
        {
            buttonImage.gameObject.SetActive(false); // Ensure the button is visible
            ItemDropBT.gameObject.SetActive(true);
           buttonImage.sprite = m_itemsDatabase.Items[ID].m_itemIcon; // Set the sprite
          

           InteractID = ID;
        }
        else if (ID ==1)
        {
            buttonImage.gameObject.SetActive(false); // Ensure the button is visible
            ItemDropBT.gameObject.SetActive(true);
            buttonImage.sprite = m_itemsDatabase.Items[ID].m_itemIcon; // Set the sprite


            InteractID = ID;
        }
        else if (ID == 2)
        {
            buttonImage.gameObject.SetActive(false); // Ensure the button is visible
            ItemDropBT.gameObject.SetActive(true);
            buttonImage.sprite = m_itemsDatabase.Items[ID].m_itemIcon; // Set the sprite


            InteractID = ID;
        }
        else
        {
#if UNITY_EDITOR

            Debug.LogWarning("Invalid Item ID!");

#endif
        }
    }



    //new codes

    /*
    if (!player.locked)
    {
    if (m_gameController.m_mobileTouchInput)
    {

        for (var i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    RaycastHit hot;
                    Ray ray2 = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                    if (Physics.Raycast(ray2, out hot, rayDistance, interactLayers))
                    {
                        if (hot.transform.gameObject.tag == interactTag)
                        {
                            print("Interactive Object NAme" + hot.transform.name);
                            CheckRaycastedObject(hot.transform.gameObject, -1);
                        }
                    }
                }

            }
        }

    }

    if (!m_gameController.m_mobileTouchInput)
    {
        if (CrossPlatformInputManager.GetButtonDown("Interact"))
        {
            if (EventSystem.current.IsPointerOverGameObject())    // is the touch on the GUI
            {
                return;
            }
            RaycastHit hot;
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray2, out hot, rayDistance, interactLayers))
            {
                if (hot.transform.gameObject.tag == interactTag)
                {
                    CheckRaycastedObject(hot.transform.gameObject, -1);
                }
            }
        }
    }
    }

            */



    private void ReadPaper()
{
if(m_readPaper && m_paper != null)
{
if(m_readState == 0)
{
    m_paper.transform.position = Vector3.Slerp(m_paper.transform.position, m_examineTransform.position, m_moveSpeed * Time.deltaTime);
    m_paper.transform.rotation = Quaternion.Slerp(m_paper.transform.rotation, m_examineTransform.rotation, m_moveSpeed * Time.deltaTime);
    float dist = Vector3.Distance(m_paper.transform.position, m_examineTransform.position);
    float ang = Quaternion.Angle(m_paper.transform.rotation, m_examineTransform.rotation);

    if (dist <= 0.5 && ang <= 0.5)
    {
        m_paper.transform.position =m_examineTransform.position;
        m_paper.transform.rotation = m_examineTransform.rotation;
        m_readState = 1;
        m_paper.ReadPaper();
        StartCoroutine(WaitForPaperShow());

        m_gameController.SetEffect(m_paper.m_pictureEffectType);

    }
}

if(m_readState == 2)
{
    m_paper.transform.position = Vector3.Slerp(m_paper.transform.position, m_paperScreenTransform.position, m_moveSpeed * Time.deltaTime);
    m_paper.transform.rotation = Quaternion.Slerp(m_paper.transform.rotation, m_paperScreenTransform.rotation, m_moveSpeed * Time.deltaTime);
    float dist = Vector3.Distance(m_paper.transform.position, m_paperScreenTransform.position);
    float ang = Quaternion.Angle(m_paper.transform.rotation, m_paperScreenTransform.rotation);

    if (dist <= 1 && ang <= 1)
    {
        m_gameController.m_nextPicture = m_paper.m_pictureIconUI;
        m_readState = 0;
        m_readPaper = false;
        Destroy(m_paper.gameObject);
        m_paper = null;                
        m_gameController.AddPaperPicture();
    }

}
}
}

    private void TakingItem()
    {
#if UNITY_EDITOR

        print("Take Item....");

#endif

        if (m_takeItem && m_item != null)
            {
                if (m_itemTakeState == 0)
                {
                    m_item.transform.position = Vector3.Slerp(m_item.transform.position, m_examineTransform.position, m_moveSpeed * Time.deltaTime);
                    m_item.transform.rotation = Quaternion.Slerp(m_item.transform.rotation, m_examineTransform.rotation, m_moveSpeed * Time.deltaTime);
                    float dist = Vector3.Distance(m_item.transform.position, m_examineTransform.position);
                    float ang = Quaternion.Angle(m_item.transform.rotation, m_examineTransform.rotation);

                    if (dist <= 0.5 && ang <= 0.5)
                    {
                        m_item.transform.position = m_examineTransform.position;
                        m_item.transform.rotation = m_examineTransform.rotation;
                        m_itemTakeState = 1;
                        m_gameController.ShowTip(m_item.itemID, 6);
                        StartCoroutine(WaitForItemShow());

                    }
                }

                if (m_itemTakeState == 2)
                {
                    m_item.transform.position = Vector3.Slerp(m_item.transform.position, m_itemScreenTransform.position, m_moveSpeed * Time.deltaTime);
                    m_item.transform.rotation = Quaternion.Slerp(m_item.transform.rotation, m_itemScreenTransform.rotation, m_moveSpeed * Time.deltaTime);
                    float dist = Vector3.Distance(m_item.transform.position, m_itemScreenTransform.position);
                    float ang = Quaternion.Angle(m_item.transform.rotation, m_itemScreenTransform.rotation);

                    if (dist <= 1 && ang <= 1)
                    {


                        m_itemTakeState = 0;




                        m_gameController.inventory.AddItem(m_item.itemID, m_item.itemCount);
                        m_takeItem = false;
                        Destroy(m_item.gameObject);
                        m_item = null;
                    }
                }
            }
            

        
        

}//take item

public void DropItemCheck(int id)
{
if (!player.locked)
{
if (m_gameController.m_mobileTouchInput)
{
   RaycastHit hot;
   Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
   if (Physics.Raycast(ray, out hot, rayDistance, interactLayers))
   {
     if (hot.transform.gameObject.tag == interactTag)
     {
       CheckRaycastedObject(hot.transform.gameObject,id);
     }
   }

}

if (!m_gameController.m_mobileTouchInput)
{

    RaycastHit hot;
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out hot, rayDistance, interactLayers))
    {
      if (hot.transform.gameObject.tag == interactTag)
      {
                CheckRaycastedObject(hot.transform.gameObject, id);
      }
    }

}
}
}

private void CheckRaycastedObject(GameObject target, int dragID)
{


if (target.GetComponent<InteractObject>())
{
   if(target.GetComponent<InteractObject>().m_itemToInteractId == -1 )
   {
    target.GetComponent<InteractObject>().Interact();
   }
  else
   {
     if (dragID == target.GetComponent<InteractObject>().m_itemToInteractId)
     {
        target.GetComponent<InteractObject>().Interact();

        if (target.GetComponent<InteractObject>().m_removeItemAfterUse)
        {
            m_gameController.inventory.RemoveItem(target.GetComponent<InteractObject>().m_itemToInteractId, 1);
        }
     }else
     {
        if (!target.GetComponent<InteractObject>().m_used || !target.GetComponent<InteractObject>().m_useOneTime)
        {
            m_gameController.ShowTip(target.GetComponent<InteractObject>().m_itemToInteractId, 0);
        }
    }
   }
}

if (target.GetComponent<Item>() && !m_takeItem && !m_readPaper)
{
    AudioSource.PlayClipAtPoint(target.GetComponent<Item>().pickupSound,transform.position);          
    m_takeItem = true;
    m_item = target.GetComponent<Item>();
    target.transform.parent = m_examineTransform;
}

if (target.GetComponent<SecurityComputer>())
{
    m_gameController.SetSecurityCameras();
}

if (target.GetComponent<PicturePaper>() && !m_readPaper && !m_takeItem)
{
    target.GetComponent<PicturePaper>().ShowPaper();
    AudioSource.PlayClipAtPoint(target.GetComponent<PicturePaper>().m_pickupSound, transform.position);
    m_readPaper = true;
    m_paper = target.GetComponent<PicturePaper>();
    target.transform.parent = m_examineTransform;
}

if (target.GetComponent<Door>())
{

   target.GetComponent<Door>().Interact();

}

if (target.GetComponent<AutomaticDoor>())
{
if (!target.GetComponent<AutomaticDoor>().m_jamned)
{
    if (target.GetComponent<AutomaticDoor>().m_locked)
    {
        if (dragID == target.GetComponent<AutomaticDoor>().m_keyId)
        {
            m_gameController.inventory.RemoveItem(target.GetComponent<AutomaticDoor>().m_keyId, 1);
            target.GetComponent<AutomaticDoor>().Unlock();
        }else
        {

            m_gameController.ShowTip(target.GetComponent<AutomaticDoor>().m_keyId, 0);
        }

    }else
    {
        if(target.GetComponent<AutomaticDoor>().m_needPower)
        {
            m_gameController.ShowTip(-1, 5);
        }
    }
}else
{
    m_gameController.ShowTip(-1,1);
}
}

}

private IEnumerator WaitForPaperShow()
{

yield return new WaitForSeconds(m_paperShowTime);
m_readState = 2;

}

private IEnumerator WaitForItemShow()
{

yield return new WaitForSeconds(m_itemShowTime);
m_itemTakeState = 2;

}
}

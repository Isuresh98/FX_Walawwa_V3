using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
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


    [Header("UI Item Settings")]
    public Button ItemInteractdBT;
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
    [Header("Timer Use Button Settings")]
    public Button DoorInteractdBT;
    private bool isHolding = false;
    public float holdTime = 0f;
    public float requiredHoldTime = 2f; // Time in seconds to trigger hold action
    public LayerMask interactDoorLayers;
    public Slider TimerForCollect;
    public Sprite[] sprites;
    public bool isInventoryFull=false;

    private void Start()
    {
        m_gameController = FindObjectOfType<GameControll>();
        m_itemsDatabase = FindObjectOfType<ItemsDatabase>();
        GetInventory= FindObjectOfType<Inventory>();
        InteractItemBehavior(0);

        player = m_gameController.player;

        HabdBT.onClick.AddListener(OnHandButtonClicked); // Add this line
        ItemInteractdBT.onClick.AddListener(InteractBTUse); // Add this line

        

    }
    private void OnHandButtonClicked()
    {
        RaycastHit hot;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        // Draw the ray for debugging
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

        if (Physics.Raycast(ray, out hot, rayDistance, interactLayers))
        {
            if (hot.transform.gameObject.tag == interactTag)
            {


                

                print("Interactive Object Name: " + hot.transform.name);
               Rigidbody rb= hot.transform.gameObject.GetComponent<Rigidbody>(); // Freezes all movement and rotation
                rb.constraints = RigidbodyConstraints.FreezeAll;
                CheckRaycastedObject(hot.transform.gameObject, -1);
            }
            else if (hot.transform.gameObject.tag == interactCoinTag)
            {
                print("Interactive Object Name: " + hot.transform.name);
                SphereCollider sphereCollider = hot.transform.gameObject.GetComponent<SphereCollider>();
                if (sphereCollider != null)
                {
                    sphereCollider.radius = 0.5f; // Set your desired radius
                }

            }
            else if (hot.transform.gameObject.tag == interactKeyTag)
            {
                print("Interactive Dor Object Name: " + hot.transform.name);

                Rigidbody rb = hot.transform.gameObject.GetComponent<Rigidbody>(); // Freezes all movement and rotation
                rb.constraints = RigidbodyConstraints.FreezeAll;
                KeyItem keyItem = hot.transform.gameObject.GetComponent<KeyItem>();
                keyItem.CollectKey();
                CheckRaycastedObject(hot.transform.gameObject, -1);

            }
            else if (hot.transform.gameObject.tag == interactKeyholTag)
            {
                print("Interactive Dor Hol Object Name: " + hot.transform.name);

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
                print("Interactive Dor Object Name: " + hot.transform.name);
                DoorSystem doorSystem = hot.transform.gameObject.GetComponent<DoorSystem>();
                doorSystem.ToggleDoor();


            }

        }
    }



    public void OnHandButtonDoorClicked()
    {
       
            Debug.Log(" Button Clicked!");
            isHolding = true;
            holdTime = 0f;
        
      
          
     
    }

    public void OnHandButtonDoorReleased()
    {
       
            if (isHolding)
            {
                if (holdTime < requiredHoldTime)
                {
                    Debug.Log("Hold Canceled. Button Released Too Early!");
                }

                isHolding = false; // Stop the hold process
                holdTime = 0f; // Reset the timer
            TimerForCollect.value = 0f; // Reset slider when released
        }
        
       
    }

    private void HoldAction()
    {
        Debug.Log(" Hold Successful! Action triggered.");

        TimerForCollect.gameObject.SetActive(false);
        HabdBT.gameObject.SetActive(false);
        if (ItemID == 5)
        {
            Debug.Log(" Helth Up");
            RaycastHit hot;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (Physics.Raycast(ray, out hot, rayDistance, interactLayers))
            {

                if (hot.transform.gameObject.tag == interactTag)
                {
                    print("Interactive Dor Hol Object Name: " + hot.transform.name);
                    CheckRaycastedObject(hot.transform.gameObject, -1);
                   
                }


            }
            // Add action when hold is completed
            isHolding = false; // Stop the hold process
            holdTime = 0f; // Reset the timer
        }
        else
        {

            Debug.Log("Unlock Up");

            RaycastHit hot;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (Physics.Raycast(ray, out hot, rayDistance, interactLayers))
            {

                if (hot.transform.gameObject.tag == interactKeyholTag)
                {
                    print("Interactive Dor Hol Object Name: " + hot.transform.name);

                    Keyhole keyhol = hot.transform.gameObject.GetComponent<Keyhole>();

                    if (keyhol.locket)
                    {

                        Debug.Log("Try Unlocket");
                        keyhol.TryUnlockDoor();

                        itemNameText.gameObject.SetActive(true);
                        itemNameText.text = keyhol.TryUnlockDoor();
                        timer = displayDuration;
                    }

                }


            }
            // Add action when hold is completed
            isHolding = false; // Stop the hold process
            holdTime = 0f; // Reset the timer
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

        // Draw the ray in the Scene view for debugging
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

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


                    //cheak Inventor Full
                    //cheak inventory fulll
                    if (!isInventoryFull)
                    {
                        // Check if the hit object has the correct tag
                        if (hitObject.CompareTag(interactTag)) // Change "PickupItem" to your desired tag
                        {
                            if (!isInventoryFull)
                            {

                                // Display the item name on the UI if it has changed
                             string itemName = hitObject.gameObject.GetComponent<Item>().itemName; // Or use a custom name if needed

                            if (itemName == "Helth")
                            {
                                Image buttonImage = DoorInteractdBT.GetComponent<Image>(); // Get the Image component of the Button
                                int Id = hitObject.gameObject.GetComponent<Item>().itemID;
                                if (itemNameText.text != itemName)
                                {
                                    itemNameText.text = "Pickup " + itemName;
                                }
                                TimerForCollect.gameObject.SetActive(true);
                                ItemID = Id;
                                buttonImage.gameObject.SetActive(true); // Ensure the button is visible
                                buttonImage.sprite = sprites[0]; // Set the sprite


                                itemNameText.gameObject.SetActive(true);
                                DoorInteractdBT.gameObject.SetActive(true);
                                timer = displayDuration;
                            }
                            else if (!(itemName == null))
                            {
                                int Id = hitObject.gameObject.GetComponent<Item>().itemID;
                                if (itemNameText.text != itemName)
                                {
                                    itemNameText.text = "Pickup " + itemName;
                                }
                                ItemID = Id;
                                itemNameText.gameObject.SetActive(true);
                                HabdBT.gameObject.SetActive(true);
                                timer = displayDuration;
                            }
                            else
                            {

                                itemNameText.gameObject.SetActive(false);
                                HabdBT.gameObject.SetActive(false);
                            }

                            }
                            else
                            {

                                itemNameText.gameObject.SetActive(true);
                                itemNameText.text = "Hand bags Item Full";
                                timer = displayDuration;
                            }


                        }




                        if (hitObject.CompareTag(interactKeyTag))
                        {

                            if (!isInventoryFull)
                            {
                                KeyItem keyItem = hitObject.GetComponent<KeyItem>();

                            itemNameText.gameObject.SetActive(true);
                            itemNameText.text = "Pickup" + keyItem.keyID;
                            HabdBT.gameObject.SetActive(true);

                            timer = displayDuration;

                            }
                            else
                            {

                                itemNameText.gameObject.SetActive(true);
                                itemNameText.text = "Hand bags Item Full";
                                timer = displayDuration;
                            }

                        }

                    }


                    if (hitObject.CompareTag(interactDorTag))
                    {
                       
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

                
                }


      

        }
        }

      
    }
   
    private void InteractBTUse()
    {
        Image buttonImage = ItemInteractdBT.GetComponent<Image>(); // Get the Image component of the Button

        if (ItemID == 0)
        {
            buttonImage.gameObject.SetActive(false); // Hide the button when ID is 0
        }
        else if (ItemID == 3)
        {
            GetInventory.DropItem(ItemID);

        }
        else
        {
            itemNameText.gameObject.SetActive(true);
            itemNameText.text = "Use Item Animation";
            print("Use Animation");
        }
    }


    public void InteractItemBehavior(int ID)
    {
        Image buttonImage = ItemInteractdBT.GetComponent<Image>(); // Get the Image component of the Button

        if (buttonImage == null)
        {
            Debug.LogError("Button does not have an Image component!");
            return;
        }

        if (ID == 0)
        {
            buttonImage.gameObject.SetActive(false); // Hide the button when ID is 0
            ItemID = ID;
        }
        else if (ID > 0 && ID < m_itemsDatabase.Items.Count)
        {
            buttonImage.gameObject.SetActive(true); // Ensure the button is visible
            buttonImage.sprite = m_itemsDatabase.Items[ID].m_itemIcon; // Set the sprite
            ItemID = ID;
        }
        else
        {
            Debug.LogWarning("Invalid Item ID!");
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
print("Take Item....");
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
        m_gameController.ShowTip(m_item.itemID,6);
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
}

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

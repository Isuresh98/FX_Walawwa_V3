using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour {

    [Header("GeneralSettings")]
    [Tooltip("Object with GameControll.cs script")]
    public GameControll gameControll;
    [HideInInspector]
    public Inventory inventory;

    [Header("PlayerSettings")]
    [Tooltip("Player walk speed")]
    public float walkSpeed;
    [Tooltip("Player run speed")]
    public float runSpeed;
    [Tooltip("Player crouch speed")]
    public float crouchSpeed;
    [HideInInspector]
    public bool locked;
    [HideInInspector]
    public bool lockedByDying;
    private CharacterController characterController;
    private float moveSpeed;
    [HideInInspector]
    public bool playerMoving;
    [HideInInspector]
    public bool m_enemySeePlayer;

    [Header("Stamina Settings")]
    public Image m_staminaBar;
    public GameObject m_runArrownImage;
    public float m_stamina = 100;
    public float m_maxStamina = 100;
    [HideInInspector]
    public bool m_unlimitedStamina = false;
    public float m_staminaConsumptionSpeed;
    public float m_staminaRecoverySpeed;
    bool m_staminaRecovery;
    [HideInInspector]
    public bool m_running;

    [Header("CameraSettings")]
    [Tooltip("Mouse Sensetivity value")]
    [Range(2f, 15f)] // Adds a slider in the Inspector
    public float mouseSensetivity = 15f; // Default value
    [Tooltip("Main camera transform")]
    public Transform cameraTransform;
    private float clampX;
    private float clampY;

    [Header("Camera Animations")]
    [Tooltip("Camera animation gameobject")]
    public Animation cameraAnimation;
    [Tooltip("Camera hit animation name")]
    public string cameraHitAnimName;
    [Tooltip("Camera idle animation name")]
    public string cameraIdleAnimName;
    [Tooltip("Camera move animation name")]
    public string cameraMoveAnimName;


    [Header("CrouchSettings")]  
    private float lerpSpeed  = 10f;
    [Tooltip("Player character controller normal height")]
    public float normalHeight;
    [Tooltip("Player character controller crouch height")]
    public float crouchHeight;
    [Tooltip("Player camera normal offset")]
    public float cameraNormalOffset;
    [Tooltip("Player camera crouch offset")]
    public float cameraCrouchOffset;
    [Tooltip("Player obstacle layers")]
    public LayerMask obstacleLayers;
    [Tooltip("Clamp camera axis")]
    public Vector2 clampXaxis;
    public Vector2 clampYaxis;
    [HideInInspector]
    public bool crouch = false;


    [Header("Sounds Settings")]
    public AudioSource m_breathAS;
    [Tooltip("Foot steps sounds")]
    public AudioClip[] footSteps;
    public AudioClip dyingSound;
    [HideInInspector]
    public AudioSource AS;

    [Header("Third Person Model Settings")]
    public Animator m_TPS_Player;
    bool isDie = false;

    [Header("Enemy AI Use")]
    public bool iSIntractableHideObject = false;
    private void Awake()
    {
        m_runArrownImage.SetActive(m_running);
        AS = GetComponent<AudioSource>();
        inventory = GetComponent<Inventory>();
        characterController = GetComponent<CharacterController>();
        clampX = 0f;
        moveSpeed = walkSpeed;


    }

    private void Update()
    {
        PlayerStates();
        if (!locked && !lockedByDying)
        {
            CameraRotation();       
            Movement();
            Controll();
            Stamina();
   
        }else
        {
            m_TPS_Player.SetFloat("DirectionY",0, 0.2f, Time.deltaTime);
            m_TPS_Player.SetFloat("DirectionX", 0, 0.2f, Time.deltaTime);
            m_TPS_Player.SetBool("Crouch", crouch);
            m_TPS_Player.SetBool("Run", m_running);
            }
        if(isDie)
        {
            m_TPS_Player.SetTrigger("Die");
            characterController.enabled = false;
        }
    }

    public void SetRun()
    {
     
        if (!m_running && !m_staminaRecovery)
        {
            m_running = true;
            m_runArrownImage.SetActive(m_running);

            if (!crouch)
            {
                moveSpeed = runSpeed;
            }
        }else
        {
            m_running = false;
            m_runArrownImage.SetActive(m_running);

            if (!crouch)
            {
                moveSpeed = walkSpeed;
            }else
            {
                moveSpeed = crouchSpeed;
            }
        }
    }

    private void Stamina()
    {

        m_staminaBar.fillAmount = m_stamina / 100f;

        if(m_running && !crouch)
        {
            if (characterController.velocity.magnitude > 3.5f && !m_unlimitedStamina)
            {
                m_stamina -= m_staminaConsumptionSpeed * Time.deltaTime;
                if (m_stamina <= 0 )
                {
                    m_running = false;
                    m_runArrownImage.SetActive(m_running);
                    m_staminaRecovery = true;
                    m_staminaBar.color = Color.red;
                    if(!crouch)
                    {
                        moveSpeed = walkSpeed;
                    }else
                    {
                        moveSpeed = crouchSpeed;
                    }
                }
            }else
            {
                if (m_stamina < m_maxStamina)
                {
                    m_stamina += m_staminaRecoverySpeed * Time.deltaTime;
                    if(m_stamina >= m_maxStamina)
                    {
                        m_staminaRecovery = false;
                        m_staminaBar.color = Color.green;
                    }
                }
            }
        }else
        {
            if(m_stamina < m_maxStamina)
            {
                m_stamina += m_staminaRecoverySpeed * Time.deltaTime;
                if (m_stamina >= m_maxStamina)
                {
                    m_staminaBar.color = Color.green;
                    m_staminaRecovery = false;
                }
            }
        }
    }

    private void PlayerStates()
    {
        if(m_enemySeePlayer)
        {
            if(m_breathAS.volume < 1)
            {
                m_breathAS.volume += 0.5f * Time.deltaTime;
            }
        }else
        {
            if (m_breathAS.volume > 0)
            {
                m_breathAS.volume -= 0.15f * Time.deltaTime;
            }
        }
    }

    private void Controll()
    {
           
        float newHeight = crouch ? crouchHeight : normalHeight;
        characterController.height = Mathf.Lerp(characterController.height, newHeight, Time.deltaTime * lerpSpeed);

        characterController.center = Vector3.down * (normalHeight - characterController.height) / 2.0f;

        float newCamPos = crouch ? cameraCrouchOffset : cameraNormalOffset;
        Vector3 newPos = new Vector3(cameraTransform.localPosition.x, newCamPos, cameraTransform.localPosition.z);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newPos, Time.deltaTime *  lerpSpeed);

        if(CrossPlatformInputManager.GetButtonDown("Run"))
        {
            SetRun();
        }

        if (CrossPlatformInputManager.GetButtonDown("Crouch"))
        {
            SetCrouch();
        }

    }

    public void CatchPlayer(int state)
    {
#if UNITY_EDITOR

        print("CatchPlayer start states=" + state);

#endif
        if (state == 1)
        {
            gameControll.ResetPlayerStates();
            m_breathAS.gameObject.SetActive(false);
            StopAllCoroutines();
            locked = true;
            lockedByDying = true;
            characterController.height = normalHeight;
            cameraTransform.localPosition = new Vector3(0f, cameraNormalOffset, 0f);
            crouch = false;
            moveSpeed = walkSpeed;
            gameControll.ScreenFade(1);
        }

        if(state == 2)
        {
            if (dyingSound)
            {
                AS.PlayOneShot(dyingSound);
            }
            gameControll.ResetPlayerStates();
            m_breathAS.gameObject.SetActive(false);
            StopAllCoroutines();
            locked = true;
            lockedByDying = true;
            isDie = true;
            gameControll.ScreenFade(2);
        }


    }

    private void Movement()
    {
        float inputX = CrossPlatformInputManager.GetAxis("Horizontal") * moveSpeed;
        float inputY = CrossPlatformInputManager.GetAxis("Vertical") * moveSpeed;

        Vector3 forvardMove = transform.forward * inputY;
        Vector3 sideMove = transform.right * inputX;
        characterController.SimpleMove(forvardMove + sideMove);



        m_TPS_Player.SetFloat("DirectionY", inputX, 0.2f, Time.deltaTime);
        m_TPS_Player.SetFloat("DirectionX", inputY, 0.2f, Time.deltaTime);
        m_TPS_Player.SetBool("Crouch", crouch);
        m_TPS_Player.SetBool("Run", m_running);

        if (characterController.velocity.magnitude > 0.5f)
        {
            playerMoving = true;
            cameraAnimation.Play(cameraMoveAnimName);
            cameraAnimation[cameraMoveAnimName].speed = characterController.velocity.magnitude / 3;       
        }
        else
        {
            playerMoving = false;
            cameraAnimation.Play(cameraIdleAnimName);         
        }


    }

    //new vertion
    [Header("Joystic Parts")]
    [SerializeField] private RectTransform joystickArea;
    private Vector2 currentRotation;
    private Vector2 targetRotation;
    private Vector2 rotationVelocity;
    private float smoothTime = 0.2f; // Adjust for smoother/faster response

    // new vertion

    private void CameraRotation()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (!RectTransformUtility.RectangleContainsScreenPoint(joystickArea, touch.position))
                {
                    if (touch.phase == TouchPhase.Moved && !gameControll.DragInteract)
                    {
                        float touchX = touch.deltaPosition.x * mouseSensetivity * Time.deltaTime;
                        float touchY = touch.deltaPosition.y * mouseSensetivity * Time.deltaTime;

                        // Set target rotation values
                        targetRotation.x -= touchY;
                        targetRotation.y += touchX;

                        // Clamp vertical rotation
                        targetRotation.x = Mathf.Clamp(targetRotation.x, clampXaxis.x, clampXaxis.y);
                    }
                }
            }
        }

        // Apply smooth delay effect
        currentRotation = Vector2.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, smoothTime);

        // Apply rotation
        m_TPS_Player.SetFloat("SpineAngle", currentRotation.x * 1.3f, 0.2f, Time.deltaTime);
        cameraTransform.localRotation = Quaternion.Euler(currentRotation.x, 0, 0);
        transform.rotation = Quaternion.Euler(0, currentRotation.y, 0);
    }








    //old code
    /*

    private void CameraRotation()
    {
        // Check if there is at least one touch on the screen
        if (Input.touchCount > 0)
        {
            // Loop through all touches
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                // Check if the touch is outside the joystick area
                if (!RectTransformUtility.RectangleContainsScreenPoint(joystickArea, touch.position))
                {
                    // Check if the touch is moving and the player is not dragging an interactable object
                    if (touch.phase == TouchPhase.Moved && !gameControll.DragInteract)
                    {
                        float touchX = touch.deltaPosition.x * (mouseSensetivity * 0.1f) * Time.deltaTime;
                        float touchY = touch.deltaPosition.y * (mouseSensetivity * 0.1f) * Time.deltaTime;

                        clampX += touchY;
                        clampY += touchX;

                        // Clamp the vertical rotation
                        if (clampX > clampXaxis.y)
                        {
                            clampX = clampXaxis.y;
                            touchY = 0.0f;
                            ClampXAxis(clampXaxis.x);
                        }
                        else if (clampX < clampXaxis.x)
                        {
                            clampX = clampXaxis.x;
                            touchY = 0.0f;
                            ClampXAxis(clampXaxis.y);
                        }

                        // Apply rotations
                        m_TPS_Player.SetFloat("SpineAngle", clampX * 1.3f, 0.2f, Time.deltaTime);
                        cameraTransform.Rotate(Vector3.left * touchY);
                        transform.Rotate(Vector3.up * touchX);
                    }
                }
            }
        }
    }
    */

    // old vertion in thoch setup
    /* private void CameraRotation()
     {
         float mouseX = CrossPlatformInputManager.GetAxis("Mouse X") * (mouseSensetivity * 2) * Time.deltaTime;
         float mouseY = CrossPlatformInputManager.GetAxis("Mouse Y") * (mouseSensetivity * 2) * Time.deltaTime;

         clampX += mouseY;
         clampY += mouseX;

         if (clampX > clampXaxis.y)
         {
             clampX = clampXaxis.y;
             mouseY = 0.0f;
             ClampXAxis(clampXaxis.x);
         }
         else if (clampX < clampXaxis.x)
         {
             clampX = clampXaxis.x;
             mouseY = 0.0f;
             ClampXAxis(clampXaxis.y);
         }

         m_TPS_Player.SetFloat("SpineAngle", clampX * 1.3f, 0.2f, Time.deltaTime);
         cameraTransform.Rotate(Vector3.left * mouseY);
         transform.Rotate(Vector3.up * mouseX);
     }
    */
    private void ClampXAxis(float value)
    {
        Vector3 camEuler = cameraTransform.eulerAngles;
        camEuler.x = value;
        cameraTransform.eulerAngles = camEuler;
    }

    private void ClampYAxis(float value)
    {
        Vector3 camEuler = transform.eulerAngles;
        camEuler.y = value;
        transform.eulerAngles = camEuler;
    }

    public void SetCrouch()
    {
        
            if (!crouch)
            {
                crouch = true;
                moveSpeed = crouchSpeed;
            }
            else
            {
                if (CheckDistance() > normalHeight)
                {
                    crouch = false;
                    if (!m_running)
                    {
                        moveSpeed = walkSpeed;
                    } else
                    {
                        moveSpeed = runSpeed;
                    }
                }
            }
        
    }

    private float CheckDistance()
    {
        Vector3 pos = transform.position + characterController.center - new Vector3(0, characterController.height / 2, 0);
        RaycastHit hit;
        if (Physics.SphereCast(pos, characterController.radius, transform.up, out hit, 10, obstacleLayers))
        {
            return hit.distance;
        }
        else
            return 3;
    }

    public void FootStepPlay()
    {
        int r = Random.Range(1, footSteps.Length);
        AS.volume = moveSpeed / 6;
        AS.PlayOneShot(footSteps[r]);
    }


}




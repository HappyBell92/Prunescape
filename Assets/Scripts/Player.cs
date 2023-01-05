using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed = 3.5f;
    public float runSpeed = 6;

    public bool running;
    public float stamina;
    public float drainRate = 1f;
    public float rechargeRate = 1f;

    [SerializeField] private Camera mainCamera;

    public int maxStamina = 100;

    public InventoryObject inventory;

    public StaminaBarScript staminaBar;
    public GameObject runActive;
    public GameObject clickWindow;
    public RightClickMenu rcMenu;

    Vector3 clickPosition;

    public GameObject moveArrow;
    public GameObject moveArrowPrefab;

    public GameObject target;
    public GameObject rcTarget;
    public GameObject rcObject;
    public bool hasTarget;
    public bool isInteracting;
    public bool interactTimeOn;
    Coroutine interactco;
    Transform interactLocation;

    public LayerMask moveLayer;
    public LayerMask arrowLayer;
    public LayerMask interactLayer;
    private Rigidbody rb;

    private bool destroying;
    public bool movingToInteract;
    public float destinationThreshhold;

    NavMeshAgent myAgent;

    public MovementState state;
    public enum MovementState
    {
        walking,
        Running
    }
    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        stamina = maxStamina;
        staminaBar.setMaxStamina(maxStamina);
        running = false;
        destroying = false;
        runActive.SetActive(false);
        movingToInteract = false;
        hasTarget = false;
        isInteracting = false;
        interactTimeOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingToInteract)
        {
            MoveToInteract();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            myAgent.isStopped = false;
            if (!EventSystem.current.IsPointerOverGameObject() || EventSystem.current.tag == "ClickThrough")
            {
                DisableOutline();
                clickWindow.SetActive(false);
                Ray rayOrigin = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hitInfo;

                moveArrow = GameObject.Find("MoveArrowPrefab(Clone)");
                Destroy(moveArrow);

                // Select interactable object and move to it
                if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity, interactLayer))
                {
                    target = hitInfo.collider.gameObject;
                    movingToInteract = true;
                }
                // move to clicked ground location
                else if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity, moveLayer))
                {
                    if (target != null)
                    {
                        if(target.gameObject.tag != "Item")
                        {
                            ObjectCompassScript compass = target.GetComponent<ObjectCompassScript>();
                            if (compass)
                            {
                                compass.compassParent.SetActive(false);
                            }
                        }
                    }
                    target = null;
                    isInteracting = false;
                    myAgent.stoppingDistance = 0;
                    myAgent.SetDestination(hitInfo.point);
                    Vector3 newPosition = hitInfo.point + new Vector3(0, 0f, 0);
                    Instantiate(moveArrowPrefab, newPosition, Quaternion.identity);
                }
            }
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Debug.Log("Click!");
            Ray rayOrigin = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hitInfo;

            if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity, interactLayer))
            {
                rcTarget = hitInfo.collider.gameObject;
            }
            else
            {
                rcTarget = null;
            }

            if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity, moveLayer))
            {
                
                Debug.Log("Boop");
                clickPosition = hitInfo.point;
                clickWindow.transform.position = Input.mousePosition;
                clickWindow.SetActive(true);
                rcMenu.Activate();
            }
        }

        //if (Mouse.current.rightButton.wasPressedThisFrame) // stops player mid movement testing purposes only. needs to be removed later
        //{
        //    myAgent.isStopped = true;
        //    moveArrow = GameObject.Find("MoveArrowPrefab(Clone)");
        //    Destroy(moveArrow);
        //}

        if (isInteracting && target.gameObject.tag != "Item")
        {
            ObjectCompassScript compass = target.GetComponent<ObjectCompassScript>();
            if (compass)
            {
                transform.LookAt(new Vector3(compass.lookAt.transform.position.x, transform.position.y, compass.lookAt.transform.position.z));
            }
            
        }
        if (!isInteracting && interactTimeOn)
        {
            Debug.Log("Stopped interaction");
            StopCoroutine(interactco);
            interactTimeOn = false;
        }

        // If player cannot reach target destory move arrow
        if(!destroying && myAgent.velocity.magnitude < 0.01f)
        {
            StartCoroutine(DestoryArrow());
        }
        if(destroying && myAgent.velocity.magnitude > 0.01f)
        {
            destroying = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inventory.Load();
        }
        

        myAgent.speed = moveSpeed;
        StateHandler();
        RunEnergy();
    }

    private void StateHandler()
    {
        // Walking
        if (!running)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            runActive.SetActive(false);
        }

        // Running
        else if (running)
        {
            state = MovementState.Running;
            moveSpeed = runSpeed;
            runActive.SetActive(true);
        }
    }

    private void RunEnergy()
    {
        // Stamina drain and regen
        // stamina min and max value
        stamina = Mathf.Clamp(stamina, 0, 100);
        // drains stamina if running and player is moving
        if (running && myAgent.velocity.magnitude > 0 && stamina > 0)
        {
            stamina -= Time.deltaTime * drainRate;

            staminaBar.SetStamina(stamina);
        }
        // stamina regen if running is enabled but player is stationary
        else if(running && myAgent.velocity.magnitude < 0.1f && stamina < 100)
        {
            stamina += Time.deltaTime * rechargeRate;

            staminaBar.SetStamina(stamina);
        }
        // stamina regen if running is not enabled
        else if(!running && stamina < 100)
        {
            stamina += Time.deltaTime * rechargeRate;

            staminaBar.SetStamina(stamina);
        }
        // turns off running if stamina reaches 0
        if (stamina == 0)
        {
            running = false;
        }
    }

    public void RunButton()
    {
        // Run button being controller by external UI
        if (!running)
        {
            running = true;
        }
        
        else if (running)
        {
            running = false;
        }
    }

    private IEnumerator DestoryArrow()
    {
        // destroy move arrow after 3 seconds
        destroying = true;
        yield return new WaitForSeconds(3);
        if(destroying && myAgent.velocity.magnitude < 0.01f)
        {
            moveArrow = GameObject.Find("MoveArrowPrefab(Clone)");
            Destroy(moveArrow);
            destroying = false;
        }
    }

    private void InteractPicker()
    {
            ObjectCompassScript compass = target.GetComponent<ObjectCompassScript>();
            if (compass)
            {
                compass.compassParent.SetActive(false);
            }

        // if target is a tree do this
        if (target.gameObject.tag == "Tree")
        {
            TreeScript treeScript = target.GetComponent<TreeScript>();
            if (treeScript)
            {
                DisableOutline();

                InteractInventoryAdd();
                treeScript.ChopDown();
                target = null;
                isInteracting = false;
            }
        }
    }

    public void ItemPickup()
    {
        if (target.gameObject.tag == "Item")
        {
            var item = target.GetComponent<Item>();
            if (item)
            {
                inventory.AddItem(item.item, 1);
                Destroy(target.gameObject);
            }
            target = null;
            isInteracting = false;
        }
    }

    IEnumerator InteractTime()
    {
        isInteracting = true;
        interactTimeOn = true;
        Debug.Log("Chop");
        yield return new WaitForSeconds(1);
        Debug.Log("Chop");
        yield return new WaitForSeconds(1);
        Debug.Log("Chop");
        yield return new WaitForSeconds(1);
        Debug.Log("Chop");
        InteractPicker();
        interactTimeOn = false;
    }

    public void MoveToInteract()
    {
        if(target != null)
        {
            if(target.gameObject.tag == "Item")
            {
                destinationThreshhold = 1;
                myAgent.SetDestination(target.gameObject.transform.position);
            }

            if(target.gameObject.tag != "Item")
            {
                destinationThreshhold = 9;
                ObjectCompassScript compass = target.GetComponent<ObjectCompassScript>();
                if (compass)
                {
                    compass.compassParent.SetActive(true);
                    myAgent.SetDestination(compass.compassLocation.gameObject.transform.position);
                }
            }
            EnableOutline();

            // check if player has reached the target object then start interaction
            float distanceToTarget = Vector3.SqrMagnitude(transform.position - target.transform.position);
            if (distanceToTarget < destinationThreshhold)
            {
                Debug.Log("Arrived");
                movingToInteract = false;
                //myAgent.isStopped = true;
                if(target.gameObject.tag == "Item")
                {
                    ItemPickup();
                }
                else
                {
                    interactco = StartCoroutine(InteractTime());
                }
                
            }
        }

        
    }

    public void ClickMove()
    {
        Debug.Log("Moving Now");
        if (target != null)
        {
            if (target.gameObject.tag != "Item")
            {
                ObjectCompassScript compass = target.GetComponent<ObjectCompassScript>();
                if (compass)
                {
                    compass.compassParent.SetActive(false);
                }
            }
            DisableOutline();
        }
        moveArrow = GameObject.Find("MoveArrowPrefab(Clone)");
        Destroy(moveArrow);
        target = null;
        isInteracting = false;
        myAgent.SetDestination(clickPosition);
        Instantiate(moveArrowPrefab, clickPosition, Quaternion.identity);
        clickWindow.SetActive(false);
    }

    public void CloseWindow()
    {
        clickWindow.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        inventory.container.Clear();
    }

    public void InteractInventoryAdd()
    {
        var item = target.GetComponent<Item>();
        if (item)
        {
            inventory.AddItem(item.item, 1);
        }
    }

    public void DisableOutline()
    {
        if(target != null)
        {
            Outline outlineScript = target.GetComponent<Outline>();
            if (outlineScript)
            {
                outlineScript.enabled = false;
            }
        }
        
    }

    public void EnableOutline()
    {
        if (target != null)
        {
            Outline outlineScript = target.GetComponent<Outline>();
            if (outlineScript)
            {
                outlineScript.enabled = true;
            }
        }
    }
}

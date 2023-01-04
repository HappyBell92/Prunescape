using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RightClickMenu : MonoBehaviour
{
    public Player playerScript;
    public GameObject moveButton;
    public GameObject interactButton;
    public GameObject pickupButton;
    public GameObject attackButton;
    public GameObject cancelButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.middleButton.wasPressedThisFrame)
        {
            DeActivate();
        }
    }

    public void Activate()
    {
        foreach(Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        Instantiate(moveButton, transform);

        if (playerScript.rcTarget != null)
        {
            if (playerScript.rcTarget.gameObject.tag == "Tree")
            {
                Instantiate(interactButton, transform);
            }

            if (playerScript.rcTarget.gameObject.tag == "Item")
            {
                Instantiate(pickupButton, transform);
            }

            if (playerScript.rcTarget.gameObject.tag == "Enemy")
            {
                Instantiate(attackButton, transform);
            }
        }
        
        Instantiate(cancelButton, transform);
    }

    public void DeActivate()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}

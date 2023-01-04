using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickButtons : MonoBehaviour
{
    public Player playerScript;

    private void Awake()
    {
        playerScript = GameObject.Find("Player").GetComponent<Player>();
    }

    private void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<Player>();
    }

    public void MoveHere()
    {
        playerScript.ClickMove();
        playerScript.CloseWindow();
    }

    public void Cancel()
    {
        
        playerScript.rcTarget = null;
        playerScript.CloseWindow();
    }

    public void Interact()
    {
        playerScript.target = playerScript.rcTarget;
        playerScript.rcTarget = null;
        playerScript.movingToInteract = true;
        playerScript.CloseWindow();
    }

    public void PickUp()
    {
        playerScript.target = playerScript.rcTarget;
        playerScript.rcTarget = null;
        playerScript.movingToInteract = true;
        playerScript.CloseWindow();
    }

    public void Attack()
    {
        playerScript.CloseWindow();
    }
}

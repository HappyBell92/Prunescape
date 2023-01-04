using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    public bool felled;
    public bool growing;
    public float growTime;
    public Collider treeCollider;
    public Renderer treeRenderer;
    public Player playerScript;
    public Outline outline;
    public Item item;

    // Start is called before the first frame update
    void Start()
    {
        felled = false;
        growing = false;
        treeCollider.enabled = true;
        treeRenderer.enabled = true;
        playerScript = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (felled)
        {
            treeCollider.enabled = false;
            treeRenderer.enabled = false;
            if (!growing)
            {
                StartCoroutine(Regrow());
            }
        }

        else if (!felled)
        {
            treeCollider.enabled = true;
            treeRenderer.enabled = true;
        }
    }

    public void ChopDown()
    {
        felled = true;
    }

    //public void IsBeingTargeted()
    //{
    //    outline.enabled = true;
    //}

    private IEnumerator Regrow()
    {
        //Debug.Log("Regrowth Started");
        growing = true;
        yield return new WaitForSeconds(growTime);
        felled = false;
        growing = false;
        //Debug.Log("I Have Grown");
    }
}

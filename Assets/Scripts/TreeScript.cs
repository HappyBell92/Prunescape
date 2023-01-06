using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    public bool infinite;
    public bool felled;
    public bool growing;
    public float growTime;
    public Collider treeCollider;
    public Renderer treeRenderer;
    public Renderer shadowRenderer;
    public Renderer stumpRenderer;
    public Player playerScript;
    public Outline outline;

    // Start is called before the first frame update
    void Start()
    {
        felled = false;
        growing = false;
        treeCollider.enabled = true;
        treeRenderer.enabled = true;
        shadowRenderer.enabled = true;
        stumpRenderer.enabled = false;
        playerScript = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (felled)
        {
            treeCollider.enabled = false;
            treeRenderer.enabled = false;
            shadowRenderer.enabled = false;
            stumpRenderer.enabled = true;
            if (!growing)
            {
                StartCoroutine(Regrow());
            }
        }

        else if (!felled)
        {
            treeCollider.enabled = true;
            treeRenderer.enabled = true;
            shadowRenderer.enabled = true;
            stumpRenderer.enabled = false;
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

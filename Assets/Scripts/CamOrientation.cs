using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamOrientation : MonoBehaviour
{
    public float speedX;
    public float speedY;
    float xRotation = 0f;
    float yRotation = 0f;
    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = player.transform.position;
    }

    private void FixedUpdate()
    {
        RotateX();
        RotateY();
    }

    private void LateUpdate()
    {
        
    }

    void RotateX()
    {
        float rotX = Input.GetAxis("Vertical") * speedX * Time.deltaTime;

        xRotation += rotX;
        xRotation = Mathf.Clamp(xRotation, -15, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void RotateY()
    {
        float rotY = Input.GetAxis("Horizontal") * speedY * Time.deltaTime;

        yRotation += rotY;

        transform.rotation = Quaternion.Euler(0, yRotation, 0f);
    }
}

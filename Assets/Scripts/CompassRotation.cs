using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassRotation : MonoBehaviour
{
	public Transform player;
	public Vector3 target;
	void Start()
	{
		player = GameObject.FindWithTag("Player").transform;
	}

	void Update()
	{
		if (player.gameObject.CompareTag("Player"))
		{
			target = transform.position + (player.position - transform.position); // Vector from self to player
			target = Vector3.ProjectOnPlane(target - transform.position, transform.up) + transform.position; //Flatten vector against own up direction
			transform.LookAt(target, transform.parent.transform.up); //Convert vector to transform rotation quaternion
			Debug.DrawLine(transform.position, target, Color.red, .1f);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ScaleGravity : MonoBehaviour {

	public float gravityScale = 1.0f;
	private float earthGravity = -9.80665f; //m/s^2 
	private Vector3 gravity;
	private Rigidbody rigidBody;
		// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
		rigidBody.useGravity = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate()
	{
		gravity = earthGravity * gravityScale * Vector3.up;
		rigidBody.AddForce (gravity, ForceMode.Acceleration);
	}
}

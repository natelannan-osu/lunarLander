using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour {

	public GameObject shipTarget;
	private float orbitDegrees;
	private Vector3 rotationMovement;
	private Vector3 offset;
	private float offsetY;
	private float radius;
	private float resetRotation;

	// Use this for initialization
	void Start () {
		offset = transform.position - shipTarget.transform.position;
		resetRotation = 0;
	}
	
	// Update is called once per frame
	void Update () {
		orbitDegrees = -Input.GetAxis ("Horizontal");
		resetRotation -= orbitDegrees;
	}

	void LateUpdate () {  
		transform.position = shipTarget.transform.position + offset;
		transform.RotateAround (shipTarget.transform.position, Vector3.up, orbitDegrees);
		offset = transform.position - shipTarget.transform.position;
	}

	public void resetCamera(){
		transform.position = shipTarget.transform.position + offset;
		transform.RotateAround (shipTarget.transform.position, Vector3.up, resetRotation);
		resetRotation = 0;
		offset = transform.position - shipTarget.transform.position;
	}
}

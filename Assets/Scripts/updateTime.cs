using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class updateTime : MonoBehaviour {

	//public Text displayTime;
	private float timer = 0f;
	private float min, sec;
	private Text time;

	// Use this for initialization
	void Start () {
		time = gameObject.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		min = (int)Mathf.Floor (timer / 60);
		sec = timer - (min * 60);
		displayTime ();
	}

	void displayTime (){
		if (sec < 10) {
			time.text = " Time: " + min.ToString () + ":0" + sec.ToString ("F2");
		} else {
			time.text = " Time: " + min.ToString () + ":" + sec.ToString ("F2");
		}
	}
}

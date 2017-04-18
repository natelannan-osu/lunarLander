using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour {

    public int bonusFactor;
    public shipController ship;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.Rotate(new Vector3(0, 0, .4f));
    }

    void OnTriggerEnter(Collider other) {
        ship.bonusFactor *= bonusFactor;
    }
}

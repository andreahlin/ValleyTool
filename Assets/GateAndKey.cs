using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAndKey : MonoBehaviour {

    // todo: why do i need this in order to make the Gameobjects a rigidbody? 

    public float speed = 0.001f;// whyyyyyy

	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    void Update () {
        Vector3 up = new Vector3(1, 1, -1);
        transform.Rotate(up, speed * Time.deltaTime);
    }
}

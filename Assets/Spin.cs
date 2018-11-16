using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {

    public float speed = 60f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 up = new Vector3(1,1,-1); 
        transform.Rotate(up, speed * Time.deltaTime);
    }
}

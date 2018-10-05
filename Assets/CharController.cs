using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo: no falling over, and make it work with Nodes
public class CharController : MonoBehaviour {

    Vector3 targetPosition;
    Vector3 lookTarget;
    Quaternion playerRot;
    float rotSpeed = 5f;
    float speed = 10f;
    bool moving = false;

	// Use this for initialization
	void Start () {		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0)) 
        {
            SetTargetPosition(); 
        }
        if (moving) 
        {
            Move();
        }
    }

    void SetTargetPosition() {
        // raycasting to find the position of mouseclick 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10000))
        {
            targetPosition = hit.point;

            // todo: this is a hard-coded value : bad 
            targetPosition.y = 0.5f; 

            lookTarget = new Vector3(targetPosition.x - transform.position.x,
                                     transform.position.y,
                                     targetPosition.z - transform.position.z);
            playerRot = Quaternion.LookRotation(lookTarget);
            moving = true;
        }
    }

    void Move()
    {
        // change rotation of char using slerp (change rotation smoothly)
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                              playerRot,
                                              rotSpeed * Time.deltaTime);
        // move position
        transform.position = Vector3.MoveTowards(transform.position,
                                                 targetPosition,
                                                 speed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            moving = false;
        }
    }
}

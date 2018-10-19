using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo: no falling over, and make it work with Nodes
public class CharController : MonoBehaviour {

    Vector3 targetPosition;
    Vector3 lookTarget;
    Quaternion playerRot;
    public float rotSpeed = 5f;
    public float speed = 3f;
    bool moving = false;

    Rigidbody rb;


    public Node currNode; // the node that the character is currently associated with
    public Node targetNode; // the node that the character is closest to from the click 

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetMouseButton(0)) // todo comment all this back in? 
        //{
        //    SetTargetPosition();
             
        //}
        //if (moving) 
        //{
        //    Move();
        //}
    }

    public void AssignCurrNode(List<Node> nodesInScene) {
        // find the Node that the char is closest to
        float closestDist = 1000f; 
        foreach (Node n in nodesInScene)
        {
            float dist = Vector3.Distance(this.transform.position, n.position);

            if (dist <= 0.5f) 
            {
                if (dist < closestDist)
                {
                    currNode = n;
                    closestDist = dist; 
                }
            }
        }

        // color the current node red (todo: get rid of) 
        GameObject visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visPos.transform.position = currNode.position;
        visPos.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        Renderer rend = visPos.GetComponent<Renderer>();
        rend.material.SetColor("_Color", Color.magenta);

        // find the nearest Node to the mouse click (in another function rn) 

        // run pathfinding and find the path list

        // move through the path list Node by Node until character reaches target

    }

    public void WalkAlongPath(List<Node> path)
    {
        // move position
        //transform.position = Vector3.MoveTowards(transform.position,
        //path[1].position,
        //speed * Time.deltaTime);

        foreach (Node nextNode in path)
        {
            while (!(Vector3.Distance(transform.position, nextNode.position) < 0.01f))
            {
                Vector3 nextPos = nextNode.position; //+ nextNode.up * .5f; // why does it freeze up? confused 
                // only hitting the CUBE, not the face 
                transform.position = Vector3.MoveTowards(transform.position,
                                                         nextPos,
                                                         speed * Time.deltaTime);
                Debug.Log(nextNode.up); 
            }

            //Node nowNode = path[1];

            //while (Vector3.Distance(transform.position, nowNode.position) < 0.5f) {
            //    transform.position = Vector3.MoveTowards(transform.position,
            //    nowNode.position,
            //    speed * Time.deltaTime);

            //}

            //todo: need to translate smoothly between the positions 
            //transform.position = nextNode.position;
            // how to hold this? 

            // color the closest Node from mouse click (todo: get rid of debugvis) 
            //GameObject visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //visPos.transform.position = nextNode.position;
            //visPos.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            //Renderer rend = visPos.GetComponent<Renderer>();
            //rend.material.SetColor("_Color", Color.red);
        }
    }


    public void SetTargetPosition(List<Node> nodesInScene) {
        // raycasting to find the position of mouseclick 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000))
        {
            targetPosition = hit.point;
            //Debug.DrawLine(ray.origin, hit.point);
            //Debug.Log("the object hit: " + hit.transform.gameObject.transform);

            // todo: this is a hard-coded value : bad 
            //targetPosition.y = 0.5f;

            lookTarget = new Vector3(targetPosition.x - transform.position.x,
                                     transform.position.y, // shouldn't this ensure that the rotation doesn't change for the y axis?
                                     targetPosition.z - transform.position.z);
            playerRot = Quaternion.LookRotation(lookTarget);

            moving = true;
        }

        // find the closest node to the TargetPos todo right place?
        float closestDist = 1000f;
        foreach (Node n in nodesInScene)
        {
            float dist = Vector3.Distance(targetPosition, n.position);

            if (dist <= 1f)
            {
                if (dist < closestDist)
                {
                    //Debug.Log(n.position); 
                    targetNode = n;
                    closestDist = dist;
                }
            }
        }
        // debug vis for clicked Node
        // color the closest Node from mouse click (todo: get rid of debugvis) 
        GameObject visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visPos.transform.position = targetNode.position;
        visPos.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        Renderer rend = visPos.GetComponent<Renderer>();
        rend.material.SetColor("_Color", Color.yellow);
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

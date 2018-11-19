using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo: no falling over, and make it work with Nodes
public class CharController : MonoBehaviour {

    Vector3 targetPosition;
    Vector3 lookTarget;
    Quaternion playerRot;
    public float rotSpeed = 5f;
    public float speed = 1f;
    bool moving = false;
    Rigidbody rb;

    bool keyButtonPressed = false; 

    // SENT BY THE MAIN CLASS TO BE CONTROLLED OVER HERE .. OK? 
    public Vector3 keyPos = new Vector3(-1,-1,-1);
    public GameObject gate1 = null;
    public GameObject gate2 = null; 

    Vector3 forward = new Vector3(0, 0, 1);
    Vector3 right = new Vector3(1, 0, 0);


    public Node currNode; // the node that the character is currently associated with
    public Node targetNode; // the node that the character is closest to from the click 

    private void OnCollisionEnter(Collision collision)
    {
        // todo: include some sort of count 
        switch (collision.gameObject.tag)
        {
            case "Prize":
                //Debug.Log("prize hit");
                Destroy(collision.collider.gameObject);
                break;
            case "FinalPrize":
                Destroy(collision.collider.gameObject);

                //Debug.Log("final prize hit");
                //collision.collider.gameObject.transform.position += new Vector3(0, 1, 0);
                //collision.collider.gameObject.GetComponent<Rigidbody>;
                //rigidbody.velocity = Vector3.zero;
                //rigidbody.angularVelocity = Vector3.zero;

                //Destroy(collision.collider.gameObject);
                // todo: this is where game over happens 
                break;
            case "Gate":
                Debug.Log("collision with gate"); 
                break;
            case "Key":
                Debug.Log("collision with key"); 
                break;
        }

    }

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
    }
	
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Movem();
        }

        // checking if the key button was pressed 
        if (!keyPos.Equals(new Vector3(-1, -1, -1)))
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.z);
            Vector2 keyPos2 = new Vector2(keyPos.x, keyPos.z);
            if (Vector2.Distance(pos, keyPos2) < 0.3) // todo: change this .. to much leeway 
            {
                // then do something about the gates ... ok 
                //Debug.Log("PRESEESSSS");
                MoveGate();
                keyButtonPressed = true;
            }
        }
    }

    void MoveGate()
    {
        float gateSpeed = .5f;
        if (gate1)
        {
            if (gate1.transform.position.y > -1) 
            {
                gate1.transform.position += new Vector3(0, -1, 0) * gateSpeed * Time.deltaTime;
            }
        }
        if (gate2)
        {
            if (gate2.transform.position.y > -1)
            {
                gate2.transform.position += new Vector3(0, -1, 0) * gateSpeed * Time.deltaTime;
            }
        }
    }

    void Movem()
    {
        // todo: check if the next step is a gate !!!!! 

        bool move = false;
        Node newCurr = null;
        Dictionary<string, Vector3> potentialDir = new Dictionary<string, Vector3>();

        Vector3 charDisplacement = new Vector3(0,0,0);// = new Vector3(-1, -1, -1);

        if (Input.GetKeyDown(KeyCode.W)) // +z 
        {
            Debug.Log("w key hit");
            potentialDir.Add("plane", new Vector3(0, 0, 1));
            potentialDir.Add("realLadderPosZ", new Vector3(0, .5f, .5f));
            potentialDir.Add("unrealLadderPosZ", new Vector3(-1, .5f, -1.5f));
        }
        else if (Input.GetKeyDown(KeyCode.S)) // -z 
        {
            Debug.Log("s key hit");
            potentialDir.Add("plane", new Vector3(0, 0, -1));
            potentialDir.Add("realLadderNegZ", new Vector3(0, -.5f, -.5f));
            potentialDir.Add("unrealLadderNegZ", new Vector3(1, -.5f, 1.5f));
        }
        else if (Input.GetKeyDown(KeyCode.D)) // +x 
        {
            Debug.Log("d key hit");
            potentialDir.Add("plane", new Vector3(1, 0, 0));
            potentialDir.Add("realLadderPosX", new Vector3(.5f, .5f, 0));
            potentialDir.Add("unrealLadderPosX", new Vector3(-1.5f, .5f, -1));
        }
        else if (Input.GetKeyDown(KeyCode.A)) // -x 
        {
            Debug.Log("a key hit");
            potentialDir.Add("plane", new Vector3(-1, 0, 0));
            potentialDir.Add("realLadderPosX", new Vector3(-.5f, -.5f, 0));
            potentialDir.Add("unrealLadderPosX", new Vector3(1.5f, -.5f, 1));
        }
        foreach (KeyValuePair<string, Vector3> pair in potentialDir)
        {
            if (currNode != null)
            {
                Vector3 direction = pair.Value;
                Vector3 dirToCheck = currNode.position + direction;

                foreach (Node n in currNode.neighList)
                {
                    // checks if one of the neighbors can be walked to 
                    if (Vector3.Distance(dirToCheck, n.position) < 0.01)
                    {
                        // check what kind of neighbor we're dealing with
                        switch (pair.Key)
                        {
                            // todo: why is the realLadder case bugging out when you go back and forth ???? IDK 
                            case "plane":
                                Debug.Log("plane");
                                if (charDisplacement.Equals(new Vector3(0,0,0)))
                                {
                                    charDisplacement = direction;
                                }
                                // direction; // vanilla 
                                break;
                            case "realLadderPosZ":
                                Debug.Log("realLadderPosZ");

                                //charDisplacement = new Vector3(0, 0.75f, 0.75f); // todo: hard coded for a char with scale (.5,.5,.5) ):< bad
                                break;
                            case "unrealLadderPosZ":
                                Debug.Log("unrealLadderPosZ");

                                break;
                            case "realLadderNegZ":
                                Debug.Log("realLadderNegZ");

                                //charDisplacement = new Vector3(0, -0.75f, -0.75f);
                                break;
                            case "unrealLadderNegZ":
                                Debug.Log("unrealLadderNegZ");

                                break;
                            case "realLadderPosX":
                                Debug.Log("realLadderPosX");

                                //charDisplacement = new Vector3(0.75f, 0.75f, 0);
                                break;
                            case "unrealLadderPosX":
                                Debug.Log("unrealLadderPosX");

                                break;
                            case "realLadderNegX":
                                Debug.Log("realLadderNegX");

                                //charDisplacement = new Vector3(-0.75f, -0.75f, 0);
                                break;
                            case "unrealLadderNegX":
                                Debug.Log("unrealLadderNegX");

                                break;
                            default:
                                Debug.Log("didn't find a direction... BUG ooo:");
                                return; 
                        }

                        move = true;
                        newCurr = n; 
                    }
                }
            }
            if (move)
            {
                Debug.Log("direction taken: " + charDisplacement); 
                transform.position += charDisplacement * 1/3f; // todo: right now it's skipping, would like for it to have a smooth movement 
                // todo: why is the position freakin 1/3???? 
                AssignCurrNode(newCurr);
                Debug.Log("new node pos: " + newCurr.position);
            }
            //////////////////////////////

            //foreach (Vector3 direction in potentialDirections) 
            //{
            //    Vector3 dirToCheck = currNode.position + direction;

            //    //Debug.Log("direction checking: " + dirToCheck); 
            //    // check if it is one of the planar neighbors 
            //    foreach (Node n in currNode.neighList)
            //    {
            //        //Debug.Log("neighbor position: " + n.position); 
            //        if (Vector3.Distance(dirToCheck, n.position) < 0.01)
            //        {
            //            // checking for ladders 
            //            if (Mathf.Abs(direction.y) > 0.01f) 
            //            {
            //                switch (ladderType) 
            //                {
            //                    // TODO: FIX THIS JAWNS 
            //                    case "ladderPosZ": // todo: there are multiple cases. how to distinguish? 
            //                        //trueDir = new Vector3(0, (1 - this.transform.localScale.y) * .5f, 
            //                        //(1 - this.transform.localScale.z) * .5f);
            //                        trueDir = new Vector3(0, 0.75f, 0.75f); // todo: this is specific to a cube that is scaled to (.5,.5,.5) oo: 
            //                        // this is also for the OVER CORNER CASE!!!! todo 
            //                        break;
            //                    case "ladderNegZ":
            //                        //trueDir = new Vector3(0, (1 - this.transform.localScale.y) * -.5f, 
            //                        //(1 - this.transform.localScale.z) * -.5f);
            //                        trueDir = new Vector3(0, -0.75f, -0.75f);

            //                        break;
            //                    case "ladderPosX":
            //                        //trueDir = new Vector3((1 - this.transform.localScale.x) * .5f,
            //                        //(1 - this.transform.localScale.y) * .5f, 0);
            //                        trueDir = new Vector3(0.75f, 0.75f, 0);
            //                        break;
            //                    case "ladderNegX":
            //                        //trueDir = new Vector3((1 - this.transform.localScale.x) * -.5f,
            //                        //(1 - this.transform.localScale.y) * -.5f, 0);
            //                        trueDir = new Vector3(-0.75f, -0.75f, 0);

            //                        break;
            //                    default:
            //                        Debug.Log("didn't find anyhin ooo:");
            //                        return;
            //                }
            //                //if (direction.y > 0) 
            //                //{
            //                //    Vector3 scaledDir = new Vector3((1 - this.transform.localScale.x) * .5f, 
            //                //                                    (1 - this.transform.localScale.y) * .5f, 
            //                //                                    (1 - this.transform.localScale.z) * .5f);
            //                //    trueDir = scaledDir;
            //                //}
            //                //else if (direction.y < 0) 
            //                //{
            //                //    // going down (negative)
            //                //    Vector3 scaledDir = new Vector3((-1 + this.transform.localScale.x) * .5f, 
            //                //                                    (-1 + this.transform.localScale.y) * .5f, 
            //                //                                    (-1 + this.transform.localScale.z) * .5f);
            //                //    trueDir = scaledDir;
            //                //}
            //            }
            //            else {
            //                trueDir = direction;
            //            }
            //            //trueDir = direction; 
            //            move = true;
            //            newCurr = n;
            //        }
            //    }
            //}
            //if (move)
            //{
            //    transform.position += direction; // todo: right now it's skipping, would like for it to have a smooth movement 
            //    AssignCurrNode(newCurr);
            //    Debug.Log("new node pos: " + newCurr.position);
            //}

            //Vector3 movement = direction;// direction * speed * Time.deltaTime;
            //transform.position += movement;
        }

    }


    bool CanWalk(Vector3 direction) 
    {
        // well you should always be on a node ? i guess so? 
        // what to check: if you are <0.5 from any given node
        // NEEDS TO BE A NODE BASED SYS??????AJGJSLKDGJ:LAJ
        // check: for gates  
        return true; 
    }



    public void AssignFirstCurrNode(List<Node> nodesInScene)
    {
        // find the Node that the char is closest to
        float closestDist = 1000f;
        foreach (Node n in nodesInScene)
        {
            float dist = Vector3.Distance(this.transform.position, n.position);

            if (dist <= 0.51f)
            {
                if (dist < closestDist)
                {
                    currNode = n;
                    closestDist = dist;
                }
            }
        }
        
        Debug.Log("current node position: " + currNode.position); 

        // todo throw error if no current node is found ... stop game? 

        // color the current node red (todo: get rid of) 
        //GameObject visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //visPos.transform.position = currNode.position;
        //visPos.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        //Renderer rend = visPos.GetComponent<Renderer>();
        //rend.material.SetColor("_Color", Color.black);
    }

    //public void AssignNextNode(Node lastNode)
    //{
    //    if (lastNode != null) 
    //    {
    //        List<Node> nodesToConsider = lastNode.neighList;
    //        nodesToConsider.Add(lastNode);
    //        float closestDist = 1000f;
    //        foreach (Node n in nodesToConsider)
    //        {
    //            float dist = Vector3.Distance(this.transform.position, n.position);
    //            if (dist <= 0.51f)
    //            {
    //                if (dist < closestDist)
    //                {
    //                    currNode = n;
    //                    closestDist = dist;
    //                }
    //            }
    //        }
    //        if (closestDist > 999f)
    //        {
    //           // currNode = null;
    //            // todo: if no next node is found, then return the original node? 
    //        }
    //    }
    //}

    public void AssignCurrNode(Node n)
    {
        currNode = n; 
    }


    public void WalkAlongPath(List<Node> path)
    {
        foreach (Node nextNode in path)
        {
            if (!nextNode.Equals(path[0])) 
            {
                Vector3 truePos = nextNode.position + nextNode.up * 0.25f;
                while (!(Vector3.Distance(transform.position, truePos) < 0.01f))
                {
                    Vector3 nextPos = truePos;
                    // todo: figure out how to slow dis down!
                    transform.position = Vector3.MoveTowards(transform.position, 
                                                             nextPos,
                                                             0.01f * Time.deltaTime);
                }
            }
        }
        // update the current node for the next path  
        if (path.Count > 0) 
        {
            currNode = path[path.Count - 1];
        }
    }

   public IEnumerator WalkAlongPath2(List<Node> path)
    {
        foreach (Node nextNode in path)
        {
            if (!nextNode.Equals(path[0]))
            {
                Vector3 truePos = nextNode.position + nextNode.up * 0.5f;
                while (!(Vector3.Distance(transform.position, truePos) < 0.01f))
                {
                    Vector3 nextPos = truePos;
                    // todo: figure out how to slow dis down!
                    yield return new WaitForSeconds(0.0f);
                    transform.position = Vector3.MoveTowards(transform.position,
                                                             nextPos,
                                                             0.01f * Time.deltaTime);
                }
            }
        }
        // update the current node for the next path  
        if (path.Count > 0)
        {
            currNode = path[path.Count - 1];
        }
    }

    public void SetTargetPosition(List<Node> nodesInScene) {
        // raycasting to find the position of mouseclick 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000))
        {
            targetPosition = hit.point;

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
                    targetNode = n;
                    closestDist = dist;
                }
            }
        }

        // debug vis for clicked Node
        // color the closest Node from mouse click (todo: get rid of debugvis) 
        //GameObject visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //visPos.transform.position = targetNode.position;
        //visPos.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        //Renderer rend = visPos.GetComponent<Renderer>();
        //rend.material.SetColor("_Color", Color.yellow);
    }

    void Move()
    {
        // change rotation of char using slerp (change rotation smoothly)w
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

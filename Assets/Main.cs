using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject[] pathFaces;
    public List<Node> allNodes = new List<Node>();
    // keeps track of whether something is on the node already 
    public Dictionary<Node, bool> map = new Dictionary<Node, bool>();

    GameObject thePlayer;
    CharController playerScript;
    Camera cam; 
    Graph g;
    Vector3 goalPos;

    // TO SEND TO CHARCONTROLLER
    Vector3 keyPos;
    GameObject gateObjX;
    GameObject gateObjZ;

    // things the user should be able to input
    //int mazeLength;
    //int mazeWidth;
    //int numPrizes;
    //int prizeScore;
    //int obstacleOption; 
    //int characterChoice;
    //int themeChoice;
    // int jams? ; 

    public bool debugMode = true; // todo: not using right now

    // Use this for initialization
    void Start()
    {
        // camera reference
        cam = Camera.main;

        // Maze Algorithm ///////////////////////////////////
        Maze m = new Maze(cam, 10,10,1); // leaves camera view ~12x12 
        goalPos = m.goalPos; 
        m.GenerateMaze();

         //display nodes in debug 
        foreach (Node node in m.allNodes)
        {
            //node.StartDebugVis(cam);

            // add all the nodes from the maze into main todo: good idea? idk 
            allNodes.Add(node);
            map.Add(node, false); 
        }

         //create a graph for later use
        g = new Graph(m.allNodes); // todo: make it allNodes instead of m.allNodes   

        ///////////////////////////////////////////////////


        // Finding Hard-Coded Node Faces ////////////////////
        if (pathFaces.Length == 0) 
        {
            pathFaces = GameObject.FindGameObjectsWithTag("PathFace"); 
        }

        for (int i = 0; i < pathFaces.Length; i++)
        {
            GameObject face = pathFaces[i];
            Vector3 normal = Vector3.Normalize(face.transform.up); // is this always the correct normal? should be 
            Node n = new Node(i, "top", face.transform.position, face.transform.up, face.transform.right);  
            float epsilon = 0.01f; 

            if (normal == new Vector3(0, 1, 0)) 
            {
                n = new Node(i, "top", face.transform.position, face.transform.up, face.transform.right);
            }
            else if (normal == new Vector3(-1, 0, 0)) 
            {
                n = new Node(i, "negx", face.transform.position, face.transform.up, face.transform.right);
            }
            else if (normal == new Vector3(0, 0, -1)) 
            {
                n = new Node(i, "negz", face.transform.position, face.transform.up, face.transform.right);
            }
            else if (normal.x > epsilon || normal.x < -epsilon)
            {
                if (normal.y > 0) 
                {
                    n = new Node(i, "diagx", face.transform.position, face.transform.up, face.transform.right);
                }
            }
            else if (normal.z > epsilon || normal.z < -epsilon)
            {
                if (normal.y > 0)
                {
                    n = new Node(i, "diagz", face.transform.position, face.transform.up, face.transform.right);
                }
            }

            allNodes.Add(n);
        }

        // assign neighbors to nodes automatically
        foreach (Node node in allNodes)
        {
            // todo: commetn back in ?  
            //node.FindGeomNeighbors(allNodes, cam);
            //node.FindIllusionNeighbors(allNodes, cam);


            //node.StartDebugVis(cam);
        }


        // referencing the character variable todo idk if this should be here ? 
        thePlayer = GameObject.Find("Character");
        playerScript = thePlayer.GetComponent<CharController>();
        playerScript.AssignFirstCurrNode(allNodes);
        // TestingFunction(); 
        ///////////////////////////////////////////////////

        MakeGatePlusKey(); // this needs to go first to lay down the gate 
        MakePrizes(10);

        // todo: do something here - send the buttonPos to charController 
        playerScript.keyPos = keyPos;
        if (gateObjX) playerScript.gate1 = gateObjX;
        if (gateObjZ) playerScript.gate2 = gateObjZ; 
    }

    // Update is called once per frame
    void Update()
    {
        // Used for Character Movement   // todo: distinguish between mouse click ? i guass 
        if (Input.GetMouseButtonDown(0)) // when mouseclick is released (once per click)
        {
            playerScript.SetTargetPosition(allNodes);

            // CALL PATHFINDING 
            List<Node> path = g.AStar(playerScript.currNode, playerScript.targetNode);

            // USE THAT TO DIRECT THE PATH OF THE CHARACTER
            playerScript.WalkAlongPath(path); 
            //StartCoroutine(playerScript.WalkAlongPath2(path));
        }
    }

    bool MakeGatePlusKey()
    {
        //todo: how to notify that they have been collided on? 
        // how to prevent the cube from standing on the gate? 

        Color buttonCol = new Color(36 / 255f, 56 / 255f, 36 / 255f);

        // potentially 2 gates  
        Vector3 gateX = goalPos + new Vector3(-1, 0, 0);
        Vector3 gateZ = goalPos + new Vector3(0, 0, -1);
        bool needXGate = false;
        bool needZGate = false;
        foreach (Node curr in allNodes)
        {
            if (curr.position.Equals(gateX)) 
            {
                needXGate = true;
                map[curr] = true; 
            }
            else if (curr.position.Equals(gateZ))
            {
                needZGate = true;
                map[curr] = true;
            }
        }
        if (needXGate)
        {
            gateObjX = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gateObjX.transform.position = goalPos + new Vector3(-1, 0.5f, 0);
            gateObjX.transform.localScale = new Vector3(0.1f, 1f, 0.9f);
            Renderer r1 = gateObjX.GetComponent<Renderer>();
            r1.material.SetColor("_Color", buttonCol);
            //gateObjX.AddComponent<GateAndKey>();
            gateObjX.tag = "Gate";

            gateObjX.AddComponent<Rigidbody>();
            gateObjX.GetComponent<Rigidbody>().useGravity = false;
            gateObjX.GetComponent<Rigidbody>().isKinematic = true;

        }
        if (needZGate)
        {
            gateObjZ = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gateObjZ.transform.position = goalPos + new Vector3(0, 0.5f, -1);
            gateObjZ.transform.localScale = new Vector3(0.9f, 1f, 0.1f);
            Renderer r1 = gateObjZ.GetComponent<Renderer>();
            r1.material.SetColor("_Color", buttonCol);
            //gateObjZ.AddComponent<GateAndKey>();
            gateObjZ.tag = "Gate";

            gateObjZ.AddComponent<Rigidbody>();
            gateObjZ.GetComponent<Rigidbody>().useGravity = false;
            gateObjZ.GetComponent<Rigidbody>().isKinematic = true;

        }

        // place the button at random node 
        var i = Random.Range(0, allNodes.Count);
        Node n = allNodes[i];
        while (!(!n.position.Equals(new Vector3(0, 0, 0)) && !n.position.Equals(goalPos) && map[n] == false
                            && n.up.Equals(new Vector3(0, 1, 0))))
        {
            i = Random.Range(0, allNodes.Count);
            n = allNodes[i];
        }

        keyPos = n.position; 
        GameObject gateButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gateButton.transform.position = n.position + new Vector3(0,0.05f,0);
        gateButton.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);
        gateButton.transform.localEulerAngles = new Vector3(0, 45, 0);
        Renderer r2 = gateButton.GetComponent<Renderer>();
        r2.material.SetColor("_Color", buttonCol);
        //gateButton.AddComponent<GateAndKey>();
        gateButton.tag = "Key"; 
        gateButton.AddComponent<Rigidbody>();
        gateButton.GetComponent<Rigidbody>().useGravity = false;
        gateButton.GetComponent<Rigidbody>().isKinematic = true;

        map[n] = true;

        return true; 
    }

    bool MakePrizes(int numPrizes)
    {
        if (allNodes.Count < 1)
        {
            Debug.Log("allNodes is empty, cannot make prizes");
            return false; 
        }
        Color prizeCol = new Color(171 / 255f, 0 / 255f, 0 / 255f);
        // Big prize (on goal node) 
        GameObject finalPrize = GameObject.CreatePrimitive(PrimitiveType.Cube);
        finalPrize.transform.position = goalPos + new Vector3(0,1,0) * 0.5f;
        finalPrize.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        finalPrize.transform.localEulerAngles = new Vector3(45, 0, 45);
        Renderer r = finalPrize.GetComponent<Renderer>();
        r.material.SetColor("_Color", prizeCol);
        finalPrize.AddComponent<Spin>();
        finalPrize.tag = "FinalPrize";
        finalPrize.AddComponent<Rigidbody>();
        finalPrize.GetComponent<Rigidbody>().useGravity = false; 

        // little prizes 
        List<Node> mixedList = RandomizeList(allNodes);
        int count = 0;
        int index = 0; 
        while (count < numPrizes)  
        {
            Node node = mixedList[index]; 

            if (node.up.Equals(new Vector3(0,1,0))) 
            {
                if (!map[node])
                {
                    if (!node.position.Equals(new Vector3(0,0,0)) && !node.position.Equals(goalPos))
                    {
                        GameObject babyPrize = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        babyPrize.transform.position = node.position + node.up * 0.3f;
                        babyPrize.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        babyPrize.transform.localEulerAngles = new Vector3(45, 0, 45);
                        Renderer r1 = babyPrize.GetComponent<Renderer>();
                        r1.material.SetColor("_Color", prizeCol);
                        babyPrize.AddComponent<Spin>();
                        babyPrize.tag = "Prize";
                        babyPrize.AddComponent<Rigidbody>();
                        babyPrize.GetComponent<Rigidbody>().useGravity = false;

                        count++;
                        map[node] = true;
                    }
                }
            }
            index++; 
        }
        return true; 
    }

    // fisher-yates shuffle
    private List<Node> RandomizeList(List<Node> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
        return ts;
    }


    void TestingFunction()
    {
        /////////// connected component graph testing ///////////////
        //Graph g = new Graph(allNodes);
        //int a = 1;
        //int b = 12; 
        //Debug.Log(g.InSameComponent(g.GetNodeById(a), g.GetNodeById(b)));

        //GameObject visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //visPos.transform.position = g.GetNodeById(a).position;
        //Renderer rend = visPos.GetComponent<Renderer>();
        //rend.material = new Material(Shader.Find("Specular"));
        //rend.material.SetColor("_Color", Color.grey);

        //GameObject visPos2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //visPos2.transform.position = g.GetNodeById(b).position;
        //Renderer rend2 = visPos2.GetComponent<Renderer>();
        //rend2.material = new Material(Shader.Find("Specular"));
        //rend2.material.SetColor("_Color", Color.grey);

        ///////////// minheap testing ///////////////
        //MinHeap h = new MinHeap(20);
        //h.InsertKey(new Node(10));
        //h.InsertKey(new Node(1));
        //h.InsertKey(new Node(5));
        //h.InsertKey(new Node(2));
        //h.InsertKey(new Node(0));
        //h.InsertKey(new Node(100));
        //h.InsertKey(new Node(20));
        //h.InsertKey(new Node(50));
        //h.InsertKey(new Node(22));
        //h.InsertKey(new Node(3));
        //h.InsertKey(new Node(40));
        //h.InsertKey(new Node(33));
        //h.InsertKey(new Node(201));
        //h.InsertKey(new Node(7));
        //h.InsertKey(new Node(9));
        //while(h.heapSize > 0) Debug.Log(h.ExtractMin().Fcost());

        ////////////// a* testing //////////////////
        //Graph g = new Graph(allNodes);
        //int a = 15;
        //int b = 12;
        //List<Node> path = g.AStar(g.GetNodeById(a), g.GetNodeById(b));

        //string outputPath = "( "; 

        // visualization 
        //foreach (Node n in path) 
        //{
        //    outputPath += n.id + " ";
        //    if (!(n.Equals(g.GetNodeById(a)) || n.Equals(g.GetNodeById(b))))
        //    {
        //        GameObject visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //        visPos.transform.position = n.position;
        //        visPos.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        //        Renderer rend = visPos.GetComponent<Renderer>();
        //        //rend.material = new Material(Shader.Find("Specular"));
        //        rend.material.SetColor("_Color", Color.green);
        //    }
        //}
        ////GameObject v2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //v2.transform.position = g.GetNodeById(a).position;
        //v2.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        //Renderer rend2 = v2.GetComponent<Renderer>();
        //rend2.material.SetColor("_Color", Color.green); // new Color(1f, 109 / 255.0f, 25 / 255.0f));

        //v2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //v2.transform.position = g.GetNodeById(b).position;
        //v2.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        //rend2 = v2.GetComponent<Renderer>();
        //rend2.material.SetColor("_Color", Color.red);

        //Debug.Log("Path: " + outputPath + ")"); 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    // goal: generate a path on faces which a character will be able to traverse 
    public GameObject[] pathFaces;
    public List<Node> allNodes = new List<Node>();

    GameObject thePlayer;
    CharController playerScript;
    Camera cam; 
    Graph g;

    public bool debugMode = true; // todo: not using right now 
    
    // Use this for initialization
    void Start()
    {
        // camera reference
        cam = Camera.main;

        // find all PathFaces in the scene
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
            node.FindGeomNeighbors(allNodes, cam);
            node.StartDebugVis(cam);

        }

        // create a graph for later use
        g = new Graph(allNodes);

        // referencing the character variable todo idk if this should be here ? 
        thePlayer = GameObject.Find("Character");
        playerScript = thePlayer.GetComponent<CharController>();
        playerScript.AssignCurrNode(allNodes);

        ///////////// connected component graph testing ///////////////
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

        //////////////// a* testing //////////////////
        //Graph g = new Graph(allNodes);
        //int a = 15;
        //int b = 12;
        //List<Node> path = g.AStar(g.GetNodeById(a), g.GetNodeById(b));

        //string outputPath = "( "; 

        //// visualization 
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
        //GameObject v2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
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

        // Update is called once per frame
        void Update()
    {
        // character movement  
        if (Input.GetMouseButtonUp(0)) // when mouseclick is released (once per click)
        {
            playerScript.SetTargetPosition(allNodes);

            // CALL PATHFINDING 
            List<Node> path = g.AStar(playerScript.currNode, playerScript.targetNode);

            // USE THAT TO DIRECT THE PATH OF THE CHARACTER
            playerScript.WalkAlongPath(path); 
            //StartCoroutine(playerScript.WalkAlongPath2(path));

        }
    }
}

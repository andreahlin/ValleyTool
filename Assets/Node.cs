using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public string id;
    public string face;
    public Vector3 position; // position of center of face, not center of cube 
    public Node[] neighbors; // node or null // [N, W, S, E] 

    public Vector3[] boundaries; // will hold the boundary points for each of the things  
    public List<Node> neighList;  // todo: decide to keep either the array OR the list 

    private GameObject visPos;
    //private GameObject[] visNeigh = new GameObject[4];

    // constructors
    public Node()
    {
        id = " ";
        face = "top";
        position = new Vector3(0, 0, 0);
        neighbors = new Node[4];

        neighList = new List<Node>();
        boundaries = new Vector3[4];

        visPos = null;
        //visNeigh = new GameObject[4];

        setBoundaries(); 
    }

    public Node(string id, string face, Vector3 position, Node[] neighbors)
    {
        this.id = id;
        this.face = face;
        this.position = position;
        this.neighbors = neighbors;

        neighList = new List<Node>();
        boundaries = new Vector3[4];

        visPos = null;
        //visNeigh = new GameObject[4];

        setBoundaries();
    }

    public void setBoundaries() 
    {
        switch (face) 
        {
            case "top":
                boundaries[0] = position + new Vector3(0.5f, 0, 0);
                boundaries[1] = position + new Vector3(0, 0, 0.5f);
                boundaries[2] = position + new Vector3(-0.5f, 0, 0);
                boundaries[3] = position + new Vector3(0, 0, -0.5f);
                break;
            case "bottom":
                break;
            case "north":
            case "east":
            case "south":
            case "west":
                break;
            default:
                break;
        }
    }

    // find neighbors that are next to the cube.
    public void findGeomNeighbors(List<Node> nodeList)
    {
        // if the boundary points are touching, then add as neighbor 
        foreach (Node n in nodeList)
        {
            if (Vector3.Distance(this.position, n.position) <= 1f)
            {
                if (!this.Equals(n))
                {
                    // check each boundary
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            // todo: double checking things?
                            if (boundaries[i] == n.boundaries[j])
                            {
                                neighList.Add(n);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    // todo: make the sphere not a rigid body so that it won't be playable? but the cube must be a rigidbody .. ? right  
    public void StartDebugVis()
    {
        // create position sphere vis
        visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visPos.transform.position = position;
        visPos.name = "vis0-pos";
        visPos.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        (visPos.GetComponent(typeof(SphereCollider)) as Collider).enabled = false;
        Renderer rend = visPos.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Specular"));
        rend.material.SetColor("_Color", Color.blue);

        // show boundaries of all cubes   
        for (int i = 0; i < 4; i++){
            GameObject boundVis = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boundVis.transform.position = boundaries[i];
            boundVis.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            (boundVis.GetComponent(typeof(BoxCollider)) as Collider).enabled = false;
            Renderer rend1 = boundVis.GetComponent<Renderer>();
            rend1.material = new Material(Shader.Find("Specular"));
            rend1.material.SetColor("_Color", Color.blue);
        }

        // todo: show the neighbors
        foreach(Node n in neighList)
        {
            if (!this.Equals(n))
            {
                // check each boundary
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        // todo: double checking things?
                        if (boundaries[i] == n.boundaries[j])
                        {
                            GameObject boundVis = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            boundVis.transform.position = boundaries[i];
                            boundVis.transform.localScale = new Vector3(0.11f, 0.11f, 0.11f);
                            (boundVis.GetComponent(typeof(BoxCollider)) as Collider).enabled = false;
                            Renderer rend1 = boundVis.GetComponent<Renderer>();
                            rend1.material = new Material(Shader.Find("Specular"));
                            rend1.material.SetColor("_Color", Color.green);
                            break;
                        }
                    }
                }
            }

        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // updates the debug node based on changes in the face position
    //public void UpdateDebugPos(Vector3 p)
    //{
    //    position = p;
    //    if (visPos == null)
    //    {
    //        StartDebugVis();
    //    }
    //    else
    //    {
    //        visPos.transform.position = position;
    //        for (int i = 0; i < 4; i++) MoveNeighVis(i);
    //    }
    //}

    //public void MoveNeighVis(int i)
    //{
    //    switch (face)
    //    {
    //        case "top":
    //            switch (i)
    //            {
    //                // North is in the +x direction, counterclockwise 
    //                case 0:
    //                    visNeigh[i].name = "vis0-0";
    //                    visNeigh[i].transform.position = position + new Vector3(0.5f, 0, 0);
    //                    break;
    //                case 1:
    //                    visNeigh[i].name = "vis0-1";
    //                    visNeigh[i].transform.position = position + new Vector3(0, 0, 0.5f);
    //                    break;
    //                case 2:
    //                    visNeigh[i].name = "vis0-2";
    //                    visNeigh[i].transform.position = position + new Vector3(-0.5f, 0, 0);
    //                    break;
    //                case 3:
    //                    visNeigh[i].name = "vis0-3";
    //                    visNeigh[i].transform.position = position + new Vector3(0, 0, -0.5f);
    //                    break;
    //            }
    //            break;
    //        case "bottom":
    //            switch (i)
    //            {
    //                case 0:
    //                    break;
    //                case 1:
    //                    break;
    //                case 2:
    //                    break;
    //                case 3:
    //                    break;
    //            }
    //            break;
    //        case "north":
    //        case "east":
    //        case "south":
    //        case "west":
    //            break;
    //        default:
    //            break;
    //    }
    //}
}

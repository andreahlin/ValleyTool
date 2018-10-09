using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    // goal: generate a path of cubes which a character will be able to traverse 
    public GameObject[] prefabCubes;
    public List<Node> allNodes = new List<Node>(); 
    
    // Use this for initialization
    void Start()
    {
        // find all prefab cubes in the scene
        if (prefabCubes.Length == 0)
        {
            prefabCubes = GameObject.FindGameObjectsWithTag("PathCube"); 
        }
        // assign a new top node to each prefab cube found in scene 
        // todo: assign new Nodes on geometry automatically !  
        int count = 0;
        foreach (GameObject curr in prefabCubes)
        {
            // manually creating a node on the top face 
            Vector3 displace = new Vector3(0f, 0.5f, 0f);
            Node n = new Node(count, "top", curr.transform.position + displace);
            allNodes.Add(n);
            count++; 
        }

        // assign neighbors to nodes automatically
        foreach (Node node in allNodes)
        {
            node.FindGeomNeighbors(allNodes); 
            node.StartDebugVis(); 
        }

        ///////////// graph testing ///////////////
        Graph g = new Graph(allNodes);
        Debug.Log(g.InSameComponent(g.GetNodeById(23), g.GetNodeById(3)));
        //Debug.Log(g.GetNodeById(0).position + ", " + g.GetNodeById(0).position);

        //Debug.Log(g.InSameComponent(g.GetNodeById(0), g.GetNodeById(1)));
        //Debug.Log(g.GetNodeById(0).position + ", " + g.GetNodeById(1).position);

        GameObject visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visPos.transform.position = g.GetNodeById(23).position;
        //visPos.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        //(visPos.GetComponent(typeof(SphereCollider)) as Collider).enabled = false;
        Renderer rend = visPos.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Specular"));
        rend.material.SetColor("_Color", Color.grey);

        GameObject visPos2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visPos2.transform.position = g.GetNodeById(3).position;
        //visPos.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        //(visPos.GetComponent(typeof(SphereCollider)) as Collider).enabled = false;
        Renderer rend2 = visPos2.GetComponent<Renderer>();
        rend2.material = new Material(Shader.Find("Specular"));
        rend2.material.SetColor("_Color", Color.grey);
    }

    // Update is called once per frame
    void Update()
    {
    }
}

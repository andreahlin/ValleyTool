using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    // goal: generate a path on faces which a character will be able to traverse 
    public GameObject[] pathFaces;
    public List<Node> allNodes = new List<Node>();
    
    // Use this for initialization
    void Start()
    {
        // find all PathFaces in the scene
        if (pathFaces.Length == 0) 
        {
            pathFaces = GameObject.FindGameObjectsWithTag("PathFace"); 
        }

        // todo: assign nodes to faces other than "top"  
        for (int i = 0; i < pathFaces.Length; i++)
        {
            
            GameObject face = pathFaces[i];
            Vector3 normal = Vector3.Normalize(face.transform.up); // is this always the right normal? should be 
            Node n = new Node(i, "top", face.transform.position, face.transform.up, face.transform.right); // todo: will change 
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
                if (normal.y > epsilon || normal.y < epsilon) 
                {
                    n = new Node(i, "diagx", face.transform.position, face.transform.up, face.transform.right);
                    Debug.Log("hit");
                }
            }
           
            allNodes.Add(n);
        }

        // assign neighbors to nodes automatically
        foreach (Node node in allNodes)
        {
            node.FindGeomNeighbors(allNodes); 
            node.StartDebugVis(); 
        }

        ///////////// graph testing ///////////////
        Graph g = new Graph(allNodes);
        int a = 1;
        int b = 12; 
        Debug.Log(g.InSameComponent(g.GetNodeById(a), g.GetNodeById(b)));

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
    }

    // Update is called once per frame
    void Update()
    {
    }
}

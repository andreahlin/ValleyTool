using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    // goal: generate a path of cubes which a character will be able to traverse 

    // cubes will be able to have up to 6 nodes 
    Node n1;
    GameObject[] cubes = new GameObject[6];
    Node[] nodes = new Node[6];
    public bool debugMode;

    // Use this for initialization
    void Start()
    {
        debugMode = true; // CAN CHANGE TO TRUE OR FALSE  

        // create cube 1 
        cubes[0] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube1.AddComponent<Rigidbody>();
        cubes[0].transform.position = new Vector3(10, 1, 5);
        cubes[0].name = "Cube 0";
        Renderer rend = cubes[0].GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Specular"));
        rend.material.SetColor("_Color", Color.red);

        // todo this displacement is only for top face... need to be in Node class? 
        Vector3 displace = new Vector3(0f, cubes[0].transform.localScale.y * 0.5f, 0f);

        // create a node on top face
        n1 = new Node("name1", "top", cubes[0].transform.position + displace, new Node[4] { null, nodes[1], null, null });

        // create 4 new cubes
        for (int i = 1; i < 6; i++)
        {
            cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cubes[i] = (GameObject)Instantiate(Resources.Load("cube"));
            cubes[i].transform.position = new Vector3(10, 1, 5 + i);
            cubes[i].name = "Cube " + i;
            rend = cubes[i].GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Specular"));
            rend.material.SetColor("_Color", Color.grey);
            displace = new Vector3(0f, cubes[i].transform.localScale.y * 0.5f, 0f);
            // create node on top face
            nodes[i] = new Node("name " + i, "top", cubes[i].transform.position + displace, new Node[4]);
        }

        // assign neighbors
        for (int i = 1; i < 6; i++)
        {
            Node[] temp = new Node[4];
            //Debug.Log(nodes[4].position); 
            switch (i)
            {
                case 1:
                    nodes[i].neighbors = new Node[] { null, nodes[2], null, n1 };
                    break;
                case 2:
                    nodes[i].neighbors = new Node[] { null, nodes[3], null, nodes[1] };
                    break;
                case 3:
                    nodes[i].neighbors = new Node[] { null, nodes[4], null, nodes[2] };
                    break;
                case 4:
                    nodes[i].neighbors = new Node[] { null, nodes[5], null, nodes[3] };
                    break;
                case 5:
                    nodes[i].neighbors = new Node[] { null, null, null, nodes[4] };
                    break;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cubes[0] != null)
        {
            if (debugMode)
            {
                Vector3 displace = new Vector3(0f, cubes[0].transform.localScale.y * 0.5f, 0f);
                n1.UpdateDebugPos(cubes[0].transform.position + displace);
            }
        }

        for (int i = 1; i < 6; i++)
        {
            if (cubes[i] != null)
            {
                if (debugMode)
                {
                    Vector3 displace = new Vector3(0f, cubes[0].transform.localScale.y * 0.5f, 0f);
                    nodes[i].UpdateDebugPos(cubes[i].transform.position + displace);
                }
            }
        }
    }
}

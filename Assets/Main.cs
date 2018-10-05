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
        foreach (GameObject curr in prefabCubes)
        {
            Vector3 displace = new Vector3(0f, 0.5f, 0f);
            Node n = new Node("name", "top", curr.transform.position + displace, new Node[4]);
            allNodes.Add(n);
        }

        // todo: assign neighbors to nodes automatically! 
        for (int i = 0; i < allNodes.Count; i++) 
        {
            Node curr = allNodes[i];

            switch (i)
            {
                case 0:
                    curr.neighbors = new Node[4] { null, allNodes[1], null, null};
                    break;
                case 1:
                    curr.neighbors = new Node[4] { null, allNodes[2], null, allNodes[0]};
                    break;
                case 2:
                    curr.neighbors = new Node[] { null, null, allNodes[3], allNodes[1] };
                    break;
                case 3:
                    curr.neighbors = new Node[] { allNodes[2], null, allNodes[4], null};
                    break;
                case 4:
                    curr.neighbors = new Node[] { allNodes[3], null, allNodes[5], null};
                    break;
                case 5:
                    curr.neighbors = new Node[] { allNodes[4], null, allNodes[6], null };
                    break;
            }

            curr.StartDebugVis(); 
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}

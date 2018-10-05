using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    // goal: generate a path of cubes which a character will be able to traverse 
    public GameObject[] prefabCubes;
    public List<Node> allNodes; 


    // Use this for initialization
    void Start()
    {
        if (prefabCubes.Length == 0)
        {
            // finding all prefab cubes in the scene
            prefabCubes = GameObject.FindGameObjectsWithTag("PathCube"); 
        }
        foreach (GameObject curr in prefabCubes)
        {
            Vector3 displace = new Vector3(0f, 0.5f, 0.5f);
            Node n = new Node("name", "top", curr.transform.position + displace, new Node[4]);
            allNodes.Add(n);
            n.UpdateDebugPos(n.position); // showing the Node visualization 
        }
        foreach (Node curr in allNodes){
            Debug.Log("Node Position: " + curr.position); 
        }

        // assign neighbors for nodes 
        //    for (int i = 1; i < 6; i++)
        //    {
        //        switch (i)
        //        {
        //            case 1:
        //                nodeArray[i].neighbors = new Node[4] { null, nodeArray[2], null, nodeArray[0] };
        //                break;
        //            case 2:
        //                nodeArray[i].neighbors = new Node[] { null, nodeArray[3], null, nodeArray[1] };
        //                break;
        //            case 3:
        //                nodeArray[i].neighbors = new Node[] { null, nodeArray[4], null, nodeArray[2] };
        //                break;
        //            case 4:
        //                nodeArray[i].neighbors = new Node[] { null, nodeArray[5], null, nodeArray[3] };
        //                break;
        //            case 5:
        //                nodeArray[i].neighbors = new Node[] { null, null, null, nodeArray[4] };
        //                break;
        //        }
        //    }
    }

    // Update is called once per frame
    void Update()
    {
    }
}

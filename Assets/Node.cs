using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    public string id;
    public string face;
    public Vector3 position = new Vector3(4, 4, 4); // position of center of face, not center of cube 
    public Node[] neighbors = new Node[4]; // node or null // [N, W, S, E] 

    // belongs to just this class  
    private GameObject visPos = null;
    private GameObject[] visNeigh = new GameObject[] { null, null, null, null };

    // constructors
    public Node()
    {
        id = "n-1";
        face = "top";
        position = new Vector3(0, 0, 0);
        neighbors = new Node[] { null, null, null, null };
    }

    public Node(string iden, string f, Vector3 pos, Node[] neigh)
    {
        id = iden;
        face = f;
        position = pos;
        neighbors = neigh;
    }

    public void MoveNeighVis(int i)
    {
        switch (face)
        {
            case "top":
                switch (i)
                {
                    // North is in the +x direction, counterclockwise 
                    case 0:
                        visNeigh[i].name = "vis0-0";
                        visNeigh[i].transform.position = position + new Vector3(0.5f, 0, 0);
                        break;
                    case 1:
                        visNeigh[i].name = "vis0-1";
                        visNeigh[i].transform.position = position + new Vector3(0, 0, 0.5f);
                        break;
                    case 2:
                        visNeigh[i].name = "vis0-2";
                        visNeigh[i].transform.position = position + new Vector3(-0.5f, 0, 0);
                        break;
                    case 3:
                        visNeigh[i].name = "vis0-3";
                        visNeigh[i].transform.position = position + new Vector3(0, 0, -0.5f);
                        break;
                }
                break;
            case "bottom":
                switch (i)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                }
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


    // todo: make the sphere not a rigid body so that it won't be playable? but the cube must be a rigidbody .. ? right  
    public void StartDebugVis()
    {
        // create position sphere vis
        visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visPos.transform.position = position;
        visPos.name = "vis0-pos";
        visPos.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        Renderer rend = visPos.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Specular"));
        rend.material.SetColor("_Color", Color.blue);

        // create neighbors vis 
        for (int i = 0; i < 4; i++)
        {
            if (neighbors[i] != null)
            {
                // should be called when there are neighbors 
            }
            else
            {
                // for now:  draw a little black cube in the correct direction 
                visNeigh[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                visNeigh[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                MoveNeighVis(i);

                Renderer rendN = visNeigh[i].GetComponent<Renderer>();
                rendN.material = new Material(Shader.Find("Specular"));
                rendN.material.SetColor("_Color", Color.black);
            }
        }
    }


    // updates the debug node based on changes in the face position
    public void UpdateDebugPos(Vector3 p)
    {
        position = p;
        if (visPos == null)
        {
            StartDebugVis();
        }
        else
        {
            visPos.transform.position = position;
            for (int i = 0; i < 4; i++) MoveNeighVis(i);
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

    void OnDrawGizmos()
    {
        //Debug.Log("n" + neighbors[0]); 
        //Gizmos.color = Color.black;

        //Gizmos.DrawSphere(position, 1f);

        //Gizmos.DrawCube(position + new Vector3(0,0.5f,0), new Vector3(1,1,1));
        //UpdateDebugPos(new Vector3(0,0,0));
    }
}

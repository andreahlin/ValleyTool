using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int id;
    public string face;
    public Vector3 position; // position of center of face 
    public Vector3[] boundaries;  
    public List<Node> neighList;

    // constructors
    public Node()
    {
        id = -1;
        face = "top";
        position = new Vector3(0, 0, 0);
        neighList = new List<Node>();
        boundaries = new Vector3[4];
        SetBoundaries(); 
    }

    public Node(int id, string face, Vector3 position)
    {
        this.id = id;
        this.face = face;
        this.position = position;
        neighList = new List<Node>();
        boundaries = new Vector3[4];
        SetBoundaries();
    }

    public void SetBoundaries() 
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
    public void FindGeomNeighbors(List<Node> nodeList)
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

    public void StartDebugVis()
    {
        // show position sphere vis
        GameObject visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visPos.transform.position = position;
        visPos.name = id.ToString();
        visPos.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        (visPos.GetComponent(typeof(SphereCollider)) as Collider).enabled = false;
        GameObject container = GameObject.Find("DebugGeom");
        visPos.transform.SetParent(container.transform, true);

        Renderer rend = visPos.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Specular"));
        rend.material.SetColor("_Color", Color.blue);

        // todo: take this out later? don't know if it's necessary 
        System.Type mType = System.Type.GetType("NodeVisualizer");
        visPos.AddComponent(mType);

        // show boundaries   
        for (int i = 0; i < 4; i++){
            GameObject boundVis = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boundVis.transform.localPosition = boundaries[i];
            boundVis.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            (boundVis.GetComponent(typeof(BoxCollider)) as Collider).enabled = false;
            boundVis.transform.SetParent(visPos.transform, true); 
            Renderer rend1 = boundVis.GetComponent<Renderer>();
            rend1.material = new Material(Shader.Find("Specular"));
            rend1.material.SetColor("_Color", Color.blue);
        }

        // show neighbors
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
                            GameObject neighVis = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            neighVis.transform.localPosition = boundaries[i];
                            neighVis.transform.localScale = new Vector3(0.11f, 0.11f, 0.11f);
                            (neighVis.GetComponent(typeof(BoxCollider)) as Collider).enabled = false;
                            neighVis.transform.SetParent(visPos.transform, true);
                            Renderer rend1 = neighVis.GetComponent<Renderer>();
                            rend1.material = new Material(Shader.Find("Specular"));
                            rend1.material.SetColor("_Color", Color.green);
                            break;
                        }
                    }
                }
            }
        }
    }
}

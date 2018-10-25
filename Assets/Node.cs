using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int id;
    public string face;
    public Vector3 position; // position of center of face 
    public Vector3 up;
    public Vector3 right;
    public Vector3[] boundaries;  
    public List<Node> neighList;

    public float hCost = 0; // todo: assign later 
    public float gCost = 1; // todo: assign later
    public Node comesFrom = null; 

    // constructors
    public Node()
    {
        id = -1;
        face = "top";
        position = new Vector3(0, 0, 0);
        neighList = new List<Node>();
        boundaries = new Vector3[4];
        up = new Vector3(0, 1, 0);
        right = new Vector3(1, 0, 0); 
        SetBoundaries(); 
    }

    public Node(int id, string face, Vector3 position, Vector3 upVec, Vector3 rightVec)
    {
        this.id = id;
        this.face = face;
        this.position = position;
        neighList = new List<Node>();
        boundaries = new Vector3[4];
        up = upVec;
        right = rightVec; 
        SetBoundaries();
    }

    // for testing purposes only 
    public Node(int i)
    {
        hCost = i;
    }

    public float Fcost()
    {
        return hCost + gCost;
    }


    public void SetBoundaries() 
    {
        switch (face) 
        {
            case "top":
                boundaries[0] = position + 0.5f * right;
                boundaries[1] = position - 0.5f * Vector3.Cross(up, right);
                boundaries[2] = position - 0.5f * right;
                boundaries[3] = position + 0.5f * Vector3.Cross(up, right);
                break;
            case "negx":
                boundaries[0] = position + 0.5f * right;
                boundaries[1] = position - 0.5f * Vector3.Cross(up, right);
                boundaries[2] = position - 0.5f * right;
                boundaries[3] = position + 0.5f * Vector3.Cross(up, right);
                break;
            case "negz":
                boundaries[0] = position + 0.5f * right;
                boundaries[1] = position - 0.5f * Vector3.Cross(up, right);
                boundaries[2] = position - 0.5f * right;
                boundaries[3] = position + 0.5f * Vector3.Cross(up, right);
                break;
            case "diagx":
                boundaries[0] = position + (Mathf.Sqrt(2f) / 2f) * right;
                boundaries[1] = position - 0.5f * Vector3.Cross(up, right); 
                boundaries[2] = position - (Mathf.Sqrt(2f) / 2f) * right;
                boundaries[3] = position + 0.5f * Vector3.Cross(up, right);
                break;
            case "diagz":
                boundaries[0] = position + (Mathf.Sqrt(2f) / 2f) * right;
                boundaries[1] = position - 0.5f * Vector3.Cross(up, right);
                boundaries[2] = position - (Mathf.Sqrt(2f) / 2f) * right;
                boundaries[3] = position + 0.5f * Vector3.Cross(up, right);
                break;
        }
    }

    // find neighbors that are next to the cube.
    public void FindGeomNeighbors(List<Node> nodeList, Camera cam)
    {
        // todo: use epsilon 
        float epsilon = 0.01f;
        // if the boundary points are touching, then add as neighbor 
        foreach (Node n in nodeList)
        {
            if (Vector3.Distance(this.position, n.position) <= 1.5f) // handle the diagonal case
            {
                if (!this.Equals(n))
                {
                    // check each boundary
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            // todo: optimize
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

        // find illusion faces 
        foreach (Node n in nodeList)
        {
            // find the world to screen point 
            //Vector3 screenPos = cam.WorldToScreenPoint(n.position);
            //Debug.Log(screenPos); // how is this calculated? 
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
        System.Type mType = System.Type.GetType("NodeInfoVis");
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

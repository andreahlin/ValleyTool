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
                            if (Mathf.Abs(boundaries[i].x - n.boundaries[j].x) < epsilon)
                            {
                                if (Mathf.Abs(boundaries[i].y - n.boundaries[j].y) < epsilon)
                                {
                                    if (Mathf.Abs(boundaries[i].z - n.boundaries[j].z) < epsilon)
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
        }

        FindIllusionNeighbors(nodeList, cam); 
    }


    public void FindIllusionNeighbors(List<Node> nodeList, Camera cam) 
    {
        // find illusion neighbors 
        foreach (Node n in nodeList)
        {
            // do not check self  
            if (!this.Equals(n)) 
            {

                // find the world to screen points 
                Vector3 screenPos = cam.WorldToScreenPoint(n.position);
                Vector2 p1 = new Vector2(screenPos.x / cam.pixelWidth, screenPos.y / cam.pixelHeight);
                Vector3 thisScreenPos = cam.WorldToScreenPoint(this.position);
                Vector2 p2 = new Vector2(thisScreenPos.x / cam.pixelWidth, thisScreenPos.y / cam.pixelHeight);

                if (Vector2.Distance(p1, p2) < 0.03) //0.05) // .03
                {
                    //make sure they are not already neighbors
                    if (!this.neighList.Contains(n))
                    {
                        //neighList.Add(n); 

                        bool disqualify = false;

                        //raycast through the potential neighbor node 
                        Ray ray = cam.ScreenPointToRay(screenPos);
                        RaycastHit[] hits;
                        hits = Physics.RaycastAll(ray, 1000.0F);

                        float shortest = Mathf.Infinity;
                        if (hits.Length < 1) 
                        {
                            Debug.Log("raycast did not hit anything"); 
                            return;
                        }
                        RaycastHit closest = hits[0]; 
                        for (int i = 0; i < hits.Length; i++)
                        {
                            RaycastHit hit = hits[i];
                            // case 1: check if THIS is also in the Ray path 
                            if (Vector3.Distance(hit.transform.position, this.position) < 0.01f)
                            {
                                // if it is, then one is completely occluding the other 
                                Debug.Log("case 1");
                                disqualify = true;
                            }
                            // case 2: they are screen-space adjacent, but there is something occluding the potential neighbor
                            if (Vector3.Distance(ray.origin, hit.point) < shortest)
                            {
                                shortest = Vector3.Distance(ray.origin, hit.point);
                                closest = hit;
                            }
                        }
                        if (shortest < Vector3.Distance(ray.origin, n.position))
                        {
                            float epsilon = 0.01f;
                            // account for floating point errors in raycasting difference 
                            if (!(Mathf.Abs(shortest - Vector3.Distance(ray.origin, n.position)) < epsilon))
                            {
                                //Debug.Log("case 2");
                                //Debug.Log("ID #" + this.id);
                                //Debug.Log("distance from potential neighbor to camera: " + Vector3.Distance(ray.origin, n.position));
                                disqualify = true;
                            }
                        }

                        // case 3: THIS is occluded by something (if so, stop connections forming) todo can check higher up outside loops 
                        Ray rayThis = cam.ScreenPointToRay(thisScreenPos);
                        RaycastHit[] hitsThis;
                        hitsThis = Physics.RaycastAll(rayThis, 1000.0F);
                        float shortestThis = Mathf.Infinity;
                        for (int i = 0; i < hitsThis.Length; i++)
                        {
                            RaycastHit hitThis = hitsThis[i];
                            if (Vector3.Distance(ray.origin, hitThis.point) < shortestThis)
                            {
                                shortestThis = Vector3.Distance(ray.origin, hitThis.point);
                            }
                        }
                        if (Vector3.Distance(rayThis.origin, this.position) > shortestThis)
                        {
                            //Debug.Log("case 3");
                            disqualify = true;
                        }

                        // todo: case 4: the penrose triangle corner problem (only half of the node is blocked) 

                        if (!disqualify) neighList.Add(n);
                    }
                }
            } 
        }

        //// SINGLE CASE TODO  
        //if (this.position.Equals(new Vector3(6.5f,4,2.5f))) //15,-8.5f,18)))
        //{
        //    Vector3 screensp = cam.WorldToScreenPoint(this.position);
        //    Ray ray = cam.ScreenPointToRay(screensp);
        //    RaycastHit[] hits;
        //    hits = Physics.RaycastAll(ray, 100.0F);

        //    // find the hit nearest to the camera 
        //    float shortest = Mathf.Infinity;
        //    for (int i = 0; i < hits.Length; i++)
        //    {
        //        RaycastHit hit = hits[i];
        //        //Debug.Log("hit pos: " + Vector3.Distance(hit.point, ray.origin));
        //        //Debug.Log("my pos:  " + Vector3.Distance(this.position, ray.origin));

        //        if (Vector3.Distance(hit.point, ray.origin) < shortest)
        //        {
        //            shortest = Vector3.Distance(hit.point, ray.origin); 
        //        }

        //        Renderer rend = hit.transform.GetComponent<Renderer>();
        //        if (rend)
        //        {
        //            //hit.transform.localScale = new Vector3(0.11f, .11f, .11f);
        //            // Change the material of all hit colliders
        //            // to use a transparent shader.
        //            rend.material.shader = Shader.Find("Transparent/Diffuse");
        //            Color tempColor = Color.black;
        //            tempColor.a = 0.5F;
        //            rend.material.color = tempColor;
        //        }
        //    }

        //    // if there is something in front of it, then disqualify it from making a connection 
        //    if (!(shortest < Vector3.Distance(this.position, ray.origin)))
        //    {
        //        // then this geometry is the nearest to the camera 
        //        //Debug.Log("winner: " + Vector3.Distance(this.position, ray.origin)); 
        //    }
        //}

    }

    public void StartDebugVis(Camera cam)
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
        foreach (Node n in neighList)
        {
            if (!this.Equals(n))
            {
                // check each boundary
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (Vector3.Distance(boundaries[i], n.boundaries[j]) < 0.01)
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

                        // debug render the impossible connection - using screen space 
                        Vector3 p1 = cam.WorldToScreenPoint(n.boundaries[j]);
                        Vector2 b1 = new Vector2(p1.x / cam.pixelWidth, p1.y / cam.pixelHeight);

                        Vector3 p2 = cam.WorldToScreenPoint(boundaries[i]);
                        Vector2 b2 = new Vector2(p2.x / cam.pixelWidth, p2.y / cam.pixelHeight);

                        if (Vector2.Distance(b1, b2) < 0.01 && Vector3.Distance(boundaries[i], n.boundaries[j]) > 0.1)
                        {
                            // draw line 
                            Debug.DrawLine(boundaries[i], n.boundaries[j], Color.green, 1000); 

                            GameObject neighVis = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            neighVis.transform.position = boundaries[i];
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

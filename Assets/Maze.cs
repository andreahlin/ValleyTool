using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum Day { Sat, Sun, Mon, Tue, Wed, Thu, Fri };

public class Maze {

    // any special properties? 
    Vector3 start;
    Vector3 goal;
    public int height; // the x boundary
    public int width; // the z boundary 
    public int depth; // the y boundary
    public Vector3 lowerBoundary; // where the lowest thing is 
    public int difficulty; // user input
    public Node[,,] grid; // keep track of where the grid is going  

    public List<Node> cells = new List<Node>(); 

    public Maze() 
    {
        start = new Vector3(5, 5, 5);
        goal = new Vector3(14, 5, 13);
        height = 10;
        width = 10;
        depth = 1;
        lowerBoundary = new Vector3(5, 5, 5);
        difficulty = 0;
        grid = new Node[(int)lowerBoundary.x + height, (int)lowerBoundary.y + depth, (int)lowerBoundary.z + width];
    }

    public Maze(Vector3 s, Vector3 g, int h, int w, int d, Vector3 lb, int diff)
    {
        start = s;
        goal = g;
        height = h;
        width = w;
        depth = d;
        lowerBoundary = lb;
        difficulty = diff;
        grid = new Node[(int)lowerBoundary.x + height, (int)lowerBoundary.y + depth, (int)lowerBoundary.z + width];
    }

    // growing tree algorithm (from maze website) : 
    // 1. Let C be a list of cells, initiallly empty. Add one cell to C at random.
    // 2. Choose a cell from C and carve a passage to any unvisited neighbor of that cell. Add that neighbor to C. 
    //    If there are no unvisited neighbors, remove cell from C.
    // 3. Repeat step 2 until empty. 
    public void GenerateMaze()
    {
        DisplayGrid(); 
        // start the growing at the Start node
        GameObject startFace = GameObject.CreatePrimitive(PrimitiveType.Plane);
        startFace.transform.position = start;
        startFace.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Renderer rend1 = startFace.GetComponent<Renderer>();
        rend1.material.SetColor("_Color", new Color(141f / 255, 173f / 255, 170f / 255));

        Vector3 normal = Vector3.Normalize(startFace.transform.up); // todo: is this always the correct normal?
        Node n = new Node(0, "top", startFace.transform.position, startFace.transform.up, startFace.transform.right);
        SetNode(n, normal, startFace);

        // 1. add one cell to C.
        cells.Add(n);

        // 2. chose a cell from C, carve a passage to any unvisited neighbor of cell. Add neighbor to C.
        //    if no unvisited neighbors, remove cell from C
        // considerations: must stay within the boundaries 
        int index = 0;
        //while (cells.Count > 0)
        //{
        index = 0;
        Node curr = cells[index];

        // choose one direction randomly - N S E W. can you get there? 

        //start with "north" 
        int nx = Mathf.FloorToInt(curr.position.x) + 1;
        int ny = Mathf.FloorToInt(curr.position.y);
        int nz = Mathf.FloorToInt(curr.position.z);

        if (nx >= lowerBoundary.x && nz >= lowerBoundary.z && nx < lowerBoundary.x + height && nz < lowerBoundary.z + width)
        {
            if (grid[nx, ny, nz] == null)
            {
                // todo: additional checks about its neighbors 
                // "carve a passage between current cell and neighbor" aka make a new node and join them as neighbor connection 
                GameObject nFace = GameObject.CreatePrimitive(PrimitiveType.Plane);
                nFace.transform.position = new Vector3(nx, ny, nz);
                nFace.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Renderer rend2 = nFace.GetComponent<Renderer>();
                rend2.material.SetColor("_Color", new Color(151f / 255, 152f / 255, 59f / 255));

                Vector3 neighNormal = Vector3.Normalize(nFace.transform.up);
                Node neighNode = new Node(0, "top", nFace.transform.position, nFace.transform.up, nFace.transform.right);
                SetNode(neighNode, neighNormal, nFace);

                // add the neighbor on in 
                cells.Add(neighNode); 

                // add that neighbor to list
                JoinNeighbors(curr, neighNode);

                // set index to nil (indicate that an unvisited neighbor was found 
                index = -1;

                // todo break out of innermost loop ... but there's only one loop ? 
            }
        }
        //}

        // if no neighbor is found, then delete the curr cell
        if (index > -1)  cells.RemoveAt(index); 


        // todo use later 
        Color goalRed = new Color(168f / 255, 0, 0);
    }

    public void JoinNeighbors(Node a, Node b) 
    {
        a.neighList.Add(b);
        b.neighList.Add(a); 
    }

    public void DisplayGrid()
    {
        for (int x = (int)lowerBoundary.x; x < (int)lowerBoundary.x + height; x++)
        {
            for (int y = (int)lowerBoundary.y; y < (int)lowerBoundary.y + depth; y++)
            {
                for (int z = (int)lowerBoundary.z; z < (int)lowerBoundary.z + width; z++)
                {
                    GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    plane.transform.position = new Vector3(x,y,z);
                    plane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // new Vector3(0.09f, 0.09f, 0.09f)
                    Renderer rend1 = plane.GetComponent<Renderer>();
                    rend1.material.shader = Shader.Find("Transparent/Diffuse");
                    rend1.material.SetColor("_Color", new Color(1,1,1,0.1f));

                }

            }
        }

    }


    public void SetNode(Node n, Vector3 normal, GameObject face)
    {
        float epsilon = 0.01f;
        if (normal == new Vector3(0, 1, 0))
        {
            n = new Node(0, "top", face.transform.position, face.transform.up, face.transform.right);
        }
        else if (normal == new Vector3(-1, 0, 0))
        {
            n = new Node(0, "negx", face.transform.position, face.transform.up, face.transform.right);
        }
        else if (normal == new Vector3(0, 0, -1))
        {
            n = new Node(0, "negz", face.transform.position, face.transform.up, face.transform.right);
        }
        else if (normal.x > epsilon || normal.x < -epsilon)
        {
            if (normal.y > 0)
            {
                n = new Node(0, "diagx", face.transform.position, face.transform.up, face.transform.right);
            }
        }
        else if (normal.z > epsilon || normal.z < -epsilon)
        {
            if (normal.y > 0)
            {
                n = new Node(0, "diagz", face.transform.position, face.transform.up, face.transform.right);
            }
        }
    }
}

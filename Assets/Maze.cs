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
    public Camera cam;
    public List<Node> allNodes = new List<Node>(); 

    public List<Node> cells = new List<Node>(); 

    public Maze(Camera c) 
    {
        cam = c; 
        start = new Vector3(5, 5, 5);
        goal = new Vector3(14, 5, 13);
        height = 10;
        width = 10;
        depth = 1;
        lowerBoundary = new Vector3(5, 5, 5);
        difficulty = 0;
        grid = new Node[(int)lowerBoundary.x + height, (int)lowerBoundary.y + depth, (int)lowerBoundary.z + width];
    }

    public Maze(Camera c, Vector3 s, Vector3 g, int h, int w, int d, Vector3 lb, int diff)
    {
        cam = c; 
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
    // todo: how do i guarantee that the goal node will be hit? 
    public void GenerateMaze()
    {
        DisplayGrid(); // todo: can get rid of eventually 
        // start the growing at the Start node todo make into a cube eventually? 
        GameObject startFace = GameObject.CreatePrimitive(PrimitiveType.Plane);
        startFace.name = "start face"; 
        startFace.transform.position = start;
        startFace.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Renderer rend1 = startFace.GetComponent<Renderer>();
        rend1.material.SetColor("_Color", new Color(141f / 255, 173f / 255, 170f / 255));
        GameObject container = GameObject.Find("Maze Geom");
        startFace.transform.SetParent(container.transform, true);

        Vector3 normal = Vector3.Normalize(startFace.transform.up); 
        Node n = new Node(100000, "top", startFace.transform.position, startFace.transform.up, startFace.transform.right);
        SetNode(n, normal, startFace);
        allNodes.Add(n); 

        // 1. add one cell to C.
        cells.Add(n);
        grid[(int)start.x, (int)start.y, (int)start.z] = n; 

        // 2. chose a cell from C, carve a passage to any unvisited neighbor of cell. Add neighbor to C.
        //    if no unvisited neighbors, remove cell from C
        // considerations: must stay within the boundaries 
        //for (int s = 0; s < 200; s++) //while (cells.Count > 0)
        //{
            while (cells.Count > 0)
            {
                //Debug.Log("count: " + cells.Count); 
                int index = cells.Count - 1; // todo: choose the index in a more interesting way 
                Node curr = cells[index];

                // check each direction (N S E W). Can you go there? 
                List<int> directions = new List<int>();
                directions.Add(0);
                directions.Add(1);
                directions.Add(2);
                directions.Add(3);
                directions = RandomizeList(directions);

                foreach (int direction in directions)
                {
                    Vector3 next = NextPos(curr.position, direction);
                    int nx = Mathf.FloorToInt(next.x);
                    int ny = Mathf.FloorToInt(next.y);
                    int nz = Mathf.FloorToInt(next.z);

                    // make sure the poential doesn't have any other neighbors  
                    int numNeigh = 0;
                    for (int l = 0; l < 4; l++)
                    {
                        Vector3 nn = NextPos(new Vector3(nx, ny, nz), l);
                        if (nn.x >= lowerBoundary.x && nn.z >= lowerBoundary.z && nn.x < lowerBoundary.x + height && nn.z < lowerBoundary.z + width)
                        {
                            if (grid[(int)nn.x, (int)nn.y, (int)nn.z] != null) numNeigh++;
                        }
                    }

                    if (nx >= lowerBoundary.x && nz >= lowerBoundary.z && nx < lowerBoundary.x + height && nz < lowerBoundary.z + width &&
                        grid[nx, ny, nz] == null && numNeigh == 1)
                    {
                        // "carve a passage between current cell and neighbor" aka make a new node and join them as neighbor connection 
                        GameObject nFace = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        nFace.name = "face " + index;
                        nFace.transform.position = new Vector3(nx, ny, nz);
                        nFace.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        Renderer rend2 = nFace.GetComponent<Renderer>();
                        rend2.material.SetColor("_Color", new Color(151f / 255, 152f / 255, 59f / 255));
                        container = GameObject.Find("Maze Geom");
                        nFace.transform.SetParent(container.transform, true);

                        Vector3 neighNormal = Vector3.Normalize(nFace.transform.up);
                        Node neighNode = new Node(index, "top", nFace.transform.position, nFace.transform.up, nFace.transform.right);
                        SetNode(neighNode, neighNormal, nFace);
                        allNodes.Add(neighNode);

                        // add that neighbor to list
                        JoinNeighbors(curr, neighNode);

                        // add the neighbor on in 
                        grid[nx, ny, nz] = neighNode;
                        cells.Add(neighNode);

                        // set index to -1  (indicate that an unvisited neighbor was found)
                        index = -1;

                        // break out of the directions loop 
                        break;
                    }
                }
                // if no neighbor is found, then delete the curr cell
                if (index > -1)
                {
                    cells.RemoveAt(index);
                }
            }
        //}
        Debug.Log("size of list: " + cells.Count); 



        // todo use later 
        Color goalRed = new Color(168f / 255, 0, 0);
    }

    // fisher-yates shuffle
    public List<int> RandomizeList(List<int> ts) 
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
        return ts; 
    }

    public Vector3 NextPos(Vector3 pos, int i)
    {
        switch (i)
        {
            case 0: // +x 
                return pos + new Vector3(1,0,0);
            case 1: // -x 
                return pos + new Vector3(-1, 0, 0);
            case 2: // +z 
                return pos + new Vector3(0, 0, 1);
            case 3: // -z 
                return pos + new Vector3(0, 0, -1);
        }
        return pos;
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
                    GameObject container = GameObject.Find("Node Grid");
                    plane.transform.SetParent(container.transform, true);


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

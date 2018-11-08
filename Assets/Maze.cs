using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GeomType { Start, Goal, Path, NonPath };

public class Maze {

    // any special properties? 
    Vector3 start;
    //Vector3 goal;
    public int height; // the x boundary
    public int width; // the z boundary 
    public int depth; // the y boundary
    public Vector3 lowerBoundary; // where the lowest thing is 
    public int difficulty; // user input
    public Node[,,] grid; // keep track of where the grid is going
    public Node[,,] spaceGrid; // the expanded grid 
    public Camera cam;
    public List<Node> allNodes = new List<Node>(); 

    public List<Node> cells = new List<Node>();
    //Maze m = new Maze(cam, new Vector3(0, 0, 0), new Vector3(4, 0, 4), 5, 5, 1, new Vector3(0, 0, 0), 0);

    public Maze(Camera c) 
    {
        cam = c; 
        start = new Vector3(0,0,0);
        //goal = new Vector3(4,0,4);
        height = 5;
        width = 5;
        depth = 1;
        lowerBoundary = new Vector3(0,0,0);
        difficulty = 0;
        grid = new Node[height, depth, width];
    }

    public Maze(Camera c, int h, int w, int d)
    {
        cam = c;
        start = new Vector3(0, 0, 0);
        //goal = new Vector3(4, 0, 4);
        height = h;
        width = w;
        depth = d; 
        lowerBoundary = new Vector3(0, 0, 0);
        difficulty = 0;
        grid = new Node[height, depth, width];
    }

    public Maze(Camera c, Vector3 s, Vector3 g, int h, int w, int d, Vector3 lb, int diff)
    {
        cam = c; 
        start = s;
        //goal = g;
        height = h;
        width = w;
        depth = d;
        lowerBoundary = lb;
        difficulty = diff;
        grid = new Node[height, depth, width];
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

         //start the growing at the Start node todo make into a cube eventually? 
        //GameObject startFace = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //startFace.name = "start face";
        //startFace.transform.position = start;
        //startFace.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //Renderer rend1 = startFace.GetComponent<Renderer>();
        //rend1.material.SetColor("_Color", new Color(141f / 255, 173f / 255, 170f / 255));
        //GameObject container = GameObject.Find("Maze Geom");
        //startFace.transform.SetParent(container.transform, true);

        Vector3 normal = Vector3.Normalize(new Vector3(0,1,0));
        Node n = new Node(100000, "top", start, normal, new Vector3(0,0,1));
        //SetNode(n, normal, startFace);
        allNodes.Add(n);

        // 1. add one cell to C.
        cells.Add(n);

        // todo: start is always at 0 
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
                    if (nn.x >= 0 && nn.z >= 0 && nn.x < height && nn.z < width)
                    {
                        if (grid[(int)nn.x, (int)nn.y, (int)nn.z] != null)
                        {
                            numNeigh++;
                        }
                    }
                }

                if (nx >= 0 && nz >= 0 && nx < height && nz < width &&
                    grid[nx, ny, nz] == null)// && numNeigh == 1)
                {
                    // todo: put back in this? idk
                    // "carve a passage between current cell and neighbor" aka make a new node and join them as neighbor connection 
                    //GameObject nFace = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    //nFace.name = "face " + index;
                    //nFace.transform.position = new Vector3(nx, ny, nz);
                    //nFace.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    //Renderer rend2 = nFace.GetComponent<Renderer>();
                    //rend2.material.SetColor("_Color", new Color(151f / 255, 152f / 255, 59f / 255));
                    //GameObject container = GameObject.Find("Maze Geom");
                    //nFace.transform.SetParent(container.transform, true);

                    //Vector3 neighNormal = Vector3.Normalize(nFace.transform.up);
                    //Node neighNode = new Node(index, "top", nFace.transform.position, nFace.transform.up, nFace.transform.right);
                    //SetNode(neighNode, neighNormal, nFace);
                    //allNodes.Add(neighNode);

                    Vector3 neighNormal = Vector3.Normalize(new Vector3(0,1,0));
                    Node neighNode = new Node(index, "top", new Vector3(nx,ny,nz), normal, new Vector3(0,0,1));
                    //SetNode(neighNode, neighNormal, nFace);
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

        this.ExpandGrid(); 
    }

    // transfer the nodes to an expaned version of the grid 
    private void ExpandGrid() 
    {
        spaceGrid = new Node[grid.GetLength(0) * 2 - 1,
                                      grid.GetLength(1), // todo: should be resizable like the others  
                                      grid.GetLength(2) * 2 - 1];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    if (grid[x, y, z] != null)
                    {
                        // space them out in real space
                        Node node = grid[x, y, z];
                        node.position.x *= 2f;
                        node.position.y *= 1f; // todo: resize the depth too 
                        node.position.z *= 2f;
                        node.SetBoundaries(); // todo: refactor this to be somewhere else 

                        // place existing nodes into the new SpaceGrid but spaced out 
                        spaceGrid[2 * x, y, 2 * z] = grid[x, y, z];


                        if (Vector3.Distance(node.position, 
                                             new Vector3(spaceGrid.GetLength(0) - 1, 
                                                         spaceGrid.GetLength(1) - 1, 
                                                         spaceGrid.GetLength(2) - 1)) < 0.01)
                        {
                            CreateFaceGeometry(node, x + y + z, GeomType.Goal);
                        }
                        else if (Vector3.Distance(node.position, new Vector3(0, 0, 0)) < 0.01)
                        {
                            CreateFaceGeometry(node, x + y + z, GeomType.Start);
                        }
                        else 
                        {
                            CreateFaceGeometry(node, x + y + z, GeomType.Path);
                        }
                    }
                }
            }
        }

        int count = 0;
        for (int x = 0; x < spaceGrid.GetLength(0); x++)
        {
            for (int y = 0; y < spaceGrid.GetLength(1); y++)
            {
                for (int z = 0; z < spaceGrid.GetLength(2); z++)
                {
                    if (spaceGrid[x, y, z] == null)
                    {
                        count++;
                        Vector3 currPos = new Vector3(x, y, z);
                        // find if the null grid area is between two existing neighbors 
                        Vector3 next = NextPos(currPos, 0);
                        FindIfInbetween(0, 1, currPos, next, spaceGrid);
                        next = NextPos(currPos, 2);
                        FindIfInbetween(2, 3, currPos, next, spaceGrid);
                    }
                }
            }
        }
    }

    // create a face based on the node and the inputed type ... use enum?
    private void CreateFaceGeometry(Node n, int name, GeomType type)
    {
        // default white 
        Color color = new Color(1,1,1); 
        switch (type) 
        {
            case GeomType.Goal:
                color = new Color(168f / 255, 0, 0); 
                break;
            case GeomType.Start:
                color = new Color(141f / 255, 173f / 255, 170f / 255); 
                break;
            case GeomType.Path:
                color = new Color(151f / 255, 152f / 255, 59f / 255); 
                break;
            case GeomType.NonPath:
                color = new Color(183f / 255, 175f / 255, 168f / 255); 
                break; 
        }

        GameObject face = GameObject.CreatePrimitive(PrimitiveType.Plane);
        face.name = "face " + name;
        face.transform.position = n.position;
        face.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Renderer rend2 = face.GetComponent<Renderer>();
        rend2.material.SetColor("_Color", color);
        GameObject container = GameObject.Find("Maze Geom");
        face.transform.SetParent(container.transform, true);
    }


    private void FindIfInbetween(int dir, int opp, Vector3 curr, Vector3 next, Node[,,] spaceGrid) 
    {
        //Debug.Log("called"); 
        if (next.x >= 0 && next.z >= 0 && next.x < spaceGrid.GetLength(0) && next.z < spaceGrid.GetLength(2))
        {
            if (spaceGrid[(int)next.x, (int)next.y, (int)next.z] != null)
            {
                Node nextNode = spaceGrid[(int)next.x, (int)next.y, (int)next.z];
                Vector3 nextOpp = NextPos(curr, opp);
                Node oppNode = spaceGrid[(int)nextOpp.x, (int)nextOpp.y, (int)nextOpp.z];
                if (oppNode == null) return;

                // check if they are neighbors
                if (nextNode.neighList.Contains(oppNode)) // check the opposite way? 
                {
                    // make a new Node @ space grid 
                    GameObject spaceGridFace = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    spaceGridFace.name = "face ";
                    spaceGridFace.transform.position = curr;
                    spaceGridFace.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    Renderer rend3 = spaceGridFace.GetComponent<Renderer>();
                    rend3.material.SetColor("_Color", new Color(151f / 255, 152f / 255, 59f / 255));
                    GameObject container = GameObject.Find("Maze Geom");
                    spaceGridFace.transform.SetParent(container.transform, true);
                    // make a new node
                    Vector3 spaceGridNor = Vector3.Normalize(spaceGridFace.transform.up);
                    Node spaceGridNode = new Node(0, "top", spaceGridFace.transform.position, spaceGridFace.transform.up, spaceGridFace.transform.right);
                    SetNode(spaceGridNode, spaceGridNor, spaceGridFace);
                    spaceGrid[(int)curr.x, (int)curr.y, (int)curr.z] = spaceGridNode;

                    // reroute neighbors... is this even working? don't know 
                    RerouteNeighbors(nextNode, oppNode, spaceGridNode);

                    // add the neighbor on in 
                    allNodes.Add(spaceGridNode); // add it to allnodes so it will show up 
                }
            }
        }
    }

    // fisher-yates shuffle
    private List<int> RandomizeList(List<int> ts)
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

    private Vector3 NextPos(Vector3 pos, int i)
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

    private void JoinNeighbors(Node a, Node b)
    {
        a.neighList.Add(b);
        b.neighList.Add(a);
    }

    private void RerouteNeighbors(Node a, Node b, Node re) 
    {
        a.neighList.Remove(b);
        b.neighList.Remove(a);
        a.neighList.Add(re);
        b.neighList.Add(re);
        re.neighList.Add(a);
        re.neighList.Add(b); 
    }

    private void SetNode(Node n, Vector3 normal, GameObject face)
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

    private void DisplayGrid()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    plane.transform.position = new Vector3(x, y, z);
                    plane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // new Vector3(0.09f, 0.09f, 0.09f)
                    Renderer rend1 = plane.GetComponent<Renderer>();
                    rend1.material.shader = Shader.Find("Transparent/Diffuse");
                    rend1.material.SetColor("_Color", new Color(1, 1, 1, 0.1f));
                    GameObject container = GameObject.Find("Node Grid");
                    plane.transform.SetParent(container.transform, true);


                }

            }
        }

    }

}

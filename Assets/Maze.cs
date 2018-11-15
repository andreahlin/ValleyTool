using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GeomType { Start, Goal, Path, NonPath };

public class Maze {
    public static Vector3 posX = new Vector3(1, 0, 0); 
    public static Vector3 negX = new Vector3(-1, 0, 0);
    public static Vector3 posY = new Vector3(0, 1, 0);
    public static Vector3 negY = new Vector3(0, -1, 0);
    public static Vector3 posZ = new Vector3(0, 0, 1);
    public static Vector3 negZ = new Vector3(0, 0, -1);

    // fields 
    public Camera cam;
    public int height; // the x boundary
    public int width; // the z boundary 
    public int depth; // the y boundary
    public int difficulty; // user input: 1 to 5 or something? 
    public Node[,,] grid; // keep track of where the grid is going
    public Node[,,] spaceGrid; // the expanded grid 
    public List<Node> cells = new List<Node>(); // used for the maze algorithm 
    public List<Node> allNodes = new List<Node>(); // used to visualize geometry 

    public List<Node> ladderXNodes = new List<Node>(); // ladders
    public List<Node> ladderZNodes = new List<Node>(); // ladders 

    public Maze(Camera c) 
    {
        cam = c; 
        height = 5;
        width = 5;
        depth = 1;
        difficulty = 1;
        grid = new Node[height, depth, width];
        spaceGrid = new Node[height * 2 - 1, depth * 2 - 1, width * 2 - 1];
    }

    public Maze(Camera c, int h, int w, int d)
    {
        cam = c;
        height = h;
        width = w;
        depth = d; 
        difficulty = 1;
        grid = new Node[height, depth, width];
        spaceGrid = new Node[height * 2 - 1, depth * 2 - 1, width * 2 - 1];
    }

    public Maze(Camera c, int h, int w, int d, int diff)
    {
        cam = c; 
        height = h;
        width = w;
        depth = d;
        difficulty = diff;
        grid = new Node[height, depth, width];
        spaceGrid = new Node[height * 2 - 1, depth * 2 - 1, width * 2 - 1];
    }

    // Growing Tree Algorithm  
    public void GenerateMaze()
    {
        // create the Start node 
        Node n = new Node(100000, "top", new Vector3(0,0,0), posY, new Vector3(0,0,1));
        allNodes.Add(n);

        // 1. add one cell to C.
        cells.Add(n);
        grid[0,0,0] = n;

        // 2. chose a cell from C, carve a passage to any unvisited neighbor of cell. Add neighbor to C.
        //    if no unvisited neighbors, remove cell from C
        while (cells.Count > 0)
        {
            int index = cells.Count - 1; // todo: vary the way index is chosen
            Node curr = cells[index];

            // check each direction (N S E W). Can you go there? 
            List<int> directions = new List<int>();
            directions.Add(0);
            directions.Add(1);
            directions.Add(2);
            directions.Add(3);
            directions.Add(4); // todo
            directions.Add(5); // todo
            directions = RandomizeList(directions);

            foreach (int direction in directions)
            {
                Vector3 next = NextPos(curr.position, direction);
                int nx = Mathf.FloorToInt(next.x);
                int ny = Mathf.FloorToInt(next.y);
                int nz = Mathf.FloorToInt(next.z);

                if (nx >= 0 && nz >= 0 && nx < height && nz < width && ny >= 0 && ny < depth &&
                    grid[nx, ny, nz] == null)
                {
                    Vector3 neighNormal = Vector3.Normalize(new Vector3(0,1,0));
                    Node neighNode = new Node(index, "top", new Vector3(nx,ny,nz), posY, new Vector3(0,0,1)); 
                    // how to tell which direction ????? idk ???? 

                    // make them neighbors 
                    JoinNeighbors(curr, neighNode);

                    // add the neighbor on in to the grid  
                    grid[nx, ny, nz] = neighNode;
                    cells.Add(neighNode);

                    // add to allnodes for display
                    allNodes.Add(neighNode);

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

        // transfer all the nodes to the expanded grid for alterations! 
        // todo idk what's happening here 
        this.ExpandGrid(); 
        this.RenderGeomInGrid(spaceGrid);

        CreateAllLadders();
        List<Node> walkPath = PrunePath(); 
    }

    private void CreateAllLadders() 
    {
        for (int x = 0; x < spaceGrid.GetLength(0); x++) {
            for (int y = 0; y < spaceGrid.GetLength(1); y++) {
                for (int z = 0; z < spaceGrid.GetLength(2); z++) {
                    if (spaceGrid[x, y, z] != null) {
                        Node node = spaceGrid[x, y, z];

                        // X face case ///////////////////////////////////////
                        // check its potential ladders. how? find the position where the nodes could be
                        Vector3 worldLadderX = node.position + new Vector3(-.5f, -.5f, 0);
                        Vector3 xScreenPos = cam.WorldToScreenPoint(worldLadderX);

                        // raycast: if you hit it correctly (within epsilon) then create a node there (also face geometry?) 
                        Ray ray = cam.ScreenPointToRay(xScreenPos);
                        RaycastHit[] hits;
                        hits = Physics.RaycastAll(ray, 1000.0F);

                        float shortest = Mathf.Infinity;
                        RaycastHit closest = hits[0];
                        for (int i = 0; i < hits.Length; i++)
                        {
                            RaycastHit hit = hits[i];
                            if (Vector3.Distance(ray.origin, hit.point) < shortest)
                            {
                                shortest = Vector3.Distance(ray.origin, hit.point);
                                closest = hit;
                            }
                        }
                        // account for floating point errors in raycasting  
                        float epsilon = 0.01f;
                        if ((Mathf.Abs(shortest - Vector3.Distance(ray.origin, worldLadderX)) < epsilon))
                        {
                            // additional raycast check that the ladder is leading down to another node 
                            Vector3 worldXNeigh = worldLadderX + new Vector3(-.5f, -.5f, 0);

                            Vector3 xPotScreenPos = cam.WorldToScreenPoint(worldXNeigh);
                            Ray ray2 = cam.ScreenPointToRay(xPotScreenPos);
                            RaycastHit hit;
                            if (Physics.Raycast(ray2, out hit, 1000.0f)) {
                                // check if the raycast hit another node 
                                foreach (Node n in allNodes)
                                {
                                    if (Vector3.Distance(hit.point, n.position) < epsilon) 
                                    {
                                        // todo: CORNER CHECK  
                                        Vector3 cornerCheck = worldLadderX + new Vector3(0, -.25f, -.25f);
                                        Vector3 cornerCheckScreen = cam.WorldToScreenPoint(cornerCheck); 
                                        Ray cornerRay = cam.ScreenPointToRay(cornerCheckScreen);
                                        RaycastHit[] cornerHits;
                                        cornerHits = Physics.RaycastAll(cornerRay, 1000.0F);

                                        float cornerShortest = Mathf.Infinity;
                                        RaycastHit cornerClosest = cornerHits[0];
                                        for (int i = 0; i < cornerHits.Length; i++)
                                        {
                                            RaycastHit cornerHit = cornerHits[i];
                                            if (Vector3.Distance(cornerRay.origin, cornerHit.point) < cornerShortest)
                                            {
                                                cornerShortest = Vector3.Distance(cornerRay.origin, cornerHit.point);
                                                cornerClosest = cornerHit;
                                            }
                                        }
                                        //// account for floating point errors in raycasting 
                                        if (Mathf.Abs(cornerShortest - Vector3.Distance(cornerRay.origin, cornerCheck)) < epsilon)
                                        {
                                            // todo: making a face (do we need? probably) 
                                            //GameObject face = GameObject.CreatePrimitive(PrimitiveType.Plane);
                                            //face.name = "ladderX";
                                            //face.transform.position = worldLadderX;
                                            //face.transform.localScale = new Vector3(0.1f, 0.1f, 0.05f);
                                            //face.transform.localEulerAngles = new Vector3(0, 0, 90); 
                                            //Renderer rend2 = face.GetComponent<Renderer>();
                                            //rend2.material.SetColor("_Color", Color.white);

                                            //make a node there 
                                            Vector3 neighNormal = Vector3.Normalize(new Vector3(-1, 0, 0));
                                            Node ladderXNode = new Node(0, "negx", worldLadderX, negX, new Vector3(0, 0, -1f));
                                            // either add it to ladderXnodes or ladderZnodes
                                            ladderXNodes.Add(ladderXNode);
                                        }
                                    }
                                }
                            }
                        }

                        /////////////////////////////////////////////////////////////////////
                        // Z face case 
                        Vector3 worldLadderZ = node.position + new Vector3(0, -.5f, -.5f);
                        Vector3 zScreenPos = cam.WorldToScreenPoint(worldLadderZ);

                        // raycast: if you hit it correctly (within epsilon) then create a node there (also face geometry?) 
                        Ray rayForZ = cam.ScreenPointToRay(zScreenPos);
                        RaycastHit[] hitsForZ;
                        hitsForZ = Physics.RaycastAll(rayForZ, 1000.0F);

                        float shortestZ = Mathf.Infinity;
                        RaycastHit closestZ = hitsForZ[0];
                        for (int i = 0; i < hitsForZ.Length; i++)
                        {
                            RaycastHit hitForZ = hitsForZ[i];
                            if (Vector3.Distance(rayForZ.origin, hitForZ.point) < shortestZ)
                            {
                                shortestZ = Vector3.Distance(rayForZ.origin, hitForZ.point);
                                closestZ = hitForZ;
                            }
                        }
                        // account for floating point errors in raycasting  
                        if (Mathf.Abs(shortestZ - Vector3.Distance(rayForZ.origin, worldLadderZ)) < epsilon)
                        {
                            // additional raycast check that the ladder is leading down to another node 
                            Vector3 worldZNeigh = worldLadderZ + new Vector3(0, -.5f, -.5f);

                            Vector3 zPotScreenPos = cam.WorldToScreenPoint(worldZNeigh);
                            Ray ray2Z = cam.ScreenPointToRay(zPotScreenPos);
                            RaycastHit hitz;
                            if (Physics.Raycast(ray2Z, out hitz, 1000.0f))
                            {
                                // check if the raycast hit another node 
                                foreach (Node n in allNodes)
                                {
                                    if (Vector3.Distance(hitz.point, n.position) < epsilon)
                                    {
                                        //// todo: CORNER CHECK  
                                        Vector3 cornerCheck = worldLadderZ + new Vector3(-.25f, -.25f, 0);
                                        Vector3 cornerCheckScreen = cam.WorldToScreenPoint(cornerCheck);
                                        Ray cornerRay = cam.ScreenPointToRay(cornerCheckScreen);
                                        RaycastHit[] cornerHits;
                                        cornerHits = Physics.RaycastAll(cornerRay, 1000.0F);

                                        float cornerShortest = Mathf.Infinity;
                                        RaycastHit cornerClosest = cornerHits[0];
                                        for (int i = 0; i < cornerHits.Length; i++)
                                        {
                                            RaycastHit cornerHit = cornerHits[i];
                                            if (Vector3.Distance(cornerRay.origin, cornerHit.point) < cornerShortest)
                                            {
                                                cornerShortest = Vector3.Distance(cornerRay.origin, cornerHit.point);
                                                cornerClosest = cornerHit;
                                            }
                                        }
                                        ////// account for floating point errors in raycasting 
                                        if (Mathf.Abs(cornerShortest - Vector3.Distance(cornerRay.origin, cornerCheck)) < epsilon)
                                        {
                                            // todo: making a face (do we need? probably) todo: this isn't the right face 
                                            //GameObject face = GameObject.CreatePrimitive(PrimitiveType.Plane);
                                            //face.name = "ladderZ";
                                            //face.transform.position = worldLadderZ;
                                            //face.transform.localEulerAngles = new Vector3(-90, 0, 0);
                                            //face.transform.localScale = new Vector3(.1f, .1f, .1f);
                                            //Renderer rend2 = face.GetComponent<Renderer>();
                                            //rend2.material.SetColor("_Color", Color.white);

                                            //make a node there 
                                            Vector3 neighNormal = Vector3.Normalize(new Vector3(0, 0, -1));
                                            Node ladderZNode = new Node(0, "negz", worldLadderZ, negZ, new Vector3(1, 0, 0));
                                            ladderZNodes.Add(ladderZNode);

                                            // really basic visualization 
                                            //GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                            //marker.transform.position = worldLadderZ;
                                            //marker.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // at the end, add all ladderXnodes and ladderZnodes to the allNodes so they are rendered 
        foreach (Node n in ladderXNodes) allNodes.Add(n);
        foreach (Node n in ladderZNodes) allNodes.Add(n); 
    }

    public List<Node> PrunePath() 
    {
        // a function which will remove a bunch of nodes until there is only one path available from start to goal
        // todo: figure out how to delete vis? i mean... don't render them in the first place 

        // create a graph out of all existing Nodes 
        Graph g = new Graph(allNodes);
        Vector3 start = new Vector3(0, 0, 0);
        Vector3 target = new Vector3(spaceGrid.GetLength(0) - 1, spaceGrid.GetLength(1) - 1, spaceGrid.GetLength(2) - 1);
        Node s = null;
        Node t = null; 

        foreach (Node n in allNodes) 
        {
            if (n.position.Equals(start)) s = n;
            else if (n.position.Equals(target)) t = n; 
        }
        if (s == null || t == null) 
        {
            Debug.Log("could not find start or target node");
            return null;
        }

        // check if in same CC 
        //while (g.InSameComponent(s,t)) 
        //{
            // do something 
        //}
        //if (g.InSameComponent(s,t)) 
        //{
        //    // remove a random node, make a new graph, and do again. 
        //    Node nodeToThrow = allNodes[1]; 
        //    if (nodeToThrow.Equals(s) || nodeToThrow.Equals(t)) 
        //    {
        //        nodeToThrow = allNodes[2]; 
        //    }
        //    //RemoveNode(nodeToThrow); 
        //    //g = new Graph(allNodes); 
        //}
        List<Node> allNodesCopy = allNodes;
        RemoveNode(allNodesCopy[2]); 

        //Debug.Log("no more paths o:"); 

        return null;
    }

    //  todo idk if they like this method 
    private void RemoveNode(Node n) 
    {
        // remove node from every place it occurred (most importantly, allNodes)  
        foreach (Node curr in allNodes) 
        {
            if (curr.Equals(n)) 
            {
                allNodes.Remove(n);
            }
            else {
                // check if it occurs in the neighbor list 
                foreach (Node neigh in curr.neighList)
                {
                    if (neigh.Equals(n)) curr.neighList.Remove(n); 
                }
            }
        }
        foreach (Node x in ladderXNodes)
        {
            if (x.Equals(n)) ladderXNodes.Remove(n);
        }
        foreach (Node z in ladderZNodes)
        {
            if (z.Equals(n)) ladderZNodes.Remove(n); 
        }
    }

    private void RenderGeomInGrid(Node[,,] g)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    if (grid[x, y, z] != null)
                    {
                        Node node = grid[x, y, z];
                        // visualize the geometry 
                        if (Vector3.Distance(node.position,
                                             new Vector3(g.GetLength(0) - 1,
                                                         g.GetLength(1) - 1,
                                                         g.GetLength(2) - 1)) < 0.01)
                        {
                            CreateCubeGeometry(node, 0, GeomType.Goal);
                        }
                        else if (Vector3.Distance(node.position, new Vector3(0, 0, 0)) < 0.01)
                        {
                            CreateCubeGeometry(node, 0, GeomType.Start);
                        }
                        else
                        {
                            CreateCubeGeometry(node, 0, GeomType.Path);
                        }
                    }
                }
            }
        }
    }

    // transfer the nodes to an expaned version of the grid 
    private void ExpandGrid()
    {
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
                        node.position.y *= 2f; // todo: resize the depth too 
                        node.position.z *= 2f;
                        node.SetBoundaries(); // todo: refactor this to be somewhere else 

                        // place existing nodes into the new SpaceGrid but spaced out 
                        spaceGrid[2 * x, 2 * y, 2 * z] = grid[x, y, z];
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
                        FindIfInbetween(0, 1, currPos, next);
                        next = NextPos(currPos, 2);
                        FindIfInbetween(2, 3, currPos, next);
                    }
                }
            }
        }
    }

    private void FindIfInbetween(int dir, int opp, Vector3 curr, Vector3 next) 
    {
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
                    // make a new node
                    Node spaceGridNode = new Node(0, "top", curr, 
                                                  new Vector3(0,1,0), 
                                                  new Vector3(0,0,1));
                    spaceGrid[(int)curr.x, (int)curr.y, (int)curr.z] = spaceGridNode;

                    // reroute neighbors (put the new node inbetween)  
                    RerouteNeighbors(nextNode, oppNode, spaceGridNode);

                    // add it to allnodes so it will show up 
                    allNodes.Add(spaceGridNode);

                    // add in the geometry 
                    CreateCubeGeometry(spaceGridNode, 0, GeomType.Path);
                }
            }
        }
    }

    // create a face based on input node 
    private void CreateFaceGeometry(Node n, int name, GeomType type)
    {
        // default white 
        Color color = new Color(1, 1, 1);
        switch (type)
        {
            case GeomType.Goal:
                color = new Color(160f / 255, 82f / 255, 65f / 255); 
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

    // todo: create a cube based on input node 
    private void CreateCubeGeometry(Node n, int name, GeomType type)
    {
        // default white 
        Color color = new Color(1, 1, 1);
        switch (type)
        {
            case GeomType.Goal:
                color = new Color(160f / 255, 82f / 255, 65f / 255);
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

        GameObject f1 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        f1.name = "nodeFace";
        f1.transform.position = n.position;
        f1.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Renderer r1 = f1.GetComponent<Renderer>();
        r1.material.SetColor("_Color", color);
        GameObject container = GameObject.Find("Maze Geom");
        f1.transform.SetParent(container.transform, true);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "cube " + name;
        Renderer r2 = cube.GetComponent<Renderer>();
        r2.material.SetColor("_Color", color);
        cube.transform.SetParent(f1.transform, true);

        if (n.up.Equals(posY))
        {
            cube.transform.position = n.position + negY * 0.5f;
        }
        else if (n.up.Equals(negX)) 
        {
            cube.transform.position = n.position + negY * 0.5f;
            f1.transform.eulerAngles = new Vector3(f1.transform.eulerAngles.x, // so it will face -x 
                                                   f1.transform.eulerAngles.y, 
                                                   f1.transform.eulerAngles.z + 90);    
        }
        else if (n.up.Equals(negZ))
        {
            cube.transform.position = n.position + negY * 0.5f;
            f1.transform.eulerAngles = new Vector3(f1.transform.eulerAngles.x - 90,  
                                                   f1.transform.eulerAngles.y,
                                                   f1.transform.eulerAngles.z);
        }
        else 
        {
            Debug.Log("Warning: invalid normal");
            return;
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

    // return the next position given a direction  
    private Vector3 NextPos(Vector3 pos, int i)
    {
        switch (i)
        {
            case 0: // +x 
                return pos + posX; 
            case 1: // -x 
                return pos + negX;
            case 2: // +z 
                return pos + posZ;
            case 3: // -z 
                return pos + negZ;
            case 4: // +y
                return pos + posY;
            case 5:
                return pos + negY; 
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

    private void DisplayGrid(Node[,,] g)
    {
        GameObject container = new GameObject("Node Grid"); 
        for (int x = 0; x < g.GetLength(0); x++)
        {
            for (int y = 0; y < g.GetLength(1); y++)
            {
                for (int z = 0; z < g.GetLength(2); z++)
                {
                    GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    plane.transform.position = new Vector3(x, y, z);
                    plane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // new Vector3(0.09f, 0.09f, 0.09f)
                    Renderer rend1 = plane.GetComponent<Renderer>();
                    rend1.material.shader = Shader.Find("Transparent/Diffuse");
                    rend1.material.SetColor("_Color", new Color(1, 1, 1, 0.1f));
                    plane.transform.SetParent(container.transform, true);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public GameObject[] pathFaces;
    public List<Node> allNodes = new List<Node>();
    // keeps track of whether something is on the node already 
    public Dictionary<Node, bool> map = new Dictionary<Node, bool>();
    public List<Node> nodesHadLittlePrizes = new List<Node>();
    public Node nodeKeyWasOn = null;

    GameObject thePlayer;
    CharController playerScript;
    Camera cam;
    Graph g;
    Vector3 goalPos;

    // TO SEND TO CHARCONTROLLER
    Vector3 keyPos;
    GameObject gateObjX;
    GameObject gateObjZ;

    // Use this for initialization
    void Start()
    {
        // hide "player panel" visibility  
        TogglePanelVisibility("playing panel"); 
    }

    // show/hide panels based on previous state 
    public void TogglePanelVisibility(string panelName)
    {
        GameObject playPanel = GameObject.Find(panelName);
        if (playPanel)
        {
            // disable all components
            bool toReset = false; 
            MonoBehaviour[] comps = playPanel.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour c in comps)
            {
                c.enabled = !c.enabled;
                // check if the start panel needs to be reset 
                if (panelName.Equals("start panel"))
                {
                    if (c.enabled) toReset = true;
                }
            }
            // deactivate all children  
            for (int i = 0; i < playPanel.transform.childCount; i++)
            {
                var child = playPanel.transform.GetChild(i).gameObject;
                if (child != null)
                    child.SetActive(!child.activeSelf);
            }
            if (toReset)
            {
                ResetStartPanelValues();
            }
        }
    }

    // manually resets all designer inputs to default value
    public void ResetStartPanelValues()
    {
        Slider mySlider = GameObject.Find("length-slider").GetComponent<Slider>();
        mySlider.value = Control.mazeLengthDefault;
        mySlider = GameObject.Find("width-slider").GetComponent<Slider>();
        mySlider.value = Control.mazeWidthDefault;

        Control.numPrizes = Control.numPrizesDefault;
        Control.prizeScore = Control.prizeScoreDefault;
        Control.numLadders = Control.numLaddersDefault;
        Control.hasGates = Control.hasGateDefault;
        Control.characterChoice = Control.characterChoiceDefault;
        Control.themeChoice = Control.themeChoiceDefault;
        Control.jams = Control.jamsDefault; 
    }

    // connected to width slider 
    public void UpdateWidthButton(float i)
    {
        Control.mazeWidth = (int)i; 
    }

    // connected to length slider 
    public void UpdateLengthButton(float i)
    {
        Control.mazeLength = (int)i;
    }

    // restart level without changing maze or ladder disribution 
    public void RestartSameLevel() 
    {
        // put char back at (0,0,0)
        thePlayer.transform.position = new Vector3(0, thePlayer.transform.localScale.y * 0.5f, 0); 

        // transform gates so they are at original position
        if (gateObjX) 
        {
            Vector3 p = gateObjX.transform.position;
            gateObjX.transform.position = new Vector3(p.x, 0.5f, p.z); 
        }
        if (gateObjZ)
        {
            Vector3 p = gateObjZ.transform.position;
            gateObjZ.transform.position = new Vector3(p.x, 0.5f, p.z);
        }

        // destroy prizes + key
        GameObject prizeGeom = GameObject.Find("Prizes");
        if (prizeGeom != null)
        {
            foreach (Transform child in prizeGeom.transform)
            {
                // clear the map  
                foreach (Node n in nodesHadLittlePrizes)
                {
                    map[n] = false;
                }
                nodesHadLittlePrizes.Clear();

                Destroy(child.gameObject);
            }
        }

        GameObject keyGeom = GameObject.Find("Key"); 
        if (keyGeom != null)
        {
            map[nodeKeyWasOn] = false;
            nodeKeyWasOn = null;
            Destroy(keyGeom.gameObject); 
        }

        MakeKey(); 

        // make more prizes 
        MakePrizes(Control.numPrizes);
    }

    // erase the level completely 
    public void EraseLevel()
    {
        // clear geometry in: "Maze Geom", "Gate and Key", "Prizes", "Ladder Geometry" 
        GameObject geometry = GameObject.Find("Maze Geometry");
        if (geometry != null)
        {
            foreach (Transform child in geometry.transform)
            {
                Destroy(child.gameObject);
            }
        }

        geometry = GameObject.Find("Ladder Geometry");
        if (geometry != null)
        {
            foreach (Transform child in geometry.transform)
            {
                Destroy(child.gameObject);
            }
        }

        geometry = GameObject.Find("Prizes");
        if (geometry != null)
        {
            foreach (Transform child in geometry.transform)
            {
                Destroy(child.gameObject);
            }
        }

        geometry = GameObject.Find("Gate and Key");
        if (geometry != null)
        {
            foreach (Transform child in geometry.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // reset variables: allNodes, map, nodeshadlittleprizes, nodekeywason, goalpos, keypos, gateobjx, gateobjz
        allNodes.Clear();
        map.Clear();
        nodesHadLittlePrizes.Clear();
        nodeKeyWasOn = null;
        goalPos = new Vector3(0, 0, 0);
        keyPos = new Vector3(0, 0, 0);
        gateObjX = null;
        gateObjZ = null;
        thePlayer.transform.position = new Vector3(0, thePlayer.transform.localScale.y * 0.5f, 0);

        // enable start menu visualization 
        TogglePanelVisibility("start panel");
        TogglePanelVisibility("playing panel");
    }

    // todo 
    public void toggleDebugMode()
    {
        //if (allNodes.Count > 0)
        //{
        //    if (GameObject.Find("DebugGeom").chil) 
        //    {

        //    }
        //}
    }

    // todo: use this somewhere i guess 
    public void EraseDebugObjects() 
    {
        foreach (Node n in allNodes) 
        {
            Destroy(n.geom.gameObject);  
        }
    }


    // todo: call in "generatethelevel" function ? 
    // todo: maybe won't use multiple scenes 
    public void ChangeToScene(string sceneToChangeTo)
    {
        SceneManager.LoadScene(sceneToChangeTo);

    }

    public void ChangeToSampleScene()
    {
        SceneManager.LoadScene("SampleScene");

    }

    public void ChangeToGameScene() 
    {
        SceneManager.LoadScene("Scene2");
    }


    // length, width, numkeys, gate/no gate, "theme"... i guess
    public void GenerateTheLevelButton() 
    {
        // camera reference
        cam = Camera.main;

        // Maze Algorithm ///////////////////////////////////
        Maze m = new Maze(cam, Control.mazeWidth, Control.mazeLength, 1); 
        goalPos = m.goalPos;
        m.GenerateMaze();

        //display nodes in debug 
        foreach (Node node in m.allNodes)
        {
            //node.StartDebugVis(cam);

            // add all the nodes from the maze into main todo: good idea? idk 
            allNodes.Add(node);
            if (Vector3.Distance(node.position, new Vector3(0,0,0)) < 0.01) 
            {
                map.Add(node, true); 
            }
            else if (Vector3.Distance(node.position, goalPos) < 0.01)
            {
                map.Add(node, true);
            }
            else 
            {
                map.Add(node, false);
            }
        }

        //create a graph for later use
        g = new Graph(m.allNodes); // todo: make it allNodes instead of m.allNodes   

        ///////////////////////////////////////////////////


        // Finding Hard-Coded Node Faces ////////////////////
        if (pathFaces.Length == 0)
        {
            pathFaces = GameObject.FindGameObjectsWithTag("PathFace");
        }

        for (int i = 0; i < pathFaces.Length; i++)
        {
            GameObject face = pathFaces[i];
            Vector3 normal = Vector3.Normalize(face.transform.up); // is this always the correct normal? should be 
            Node n = new Node(i, "top", face.transform.position, face.transform.up, face.transform.right);
            float epsilon = 0.01f;

            if (normal == new Vector3(0, 1, 0))
            {
                n = new Node(i, "top", face.transform.position, face.transform.up, face.transform.right);
            }
            else if (normal == new Vector3(-1, 0, 0))
            {
                n = new Node(i, "negx", face.transform.position, face.transform.up, face.transform.right);
            }
            else if (normal == new Vector3(0, 0, -1))
            {
                n = new Node(i, "negz", face.transform.position, face.transform.up, face.transform.right);
            }
            else if (normal.x > epsilon || normal.x < -epsilon)
            {
                if (normal.y > 0)
                {
                    n = new Node(i, "diagx", face.transform.position, face.transform.up, face.transform.right);
                }
            }
            else if (normal.z > epsilon || normal.z < -epsilon)
            {
                if (normal.y > 0)
                {
                    n = new Node(i, "diagz", face.transform.position, face.transform.up, face.transform.right);
                }
            }

            allNodes.Add(n);
        }

        // assign neighbors to nodes automatically
        foreach (Node node in allNodes)
        {
            // todo: commetn back in ?  
            //node.FindGeomNeighbors(allNodes, cam);
            //node.FindIllusionNeighbors(allNodes, cam);

            //node.StartDebugVis(cam);
        }

        // referencing the character variable todo idk if this should be here ? 
        thePlayer = GameObject.Find("Character");
        playerScript = thePlayer.GetComponent<CharController>();
        playerScript.AssignFirstCurrNode(allNodes);
        ///////////////////////////////////////////////////
        if (Control.hasGates) MakeGatePlusKey(); // this needs to go first to lay down the gate 
        MakePrizes(Control.numPrizes); // todo: make some limits on dis (i.e. what is the max prizes, also same concern for ladders) 

        // show the play panel and hide the other panel 
        TogglePanelVisibility("start panel");
        TogglePanelVisibility("playing panel");

    }
    ////////////////////////////////////////////////////////////////////////////////////


    // Update is called once per frame
    void Update()
    {
        // Used for (debug) Character Movement    
        if (Input.GetMouseButtonDown(0))
        {
            if (playerScript)
            {
                playerScript.SetTargetPosition(allNodes);
                List<Node> path = g.AStar(playerScript.currNode, playerScript.targetNode);
                // USE THAT TO DIRECT THE PATH OF THE CHARACTER
                playerScript.WalkAlongPath(path);
            }

        }
    }

    bool MakeGatePlusKey()
    {
        GameObject gateGeom = GameObject.Find("Gate and Key"); 
        if (gateGeom == null) 
        {
            gateGeom = new GameObject();
            gateGeom.name = "Gate and Key";
        }

        Color buttonCol = new Color(36 / 255f, 56 / 255f, 36 / 255f);

        // potentially 2 gates  
        Vector3 gateX = goalPos + new Vector3(-1, 0, 0);
        Vector3 gateZ = goalPos + new Vector3(0, 0, -1);
        bool needXGate = false;
        bool needZGate = false;
        foreach (Node curr in allNodes)
        {
            if (curr.position.Equals(gateX)) 
            {
                needXGate = true;
                map[curr] = true; 
            }
            else if (curr.position.Equals(gateZ))
            {
                needZGate = true;
                map[curr] = true;
            }
        }
        if (needXGate)
        {
            gateObjX = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gateObjX.transform.position = goalPos + new Vector3(-1, 0.5f, 0);
            gateObjX.transform.localScale = new Vector3(0.1f, 1f, 0.9f);
            Renderer r1 = gateObjX.GetComponent<Renderer>();
            r1.material.SetColor("_Color", buttonCol);
            gateObjX.tag = "Gate";
            gateObjX.name = "Gate X"; 
            gateObjX.transform.SetParent(gateGeom.transform, true);


            gateObjX.AddComponent<Rigidbody>();
            gateObjX.GetComponent<Rigidbody>().useGravity = false;
            gateObjX.GetComponent<Rigidbody>().isKinematic = true;

        }
        if (needZGate)
        {
            gateObjZ = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gateObjZ.transform.position = goalPos + new Vector3(0, 0.5f, -1);
            gateObjZ.transform.localScale = new Vector3(0.9f, 1f, 0.1f);
            Renderer r1 = gateObjZ.GetComponent<Renderer>();
            r1.material.SetColor("_Color", buttonCol);
            gateObjZ.tag = "Gate";
            gateObjZ.name = "Gate Z";
            gateObjZ.transform.SetParent(gateGeom.transform, true);

            gateObjZ.AddComponent<Rigidbody>();
            gateObjZ.GetComponent<Rigidbody>().useGravity = false;
            gateObjZ.GetComponent<Rigidbody>().isKinematic = true;

        }

        // send to charController 
        if (gateObjX) playerScript.gate1 = gateObjX;
        if (gateObjZ) playerScript.gate2 = gateObjZ;

        MakeKey(); 
        return true; 
    }

    public bool MakeKey()
    {
        // todo: do some pathfinding to make sure that you can find the key!!  IDK MAN 

        GameObject gateGeom = GameObject.Find("Gate and Key");
        if (gateGeom == null)
        {
            gateGeom = new GameObject();
            gateGeom.name = "Gate and Key";
        }

        Color buttonCol = new Color(36 / 255f, 56 / 255f, 36 / 255f);

        List<Node> nodesToRestoreNeighs = new List<Node>(); 

        // make a temp graph to check if the button can be reached 
        List<Node> nodesWithoutGoalNode = new List<Node>();
        Node startNode = null; 
        foreach (Node curr in allNodes)
        {
            bool keepOut = false; 
            Vector3 displaceY = new Vector3(0, -.5f, 0); 
            if (gateObjX) 
            {
                if (Vector3.Distance(curr.position, gateObjX.transform.position + displaceY) < 0.01f)
                {
                    keepOut = true;
                    foreach (Node neigh in curr.neighList)
                    {
                        if (!nodesToRestoreNeighs.Contains(curr)) nodesToRestoreNeighs.Add(curr);
                        neigh.neighList.Remove(curr);
                    }
                }
            }
            if (gateObjZ)
            {
                if (Vector3.Distance(curr.position, gateObjZ.transform.position + displaceY) < 0.01f)
                {
                    keepOut = true;
                    foreach (Node neigh in curr.neighList)
                    {
                        if (!nodesToRestoreNeighs.Contains(curr)) nodesToRestoreNeighs.Add(curr);
                        neigh.neighList.Remove(curr);
                    }
                }
            }
            if (Vector3.Distance(curr.position, goalPos) < 0.01f)
            {
                keepOut = true;
                foreach (Node neigh in curr.neighList)
                {
                    if (!nodesToRestoreNeighs.Contains(curr)) nodesToRestoreNeighs.Add(curr);
                    neigh.neighList.Remove(curr);
                }
            }
            if (Vector3.Distance(curr.position, new Vector3(0,0,0)) < 0.01f)
            {
                startNode = curr;
            }
            if (!keepOut)
            {
                nodesWithoutGoalNode.Add(curr);
            }
            else 
            {
                // remove it from neighbors todo: this is affecting the actual nodes: put back later?? 
                //foreach (Node neigh in curr.neighList)
                //{
                //    //Node dummy = new Node(neigh.id, neigh.face, neigh.position, neigh.up, neigh.right);
                //    //foreach (Node ugh in neigh.neighList)
                //    //{
                //    //    if (!ugh.Equals(curr)) dummy.neighList.Add(ugh); 
                //    //}

                //    //// replace real node with dummy in the list 
                //    //nodesWithoutGoalNode.Remove(neigh);
                //    //nodesWithoutGoalNode.Add(dummy);  // i am defeated 

                //    // todo: find a different way :c 
                //    if (!nodesToRestoreNeighs.Contains(curr)) nodesToRestoreNeighs.Add(curr); 
                //    neigh.neighList.Remove(curr);
                //}

            }
        }

        // make a new graph - returning the right count 
        //Debug.Log("allNode Count: " + allNodes.Count);
        //Debug.Log("Otherwa Count: " + nodesWithoutGoalNode.Count); 

        Graph tempGraph = new Graph(nodesWithoutGoalNode);

        // place the button at random node 
        var i = Random.Range(0, allNodes.Count);
         Node n = allNodes[i];
        //var i = Random.Range(0, nodesWithoutGoalNode.Count);
        //Node n = nodesWithoutGoalNode[i];

        bool inSameComponent = tempGraph.InSameComponent(startNode, n);
        List<Node> path = tempGraph.AStar(startNode, n);

        //Debug.Log("length of path: " + path.Count);
        foreach (Node p in path) 
        {
            Debug.Log(p.position); 
        }
        //Debug.Log("result of inSameComponent: " + inSameComponent); // todo: WHY ISN'T THIS WORKING 
        //todo: have to go about removing it from its neighbors )))): AHHHGHGHGH 

        while (!(!n.position.Equals(new Vector3(0, 0, 0)) && !n.position.Equals(goalPos) && map[n] == false
                 && n.up.Equals(new Vector3(0, 1, 0)) && inSameComponent))
        {
            i = Random.Range(0, allNodes.Count);
            n = allNodes[i];
            inSameComponent = tempGraph.InSameComponent(startNode, n);
        }
        nodeKeyWasOn = n;
        keyPos = n.position;
        GameObject gateButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gateButton.transform.position = n.position + new Vector3(0, 0.05f, 0);
        gateButton.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);
        gateButton.transform.localEulerAngles = new Vector3(0, 45, 0);
        Renderer r2 = gateButton.GetComponent<Renderer>();
        r2.material.SetColor("_Color", buttonCol);
        gateButton.transform.SetParent(gateGeom.transform, true);
        gateButton.tag = "Key";
        gateButton.name = "Key";
        gateButton.AddComponent<Rigidbody>();
        gateButton.GetComponent<Rigidbody>().useGravity = false;
        gateButton.GetComponent<Rigidbody>().isKinematic = true;
        map[n] = true;

        // send to char controller 
        playerScript.keyPos = keyPos;

        // restore the neighbors
        Debug.Log("num in restorelist: " + nodesToRestoreNeighs.Count); 
        foreach (Node node in nodesToRestoreNeighs)
        {
            foreach (Node neigh in node.neighList)
            {
                neigh.neighList.Add(node);
            }
        }

        return true;
    }

    bool MakePrizes(int numPrizes)
    {
        GameObject prizeGeom; 
        if (GameObject.Find("Prizes") != null) 
        {
            prizeGeom = GameObject.Find("Prizes"); 
        }
        else {
            prizeGeom = new GameObject();
            prizeGeom.name = "Prizes";
        }

        if (allNodes.Count < 1)
        {
            Debug.Log("allNodes is empty, cannot make prizes");
            return false; 
        }
        Color prizeCol = new Color(171 / 255f, 0 / 255f, 0 / 255f);
        // Big prize (on goal node) 
        GameObject finalPrize = GameObject.CreatePrimitive(PrimitiveType.Cube);
        finalPrize.transform.position = goalPos + new Vector3(0,1,0) * 0.5f;
        finalPrize.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        finalPrize.transform.localEulerAngles = new Vector3(45, 0, 45);
        Renderer r = finalPrize.GetComponent<Renderer>();
        r.material.SetColor("_Color", prizeCol);
        finalPrize.AddComponent<Spin>();
        finalPrize.tag = "FinalPrize";
        finalPrize.name = "Final Prize"; 
        finalPrize.AddComponent<Rigidbody>();
        finalPrize.GetComponent<Rigidbody>().useGravity = false;
        finalPrize.transform.SetParent(prizeGeom.transform, true);

        // little prizes 
        List<Node> mixedList = RandomizeList(allNodes);
        int count = 0;
        int index = 0; 
        while (count < numPrizes)  
        {
            // ohh jsut check mixedList i guess ... ?
            if (index >= mixedList.Count)
            {
                return false;
            }
            Node node = mixedList[index]; 

            if (node.up.Equals(new Vector3(0,1,0))) 
            {
                if (!map[node])
                {
                    //bool stillRoom = false; 
                    //foreach (KeyValuePair<Node, bool> pair in map)
                    //{
                    //    if (!pair.Value) stillRoom = true;
                    //}
                    //if (!stillRoom) return false;

                    if (!node.position.Equals(new Vector3(0,0,0)) && !node.position.Equals(goalPos))
                    {
                        GameObject babyPrize = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        babyPrize.transform.position = node.position + node.up * 0.3f;
                        babyPrize.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        babyPrize.transform.localEulerAngles = new Vector3(45, 0, 45);
                        Renderer r1 = babyPrize.GetComponent<Renderer>();
                        r1.material.SetColor("_Color", prizeCol);
                        babyPrize.AddComponent<Spin>();
                        babyPrize.tag = "Prize";
                        babyPrize.name = "Prize";
                        babyPrize.AddComponent<Rigidbody>();
                        babyPrize.GetComponent<Rigidbody>().useGravity = false;
                        babyPrize.transform.SetParent(prizeGeom.transform, true);

                        count++;
                        nodesHadLittlePrizes.Add(node);
                        map[node] = true;
                    }
                }
            }
            index++; 
        }
        return true; 
    }

    // fisher-yates shuffle for Nodes 
    private List<Node> RandomizeList(List<Node> ts)
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


    void TestingFunction()
    {
        /////////// connected component graph testing ///////////////
        //Graph g = new Graph(allNodes);
        //int a = 1;
        //int b = 12; 
        //Debug.Log(g.InSameComponent(g.GetNodeById(a), g.GetNodeById(b)));

        //GameObject visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //visPos.transform.position = g.GetNodeById(a).position;
        //Renderer rend = visPos.GetComponent<Renderer>();
        //rend.material = new Material(Shader.Find("Specular"));
        //rend.material.SetColor("_Color", Color.grey);

        //GameObject visPos2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //visPos2.transform.position = g.GetNodeById(b).position;
        //Renderer rend2 = visPos2.GetComponent<Renderer>();
        //rend2.material = new Material(Shader.Find("Specular"));
        //rend2.material.SetColor("_Color", Color.grey);

        ///////////// minheap testing ///////////////
        //MinHeap h = new MinHeap(20);
        //h.InsertKey(new Node(10));
        //h.InsertKey(new Node(1));
        //h.InsertKey(new Node(5));
        //h.InsertKey(new Node(2));
        //h.InsertKey(new Node(0));
        //h.InsertKey(new Node(100));
        //h.InsertKey(new Node(20));
        //h.InsertKey(new Node(50));
        //h.InsertKey(new Node(22));
        //h.InsertKey(new Node(3));
        //h.InsertKey(new Node(40));
        //h.InsertKey(new Node(33));
        //h.InsertKey(new Node(201));
        //h.InsertKey(new Node(7));
        //h.InsertKey(new Node(9));
        //while(h.heapSize > 0) Debug.Log(h.ExtractMin().Fcost());

        ////////////// a* testing //////////////////
        //Graph g = new Graph(allNodes);
        //int a = 15;
        //int b = 12;
        //List<Node> path = g.AStar(g.GetNodeById(a), g.GetNodeById(b));

        //string outputPath = "( "; 

        // visualization 
        //foreach (Node n in path) 
        //{
        //    outputPath += n.id + " ";
        //    if (!(n.Equals(g.GetNodeById(a)) || n.Equals(g.GetNodeById(b))))
        //    {
        //        GameObject visPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //        visPos.transform.position = n.position;
        //        visPos.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        //        Renderer rend = visPos.GetComponent<Renderer>();
        //        //rend.material = new Material(Shader.Find("Specular"));
        //        rend.material.SetColor("_Color", Color.green);
        //    }
        //}
        ////GameObject v2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //v2.transform.position = g.GetNodeById(a).position;
        //v2.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        //Renderer rend2 = v2.GetComponent<Renderer>();
        //rend2.material.SetColor("_Color", Color.green); // new Color(1f, 109 / 255.0f, 25 / 255.0f));

        //v2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //v2.transform.position = g.GetNodeById(b).position;
        //v2.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        //rend2 = v2.GetComponent<Renderer>();
        //rend2.material.SetColor("_Color", Color.red);

        //Debug.Log("Path: " + outputPath + ")"); 
    }
}

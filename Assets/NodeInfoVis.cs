using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInfoVis : MonoBehaviour {

    public int id;
    public Vector3 pos; // only public for visualization purposes ! 


    // Use this for initialization
    void Start()
    {
        int idVal;
        // parse the string name to int
        if (!int.TryParse(this.name, out idVal)) idVal = -1;
        pos = this.transform.position;

    }

    // Update is called once per frame
    void Update()
    {

    }
}

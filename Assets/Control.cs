using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

    // "designer" controls 
    public static int mazeLength = 2;
    public static int mazeWidth = 2;
    public static int numPrizes = 10;
    public static int prizeScore = 1;
    public static int numLadders = 4;
    public static bool hasGates = true;
    public static int characterChoice;
    public static int themeChoice;
    public  int jams; // ? 

    // "designer" controls during gameplay 
    public static bool mouseclickControls = false;
    public static bool debugMode = false; 


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

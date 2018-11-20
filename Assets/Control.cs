using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

    // "designer" controls 
    public static int mazeLength = 2;
    public static int mazeWidth = 2;
    public static int numPrizes = 10;
    public static int prizeScore = 1;
    public static int numLadders = 40;
    public static bool hasGates = true;
    public static int characterChoice;
    public static int themeChoice;
    public  int jams; // ? 

    // default used for resetting game 
    public const int mazeLengthDefault = 2;
    public const int mazeWidthDefault = 2;
    public const int numPrizesDefault = 10;
    public const int prizeScoreDefault = 1;
    public const int numLaddersDefault = 40;
    public const bool hasGateDefault = true;
    public const int characterChoiceDefault = 0;
    public const int themeChoiceDefault = 0;
    public const int jamsDefault = 0; // ? 


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

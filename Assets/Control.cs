using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

    // "designer" controls 
    public static int mazeLength = 2;
    public static int mazeWidth = 2;
    public static int numPrizes = 0;
    public static int prizeScore = 1;
    public static int numLadders = 0;
    public static bool hasGates = false;
    public static int characterChoice;
    public static int themeChoice;
    public static int jams; // ? 

    // colors - todo hardcoded  
    public static Color charColor; //= new Color(137/255f, 162/255f, 180/255f);
    public static Color prizeColor = new Color(194 / 255f, 241 / 255f, 225 / 255f);
    public static Color pathColor = new Color(66 / 255f, 95 / 255f, 101 / 255f);
    public static Color gateColor = new Color(70 / 255f, 108 / 255f, 85 / 255f);
    public static Color backgroundColor; // already exists before runtime  
    
    // default used for resetting game 
    public const int mazeLengthDefault = 2;
    public const int mazeWidthDefault = 2;
    public const int numPrizesDefault = 0;
    public const int prizeScoreDefault = 1;
    public const int numLaddersDefault = 0;
    public const bool hasGateDefault = false;
    public const int characterChoiceDefault = 0;
    public const int themeChoiceDefault = 0;
    public const int jamsDefault = 0; // ? 


    // "designer" controls during gameplay 
    public static bool mouseclickControls = false;
    public static bool debugMode = false;

    // "player" scorekeeping
    public static int playerScore = 0;
    public static bool isGameOver = false; // todo: use this 

    // todo: add timer??? 

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

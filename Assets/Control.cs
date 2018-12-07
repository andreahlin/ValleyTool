using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Control : MonoBehaviour {

    public static bool firstLanding = true;

    // "designer" controls 
    public static int mazeLength = 2;
    public static int mazeWidth = 2;
    public static int numPrizes = 0;
    public static int prizeScore = 1;
    public static int numLadders = 0;
    public static bool hasGates = false;
    public static int characterChoice;
    public static int themeChoice;

    // default used for resetting game 
    public const int mazeLengthDefault = 2;
    public const int mazeWidthDefault = 2;
    public const int numPrizesDefault = 0;
    public const int prizeScoreDefault = 1;
    public const int numLaddersDefault = 0;
    public const bool hasGateDefault = false;
    public const int characterChoiceDefault = 0;
    public const int themeChoiceDefault = 0;

    // "designer" controls during gameplay 
    public static bool mouseclickControls = false;
    //public static bool pathTestingControls = false;
    public static bool showingDesignerControl = false;

    // "player" scorekeeping
    public static int playerScore = 0;
    public static bool isGameOver = true;
    public static float timer = 0.0f;

    // color options 
    public static Color lagoonCharacter = new Color(171 / 255f, 123 / 255f, 27 / 255f);
    public static Color lagoonBackground = new Color(158 / 255f, 164 / 255f, 148 / 255f);
    public static Color lagoonPrize = new Color(194 / 255f, 241 / 255f, 225 / 255f);
    public static Color lagoonPath = new Color(66 / 255f, 95 / 255f, 101 / 255f);
    public static Color lagoonGate = new Color(12 / 255f, 29 / 255f, 58 / 255f);

    public static Color melonCharacter = new Color(192 / 255f, 23 / 255f, 14 / 255f);
    public static Color melonBackground = new Color(239 / 255f, 206 / 255f, 131 / 255f);
    public static Color melonPrize = new Color(247 / 255f, 186 / 255f, 50 / 255f); 
    public static Color melonPath = new Color(212 / 255f, 70 / 255f, 49 / 255f);
    public static Color melonGate = new Color(237 / 255f, 123 / 255f, 43 / 255f);

    public static Color chinaCharacter = new Color(255 / 255f, 220 / 255f, 117 / 255f);
    public static Color chinaBackground = new Color(84 / 255f, 84 / 255f, 84 / 255f);
    public static Color chinaPrize = new Color(184 / 255f, 179 / 255f, 147 / 255f);
    public static Color chinaPath = new Color(154 / 255f, 9 / 255f, 0 / 255f);
    public static Color chinaGate = new Color(84 / 255f, 84 / 255f, 84 / 255f);

    public static Color pastelCharacter = new Color(220 / 255f, 117 / 255f, 137 / 255f);
    public static Color pastelBackground = new Color(223 / 255f, 175 / 255f, 161 / 255f);
    public static Color pastelPrize = new Color(241 / 255f, 198 / 255f, 196 / 255f);
    public static Color pastelPath = new Color(170 / 255f, 168 / 255f, 192 / 255f); 
    public static Color pastelGate = new Color(255 / 255f, 175 / 255f, 175 / 255f);

    public static Color floraCharacter = new Color(91 / 255f, 145 / 255f, 135 / 255f);
    public static Color floraPrize = new Color(214 / 255f, 74 / 255f, 53 / 255f);
    public static Color floraPath = new Color(224 / 255f, 163 / 255f, 156 / 255f);
    public static Color floraGate = new Color( 160/ 255f,45 / 255f, 37/ 255f);
    public static Color floraBackground = new Color(173 / 255f, 207 / 255f, 207 / 255f);

    // todo: not used 
    public static Color desertCharacter = new Color(166 / 255f, 160 / 255f, 144 / 255f);
    public static Color desertPrize = new Color(214 / 255f, 74 / 255f, 53 / 255f);
    public static Color desertPath = new Color(111 / 255f, 109 / 255f, 80 / 255f);
    public static Color desertGate = new Color(162 / 255f, 165 / 255f, 138 / 255f);
    public static Color desertBackground = new Color(180 / 255f, 110 / 255f, 90 / 255f);

    public static Color citrusCharacter = new Color(239 / 255f, 224 / 255f, 197 / 255f);
    public static Color citrusPrize = new Color(203 / 255f, 43 / 255f, 84 / 255f);
    public static Color citrusPath = new Color(242 / 255f, 202 / 255f, 88 / 255f);
    public static Color citrusGate = new Color(255 / 255f, 144 / 255f, 114 / 255f);
    public static Color citrusBackground = new Color(217 / 255f, 130 / 255f, 120 / 255f);

    // Control Colors   
    public static Color charColor = lagoonCharacter;
    public static Color backgroundColor = lagoonBackground;
    public static Color prizeColor = lagoonPrize;
    public static Color pathColor = lagoonPath;
    public static Color gateColor = lagoonGate;
}

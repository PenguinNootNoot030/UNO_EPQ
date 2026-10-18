using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class PlayerTurnIndicator : MonoBehaviour
{
    private static Text TurnIndicator;

    void Start()
    {
        TurnIndicator = GameObject.Find("PlayerTurnIndicator")?.GetComponent<Text>();
    }

    
    public static void UpdateTurnIndication(int currentPlayerIndex,bool directionClockwise, cardColour colour) => TurnIndicator.text = $"Current Player : Player {currentPlayerIndex} \n DirectionIsClockwise: {directionClockwise} \n CurrentColour: {colour}";
}

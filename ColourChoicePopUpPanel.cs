using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ColourChoicePopUpPanel : MonoBehaviour
{

    private static GameObject popUpPanel;
    private Button ChoiceRed;
    private Button ChoiceBlue;
    private Button ChoiceYellow;
    private Button ChoiceGreen;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("EGO").GetComponent<GameManager>();
        popUpPanel = GameObject.Find("ColourSelectionPanel");
        ChoiceRed = GameObject.Find("ChoiceRed")?.GetComponent<Button>();
        ChoiceBlue = GameObject.Find("ChoiceBlue")?.GetComponent<Button>();
        ChoiceGreen = GameObject.Find("ChoiceGreen")?.GetComponent<Button>();
        ChoiceYellow = GameObject.Find("ChoiceYellow")?.GetComponent<Button>();
        popUpPanel.SetActive(false);
        //lambda expression used for simplifying the colour selection functions into one.
        ChoiceRed.onClick.AddListener(() => ColourSelection("red"));
        ChoiceBlue.onClick.AddListener(() => ColourSelection("blue"));
        ChoiceYellow.onClick.AddListener(() => ColourSelection("yellow"));
        ChoiceGreen.onClick.AddListener(() => ColourSelection("green"));
    }
    public static void ShowPopUpPanel() => popUpPanel.SetActive(true);

    private void ColourSelection(string colour)
    {
        popUpPanel.SetActive(false);
        switch (colour)
        {
            case "red":
                gameManager.GetComponent<PhotonView>().RPC("SetCardColour", RpcTarget.AllBuffered, cardColour.red);
                Debug.Log($"The current colour is set to {gameManager.currentCardColour}.");
                break;
            case "blue":
                gameManager.GetComponent<PhotonView>().RPC("SetCardColour", RpcTarget.AllBuffered, cardColour.blue);
                Debug.Log($"The current colour is set to {gameManager.currentCardColour}.");
                break;
            case "green":
                gameManager.GetComponent<PhotonView>().RPC("SetCardColour", RpcTarget.AllBuffered, cardColour.green);
                Debug.Log($"The current colour is set to {gameManager.currentCardColour}.");
                break;
            case "yellow":
                gameManager.GetComponent<PhotonView>().RPC("SetCardColour", RpcTarget.AllBuffered, cardColour.yellow);
                Debug.Log($"The current colour is set to {gameManager.currentCardColour}.");
                break;
            default:
                Debug.Log("Error: none existing card selcted");
                break;
        }
    }
}

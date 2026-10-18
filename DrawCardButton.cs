using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardButton : MonoBehaviour
{
    private Button DrawFromPileButton;

    void Start()
    {
        DrawFromPileButton = GameObject.Find("DrawFromPileButton")?.GetComponent<Button>();
        DrawFromPileButton.interactable = false;
       
        DrawFromPileButton.onClick.RemoveAllListeners();
        DrawFromPileButton.onClick.AddListener(DFPButtonClicked);
    }
    //logic for button being deactivated until the player's turn is reached AND does not have playable card is required.

    public void EnableDrawCard()=> DrawFromPileButton.interactable = true;
    public void DisableDrawCard() => DrawFromPileButton.interactable = false;

    void DFPButtonClicked()
    {
        GameObject.Find("EGO").GetComponent<PlayerBehaviour>().DrawCard();
    }
}

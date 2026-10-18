using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ButtonControl : MonoBehaviourPunCallbacks
{
    public Button StartButton;
    public CallOutButton callOutButton;
    
    
    private DeckManager deckManager;
    private GameManager gameManager;

    void Start()
    {
        StartButton = GameObject.Find("StartButton")?.GetComponent<Button>();
        callOutButton = GameObject.Find("CallOutButton")?.GetComponent<CallOutButton>();

        if (PhotonNetwork.IsMasterClient)
        {
            StartButton.onClick.RemoveAllListeners();
            StartButton.onClick.AddListener(StartButtonClicked);
        }
        else { StartButton.gameObject.SetActive(false); }
    }

    void StartButtonClicked()
    {
        deckManager = GameObject.Find("EGO").GetComponent<DeckManager>();
        gameManager = GameObject.Find("EGO").GetComponent<GameManager>();
        StartButton.gameObject.SetActive(false);
        deckManager.GetComponent<PhotonView>().RPC("DealCards", RpcTarget.AllBuffered, gameManager.GetNumOfPlayers());
        gameManager.GetComponent<PhotonView>().RPC("StartTurn", RpcTarget.AllBuffered);
        callOutButton.GetComponent<PhotonView>().RPC("EnableCallOut",RpcTarget.AllBuffered);
    }
    
}

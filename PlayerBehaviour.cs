using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class PlayerBehaviour : MonoBehaviour
{
    private DeckManager deckManager;
    private DrawCardButton drawButton;
    public bool Playable = true;
    private GameManager gameManager;
    public bool[] UNOStatus;
    public bool[] StackablePlusCardPresecence;

    void Start()
    {
        deckManager = GameObject.Find("EGO").GetComponent<DeckManager>();
        drawButton = GameObject.Find("DrawFromPileButton")?.GetComponent<DrawCardButton>();
        gameManager = GameObject.Find("EGO").GetComponent<GameManager>();
        UNOStatus = new bool[gameManager.GetNumOfPlayers()];
        StackablePlusCardPresecence = new bool[gameManager.GetNumOfPlayers()];
        for (int i = 0; i < gameManager.GetNumOfPlayers();i++)
        {
            UNOStatus[i] = false;
            StackablePlusCardPresecence[i] = false;
        }
    }

    public void DrawCard()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        deckManager.shuffledDeck[0].GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
        Card topcard = deckManager.shuffledDeck[0];

        topcard.isFaceUp = true;
        topcard.interactable = true;
        topcard.GetComponent<PhotonView>().RPC("SynchroniseCardAdding", RpcTarget.AllBuffered, playerIndex);
        topcard.DisplayCard(playerIndex, deckManager.playerHands[playerIndex].Count - 1);
        topcard.UpdateCard();

        if (topcard.CanBePlayedOn())
        {
            gameManager.Playable = true;
            drawButton.DisableDrawCard();
        }

        if (deckManager.playerHands[playerIndex].Count == 2 && gameManager.Playable)
        { DeclareUNOButton.EnableShoutOut(); }
        else { DeclareUNOButton.DisableShoutOut(); }

        deckManager.GetComponent<PhotonView>().RPC("SynchroniseOthersDeckAfterDrawCard", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void DeclareUNO() => UNOStatus[gameManager.GetPlayerIndex()] = true;
}
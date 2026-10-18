using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    private DeckManager deckManager;
    private ActionCard actionCard;
    private SceneLoader sceneLoader;
    private PlayerBehaviour playerBehaviour;
    private DrawCardButton drawButton;
    
    public bool directionIsClockwise = true;
    public cardColour currentCardColour = cardColour.none;
    public int currentCardDigit = -1;
    int numOfPlayers = 4;
    public int numOfplayersInRoom = 0;
    int currentPlayerIndex = 0;
    public List<Card> DiscardPile = new List<Card>();
    public bool Playable = false;
    public int stackedPlusCards = 0;
    public bool IsMyTurn = false;
    public bool winConAcieved = false;
    public bool CurrentPlayerStackable = false;


    
    public int GetNumOfPlayers() => numOfPlayers;
    public int GetDicardPileCount() => DiscardPile.Count;
    public int GetPlayerIndex() => currentPlayerIndex;

    [PunRPC]
    public void SetStackableState(bool IsStackble) => CurrentPlayerStackable = IsStackble;

    [PunRPC]
    public void SetCardColour(cardColour newColour) => currentCardColour = newColour;

    private void Start()
    {
        playerBehaviour = GameObject.Find("EGO").GetComponent<PlayerBehaviour>();
        drawButton = GameObject.Find("DrawFromPileButton")?.GetComponent<DrawCardButton>();
        deckManager = GameObject.Find("EGO").GetComponent<DeckManager>();
        actionCard = GameObject.Find("EGO").GetComponent<ActionCard>();
        sceneLoader = GameObject.Find("EGO").GetComponent<SceneLoader>();
        currentPlayerIndex = 0;
    }

    [PunRPC]
    public void AdvanceTurn()
    {
        if (directionIsClockwise)
        { currentPlayerIndex = (currentPlayerIndex + 1) % numOfPlayers; }
        else
        {
            currentPlayerIndex = currentPlayerIndex - 1;
            if (currentPlayerIndex == -1) { currentPlayerIndex = numOfPlayers - 1; }
        }
    }

    public void DisplayUnplayability()
    {
        foreach (Card card in deckManager.playerHands[currentPlayerIndex])
        {
            card.UnplayableCardVisualUpdate();
        }
    }

    [PunRPC]
    public void StartTurn()
    {
        PlayerTurnIndicator.UpdateTurnIndication(currentPlayerIndex,directionIsClockwise,currentCardColour);
        if (currentPlayerIndex == PhotonNetwork.LocalPlayer.ActorNumber - 1)
        { IsMyTurn = true; }
        Playable = false;
        playerBehaviour.UNOStatus[currentPlayerIndex] = false;
        Debug.Log($"It is Player {currentPlayerIndex}'s turn. previous card {currentCardColour},{currentCardDigit}");
        if (IsMyTurn)
        {
            foreach (Card card in deckManager.playerHands[currentPlayerIndex])
            {
                card.interactable = true;
                if (card.CanBePlayedOn())
                {
                    Playable = true;
                    card.PlayableCardVisualUpdate();
                }
                else { card.UnplayableCardVisualUpdate(); }
            }
        }
        
        if (!Playable && IsMyTurn)
        { drawButton.EnableDrawCard(); }
        else
        { drawButton.DisableDrawCard(); }
        if (deckManager.playerHands[currentPlayerIndex].Count == 2 && Playable && IsMyTurn) 
        { DeclareUNOButton.EnableShoutOut(); }
        else { DeclareUNOButton.DisableShoutOut(); }
        IsMyTurn = false;
    }

    [PunRPC]
    public void PlayerHandUpdate()
    {
        for (int i = 0; i < deckManager.playerHands[currentPlayerIndex].Count;i++)
        { deckManager.playerHands[currentPlayerIndex][i].DisplayCard(currentPlayerIndex, i); }
    }

    [PunRPC]
    public void TargetHandUpdate(int targetIndex)
    {
        for (int i = 0; i < deckManager.playerHands[targetIndex].Count; i++)
        { deckManager.playerHands[targetIndex][i].DisplayCard(targetIndex, i); }
    }

    [PunRPC]
    public void IncreaseNumOfStackedPlusCard(int numOfCards)
    { stackedPlusCards += numOfCards; }

    [PunRPC]
    public void ClearNumOfStackedPlusCard()
    { stackedPlusCards = 0; }

    [PunRPC]
    public void ChangeDirection()
    { directionIsClockwise = !directionIsClockwise; }

    
    public void PerformAction()
    {
        Card previousCard = DiscardPile[DiscardPile.Count - 1];
        switch (previousCard.type)
        {
            case cardType.drawTwo:
                actionCard.PlusTwo(currentPlayerIndex, numOfPlayers,
                    deckManager.playerHands, deckManager.shuffledDeck);
                Debug.Log("Card Added.");
                break;
            case cardType.reverse:
                actionCard.Reverse();
                Debug.Log($"Reversed to {directionIsClockwise}.");
                break;
            case cardType.skip:
                actionCard.Skip();
                Debug.Log("SKIP.");
                break;
            case cardType.wild:
                actionCard.WildChangeColour();
                Debug.Log("CC.");
                break;
            case cardType.wildDrawFour:
                actionCard.WildPlusFour(currentPlayerIndex,numOfPlayers,
                    deckManager.playerHands,deckManager.shuffledDeck);
                Debug.Log("Card Added.");
                break;
            default:
                break;
        }
        previousCard.GetComponent<PhotonView>().RPC("DiscardCardUpdate", RpcTarget.AllBuffered);
    }

    int[] playerIndices = { 0, 1, 2, 3 };
    List<int> LplayerCardCounts = new List<int>();

    [PunRPC]
    public void WiningConAchieved()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (List<Card> playerhand in deckManager.playerHands)
        { LplayerCardCounts.Add(playerhand.Count); }

        int[] playerCardCounts = LplayerCardCounts.ToArray();
        Array.Sort(playerCardCounts, playerIndices);

        ExitGames.Client.Photon.Hashtable customGameProperties = new ExitGames.Client.Photon.Hashtable
        {{ "playerPlacement", playerIndices }};
        PhotonNetwork.CurrentRoom.SetCustomProperties(customGameProperties);

        Debug.Log($"Placement: {playerIndices}");

        /*
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("playerPlacement", out object value))
        {
            Debug.Log($"Successfully set playerPlacement: {customGameProperties["playerPlacement"]}");
        }
        else
        {
            Debug.LogError("Failed to set playerPlacement");
        }
        */
    }
}

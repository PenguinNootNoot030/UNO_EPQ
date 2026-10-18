using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionCard : MonoBehaviour
{
    private DeckManager deckManager;
    private GameManager gameManager;
    int targetIndex = 0;

    void Start()
    {
        deckManager = GameObject.Find("EGO").GetComponent<DeckManager>();
        gameManager = GameObject.Find("EGO").GetComponent<GameManager>();
    }

    //----------------------------- Card functionalities
    public void Reverse() => gameManager.GetComponent<PhotonView>().RPC("ChangeDirection", RpcTarget.AllBuffered);

    public void PlusTwo(int playerIndex, int numOfPlayers,List<List<Card>> playerHands, List<Card> SourcePile)
    {
        targetIndex = TargetIndexCalc(playerIndex, numOfPlayers);
        if (TargetHasStackablePlusCard())
        {
            gameManager.GetComponent<PhotonView>().RPC("IncreaseNumOfStackedPlusCard", RpcTarget.AllBuffered, 2);
            gameManager.GetComponent<PhotonView>().RPC("SetStackableState", RpcTarget.AllBuffered, true);
        }
        else
        {
            deckManager.GetComponent<PhotonView>().RPC("AddCardToTargetHand", RpcTarget.AllBuffered, 2 + gameManager.stackedPlusCards, targetIndex);
            gameManager.GetComponent<PhotonView>().RPC("SetStackableState", RpcTarget.AllBuffered, false);
            gameManager.GetComponent<PhotonView>().RPC("ClearNumOfStackedPlusCard", RpcTarget.AllBuffered);
            Skip();
        }
    }

    public void Skip() => gameManager.GetComponent<PhotonView>().RPC("AdvanceTurn", RpcTarget.AllBuffered);

    public void WildChangeColour()
    {
        ColourChoicePopUpPanel.ShowPopUpPanel();
    }

    public void WildPlusFour(int playerIndex, int numOfPlayers, List<List<Card>> playerHands, List<Card> SourcePile)
    {
        WildChangeColour();
        targetIndex = TargetIndexCalc(playerIndex, numOfPlayers);
        if (TargetHasStackablePlusCard())
        {
            gameManager.GetComponent<PhotonView>().RPC("IncreaseNumOfStackedPlusCard", RpcTarget.AllBuffered, 4);
            //
            gameManager.GetComponent<PhotonView>().RPC("SetStackableState", RpcTarget.AllBuffered, true);
        }
        else
        {
            deckManager.GetComponent<PhotonView>().RPC("AddCardToTargetHand", RpcTarget.AllBuffered, 4 + gameManager.stackedPlusCards, targetIndex);
            gameManager.GetComponent<PhotonView>().RPC("SetStackableState", RpcTarget.AllBuffered, false); //
            gameManager.GetComponent<PhotonView>().RPC("ClearNumOfStackedPlusCard", RpcTarget.AllBuffered);
            Skip();
        }
    }


    //-------------------------------Other methods

    private bool TargetHasStackablePlusCard()
    {
        foreach (Card card in deckManager.playerHands[targetIndex])
        {
            if (card.type == cardType.drawTwo | card.type == cardType.wildDrawFour)
            { return true; }
        }
        return false;
    }
    
    private int TargetIndexCalc(int playerIndex, int numOfPlayers)
    {
        int target = 0;
        if (gameManager.directionIsClockwise)
        { target = (playerIndex + 1) % numOfPlayers; }
        else
        {
            target = playerIndex - 1;
            if (target == -1) { target = numOfPlayers - 1; }
        }
        return target;
    }
    /*    private IEnumerator WaitForColourSelection()
    {
        yield return null;
        yield return new WaitUntil(() => gameManager.currentCardColour != cardColour.none);
        Debug.Log($"Color selected: {gameManager.currentCardColour}");
    }
    */

    /*
    [PunRPC]
    public void AddCardToTargetHand(int times, int targetIndex)
    {
        for (int i = 0; i < times; i++)
        {
            deckManager.shuffledDeck[0].GetComponent<PhotonView>().TransferOwnership(targetIndex + 1);
            deckManager.playerHands[targetIndex].Add(deckManager.shuffledDeck[0]);
            Debug.Log($"Added card into player {targetIndex}'s hand");
            deckManager.shuffledDeck[0].DisplayCard(targetIndex, deckManager.playerHands[targetIndex].Count - 1);
            if (targetIndex + 1 == PhotonNetwork.LocalPlayer.ActorNumber)
            { deckManager.shuffledDeck[0].FlipToFront(); }
            deckManager.shuffledDeck.RemoveAt(0);
        }
    }
    */

}

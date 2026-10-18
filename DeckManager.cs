using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DeckManager : MonoBehaviour
{ 
    List<Card> initialDeck = new List<Card>();
    public List<Card> shuffledDeck = new List<Card>();
    public GameObject cardPrefab;
    private Dictionary<string, Material> frontMaterials = new Dictionary<string, Material>();
    public Material backMaterial;
    public List<List<Card>> playerHands = new List<List<Card>>();
    List<int> shuffledCardIndex = new List<int>();

    void Start()
    {
        LoadMaterials();
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeDeck();
            ShuffleDeckID();
            GetComponent<PhotonView>().RPC("ShuffledDeckSynchronisation", RpcTarget.AllBuffered, shuffledCardIndex.ToArray());
        }
    }

    private void InitializeDeck()
    {
        foreach (cardColour cc in System.Enum.GetValues(typeof(cardColour)))
        {
            if (cc == cardColour.none) continue;
            // one 0, two of 1-9
            for (int i = 0; i <= 9; i++)
            { AddCardToDeck(cardType.number, cc, i); }
            for (int i = 1; i <= 9; i++)
            { AddCardToDeck(cardType.number, cc, i); }

            // two of each colour actions
            AddCardToDeck(cardType.drawTwo, cc, 10);
            AddCardToDeck(cardType.drawTwo, cc, 10);

            AddCardToDeck(cardType.reverse, cc, 11);
            AddCardToDeck(cardType.reverse, cc, 11);

            AddCardToDeck(cardType.skip, cc, 12);
            AddCardToDeck(cardType.skip, cc, 12);
        }
        //four of each wild
        for (int i = 0; i < 4; i++)
        {
            AddCardToDeck(cardType.wild, cardColour.none, 13);
            AddCardToDeck(cardType.wildDrawFour, cardColour.none, 14);

        }
    }

    public void LoadMaterials()
    {
        Material[] loadedMaterial = Resources.LoadAll<Material>("CardMaterials");
        foreach (Material material in loadedMaterial)
        {
            frontMaterials[material.name] = material;
        }
    }

    
    private void AddCardToDeck(cardType ct, cardColour cc, int digit )
    {
        GameObject cardObject = PhotonNetwork.Instantiate(cardPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
        GetComponent<PhotonView>().RPC("AssigningValueToCard", RpcTarget.AllBuffered, ct, cc, digit, cardObject.GetComponent<PhotonView>().ViewID);
        
    }

    [PunRPC]
    public void AssigningValueToCard(cardType ct, cardColour cc, int digit, int viewID)
    {
        
        Card card = PhotonView.Find(viewID).GetComponent<Card>();

        string colour = "";
        if (cc == cardColour.red) { colour = "R"; }
        else if (cc == cardColour.blue) { colour = "B"; }
        else if (cc == cardColour.yellow) { colour = "Y"; }
        else if (cc == cardColour.green) { colour = "G"; }
        else if (cc == cardColour.none) { colour = "Wild"; }
        string followUp = "";
        if (ct == cardType.number) { followUp = digit.ToString(); }
        else if (ct == cardType.drawTwo) { followUp = "P2"; }
        else if (ct == cardType.reverse) { followUp = "T"; }
        else if (ct == cardType.skip) { followUp = "Stop"; }
        else if (ct == cardType.wild) { followUp = "PickColour"; }
        else if (ct == cardType.wildDrawFour) { followUp = "PlusFour"; }

        string materialName = colour + followUp;
        backMaterial = frontMaterials["BG"];

        if (frontMaterials.TryGetValue(materialName, out Material frontMaterial))
        {
            card.Initialize(ct, cc, digit, frontMaterial, backMaterial, false);
            initialDeck.Add(card);
            PhotonView.Find(viewID).gameObject.SetActive(false);
        }
    }

    public void ShuffleDeckID()
    {
        List<Card> sourcePile = new (initialDeck);
        while (sourcePile.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, sourcePile.Count);
            shuffledCardIndex.Add(randomIndex);
            sourcePile.RemoveAt(randomIndex);
        }
    }

    [PunRPC]
    public void ShuffledDeckSynchronisation(int[] shuffledCardIndex)
    {
        for (int i = 0; i < shuffledCardIndex.Length; i++)
        {
            shuffledDeck.Add(initialDeck[shuffledCardIndex[i]]);
            initialDeck.RemoveAt(shuffledCardIndex[i]);
        }
    }


    [PunRPC]
    public void DealCards(int numOfPlayers)
    {
        for (int i = 0; i < numOfPlayers; i++)
        { playerHands.Add(new List<Card>()); }
        //for (int cardIndex = 0; cardIndex < 7; cardIndex++)
        for (int cardIndex = 0; cardIndex < 7; cardIndex++)
        {
            for (int playerIndex = 0; playerIndex < numOfPlayers; playerIndex++)
            {
                playerHands[playerIndex].Add(shuffledDeck[0]);
                shuffledDeck[0].GetComponent<PhotonView>().TransferOwnership(playerIndex + 1);
                if (playerIndex + 1 == PhotonNetwork.LocalPlayer.ActorNumber) 
                { shuffledDeck[0].FlipToFront(); }
                shuffledDeck[0].DisplayCard(playerIndex, cardIndex);
                shuffledDeck.RemoveAt(0);
            }
        }
    }

    [PunRPC]
    public void SynchroniseOthersDeckAfterDrawCard()
    { shuffledDeck.RemoveAt(0); }

    [PunRPC]
    public void SynchronisePlayerPrefab()
    {
        int mainControllerID = PhotonNetwork.LocalPlayer.ActorNumber;
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            int currentPlayerID = player.GetPlayerID();
            if (currentPlayerID < mainControllerID) 
            { currentPlayerID = currentPlayerID + 4; }
            int relativeDiffernce = currentPlayerID - mainControllerID;
            Debug.Log($"Player {player.GetComponent<Player>().GetPlayerID()}");
            Debug.Log($"@client  {mainControllerID}");
            Vector3 pos;
            switch (relativeDiffernce)
            {
                case 0:
                    pos = new(-5.5f, -3.3f, 0);
                    break;
                case 1:
                    pos = new Vector3(-7, 2.7f, 0);
                    break;
                case 2:
                    pos = new Vector3(7, 3.7f, 0);
                    break;
                case 3:
                    pos = new Vector3(8.8f, -1.7f, 0);
                    break;
                default:
                    Debug.Log($"none exisiting playerID");
                    pos = new Vector3(0, 0, 0);
                    break;
            }
            player.transform.position = pos;
        }
    }

    [PunRPC]
    public void AddCardToTargetHand(int times, int targetIndex)
    {
        for (int i = 0; i < times; i++)
        {
            shuffledDeck[0].GetComponent<PhotonView>().TransferOwnership(targetIndex + 1);
            playerHands[targetIndex].Add(shuffledDeck[0]);
            Debug.Log($"Added card into player {targetIndex}'s hand");
            shuffledDeck[0].DisplayCard(targetIndex, playerHands[targetIndex].Count - 1);
            if (targetIndex + 1 == PhotonNetwork.LocalPlayer.ActorNumber)
            { shuffledDeck[0].FlipToFront(); }
            shuffledDeck.RemoveAt(0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CallOutPopUpPanel : MonoBehaviour
{
    private static GameObject popUpPanel;
    private Button Player0;
    private Button Player1;
    private Button Player2;
    private Button Player3;

    private DeckManager deckManager;
    private PlayerBehaviour playerBehaviour;
    private GameManager gameManager;

    void Start()
    {
        playerBehaviour = GameObject.Find("EGO").GetComponent<PlayerBehaviour>();
        deckManager = GameObject.Find("EGO").GetComponent<DeckManager>();
        gameManager = GameObject.Find("EGO").GetComponent<GameManager>();
        popUpPanel = GameObject.Find("CallOutPanel");
        Player0 = GameObject.Find("Player0")?.GetComponent<Button>();
        Player1 = GameObject.Find("Player1")?.GetComponent<Button>();
        Player2 = GameObject.Find("Player2")?.GetComponent<Button>();
        Player3 = GameObject.Find("Player3")?.GetComponent<Button>();
        popUpPanel.SetActive(false);
        //lambda expression used for simplifying the colour selection functions into one.
        Player0.onClick.AddListener(() => CallOutPlayer(0));
        Player1.onClick.AddListener(() => CallOutPlayer(1));
        Player2.onClick.AddListener(() => CallOutPlayer(2));
        Player3.onClick.AddListener(() => CallOutPlayer(3));
    }
    public static void ShowPopUpPanel() => popUpPanel.SetActive(true);
    public static void HidePopUpPanel() => popUpPanel.SetActive(false);

    private void CallOutPlayer(int targetIndex)
    {
        if (deckManager.playerHands[targetIndex].Count == 1 && !playerBehaviour.UNOStatus[targetIndex] && targetIndex!= gameManager.GetPlayerIndex())
        {
            deckManager.GetComponent<PhotonView>().RPC("AddCardToTargetHand", RpcTarget.AllBuffered, 2, targetIndex);
            /*
            Debug.Log($"Successfully called out!{targetIndex}");
            for (int i = 0; i < 2; i++)
            {
                deckManager.shuffledDeck[0].GetComponent<PhotonView>().TransferOwnership(targetIndex+1);
                Card currentCard = deckManager.shuffledDeck[0];
                currentCard.GetComponent<PhotonView>().RPC("SynchroniseCardAdding", RpcTarget.AllBuffered, targetIndex);
                Debug.Log($"Added card into player {targetIndex}'s hand");
                currentCard.DisplayCard(targetIndex, deckManager.playerHands[targetIndex].Count - 1);
                if (targetIndex + 1 == PhotonNetwork.LocalPlayer.ActorNumber)
                { currentCard.FlipToFront(); }
                deckManager.GetComponent<PhotonView>().RPC("SynchroniseOthersDeckAfterDrawCard", RpcTarget.AllBuffered);
                gameManager.GetComponent<PhotonView>().RPC("TargetHandUpdate", RpcTarget.AllBuffered, targetIndex);
            }
            */

        }
        else
        {
            Debug.Log($"Selected player cannot be called out. Target's UNO Status :{playerBehaviour.UNOStatus[targetIndex]}, " +
            $"target's card count {deckManager.playerHands[targetIndex].Count}");
        }
        HidePopUpPanel();
    }
}

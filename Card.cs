using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;


public enum cardType { number, drawTwo, skip, reverse, wild, wildDrawFour }
public enum cardColour { red, blue, green, yellow, none }

public class Card : MonoBehaviourPunCallbacks, IPointerClickHandler
{
    private GameManager gameManager;
    private DeckManager deckManager;
    private MeshRenderer meshRenderer;
    
    public int digit;
    public cardType type;
    public cardColour colour;
    [SerializeField]
    private Material frontMaterial;
    [SerializeField]
    private Material backMaterial;
    public bool isFaceUp = true;
    public bool interactable = true;
    
    public void Initialize(cardType type, cardColour colour, int digit,Material frontMaterial, Material backMaterial, bool interactable)
    {
        gameManager = GameObject.Find("EGO").GetComponent<GameManager>();
        deckManager = GameObject.Find("EGO").GetComponent<DeckManager>();
        this.type = type;
        this.colour = colour;
        this.digit = digit;
        this.frontMaterial = frontMaterial;
        this.backMaterial = backMaterial;
        this.isFaceUp = false;
        this.interactable = false;
        meshRenderer = GetComponent<MeshRenderer>();
        this.GetComponent<PhotonView>().RPC("UpdateCard",RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void UpdateCard()
    {
        if (meshRenderer == null) { meshRenderer = GetComponentInChildren<MeshRenderer>(); }
        meshRenderer.material = isFaceUp ? frontMaterial : backMaterial;
    }

    
    public void FlipToFront()
    {
        isFaceUp = true;
        if (meshRenderer == null) { meshRenderer = GetComponentInChildren<MeshRenderer>(); }
        meshRenderer.material = isFaceUp ? frontMaterial : backMaterial;
    }

    public void FlipToBack()
    {
        isFaceUp = false;
        if (meshRenderer == null) { meshRenderer = GetComponentInChildren<MeshRenderer>(); }
        meshRenderer.material = isFaceUp ? frontMaterial : backMaterial;
    }

    public void OnPointerClick(PointerEventData pointerEvent)
    {
        if (this.GetComponent<PhotonView>().IsMine && interactable && CanBePlayedOn())
        {
            int clickCount = pointerEvent.clickCount;
            if (clickCount == 1)
            { SelectCard(); }
            else if (clickCount >= 2)
            {
                PlayCard();
                foreach (Card card in deckManager.playerHands[PhotonNetwork.LocalPlayer.ActorNumber - 1]) 
                {
                    card.interactable = false;
                }

                if (deckManager.playerHands[gameManager.GetPlayerIndex()].Count == 0 | deckManager.shuffledDeck.Count == 0)
                {
                    
                    //if (PhotonNetwork.IsMasterClient && gameManager.winConAcieved)
                    if (PhotonNetwork.IsMasterClient)
                    {
                        gameManager.WiningConAchieved();
                        Debug.Log($"Before loading FinishScreen: {PhotonNetwork.CurrentRoom.CustomProperties["playerPlacement"]}");
                        GetComponent<PhotonView>().RPC("LoadFinish", RpcTarget.AllBuffered);
                    }
                    else
                    {
                        gameManager.GetComponent<PhotonView>().RPC("WiningConAchieved", RpcTarget.MasterClient);
                        Debug.Log($"Before loading FinishScreen: {PhotonNetwork.CurrentRoom.CustomProperties["playerPlacement"]}");
                        GetComponent<PhotonView>().RPC("LoadFinish", RpcTarget.AllBuffered);
                    }
                    //gameManager.WiningConAchieved();
                }
                else
                {
                    GamePlayProceed();
                }
            }
        }
    }

    [PunRPC]
    public void LoadFinish() => PhotonNetwork.LoadLevel("FinishScreen");

    public void SelectCard()
    {
        Debug.Log($"Card: {colour},{type},{digit}");
        //display effects of card being selected
    }

    public void PlayCard()
    {
        interactable = false;
        
        GetComponent<PhotonView>().RPC("SynchronisePlayerHand", RpcTarget.AllBuffered);//remove from playerhand
    
        GetComponent<PhotonView>().RPC("SynchronisePlayedCardInfo", RpcTarget.AllBuffered); //update played card info in GM

        GetComponent<PhotonView>().RPC("AddToDiscardPile", RpcTarget.AllBuffered); //add to discard pile

        GetComponent<PhotonView>().RPC("UpdateCard", RpcTarget.AllBuffered);

        deckManager.GetComponent<PhotonView>().RPC("PlayerHandUpdate", RpcTarget.AllBuffered);//update playerhand display
        
    }

    [PunRPC]
    public void SynchronisePlayerHand()
    {
        int playerIndex = gameManager.GetPlayerIndex();
        deckManager.playerHands[playerIndex].Remove(this);
    }

    [PunRPC]
    public void SynchroniseCardAdding(int playerIndex)
    {
        deckManager.playerHands[playerIndex].Add(this);
    }

    [PunRPC]
    public void SynchronisePlayedCardInfo()
    {
        gameManager.currentCardColour = this.colour;
        gameManager.currentCardDigit = this.digit;
        Debug.Log($"CurrentCard{colour} {digit} {type}");
    }

    public void PlayableCardVisualUpdate()
    {
        meshRenderer.material.color = Color.white;
    }

    public void UnplayableCardVisualUpdate()
    {
        Color darkerColor = Color.white * 0.5f; 
        meshRenderer.material.color = darkerColor;
    }

    public bool CanBePlayedOn()
    {
        cardColour prevColour = gameManager.currentCardColour;
        int prevDigit = gameManager.currentCardDigit;
        if ((prevDigit == 10 | prevDigit == 14)&& gameManager.CurrentPlayerStackable)
        {
            if (digit == 10 | digit == 14)
            { return true; }
            else
            { return false; }
        }

        if (colour == prevColour | digit == prevDigit | colour == cardColour.none| prevDigit == -1)
        { return true; }
        else
        { return false; }

    }



    private void GamePlayProceed()
    {

        gameManager.PerformAction();
        gameManager.DisplayUnplayability();////

        Debug.Log($"Wait, {gameManager.currentCardColour}");

        StartCoroutine(WaitForColourSelection());

        /* // moved to Wait for colour selection
        gameManager.GetComponent<PhotonView>().RPC("AdvanceTurn", RpcTarget.AllBuffered);

        gameManager.GetComponent<PhotonView>().RPC("StartTurn", RpcTarget.AllBuffered);
        */

    }
    private IEnumerator WaitForColourSelection()
    {
        yield return null;
        yield return new WaitUntil(() => gameManager.currentCardColour != cardColour.none);

        Debug.Log($"Wait fin, Color selected: {gameManager.currentCardColour}");

        //////new changes
        gameManager.GetComponent<PhotonView>().RPC("AdvanceTurn", RpcTarget.AllBuffered);
        gameManager.GetComponent<PhotonView>().RPC("StartTurn", RpcTarget.AllBuffered);
        /////////
    }

    [PunRPC]
    public void AddToDiscardPile()

    {
        isFaceUp = true;
        if (gameManager.DiscardPile.Count >= 1)
        {
            gameManager.DiscardPile[gameManager.GetDicardPileCount() - 1].gameObject.SetActive(false);
        }
        gameManager.DiscardPile.Add(this);
    }

    public void DisplayCard(int currentActivePlayerID, int cardIndex)
    {
        Vector3[] PlayerTransform = InitialPlayerCardPos(currentActivePlayerID);
        this.gameObject.transform.SetPositionAndRotation(PlayerTransform[0] + cardIndex * PlayerTransform[1], PlayerBasedCardOrientation(currentActivePlayerID));
        this.gameObject.SetActive(true);
    }

    [PunRPC]
    public void DiscardCardUpdate()
    {
        Vector3 cardTransform = new(1, 0.5f, 90);
        this.gameObject.transform.SetPositionAndRotation(cardTransform, Quaternion.Euler(0, 0, 180));
        this.gameObject.SetActive(true);
    }

    
    public Vector3[] InitialPlayerCardPos(int playerIndex) // player index is the target index in the 0-3 form
    {
        Vector3[] playerSpecificTransform = new Vector3[2];
        int localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        if (playerIndex < localPlayerIndex)
        { playerIndex += 4; }
        int relativeDifference = playerIndex - localPlayerIndex;
        switch (relativeDifference)
        {
            //rougly z = 90 for the bgimage
            case 0://本家
                playerSpecificTransform[0] = new Vector3(-3f, -3f, 0);
                playerSpecificTransform[1] = new Vector3(0.4f, 0, -0.1f);
                return playerSpecificTransform;
            case 1://左置位
                playerSpecificTransform[0] = new Vector3(-5f, 3f, 0);
                playerSpecificTransform[1] = new Vector3(0, -0.4f, -0.1f);
                return playerSpecificTransform;
            case 2://对家
                playerSpecificTransform[0] = new Vector3(5f, 3.75f, 0);
                playerSpecificTransform[1] = new Vector3(-0.4f, 0, -0.1f);
                return playerSpecificTransform;
            case 3://右置位
                playerSpecificTransform[0] = new Vector3(7f, -2f, 0);
                playerSpecificTransform[1] = new Vector3(0, 0.4f, -0.1f);
                return playerSpecificTransform;
            default://错误
                Debug.Log("error none existing player index.");
                return null;
        }
    }
    
    public Quaternion PlayerBasedCardOrientation(int playerIndex)
    {
        int localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        if (playerIndex < localPlayerIndex)
        { playerIndex += 4; }
        int relativeDifference = playerIndex - localPlayerIndex;
        switch (relativeDifference)
        {
            case 0:
                return Quaternion.Euler(0, 0, 180);
            case 1:
                return Quaternion.Euler(0, 0, 90);
            case 2:
                return Quaternion.Euler(0, 0, 0);
            case 3:
                return Quaternion.Euler(0, 0, 270);
            default:
                Debug.Log("Error Encountered: none existing player index.");
                return Quaternion.Euler(0, 0, 0);
        }
    }
}


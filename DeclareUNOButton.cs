using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DeclareUNOButton : MonoBehaviour
{
    [SerializeField]
    private static Button declareUNOButton;
    private PlayerBehaviour playerBehaviour;

    void Start()
    {
        playerBehaviour = GameObject.Find("EGO").GetComponent<PlayerBehaviour>();
        declareUNOButton = GameObject.Find("UNOButton")?.GetComponent<Button>();
        declareUNOButton.interactable = false;
        declareUNOButton.onClick.RemoveAllListeners();
        declareUNOButton.onClick.AddListener(D_UNOButtonClicked);
    }
    //logic for button being deactivated until the player's turn is reached AND does not have playable card is required.
    public static void EnableShoutOut() => declareUNOButton.interactable = true;
    public static void DisableShoutOut() => declareUNOButton.interactable = false;
    public void D_UNOButtonClicked()
    {
        playerBehaviour.GetComponent<PhotonView>().RPC("DeclareUNO", RpcTarget.AllBuffered);
        Debug.Log("UNO");
    }

}

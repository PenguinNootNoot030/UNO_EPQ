using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    private Player player;
    public GameObject playerPrefab;
    private DeckManager deckManager;
    public InputField createInput;
    public InputField joinInput;
    public GameObject playerObject;

    
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(createInput.text,roomOptions);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        PhotonNetwork.LoadLevel("InGame");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        deckManager = GameObject.Find("EGO").GetComponent<DeckManager>();
        playerObject = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
        player = playerObject.GetComponent<Player>();
        player.PlayerInitialize(); //RPC called inside
        deckManager.GetComponent<PhotonView>().RPC("SynchronisePlayerPrefab", RpcTarget.AllBuffered); //move player prefab
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

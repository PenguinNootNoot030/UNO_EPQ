using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class WinningScenePrefabChange : MonoBehaviourPunCallbacks
{
    Dictionary<string, Material> playerMaterial = new Dictionary<string, Material>();
    int[] playerPlacement = new int[4];
    public GameObject firstPlacePlayer;
    public GameObject secondPlacePlayer;
    public GameObject thirdPlacePlayer;
    public GameObject fourthPlacePlayer;

    /*
    void Start()
    {
        
         * LoadMaterials();
        gameObjects[0] = firstPlacePlayer;
        gameObjects[1] = secondPlacePlayer;
        gameObjects[2] = thirdPlacePlayer;
        gameObjects[3] = fourthPlacePlayer;
        GetRoomProperty();

        if (playerPlacement == null || playerPlacement.Length != 4)
        {
            Debug.LogError("Player placement data is missing or incomplete.");
            return;
        }

        GameObject[] gameObjects = { firstPlacePlayer, secondPlacePlayer, thirdPlacePlayer, fourthPlacePlayer };
        for (int i = 0; i < playerPlacement.Length; i++) 
        {
            MeshRenderer meshRenderer = gameObjects[i].GetComponent<MeshRenderer>();
            string pID = "P" + i.ToString();
            meshRenderer.material = playerMaterial[pID]; 
        }
        
}
    */

    /*
    private void GetRoomProperty()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("playerPlacement", out object value))
        {
            playerPlacement = value as int[];
            if (playerPlacement != null)
            {
                Debug.Log("Player placement data successfully retrieved.");
            }
            else
            {
                Debug.LogError("Player placement data could not be retrieved or is in an incorrect format.");
            }
        }
        else
        {
            Debug.Log("Player placement data not found in room properties.");
        }
    }
    */


    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        LoadMaterials();
        GameObject[] gameObjects = { firstPlacePlayer, secondPlacePlayer, thirdPlacePlayer, fourthPlacePlayer };
        // Check if a specific property was changed
        if (propertiesThatChanged.TryGetValue("playerPlacement", out object value))
        {
            playerPlacement = value as int[];
            Debug.Log("Player placement updated: " + string.Join(", ", playerPlacement));
        }
        foreach (int i in playerPlacement)
        { Debug.Log(i); }

        for (int i = 0; i < 4; i++)
        {
            MeshRenderer meshRenderer = gameObjects[i].GetComponent<MeshRenderer>();
            string pID = "P" + (playerPlacement[i].ToString());
            meshRenderer.material = playerMaterial[pID];
            Debug.Log($"{pID} filled");
        }
    }

    private void LoadMaterials()
    {
        Material[] loadedMaterial = Resources.LoadAll<Material>("PlayerMaterial");
        foreach (Material material in loadedMaterial)
        {
            playerMaterial[material.name] = material;
        }
        Debug.Log($"Loaded{playerMaterial.Count} materials.");
    }

    
}

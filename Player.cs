using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour
{
    public int playerID = 0;
    private Dictionary<string, Material> playerMaterial = new Dictionary<string, Material>();

    private void LoadMaterials()
    {
        Material[] loadedMaterial = Resources.LoadAll<Material>("PlayerMaterial");
        foreach (Material material in loadedMaterial)
        {
            playerMaterial[material.name] = material;
        }
        Debug.Log($"Loaded{playerMaterial.Count} materials.");
    }

    public int GetPlayerID() => playerID;

    
    public void PlayerInitialize()
    { GetComponent<PhotonView>().RPC("ChangePlayerPrefabMaterial", RpcTarget.AllBuffered); }

    [PunRPC]
    public void ChangePlayerPrefabMaterial()
    {
        LoadMaterials();
        playerID = GetComponent<PhotonView>().OwnerActorNr;
        int pid = playerID - 1;
        string PName = "P" + pid.ToString();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = playerMaterial[PName];
    }
}

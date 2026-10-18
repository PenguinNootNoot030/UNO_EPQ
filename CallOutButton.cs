using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CallOutButton : MonoBehaviour
{
    private static Button COButton;

    void Start()
    {
        COButton = GameObject.Find("CallOutButton")?.GetComponent<Button>();
        COButton.interactable = false;
        COButton.onClick.RemoveAllListeners();
        COButton = GetComponent<Button>();
        COButton.onClick.AddListener(COButtonClicked);
    }

    [PunRPC]
    public void EnableCallOut() => COButton.interactable = true;

    void COButtonClicked()
    {
        CallOutPopUpPanel.ShowPopUpPanel();
    }

}

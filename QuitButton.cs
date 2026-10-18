using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    public Button QButton;
    void Start()
    {
        QButton = GameObject.Find("QuitButton")?.GetComponent<Button>();
        QButton.onClick.RemoveAllListeners();
        QButton.onClick.AddListener(QuitButtonClicked);
    }

    void QuitButtonClicked()
    {
        Debug.Log("Shutting down...");
        Application.Quit();

    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif 

    }

}

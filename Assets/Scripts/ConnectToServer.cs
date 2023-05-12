using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using System;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    /*void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }*/
    public TMP_InputField usernameInput;
    public GameObject welcomePanel;
    public GameObject namePanel;

    public GameObject connectButton;
    public Sprite connecting;

    public AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.anyKey)
        {
            welcomePanel.SetActive(false);
            namePanel.SetActive(true);
        }
    }

    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 1)
        {
            audioSource.Play();
            connectButton.GetComponent<Image>().sprite = connecting;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = usernameInput.text;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}

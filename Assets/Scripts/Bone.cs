using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Bone : MonoBehaviour
{
    public int bone;
    public int numOfBones;

    public Image[] bones;
    public Sprite fullBone;
    public Sprite emptyBone;

    public GameObject endgame;
    private int dogTags;

    PhotonView view;

    void Start() {
        view = GetComponent<PhotonView>();
        dogTags = GameObject.FindGameObjectsWithTag("Dog").Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (bone > numOfBones) {
            bone = numOfBones;
        }
        
        dogTags = GameObject.FindGameObjectsWithTag("Dog").Length;
        if (dogTags == 0)
        {
            endgame.SetActive(true);
        }

        for (int i = 0; i < bones.Length; i++) {
            if (i < bone) {
                bones[i].sprite = fullBone;
            } else {
                bones[i].sprite = emptyBone;
            }

            if (i < numOfBones) {
                bones[i].enabled = true;
            } else {
                bones[i].enabled = false;
            }
        }
    }

    [PunRPC]
    public void addBone() {
        bone++;
        if (bone >= 1)
        {
            endgame.SetActive(true);
        }
    }

    public void QuitGame()
    {
        PhotonNetwork.Disconnect();
        Application.Quit();
    }

    //[PunRPC]
    public void ReturnLobby()
    {
        // Đợi cho client kết nối lại với Master Server
        //if (PhotonNetwork.NetworkClientState)
        {
            //PhotonNetwork.ConnectUsingSettings();
            // Xoá các đối tượng trong scene game
            //ResetGameScene();
            //PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("Lobby");
            return;
        }
    }

    public int getBone() {
        return bone;
    }
}

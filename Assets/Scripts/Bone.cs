﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Bone : MonoBehaviour
{
    public int bone;
    public int numOfBones;
    public AudioSource audioSource;
    public AudioClip hunterWinClip;
    public AudioClip dogWinClip;
    public AudioClip bonePickClip;
    public AudioClip killDog;
    public Image[] bones;
    public Sprite fullBone;
    public Sprite emptyBone;

    public GameObject endgame;
    public GameObject EndTitle;
    public GameObject EndBrg;
    private int dogTags;
    PhotonView view;

    public Sprite dogWin;
    public Sprite hunterWin;
    public Sprite dogBrg;
    public Sprite hunterBrg;

    void Start() {
        view = GetComponent<PhotonView>();
        dogTags = GameObject.FindGameObjectsWithTag("Dog").Length;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = hunterWinClip;
    }

    // Update is called once per frame
    void Update()
    {
        if (bone > numOfBones) {
            bone = numOfBones;
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
        audioSource.clip = bonePickClip;
        audioSource.Play();
        if (bone >= 1)
        {
            EndBrg.GetComponent<Image>().sprite = dogBrg;
            EndTitle.GetComponent<Image>().sprite = dogWin;
            audioSource.clip = dogWinClip;
            audioSource.Play();
            endgame.SetActive(true);
        }
    }

    [PunRPC]
    public void oneDown() {
        dogTags = GameObject.FindGameObjectsWithTag("Dog").Length;
        audioSource.clip = killDog;
        audioSource.Play();
        if (dogTags == 0)
        {
            EndBrg.GetComponent<Image>().sprite = hunterBrg;
            EndTitle.GetComponent<Image>().sprite = hunterWin;
            audioSource.clip = hunterWinClip;
            audioSource.Play();
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

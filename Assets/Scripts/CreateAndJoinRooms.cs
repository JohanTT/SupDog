using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using Photon.Realtime;
using TMPro;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{


    public InputField createInput;
    public InputField joinInput;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public TMP_Text roomName;


    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    //public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    //public GameObject playerItemPrefab;
    //public GameObject playerItemParent;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text, new RoomOptions() { BroadcastPropsChangeToAll = true });
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
        //lobbyPanel.SetActive(false);
        //roomPanel.SetActive(true);
        //roomName.text = "Room Name :" + PhotonNetwork.CurrentRoom.Name;
        //UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    /*
    void UpdatePlayerList()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            GameObject newPlayerItem = PhotonNetwork.Instantiate(playerItemPrefab.name, Vector3.zero, Quaternion.identity, 0);
            newPlayerItem.transform.SetParent(playerItemParent.transform, false);
            // L?y script t? ??i t??ng newPlayerItem
            PlayerItem playerItemScript = newPlayerItem.GetComponent<PlayerItem>();

            // G?i hàm SetPlayerInfo t? script
            playerItemScript.SetPlayerInfo(player.Value);
            /*
            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }
            playerItemsList.Add(newPlayerItem);
             */
        //}
    //}
    /*
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }
    */
}
    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using  Photon.Pun;
using System;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    public void CreateRoom() {
        print("Create ROOM" + createInput.text);
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom() {
        print("ROOM" + joinInput.text);
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnCreateRoomFailed(short errorCode, string errorMessage)
    {
        Debug.Log("Failed to create room. Error code: " + errorCode + ", message: " + errorMessage);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully!");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");    
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print(message);
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            Debug.Log("Room Name: " + room.Name);
        }
        UpdateRoomList(roomList);
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach(RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach(RoomInfo room in list)
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

    public void RefreshRoom(List<RoomInfo> roomList) {
        OnRoomListUpdate(roomList);
    }
}

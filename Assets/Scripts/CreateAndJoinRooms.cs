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
    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    public GameObject playButton;

    public GameObject selectRoleButton;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    public List<GameObject> playerItemsList = new List<GameObject>();
    public GameObject playerItemPrefab;
    public Transform playerItemParent;

    public List<GameObject> unPlayerItemsList = new List<GameObject>();
    public Transform unPlayerItemParent;

    private bool IsHunter = false;
    private string NickName = "";
    private bool IsTurnHunter = false;
    private bool IsTurnDog = false;

    

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
        //PhotonNetwork.LoadLevel("Game");
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        // Spawn đối tượng chờ
        //GameObject newPlayerItem = PhotonNetwork.Instantiate(playerItemPrefab.name, Vector3.zero, Quaternion.identity, 0);
        //newPlayerItem.transform.SetParent(playerItemParent.transform, false);
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
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

    public void switchRoleHunterBtn()
    {
        if (IsHunter == false)
        {
            NickName = PhotonNetwork.LocalPlayer.NickName;
            IsHunter = true;
            UpdatePlayerList();
        }
    }

    public void switchRoleDogBtn()
    {
        if (IsHunter == true)
        {
            NickName = PhotonNetwork.LocalPlayer.NickName;
            IsHunter = false;
            UpdatePlayerList();
        }
    }

    void UpdatePlayerList()
    {
        foreach (GameObject item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        foreach (GameObject item in unPlayerItemsList)
        {
            Destroy(item.gameObject);
        }
        unPlayerItemsList.Clear();

        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("PlayerItem");
        if (objectsWithTag != null)
        {
            foreach (GameObject gob in objectsWithTag)
            {
                PhotonNetwork.Destroy(gob);
            }
        }

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            GameObject newPlayerItem = PhotonNetwork.Instantiate(playerItemPrefab.name, Vector3.zero, Quaternion.identity);
            PlayerItem playerItemScript = newPlayerItem.GetComponent<PlayerItem>();
            playerItemScript.SetPlayerInfo(player.Value);
            if (player.Value.ActorNumber == 1)
            {
                playerItemScript.setPlayerRole(true);
                unPlayerItemsList.Add(newPlayerItem);
                print("I'm HUNTER");
                newPlayerItem.transform.SetParent(unPlayerItemParent.transform, false);
            }
            else
            {
                playerItemScript.setPlayerRole(false);
                playerItemsList.Add(newPlayerItem);
                print("I'm DOG");
                newPlayerItem.transform.SetParent(playerItemParent.transform, false);
            }
            /*
            GameObject newPlayerItem = PhotonNetwork.Instantiate(playerItemPrefab.name, Vector3.zero, Quaternion.identity);
            PlayerItem playerItemScript = newPlayerItem.GetComponent<PlayerItem>();
            playerItemScript.SetPlayerInfo(player.Value);

            if (player.Value.NickName == NickName && IsHunter == true)
            {
                playerItemScript.setPlayerRole(true);
            }
            else if (player.Value.NickName == NickName && IsHunter == false)
            {
                playerItemScript.setPlayerRole(false);
                NickName = "";
            }

            if (playerItemScript.role == false)
            {
                playerItemsList.Add(newPlayerItem);
                print("I'm DOG");
                newPlayerItem.transform.SetParent(playerItemParent.transform, false);
                unPlayerItemsList.Remove(newPlayerItem);
            }
            else
            {
                unPlayerItemsList.Add(newPlayerItem);
                print("I'm HUNTER");
                newPlayerItem.transform.SetParent(unPlayerItemParent.transform, false);
                playerItemsList.Remove(newPlayerItem);
            }
            // Đồng bộ hoá việc thay đổi parent của newPlayerItem
            PhotonView photonView = newPlayerItem.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                IsTurnHunter = false;
                IsTurnDog = false;
                photonView.RPC("SetParentRPC", RpcTarget.AllBuffered, newPlayerItem.transform.parent.name, photonView.ViewID, player.Value);
            }
            */
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        roomName.text = "";
        //OnJoinedLobby();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    // Phương thức RPC được gọi bởi owner của đối tượng, nó sẽ được gọi trên tất cả các client để đồng bộ hoá việc thay đổi parent
    [PunRPC]
    void SetParentRPC(int ViewID, string parent, Player player)
    {
        GameObject playerItemObject = PhotonView.Find(ViewID).gameObject;
        if (playerItemObject != null)
        {
            Transform parentTrans = GameObject.Find(parent)?.transform;
            PlayerItem playerItemScript = playerItemObject.GetComponent<PlayerItem>();
            player.NickName = player.NickName + "clone";
            playerItemScript.SetPlayerInfo(player);
            if (parentTrans != null)
            {
                playerItemObject.transform.SetParent(parentTrans.transform, false);
            }
        }
    }
    
    private void Update()
    {
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 3)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
    }
    
    public void OnClickPlayButton()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    // public GameObject dogPrefab;
    public DragScript playerDragScript;
    // Đặt vị trí spawn
    public float minX = -1.961f;
    public float maxX = -1.007f;
    public float minY = -0.608f;
    public float maxY = -0.328f;

    GameObject player;

    private void Start() 
    {
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            print(player.Value.NickName);
        }
        // Vector3 dogPosition1 = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        // Vector3 dogPosition2 = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        // Vector3 dogPosition3 = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f));

        Vector3 position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
        
        player = PhotonNetwork.Instantiate(playerPrefab.name, position, Quaternion.identity);   
        // dog1 = PhotonNetwork.Instantiate(dogPrefab.name, dogPosition1, Quaternion.identity);
        // dog2 = PhotonNetwork.Instantiate(dogPrefab.name, dogPosition2, Quaternion.identity);
        // dog3 = PhotonNetwork.Instantiate(dogPrefab.name, dogPosition3, Quaternion.identity);
        
        // print("Tới đây rồi");
        playerDragScript = player.GetComponentInChildren<DragScript>();
        // print("Qua rồi");

        // print(playerDragScript);
        // // if (playerDragScript != null) 
        // {
        //     dog1.GetComponent<DogScript>().dragScript = playerDragScript;
        //     dog2.GetComponent<DogScript>().dragScript = playerDragScript;
        //     dog3.GetComponent<DogScript>().dragScript = playerDragScript;
        // }
    }

    public DragScript getDragScript() {
        return playerDragScript;
    }
}

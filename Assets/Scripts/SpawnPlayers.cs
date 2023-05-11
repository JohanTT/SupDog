using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject dogPrefab_1;
    public GameObject dogPrefab_2;
    public GameObject dogPrefab_3;
    public GameObject dogPrefab_4;
    public DragScript playerDragScript;
    // Đặt vị trí spawn
    public float minX = -1.961f;
    public float maxX = -1.007f;
    public float minY = -0.608f;
    public float maxY = -0.328f;

    GameObject player;

    private void Start() 
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1) {
            // Vector3 dogPosition1 = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            // Vector3 dogPosition2 = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            // Vector3 dogPosition3 = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f));

            Vector3 position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
            
            player = PhotonNetwork.Instantiate(playerPrefab.name, position, Quaternion.identity);   
            
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
        else {
            Vector3 position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
            switch (PhotonNetwork.LocalPlayer.ActorNumber) {
                case 2: 
                    player = PhotonNetwork.Instantiate(dogPrefab_1.name, position, Quaternion.identity);   
                    break;
                case 3: 
                    player = PhotonNetwork.Instantiate(dogPrefab_2.name, position, Quaternion.identity);   
                    break;
                case 4: 
                    player = PhotonNetwork.Instantiate(dogPrefab_3.name, position, Quaternion.identity);   
                    break;
                case 5: 
                    player = PhotonNetwork.Instantiate(dogPrefab_4.name, position, Quaternion.identity);   
                    break;
                default:
                    player = PhotonNetwork.Instantiate(dogPrefab_1.name, position, Quaternion.identity);   
                    break;
            } 
            playerDragScript = player.GetComponentInChildren<DragScript>();
        }
    }

    public DragScript getDragScript() {
        return playerDragScript;
    }
}

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
    public Vector3 hunterSpawn;
    public Vector3 dog_1Spawn;
    public Vector3 dog_2Spawn;
    public Vector3 dog_3Spawn;
    public Vector3 dog_4Spawn;

    GameObject player;

    private void Start() 
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1) {            
            player = PhotonNetwork.Instantiate(playerPrefab.name, hunterSpawn, Quaternion.identity);   
            
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
            switch (PhotonNetwork.LocalPlayer.ActorNumber) {
                case 2: 
                    player = PhotonNetwork.Instantiate(dogPrefab_1.name, dog_1Spawn, Quaternion.identity);   
                    break;
                case 3: 
                    player = PhotonNetwork.Instantiate(dogPrefab_2.name, dog_2Spawn, Quaternion.identity);   
                    break;
                case 4: 
                    player = PhotonNetwork.Instantiate(dogPrefab_3.name, dog_3Spawn, Quaternion.identity);   
                    break;
                case 5: 
                    player = PhotonNetwork.Instantiate(dogPrefab_4.name, dog_4Spawn, Quaternion.identity);   
                    break;
                default:
                    player = PhotonNetwork.Instantiate(dogPrefab_1.name, dog_1Spawn, Quaternion.identity);   
                    break;
            } 
            playerDragScript = player.GetComponentInChildren<DragScript>();
        }
    }

    public DragScript getDragScript() {
        return playerDragScript;
    }
}

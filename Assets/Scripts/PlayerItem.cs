using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.Networking;
using ExitGames.Client.Photon;


public class PlayerItem : MonoBehaviourPunCallbacks
{
    public TMP_Text PlayerName;
    public bool role;
    public Sprite myDog;
    public Sprite theirDog;
    public Sprite myHunter;
    public Sprite theirHunter;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    Player player;

    PhotonView view;

    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    public void SetPlayerInfo(Player _player)
    {
        //if (view.IsMine)
        {
            string Name = _player.NickName;
            PlayerName.text = Name;
            player = _player;
            role = false;
            UpdatePlayerItem(player);
        }
    }

    public void Update()
    {
        
    }

    public void setSpriteRole()
    {
        if (role == true)
        {
            gameObject.GetComponent<Image>().sprite = theirHunter;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = theirDog;
        }
    }

    public void setMySpriteRole()
    {
        if (role == true)
        {
            gameObject.GetComponent<Image>().sprite = myHunter;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = myDog;
        }
        
    }

    public void setPlayerRole(bool R)
    {
        role = R;
    }

    public bool getPlayerRole()
    {
        return role;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (view.IsMine)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }

    void UpdatePlayerItem(Player player)
    {

        if (player.CustomProperties.ContainsKey("Player"))
        {
            //if (view.IsMine)
            {
                //playerAvatar.sprite = avatars[(int)player.CustomProperties["Player"]];
                playerProperties["Player"] = (int)player.CustomProperties["Player"];
            }
        }
        else
        {
            playerProperties["Player"] = 0;
        }
    }

    // Phương thức RPC được gọi bởi owner của đối tượng, nó sẽ được gọi trên tất cả các client để đồng bộ hoá việc thay đổi parent
    [PunRPC]
    void SetParentRPC(string parent, int ViewID, Player player)
    {
        GameObject playerItemObject = PhotonView.Find(ViewID).gameObject;
        if (playerItemObject != null)
        {
            Transform parentTrans = GameObject.Find(parent)?.transform;
            PlayerItem playerItemScript = playerItemObject.GetComponent<PlayerItem>();
            playerItemScript.SetPlayerInfo(player);
            if (parentTrans != null)
            {
                playerItemObject.transform.SetParent(parentTrans.transform, false);
            }
        }
    }
}

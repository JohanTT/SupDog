using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DogScript : MonoBehaviour
{
    public DragScript dragScript;
    public SpawnPlayers spawnPlayers;
    public float health;
    public float maxHealth;
    bool holding = false;
    Rigidbody2D rgDog;
    PhotonView view;
    SpriteRenderer spriteRenderer;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    // Lưu trữ vị trí ban đầu của đối tượng
    private Vector3 initialPosition;
    public float Health {
        set {
            health = value;
                print("Take this!");
            if (health <= 0) {
                Defeated();
            }
        }
        get {
            return health;
        }
    }
    
    private void Start() {
        dragScript = spawnPlayers.getDragScript();
        view = GetComponent<PhotonView>();
        rgDog = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;
    }

    private void Update() {
        if (spawnPlayers.getDragScript() != null) {
        dragScript = spawnPlayers.getDragScript();
        // print (spawnPlayers.getDragScript());
        }
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    private void FixedUpdate() {
        if (spawnPlayers.getDragScript() != null) {
            if (dragScript.getTriggerDrag() && dragScript.getCurrentDog() == view.ViewID) {
                if (view.IsMine == false)
                {
                    view.TransferOwnership(PhotonNetwork.LocalPlayer);        
                }
                BeingDrag();
                // Chỉnh lại layer để khi đi lên nó không bị đè
                if (dragScript.getX() == 0 && dragScript.getY() == 1) {
                    spriteRenderer.sortingOrder = 0;
                } else {
                    spriteRenderer.sortingOrder = 1;
                }
            }
            if (!dragScript.getTriggerDrag() && dragScript.getCurrentDog() == view.ViewID && holding == true) 
            {
                if (view.IsMine == false)
                {
                    view.TransferOwnership(PhotonNetwork.LocalPlayer);        
                }
                ThrowDrag();
            }
        }
    }

    public void TakeDamage(float dame) {
        if (view.IsMine == false)
        {
            view.TransferOwnership(PhotonNetwork.LocalPlayer);
            // Debug.LogError("PhotonView with ID=" + view.ViewID + " does not exist.");
        }
        Health -= dame;
    }


    public void BeingDrag() {
        transform.position = Vector3.Lerp(transform.position, dragScript.getDragOffset(), 0.5f);
        rgDog.simulated = false;
        holding = true;
        dragScript.StopDrag();
    }

    public void ThrowDrag() {
        this.transform.position = Vector3.Lerp(transform.position, dragScript.getDragOffset(), 0.5f);
        rgDog.simulated = true;
        holding = false;
    }

    public void Defeated() {
        PhotonNetwork.Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Collider2D swordCollider;
    Vector2 attackOffset;
    private int playerDame = 1;
    
    PhotonView view;

    private void Start() {
        attackOffset = transform.position;
    }

    public void AttackRight() {
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(attackOffset.x * -0.7f, -0.1f);
    }
    public void AttackLeft() {
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(attackOffset.x * 0.8f, -0.1f);;
    }
    public void AttackTop() {
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(0, attackOffset.y * 0.1f);
    }
    public void AttackBot() {
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(0, attackOffset.y * 0.3f);
    }
    public void StopAttack() {
        swordCollider.enabled = false;
    }

    public void setView(PhotonView playerView) {
        view = playerView;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Dog") {
            // DogScript dog = other.GetComponent<DogScript>();
            GameObject dog = other.gameObject;
            PhotonView dogView = dog.GetComponent<PhotonView>();

            if (dog != null) {
                // dog.TakeDamage(playerDame);
                dogView.RPC("TakeDamage", RpcTarget.All, playerDame);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Collider2D swordCollider;
    Vector2 attackOffset;
    private int playerDame = 1;
    
    private void Start() {
        attackOffset = transform.position;
    }

    public void AttackRight() {
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(attackOffset.x * -0.1f, -0.1f);
    }
    public void AttackLeft() {
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(attackOffset.x * 0.1f, -0.1f);;
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

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Dog") {
            DogScript dog = other.GetComponent<DogScript>();

            if (dog != null) {
                dog.TakeDamage(playerDame);
            }
        }
    }
}

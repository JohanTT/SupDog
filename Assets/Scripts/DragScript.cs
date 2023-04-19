using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DragScript : MonoBehaviour
{
    public Collider2D dragCollider;
    Vector2 dragOffset;
    Vector2 playerTransForm;
    private bool IsTrigger = false;
    // Kiểm tra hướng di chuyển
    float x, y;
    private int currentDog = 0;

    private void Start() {
        dragOffset = transform.position;
        x = 0;
        y = -1;
    }

    private void Update() {
        if(IsTrigger == true) {
            checkInput(x, y);
        }    
    }

    public void checkInput(float x, float y) {
        if (x >=1 && y == 0) {
            // phải
            transform.localPosition = new Vector3(dragOffset.x * -0.05f, -0.1f);
        } else if (x <= -1 && y == 0) {
            // trái
            transform.localPosition = new Vector3(dragOffset.x * 0.05f, -0.1f);
        } else if (x == 0 && y == 1) {
            // trên
            transform.localPosition = new Vector3(0, dragOffset.y * 0.1f);
        } else if (x == 0 && y == -1) {
            // dưới
            transform.localPosition = new Vector3(0, dragOffset.y * 0.3f);
        }
    }

    public void Throw(float x, float y) {
        print("THROW");
        if (x >=1 && y == 0) {
            // phải
            transform.localPosition =  new Vector3(dragOffset.x * -0.5f, -0.1f);
        } else if (x <= -1 && y == 0) {
            // trái
            transform.localPosition =  new Vector3(dragOffset.x * 0.5f, -0.1f);
        } else if (x == 0 && y == 1) {
            // trên
            transform.localPosition =  new Vector3(0, dragOffset.y * -0.5f);
        } else if (x == 0 && y == -1) {
            // dưới
            transform.localPosition =  new Vector3(0, dragOffset.y * 1.1f);
        }
    }


    public void DragRight() {
        dragCollider.enabled = true;
        transform.localPosition = new Vector3(dragOffset.x * -0.2f, -0.1f);
    }
    public void DragLeft() {
        dragCollider.enabled = true;
        transform.localPosition = new Vector3(dragOffset.x * 0.2f, -0.1f);
    }
    public void DragTop() {
        dragCollider.enabled = true;
        transform.localPosition = new Vector3(0, dragOffset.y * 0.1f);
    }
    public void DragBot() {
        dragCollider.enabled = true;
        transform.localPosition = new Vector3(0, dragOffset.y * 0.3f);
    }
    public void StopDrag() {
        dragCollider.enabled = false;
    }

    public Vector2 getDragOffset() {
        return transform.position;
    }

    public Vector2 getPlayerTransform() {
        return playerTransForm;
    }

    public void setPlayerTransform(Vector2 tmp) {
        playerTransForm = tmp;
    }

    public bool getTriggerDrag() {
        return IsTrigger;
    }

    public void setTrigger(bool set) {
        IsTrigger = set;
    }

    public int getCurrentDog() {
        return currentDog;
    }

    public void setCurrentDog(int tmp) {
        currentDog = tmp;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Dog") {
            DogScript dog = other.GetComponent<DogScript>();
            int tmp = other.GetComponent<PhotonView>().ViewID;
            if (dog != null) {
                setCurrentDog(tmp);
                setTrigger(true);
            }
        }
    }

    // public void OnTriggerExit2D(Collider2D other) {
    //     if (other.tag == "Dog") {
    //         DogScript dog = other.GetComponent<DogScript>();

    //         if (dog != null) {
    //             setTrigger(false);
    //         }
    //     }
    // }

    public void setX(float x) {
        this.x = x;
    }
    public float getX() {
        return x;
    }
    public void setY(float y) {
        this.y = y;
    }
    public float getY() {
        return y;
    }
}

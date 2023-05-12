using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private Transform destination;

    public bool isOrange;
    public bool isBlue;
    public bool isBlack;
    public bool isWhite;
    public float distance = 0.2f;

    void Start()
    {
        if (isOrange == true) 
        {
            destination = GameObject.FindGameObjectWithTag("Blue Portal").GetComponent<Transform>();
        } else if (isBlue == true)
        {
            destination = GameObject.FindGameObjectWithTag("Orange Portal").GetComponent<Transform>();
        } else if (isBlack == true)
        {
            destination = GameObject.FindGameObjectWithTag("White Portal").GetComponent<Transform>();
        } else if (isWhite == true)
        {
            destination = GameObject.FindGameObjectWithTag("Black Portal").GetComponent<Transform>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Vector2.Distance(transform.position, other.transform.position) > distance)
        {
            other.transform.position = new Vector2 (destination.position.x, destination.position.y);
        }
    }
}

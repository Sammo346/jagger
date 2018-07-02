using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour {

    [SerializeField] private float speed = 2f;
    private Vector3 startingPosition;

    private GameManager gm;

    private bool resetting = false;

    public bool Resetting
    {
        get
        {
            return resetting;
        }

        set
        {
            resetting = value;
        }
    }

    void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();

        startingPosition = transform.position;
	}
	
	void FixedUpdate ()
    {
        if (resetting)
        {
            ResetPosition();
        }
        else
        {
            transform.position = transform.position + Vector3.up * speed * Time.deltaTime;
        }
        
	}

    public void ResetPosition()
    {
        transform.position = Vector3.Lerp(transform.position, startingPosition, 10f * Time.deltaTime);

        if (transform.position == startingPosition)
            resetting = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().DamagePlayer();
        }
    }
}

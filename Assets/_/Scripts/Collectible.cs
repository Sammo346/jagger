using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    GameManager gm;

    private void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            gm.CollectibleCollected = true;

            // TODO: Play collect animation

            Destroy(this.gameObject);
        }
    }
}

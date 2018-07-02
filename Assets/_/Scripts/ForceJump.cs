using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceJump : MonoBehaviour {

    private Collider2D collider;

	// Use this for initialization
	void Start () {
        collider = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().ForceJump(true);
        }
    }
}

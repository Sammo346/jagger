using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {

    [SerializeField] float springPower = 10f;
    private Animator anim;

	void Start () {
        anim = GetComponent<Animator>();
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.up * springPower;
            anim.SetTrigger("Spring");
        }
    }
}

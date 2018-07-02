using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayer : MonoBehaviour {

    Animator anim;
    bool moving = false;

	void Start () {
        anim = GetComponent<Animator>();

        anim.SetBool("Grounded", true);
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.A))
        {
            StartMoving();
        }
	}

    void FixedUpdate()
    {
        if (moving)
            transform.position = transform.position + (Vector3.right * 5f * Time.deltaTime);
    }

    public void StartMoving()
    {
        moving = true;
        anim.SetFloat("CurrentSpeed", 1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float speed = 2f;
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 1.3f;
    [SerializeField] private float maxX = 75f;
    [SerializeField] private float minY = 0f;
    [SerializeField] private float maxY = 20f;
    [SerializeField] private bool lockY, lockX = false;
    [SerializeField] private bool alwaysFollow = false;

    private float targetY, targetX;
    private Vector3 target;
    private PlayerController player;

    public PlayerController Player
    {
        set
        {
            player = value;
        }
    }

    void Update () {

        if (!player || !player.Alive)
            return;

        if (alwaysFollow || player.Grounded || player.Climbing)
        {
            target = player.transform.position;
        }

        float targetX = lockX ? 0f : Mathf.Clamp(target.x, 0f, maxX - offsetX);
        float targetY = lockY ? 0f : Mathf.Clamp(target.y, minY, maxY - offsetY);
        transform.position = new Vector3(targetX + offsetX /*Mathf.Lerp(transform.position.x, targetX + offsetX, speed * 2 * Time.deltaTime)*/,
                                         Mathf.Lerp(transform.position.y, targetY + offsetY, speed * Time.deltaTime), -10f);
	}
}

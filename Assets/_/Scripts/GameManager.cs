using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [SerializeField] private float restartDelay = 0.5f;
    [SerializeField] private Transform playerSpawnLocation;
    [SerializeField] private Transform collectibleSpawnLocation;
    [SerializeField] private Chaser chaser;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private CameraController cameraController;

    private GameObject player;

    bool collectibleCollected = false;

    public bool CollectibleCollected
    {
        get
        {
            return collectibleCollected;
        }

        set
        {
            collectibleCollected = value;
        }
    }

    void Awake () {
        SpawnPlayer();
        SpawnCollectible();
	}
	
	void Update () {
		
	}

    private void SpawnPlayer()
    {
        player = Instantiate(playerPrefab, playerSpawnLocation.position, new Quaternion());
        cameraController.Player = player.GetComponent<PlayerController>();
    }

    private void SpawnCollectible()
    {
        Instantiate(collectiblePrefab, collectibleSpawnLocation.position, new Quaternion());
        collectibleCollected = false;
    }

    public void PlayerDeath()
    {
        StartCoroutine(RestartDelay());
    }

    private IEnumerator RestartDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        SpawnPlayer();

        //chaser.Resetting = true;

        if (collectibleCollected)
            SpawnCollectible();
    }

    public void PauseGame(bool paused = true)
    {
        player.GetComponent<PlayerController>().FreezePlayer(paused);
    }
}

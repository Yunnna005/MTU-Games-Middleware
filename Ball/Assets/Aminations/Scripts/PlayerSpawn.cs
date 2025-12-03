using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{

    public PlayerInputManager playerInputManager;
    public Transform[] spawnPoints;

    private int nextSpawnIndex = 0;

    void OnEnable()
    {
        playerInputManager.onPlayerJoined += HandlePlayerJoined;
    }

    void OnDisable()
    {
        playerInputManager.onPlayerJoined -= HandlePlayerJoined;
    }

    private void HandlePlayerJoined(PlayerInput playerInput)
    {
        if (spawnPoints.Length == 0) return;

        playerInput.transform.position = spawnPoints[nextSpawnIndex].position;
        playerInput.transform.rotation = spawnPoints[nextSpawnIndex].rotation;

        nextSpawnIndex = (nextSpawnIndex + 1) % spawnPoints.Length;
    }
}

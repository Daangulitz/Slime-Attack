using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject[] playerPrefabs; // Array of different player prefabs
    public Transform[] spawnPoints;   // Assign spawn points in the inspector

    private void Awake()
    {
        int spawnIndex = 0; // Tracks the current spawn point index

        // Iterate over all connected gamepads
        foreach (var device in Gamepad.all)
        {
            if (spawnIndex >= spawnPoints.Length)
            {
                Debug.LogWarning("Not enough spawn points for the number of connected controllers!");
                break;
            }

            // Spawn a player at the current spawn point
            GameObject prefabToSpawn = playerPrefabs[spawnIndex % playerPrefabs.Length];
            GameObject player = Instantiate(prefabToSpawn, spawnPoints[spawnIndex].position, Quaternion.identity);

            // Assign the gamepad to the player
            var playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.SwitchCurrentControlScheme(device, device);
                Debug.Log($"Assigned {device.displayName} to Player {spawnIndex + 1}");
            }

            spawnIndex++;
        }

        // If no controllers are connected, log a warning
        if (Gamepad.all.Count == 0)
        {
            Debug.LogWarning("No gamepads connected! No players spawned.");
        }
    }
}
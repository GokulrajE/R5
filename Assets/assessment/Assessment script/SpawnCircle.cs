using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCircle : MonoBehaviour
{
    public GameObject circlePrefab; // Reference to the circle prefab
    public KeyCode spawnKey = KeyCode.Space; // Key to spawn the circle

    void Update()
    {
        // Check if the specified key is pressed
        if (Input.GetKeyDown(spawnKey))
        {
            // Spawn the circle at the player's position
            SpawnCircleAtPlayerPosition();
        }
    }

    void SpawnCircleAtPlayerPosition()
    {
        // Check if the player GameObject is tagged as "Player"
        GameObject Player = GameObject.FindGameObjectWithTag("Player");

        if (Player != null)
        {
            // Get the player's position
            Vector2 PlayerPosition = Player.transform.position;

            // Instantiate the circle prefab at the player's position
            Instantiate(circlePrefab, PlayerPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Player not found in the scene. Make sure the player has the 'Player' tag.");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newscript : MonoBehaviour
{
    public GameObject homePositionPrefab;  // Assign the home position prefab in the Unity Inspector
    public GameObject targetPrefab;        // Assign the target prefab in the Unity Inspector

    private GameObject homePositionObject;
    private GameObject[] targetObjects = new GameObject[7]; // Array to store 7 target objects

    private bool inHomePosition = false;
    private bool[] inTargetPositions = new bool[7]; // Array to track if player is in each target position

    void Update()
    {
        // Check if the player presses the "h" key
        if (Input.GetKeyDown(KeyCode.H))
        {
            SetHomePosition();
        }

        // Check if the player is close to the home position
        float distanceToHome = Vector2.Distance(transform.position, homePositionObject.transform.position);

        // If the player is inside the home position
        if (inHomePosition)
        {
            // Change home position color based on player presence
            if (distanceToHome < 0.5f)
            {
                homePositionObject.GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                homePositionObject.GetComponent<Renderer>().material.color = Color.green;

                // Player leaves the home position
                inHomePosition = false;
                Destroy(homePositionObject);
                ShowTargets();
            }
        }

        // Check if the player is close to any of the target positions
        for (int i = 0; i < targetObjects.Length; i++)
        {
            float distanceToTarget = Vector2.Distance(transform.position, targetObjects[i].transform.position);

            if (inTargetPositions[i])
            {
                // Change target color or perform other actions as needed
                if (distanceToTarget < 0.5f)
                {
                    targetObjects[i].GetComponent<Renderer>().material.color = Color.red;
                }
                else
                {
                    targetObjects[i].GetComponent<Renderer>().material.color = Color.green;

                    // Player leaves the target position
                    inTargetPositions[i] = false;
                    Destroy(targetObjects[i]);
                    SetHomePosition(); // After leaving target, set a new home position
                }
            }
        }
    }

    void SetHomePosition()
    {
        // Spawn the home position circle at the player's position
        homePositionObject = Instantiate(homePositionPrefab, transform.position, Quaternion.identity);
        inHomePosition = true;
    }

    void ShowTargets()
    {
        // Spawn 7 targets in a semicircle at a distance of 15 cm from the home position
        float distanceFromHome = 0.15f; // Assuming 0.15f is the radius (15 cm)
        Vector2 homePosition = transform.position;

        for (int i = 0; i < targetObjects.Length; i++)
        {
            float angle = i * (180f / (targetObjects.Length - 1));
            float radians = angle * Mathf.Deg2Rad;

            float x = homePosition.x + distanceFromHome * Mathf.Cos(radians);
            float y = homePosition.y + distanceFromHome * Mathf.Sin(radians);

            Vector2 targetPosition = new Vector2(x, y);

            targetObjects[i] = Instantiate(targetPrefab, targetPosition, Quaternion.identity);
            inTargetPositions[i] = true;
        }
    }
}
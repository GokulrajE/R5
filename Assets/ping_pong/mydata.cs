using System.Collections.Generic;
using UnityEngine;

public class myData : MonoBehaviour
{
    public static myData instance;

    // Define the properties you need
    public static int startGameLevelRom = 1; // Starting level for the game
    public static int startGameLevelSpeed = 1; // Initial speed level

    public PlutoData plutoData;

    void Awake()
    {
        // Singleton pattern to ensure only one instance of AppData
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize PlutoData
        plutoData = new PlutoData();
    }
}

public class PlutoData
{
    public List<Mech> mechs = new List<Mech>();
    public int mechIndex = 0;
    public float desTorq = 0.0f;
}

public class Mech
{
    // Define properties of Mech
    public string name;
    public int id;
    // Add other properties as needed
}

public enum ControlType
{
    TORQUE,
    POSITION,
    VELOCITY
}

public static class SendToRobot
{
    public static void ControlParam(Mech mech, ControlType controlType, bool param1, bool param2)
    {
        // Implement the method to control the robot based on the parameters
        Debug.Log($"Controlling {mech.name} with {controlType} type, param1: {param1}, param2: {param2}");
    }
}

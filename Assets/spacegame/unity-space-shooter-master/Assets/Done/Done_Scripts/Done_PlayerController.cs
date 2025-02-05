

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEditor;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;

[System.Serializable]
public class Done_Boundary
{
    public float xMin;
    public float xMax;
    public float zMin;
    public float zMax;

    public Done_Boundary(float minX, float maxX, float minZ, float maxZ)
    {
        xMin = minX;
        xMax = maxX;
        zMin = minZ;
        zMax = maxZ;
    }
}

public class Done_PlayerController : MonoBehaviour
{
   
    public static Done_PlayerController instance;
    public float speed;
    public float tilt;
    public Done_Boundary boundary;
    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    public float nextFire;
    public Rigidbody rb;
    public float previousY;
    public float previousx;
    public int MovingAverageLength = 10;
    private float movingAverage;
    public static float timer_;
   
    private bool isRunning;
    private string latestData;
    public float Xp;
    public float Yp;
    public static string Date;
    public float[] angXRange;
    public float[] angZRange;

    private double x_c;
    private double y_c;
    private Vector2 playerDirection;
    public AudioClip[] sounds;
    private AudioSource source;

    public float UnityXRange;
    public float UnityZRange;
    public float fireStopClock;
    private float PlayerX, PlayerY, hazardX, hazardY, savedState;
    public float singleFireTime;

    private string welcompath = Staticvlass.FolderPath;
    private static string spacePath;
     private GameState currentState;
    string CurrentStat;
    public static string csvFilePath;
   
    public Text uiTextComponent;
    public static class spaceclass
    {
        public static string spacepath;
        public static string relativepath;
    }

    //Actual X and Y maximum and minimum position values of the robot (hand holder or the cone used to hold the robot)
    float romMinY = 67f;
    float romMaxY = 17f;
    float romMinX = 53f;
    float romMaxX = -24f;

    void Awake()
    {
       
        instance = this;
    }

    void Start()
    {
        Date = System.DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy HH-mm-ss");
        rb = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();


        //Maximum and minimum X and Z coordinate values in the game scene for the playerGameObject.
        boundary = new Done_Boundary(-17, 17, -4, 13); //before 17-15, y 13 - 15



        string DataPath = Path.Combine(welcompath, "spaceshooter");
       // string assessfile = Path.Combine(welcompath, "space_Data");
        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }
        spacePath = Path.Combine(DataPath, "space_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".csv");
        spaceclass.spacepath = spacePath;



        string fullFilePath = spaceclass.spacepath;

        // Define the part of the path you want to store
        string partOfPath = @"Application.dataPath";

        // Use Path class to get the relative path
        string relativePath = Path.GetRelativePath(partOfPath, fullFilePath);
        spaceclass.relativepath = relativePath;
        // Ensure CSV file is ready
        PrepareCsvFile(spacePath);
        // Retrieve the game state from PlayerPrefs
        int gameStateValue = PlayerPrefs.GetInt("GameState", (int)GameState.TargetMoving);
        GameState gameState = (GameState)gameStateValue;
        //UpdateGameState(gameState);
        LogGameState(currentState);
        
        //Active Range of Motion of the patient to be clamped with game scene
        //Debug.Log(ChooseGame.instance.max_x + ", " + ChooseGame.instance.min_x + ", " + ChooseGame.instance.max_y + ", " + ChooseGame.instance.min_y);
        romMinX = ChooseGame.instance.max_x;
        romMaxX = ChooseGame.instance.min_x;
        romMaxY = ChooseGame.instance.min_y;
        romMinY = ChooseGame.instance.max_y;
    }
    private void UpdateGameState(GameState state)
    {
        
        currentState = state;
        AppendDataToCsv(spaceclass.spacepath, currentState);
    }
    void Update()
    {
        PlayerX = PlayerPrefs.GetFloat("Playerx");
        PlayerY = PlayerPrefs.GetFloat("Playery");
        hazardX = PlayerPrefs.GetFloat("HazardX");
        hazardY = PlayerPrefs.GetFloat("HazardY");
        
        timer_ += Time.deltaTime;
        movePlayer();
    }
    
   
    void movePlayer()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

      
        float movementSpeed = 15f;  // Adjust speed as needed
        float x = MapXpToScreenX(R5comm.Xp);
        float y = MapYpToScreenZ(R5comm.Yp);

        Vector3 targetPosition = new Vector3(
            x,
            0.0f,
            y
        );

        //Smooth movement with Rigidbody
        rb.MovePosition(Vector3.Lerp(rb.position, targetPosition, movementSpeed * Time.deltaTime));
        rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);

        PlayerPrefs.SetFloat("PlayerX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerZ", transform.position.z);

        // Append Data to CSV
        AppendDataToCsv(spaceclass.spacepath, currentState);
    }

    //// Your existing LineRenderer workspace values
    public float MapXpToScreenX(float xp)
    {
        float playSizeX = boundary.xMax - boundary.xMin;

        // Map xp to screen X range
        float screenX = boundary.xMin + ((xp - romMinX ) / (romMaxX - romMinX)) * playSizeX;

        // Clamp to ensure it stays within valid range
        return Mathf.Clamp(screenX, boundary.xMin - 0.2f * playSizeX, boundary.xMax + 0.2f * playSizeX);
    }

    public float MapYpToScreenZ(float yp)
    {
        float playSizeZ = boundary.zMax - boundary.zMin;

        // Map yp to screen Z range
        float screenZ = boundary.zMin + ((yp - romMinY) / (romMaxY- romMinY)) * playSizeZ;

        // Clamp to keep it within the range
        return Mathf.Clamp(screenZ, boundary.zMin - 0.2f * playSizeZ, boundary.zMax + 0.2f * playSizeZ);
    }

  
    private void PrepareCsvFile(string filepath)
    {
        if (!File.Exists(filepath))
        {
            string header = "Time,PlayerX,PlayerY,HazardX,HazardY,GameState\n";
            File.WriteAllText(filepath, header);
        }
    }
    public void LogGameState(GameState currentState)
    {
        AppendDataToCsv(spaceclass.spacepath, currentState);
    }
    private void AppendDataToCsv(string filepath, GameState currentState)
    {
        DateTime currentDateTime = DateTime.Now;
        string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string data = $"{formattedDateTime},{PlayerX},{PlayerY},{hazardX},{hazardY},{(int)currentState}\n";
        File.AppendAllText(filepath, data);
    }
   
}






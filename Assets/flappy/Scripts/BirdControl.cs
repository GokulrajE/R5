




using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using PlutoDataStructures;

using System.IO;
using System;
using System.Linq;
using System.Text;
using System.Data;
using System.IO.Enumeration;
using static Done_PlayerController;
using static UnityEditor.Experimental.GraphView.GraphView;
//using static UnityEngine.Rendering.DebugUI;
//using static UnityEditor.PlayerSettings;





public class BirdControl : MonoBehaviour
{
   
    public float tilt;

    //Maximum and minimum y posistion of the PlayerGameObject in the game scene
    static float topBound = 6F;
    static float bottomBound = -3.2F;

    public static BirdControl instance;
    private bool isDead = false;
    public static Rigidbody2D rb2d;
    Animator anime;
    
    public int controlValue;
    public static float playSize;
    public static int FlipAngle = 1;
    public static float tempRobot, tempBird;
    public bool set = false;
    public TMPro.TMP_Dropdown ControlMethod;
    public float angle1;

    int totalLife = 5;
    int currentLife = 0;
    bool columnHit;
    public Image life1;
    public Image life2;
    public Image life3;

    public float spriteBlinkingTimer = 0.0f;
    public float spriteBlinkingMiniDuration = 0.1f;
    public float spriteBlinkingTotalTimer = 0.0f;
    public float spriteBlinkingTotalDuration = 2f;
    public bool startBlinking = false;

    public float happyTimer = 200.0f;

    public float speed = 0.001f;
   
    public float Player_translate_UpSpeed = 0.03f;
    public float Player_translate_DownSpeed = -0.03f;

    public static float spawntime = 3f;
    private Vector2 direction;

    float startTime;
    float endTime;
    float loadcell;
   
    float targetAngle;

    public FlappyGameControl FGC;

    //flappybird style for hand grip
    private Vector3 Direction;
    public float gravity = -9.8f;
    public float strength;

    long temp_ms = 0;

    int collision_count;
    public int total_life = 3;
    public int overall_life = 3;

    string date_now;
    string hospno;
    int hand_use;
    int count_session;

    public static int hit_count = 0;

    public static bool reduce_speed;

    StringBuilder csv = new StringBuilder();

    List<Vector3> paths;
    float max_y;
    float min_y;
    float y_c;

    private string filePath;
    private string welcompath = Staticvlass.FolderPath;
    public static string fileName;
    private FlappyColumn flappyColumn;

    //Actual x and z coordinates of the robot posistion
    float romMinY = 80f;
    float romMaxY = 0f;
    //from real device possition
    public float aromMinY;
    public float aromMaxY;
    public float promMinY;
    public float promMaxY;
    public static class flappyclass
    {
        public static string flappypath;
        public static string relativepath;
    }

    void Start()
    {
        // Debug.Log("START");
        instance = this;
        hit_count = 0;
        collision_count = 0;
        startTime = 0;
        endTime = 0;
        currentLife = 0;
        anime = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
       
        reduce_speed = false;

        ////Active Range of Motion of the patient to be clamped with game scene
        //romMaxY = ChooseGame.instance.min_y;
        //romMinY = ChooseGame.instance.max_y;



        string flappyfile = welcompath + "\\" + "flappy_Data";
        if (Directory.Exists(flappyfile))
        {
             fileName = "FlappyGameData_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".csv";
        }
        else
        {
            Directory.CreateDirectory(flappyfile);
             fileName = "FlappyGameData_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".csv";
        }
        flappyclass.flappypath = Path.Combine(flappyfile,  fileName);

        string fullFilePath = flappyclass.flappypath;

        // Define the part of the path you want to store
        string partOfPath = @"Application.dataPath";

        // Use Path class to get the relative path
        string relativePath = Path.GetRelativePath(partOfPath, fullFilePath);
        flappyclass.relativepath = relativePath;

        flappyColumn = FindObjectOfType<FlappyColumn>();
        WriteHeader();

        playSize = topBound - bottomBound;
        aromMaxY = ChooseGame.instance.min_y;
        aromMinY = ChooseGame.instance.max_y;
        //promMinY = Drawlines.PRomYmax;
        //promMaxY = Drawlines.PRomYmin;
        promMinY = Drawlines.PRomYmax < romMinY ? promMinY = Drawlines.PRomYmax : promMinY = romMinY;//67
        promMinY = Drawlines.PRomYmin < romMaxY ? promMinY = Drawlines.PRomYmin : promMaxY = romMaxY;//17
        aromMaxY = MapYpToScreenY(aromMaxY);
        aromMinY = MapYpToScreenY(aromMinY);
        Debug.Log(aromMinY + "/" + aromMaxY + "birdspawnBoundary");
    }

    void WriteHeader()
    {
        if (!File.Exists(filePath))
        {

            string header = "CurrentDateTime,PlayerX,PlayerY,ColumnX,ColumnY,HitData\n";
            File.WriteAllText(flappyclass.flappypath, header);
            
        }
    }
    // Update is called once per frame

    private void Update()
    {
        movePlayer();

        if (startBlinking == true)
        {
            hit_count = collision_count;

            if (collision_count < total_life)
            {
                SpriteBlinkingEffect();
            }
            else
            {
                overall_life = overall_life - 1;
                
                FlappyGameControl.instance.BirdDied();
               
                collision_count = 0;
                life1.enabled = true;
                life2.enabled = true;
                life3.enabled = true;
                
                if (overall_life == 0)
                {
                    reduce_speed = true;
                    overall_life = 3;
                  
                }
                else
                {
                    reduce_speed = false;
                   
                }

            }

        }

    }
  
    // WORKING CODE
    void movePlayer()
    {
       
        // Smooth movement using Vector3.Lerp or Vector3.MoveTowards
        float moveVertical = Input.GetAxis("Vertical");
        

        float targetY = MapYpToScreenY(R5comm.Yp);

        // Interpolating the Y position for smooth transition
        float smoothSpeed = 5f; // Adjust this value for smoother or faster movement
        float smoothedY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * smoothSpeed);
        transform.position = new Vector3(transform.position.x, smoothedY, transform.position.z);

        // Boundary checks
        if (transform.position.y > topBound)
        {
            transform.position = new Vector3(transform.position.x, topBound, transform.position.z);
        }
        else if (transform.position.y < bottomBound)
        {
            transform.position = new Vector3(transform.position.x, bottomBound, transform.position.z);
        }

        // Conditional auto-data update
        int control = BirdControl.hit_count;
        if (control < 3)
        {
            AutoData();
        }

    }
    public void AutoData()
    {
        // Get the current data to be logged
        float playerX = transform.position.x;
        float playerY = transform.position.y;
       
        float columnX = flappyColumn.transform.position.x;
        float columnY = flappyColumn.transform.position.y;

        int hitData = BirdControl.hit_count;
       
        string currentTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

        string newData = $"{ currentTime}, {playerX}, {playerY}, {columnX}, {columnY}, {hitData}\n";
        File.AppendAllText(flappyclass.flappypath, newData);
    }
    public float MapYpToScreenY(float yp)
    {
        float playSizeZ = topBound - bottomBound;

        // Map yp to screen Z range
        float screenZ = bottomBound + ((yp - promMinY) / (promMaxY - promMinY)) * playSizeZ;

        // Clamp to keep it within the range
        return Mathf.Clamp(screenZ, bottomBound - 3.6f * playSizeZ, topBound + 3.6f * playSizeZ);
    }

    public void SpriteBlinkingEffect()
    {
        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            startBlinking = false;
            spriteBlinkingTotalTimer = 0.0f;
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;   // according to 
                                                                             //your sprite
            return;
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;
            if (this.gameObject.GetComponent<SpriteRenderer>().enabled == true)
            {
                this.gameObject.GetComponent<SpriteRenderer>().enabled = false;  //make changes
            }
            else
            {
                this.gameObject.GetComponent<SpriteRenderer>().enabled = true;   //make changes
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        startBlinking = true;
        collision_count++;
        // Debug.Log(collision_count+" :collision");
        if (collision_count == 1)
        {
            life1.enabled = false;
        }
        else if (collision_count == 2)
        {
            life2.enabled = false;
        }
        else if (collision_count == 3)
        {
            life3.enabled = false;
        }


    }
    public void ResetLives()
    {
        collision_count = 0;
        life1.enabled = true;
        life2.enabled = true;
        life3.enabled = true;
      
    }


}

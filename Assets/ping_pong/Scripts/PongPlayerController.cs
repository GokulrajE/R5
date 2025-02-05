
using UnityEngine;
using System;
using UnityEngine.UI; // Add this line to use UI elements
using System.IO;
using UnityEditor.Rendering;
//using static UnityEditor.PlayerSettings;
using UnityEngine.Rendering;

public static class pongclass
{
    public static string filepath;
    public static string relativepath;
}

public class PongPlayerController : MonoBehaviour
{
    public static PongPlayerController instance;

    //maximum and minmum y posisiton of the playerGameObject in the scene
    public static float topBound = 3.6f;
    public static float bottomBound = -3.6f;
   
    public static float playSize;
    public static float player_x, player_y;

    private string welcompath = Staticvlass.FolderPath;
    private string filePath;
    private bool isPaused = false;
    private bool gameWon = false; // Variable to track if the game is won
    private int previousPlayerScore = 0; // Variable to track the player's previous score
 
    private DateTime startTime; // Start time of the game
   
    public Text LevelText;
    public int currentLevel = 1;
   
    public bool levelComplete = false; // Flag to indicate level completion
    public GameObject GameOverText;
    public Transform playerPaddle; // Reference to the player's paddle
    public bool transitioningToNextLevel = false; // Flag to indicate transition to next level
    public static string relativepath;
    public static string gameparameterpong;
    public static float paddleparameter = 1;
    public static float updatepaddleparameter;
    public static float ppaddleparameter;

    //from real device possition
    public float romMinY = 67f;
    public float romMaxY = 17f;

    void Start()
    {
        
        string pongfile = welcompath + "\\" + "Pong_Data";
        if (Directory.Exists(pongfile))
        {
            filePath = Path.Combine(pongfile, "Pong_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".csv");
        }
        else
        {
            Directory.CreateDirectory(pongfile);
            filePath = Path.Combine(pongfile, "Pong_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".csv");
        }
        pongclass.filepath = filePath;

        string fullFilePath = pongclass.filepath;

        // Define the part of the path you want to store
        string partOfPath = @"Application.dataPath";

        // Use Path class to get the relative path
        string relativePath = Path.GetRelativePath(partOfPath, fullFilePath);
        pongclass.relativepath = relativePath;
        relativepath = relativePath;
        WriteHeader();
       
        GameOverText.SetActive(false); 

        AdjustPaddleSizeForNextLevel();
        // for now
        //Active Range of Motion of the patient to be clamped with game scene
        romMaxY = ChooseGame.instance.min_y;
        romMinY = ChooseGame.instance.max_y;

    }

    void Update()
    {
        playSize = topBound - bottomBound;
        if (!isPaused && !gameWon)//!transitioningToNextLevel
        {
            LogData();
            Game();
            CheckLevelCompletion(); // Check for level completion
            //UpdateDurationText();
        }

        LevelText.text = "Level: " + currentLevel;

    }
    void CheckLevelCompletion()
    {
      
        int scoreThreshold = GetScoreThresholdForLevel(currentLevel); // Get score threshold for the current level

        // Check if the score threshold for the current level has been reached
        if ((scoreclass.playerpoint >= scoreThreshold || scoreclass.enemypoint >= scoreThreshold)) //!transitioningToNextLevel
        {

        }
    }

    int GetScoreThresholdForLevel(int level)
    {
       
        if (level == 1) return 5;
      
        return 5;
    }

    void Game()
    {
        
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

        player_x = transform.position.x;
        player_y = transform.position.y;

    }
    public float MapYpToScreenY(float yp)
    {
        float playSizeZ = topBound - bottomBound;

        // Map yp to screen Z range
        float screenZ = bottomBound + ((yp - romMinY) / (romMaxY - romMinY)) * playSizeZ;

        // Clamp to keep it within the range
        return Mathf.Clamp(screenZ, bottomBound - 3.6f * playSizeZ, topBound + 3.6f * playSizeZ);
    }
   
    void WriteHeader()
    {
        if (!File.Exists(filePath))
        {
            string header = "Time,PlayerX,PlayerY,EnemyX,EnemyY,BallX,BallY,PlayerScore,EnemyScore\n";
            File.WriteAllText(pongclass.filepath, header);
        }
    }

    void LogData()
    {
        GameObject ball = GameObject.FindGameObjectWithTag("Target");
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");

        float ball_x = ball.transform.position.x;
        float ball_y = ball.transform.position.y;
        float enemy_x = enemy.transform.position.x;
        float enemy_y = enemy.transform.position.y;

        string currentTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        string data = $"{currentTime},{player_x},{player_y},{enemy_x},{enemy_y},{ball_x},{ball_y},{scoreclass.playerpoint},{scoreclass.enemypoint}\n";

        File.AppendAllText(pongclass.filepath, data);

    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }
   
    public void NextLevel()
    {

        AdjustPaddleSizeForNextLevel();
        currentLevel++; // Increment the level
       
        LevelText.text = "Level: " + currentLevel;
        //levelComplete = false; // Reset the levelComplete flag
        gameWon = false; // Reset the gameWon flag

        Debug.Log("NextLevel Method Called: currentLevel = " + currentLevel);
        //Reset the ball's movement
        GameObject ball = GameObject.FindGameObjectWithTag("Target");
        if (ball != null)
        {
            BallController ballController = ball.GetComponent<BallController>();
            if (ballController != null)
            {
                ballController.ResetBall(); // Reset the ball's movement
            }
        }

        scoreclass.playerpoint = 0; // Reset player score for the next level
        scoreclass.enemypoint = 0; // Reset enemy score for the next level
        transitioningToNextLevel = false; // Reset the flag after transitioning
    }


    public void AdjustPaddleSizeForNextLevel()
    {
        playerPaddle.localScale = new Vector3(playerPaddle.localScale.x, playerPaddle.localScale.y * UIManager.pong_speed, playerPaddle.localScale.z);

    }



    public static float GetBallSpeed()
    {
        // Implement the logic to retrieve the ball speed
        // For example:
        GameObject ball = GameObject.FindGameObjectWithTag("Target");
        if (ball != null)
        {
            BallController ballController = ball.GetComponent<BallController>();
            if (ballController != null)
            {
                return ballController.speed; // Assuming BallController has a speed property
            }
        }
        return 0f; // Default speed if not found
    }

}
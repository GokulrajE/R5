
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEngine.SceneManagement;
using System.Linq;
using static Done_PlayerController;
using NeuroRehabLibrary;


public class Done_GameController : MonoBehaviour
{
    public static Done_GameController instance;
    public AudioClip sounds;
    private AudioSource source;

    public GameObject[] hazards;
    public GameObject Player;
    public Vector3 spawnValues;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;

    public Text scoreText;
    public Text restartText;
    public Text gameOverText;
    public Text durationText;
    public Text LevelText;

    public GameObject imagelife1;
    public GameObject imagelife2;
    public GameObject imagelife3;

    private bool gameOver;
    private bool restart;
    private bool nextlevel;
    private int score;
    private int duration = 0;

    public static string gameend;

    public float levelspeed;
    public float start_levelspeed;

    public GameObject GameOverCanvas;
    public GameObject CongratsCanvas;

    string p_hospno;
    int gameover_count = 0;
    int overall_life_count = 0;
    int hit_count = 0;
    int life_count_completed = 0;
    int hazardsDestroyed = 0;

    string start_time;
    string end_time;

    float timeToAppear = 1f;
    float timeWhenDisappear;

    IEnumerator timecoRoutine;
    IEnumerator wavecoRoutine;

    float player_level;
    int level_playing = 1;

    string path_to_data;
    public static string datetime;
    // New variables
    private GameObject currentHazard;
    private bool canSpawn;
    private bool hazardDestroyedByBoundary;
    private State currentstate;
    // Add a private variable to store the current level
    private int currentLevel = 1;
    int HazardsDestroyed = 0; // Track hazards destroyed
    int hazardsSpawned = 0;   // Track hazards spawned
    public Text totalScoreText; // Add a new Text object for total score display
    public float PlayerMoveTime;
    public static float spacegameparametervalue;


    private GameSession currentGameSession;

    public KeyCode physicalButtonKey = KeyCode.Space; // Simulate the physical button with the space key for testing


    public void Start()
    {

        InitializeGame();
        StartNewGameSession();
        RetrieveAndAdjustGameParameterFromCSV();
    }
    void InitializeGame()
    {
        path_to_data = Application.dataPath;
        start_time = DateTime.Now.ToString("HH:mm:ss.fff");
        datetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff");

        source = GetComponent<AudioSource>();
        GameOverCanvas.SetActive(false);
        CongratsCanvas.SetActive(false);

        gameOver = false;
        restart = false;
        nextlevel = false;
        restartText.text = "";
        gameOverText.text = "";
        totalScoreText.text = ""; // Initialize the total score text
        duration = 60;
        score = 0;
        gameover_count = 0;
        overall_life_count = 0;
        PlayerMoveTime = 0;
        imagelife1.SetActive(true);
        imagelife2.SetActive(true);
        imagelife3.SetActive(true);

        LevelText.text = "Level: " + currentLevel;

        canSpawn = true;

        UpdateScore();

        timecoRoutine = SpawnTimer();
        StartCoroutine(timecoRoutine);

        wavecoRoutine = SpawnWaves();
        StartCoroutine(wavecoRoutine);
    }
    void Data(State state)
    {
        // Now save the current data
        string currentStateStr = ((int)state).ToString();
        // AutoData();

    }


    void OnDestroy()
    {

        // StartNewGameSession();
        // AutoData();
    }


    void StartNewGameSession()
    {
        currentGameSession = new GameSession
        {
            GameName = "SPACESHOOTER",
            Assessment = 0 // Example assessment value, adjust as needed
        };

        SessionManager.Instance.StartGameSession(currentGameSession);
        Debug.Log($"Started new game session with session number: {currentGameSession.SessionNumber}");

        SetSessionDetails();
    }


    private void SetSessionDetails()
    {
        string device = "R5"; // Set the device name
        string assistMode = "Null"; // Set the assist mode
        string assistModeParameters = "Null"; // Set the assist mode parameters
        string deviceSetupLocation = "Null"; // Set the device setup location
      
        string gameParameter = spacegameparametervalue.ToString();
        SessionManager.Instance.SetGameParameter(gameParameter, currentGameSession);

        SessionManager.Instance.SetDevice(device, currentGameSession);
        SessionManager.Instance.SetAssistMode(assistMode, assistModeParameters, currentGameSession);
        SessionManager.Instance.SetDeviceSetupLocation(deviceSetupLocation, currentGameSession);
        


    }


    private void RetrieveAndAdjustGameParameterFromCSV()
    {
        string csvFilePath = Path.Combine(circleclass.circlePath, "Sessions.csv");

        if (!File.Exists(csvFilePath))
        {
            Debug.LogError("CSV file not found at: " + csvFilePath);
            return;
        }

        string[] csvLines = File.ReadAllLines(csvFilePath);
        string[] headers = csvLines[0].Split(',');

        // Find the indices for session number, game name, and game parameter columns
        int sessionIndex = Array.IndexOf(headers, "SessionNumber");
        int gameNameIndex = Array.IndexOf(headers, "GameName");
        int gameParameterIndex = Array.IndexOf(headers, "GameParameter");

        if (sessionIndex == -1 || gameNameIndex == -1 || gameParameterIndex == -1)
        {
            Debug.LogError("Required columns not found in the CSV file.");
            return;
        }

        // Find the last row with the same session number and game name
        string currentSessionNumber = currentGameSession.SessionNumber.ToString();
        string currentGameName = currentGameSession.GameName;

        string lastGameParameterValue = null;
        float spacegameplayPercentage = PlayerPrefs.GetFloat("spacegamepercentage"); ; // Assume this is tracked somewhere

        for (int i = csvLines.Length - 1; i > 0; i--)
        {
            string[] row = csvLines[i].Split(',');

            if ( row[gameNameIndex] == currentGameName)
            {
                lastGameParameterValue = row[gameParameterIndex];
                break;
            }
        }

        if (lastGameParameterValue != null)
        {
          float  gameparametervalue = float.Parse(lastGameParameterValue);
            if (spacegameplayPercentage >= 0.8f)
            {
                gameparametervalue = gameparametervalue + 0.1f; // Increase speed by 0.1
               

            }
            else //if (gameplayPercentage < 80)
            {
                gameparametervalue = gameparametervalue - 0.1f; // Decrease speed by 0.1
                if (gameparametervalue < 0.1f)
                {
                    gameparametervalue = 0.5f; // Reset to default speed
                }

            }

           
            
            levelspeed = gameparametervalue; // Assign the adjusted value to levelspeed
            //player_level = (levelspeed - 0.5f) / 0.25f;
            player_level = (levelspeed - 0.5f) / 0.25f;
            //level_playing = (int)(player_level + 1);
            spacegameparametervalue = levelspeed;
            Debug.Log("Game parameter retrieved and adjusted from CSV: " + gameparametervalue);

            SetSessionDetails();
        }
        else
        {
            levelspeed = 0.5f; // Default speed for the first time
            //player_level = (levelspeed - 0.5f) / 0.25f;

            player_level = (levelspeed - 0.5f) / 0.25f;
            level_playing = (int)( 1);
            Debug.Log("No matching session and game name found in the CSV file. Using default game parameter.");
            spacegameparametervalue = levelspeed;
            SetSessionDetails();
        }

        // Save the new game parameter to the CSV file



    }


    void EndCurrentGameSession()
    {
        if (currentGameSession != null)
        {
            int moveTime = (int)PlayerMoveTime;
            string trialDataFileLocation = spaceclass.relativepath;
            SessionManager.Instance.SetTrialDataFileLocation(trialDataFileLocation, currentGameSession);
            SessionManager.Instance.moveTime(moveTime.ToString(), currentGameSession);
            SessionManager.Instance.EndGameSession(currentGameSession);
        }
    }

    void Update()
    {
        Time.timeScale = levelspeed;
        if (!gameOver)
        {
            PlayerMoveTime += Time.deltaTime;
        }
        HandleTargetExitedBoundary();
        // Detect button press
        if (gameOver && Input.GetKeyDown(physicalButtonKey))
        {
            EndCurrentGameSession(); // End the current game session and save details
            StartNewGameSession(); // Start a new game session for the next trial
            RetrieveAndAdjustGameParameterFromCSV();
            onclick_nextlevel(); // Restart the game

        }

    }
    public void HandleTargetExitedBoundary()
    {
        // Check if the current hazard is destroyed or out of bounds
        if (currentHazard == null && !gameOver)
        {
            canSpawn = true;
            Debug.Log("Current hazard is null, transitioning to TargetExitedBoundary state.");
            GameStateMachine.Instance.TransitionToState(GameState.TargetExitedBoundary);
        }
    }

    public void OnApplicationQuit()
    {
        end_time = DateTime.Now.ToString("HH:mm:ss.fff");
        string duration_played = (DateTime.Parse(end_time) - DateTime.Parse(start_time)).ToString();
        //string newFileName = @"D:\AREBO\Unity\MARS demo3\Data\" + p_hospno+"\\"+"hits.csv";
        System.DateTime today = System.DateTime.Today;
        string data_csv = today.ToString() + "," + start_levelspeed + "," + levelspeed + "," + start_time + "," + end_time + "," + duration_played + "," + hit_count + "\n";
        //File.AppendAllText(newFileName,data_csv);
        Application.Quit();

    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);

        while (!gameOver)
        {
            if ((level_playing == 1 || level_playing == 2) && canSpawn && currentHazard == null)
            {
                canSpawn = false;

                // Determine where to spawn the hazard based on weighted probability
                float randomValue = UnityEngine.Random.value; // Random value between 0 and 1
                Vector3 spawnPosition;

                if (randomValue <= 0.5f) // 50% chance
                {
                    // 50% chance to spawn within the range (-16.5 to +16.5)
                    spawnPosition = new Vector3(
                        UnityEngine.Random.Range(-spawnValues.x, spawnValues.x),
                        spawnValues.y,
                        spawnValues.z
                    );
                }
                else if (randomValue <= 0.8f) // 30% chance
                {
                    // 30% chance to spawn slightly outside the range (-18 to +18)
                    float xRange = spawnValues.x + 1.5f; // Slightly farther than the screen width
                    spawnPosition = new Vector3(
                        UnityEngine.Random.Range(-xRange, xRange),
                        spawnValues.y,
                        spawnValues.z
                    );
                }
                else // 20% chance
                {
                    // 20% chance to spawn even farther outside the range (-20 to +20)
                    float xRange = spawnValues.x + 3.5f; // Farther than the screen width
                    spawnPosition = new Vector3(
                        Mathf.Clamp(UnityEngine.Random.Range(-xRange, xRange), -16.5f, 16.5f), // Clamp within screen limits
                        spawnValues.y,
                        spawnValues.z
                    );
                }

                GameObject hazard = hazards[UnityEngine.Random.Range(0, hazards.Length)];
                Quaternion spawnRotation = Quaternion.identity;
                currentHazard = Instantiate(hazard, spawnPosition, spawnRotation);
                hazardsSpawned++;

                Debug.Log("Spawned Hazard at: " + spawnPosition);
            }

            if (currentHazard == null)
            {
                canSpawn = true;  // Allow the next hazard to spawn
            }

            yield return new WaitForSeconds(spawnWait);
        }
    }


    private IEnumerator SpawnTimer()
    {
        while (!gameOver)
        {

            duration = duration - 1;
            UpdateDuration();
            //UpdateLevel();

            if (duration == 0)
            {
                life_count_completed = life_count_completed + 1;


                if (level_playing == 1)
                {
                    // StartNextlevel();
                    AdjustSpeedForNextLevel();
                    EndGame();
                }
             
            }

            if (duration == 40)
            {
                source.clip = sounds;
                source.PlayOneShot(source.clip);
            }

            if (duration == 20)
            {
                source.clip = sounds;
                source.PlayOneShot(source.clip);
            }

            yield return new WaitForSeconds(levelspeed);
            // yield return new WaitForSeconds(1.0f/levelspeed);

        }

    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        Debug.Log("Score updated: " + score);
        UpdateScore();
    }
    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    void UpdateDuration()
    {
        if (!nextlevel)
        {
            durationText.text = "Duration: " + duration;
        }



    }

    void UpdateLevel()
    {
        LevelText.text = "Level: " + level_playing;
        // Debug.Log(duration+"..."+Time.time);

    }

    public void GameOver()
    {
        //duration = 60;
        hit_count = hit_count + 1;

        // Decrease remaining lives and update visuals
        gameover_count = gameover_count + 1;

        if (gameover_count == 1)
        {
            imagelife1.SetActive(false);
        }
        if (gameover_count == 2)
        {
            imagelife2.SetActive(false);
        }
        if (gameover_count == 3)
        {
            imagelife3.SetActive(false);

        }
        if (gameover_count > 2)
        {

            gameOver = true;
            // Destroy the player object
            if (Player != null)
            {
                Destroy(Player);
            }
            EndCurrentGameSession();
            AdjustSpeedForNextLevel();
            StopCoroutine(timecoRoutine);
            StopCoroutine(wavecoRoutine);
            GameOverCanvas.SetActive(true);
            gameOverText.text = "Try again";
            totalScoreText.text = "Total Score: " + score;

            // StartNewGameSession();
            //AutoData();
            Time.timeScale = 0;

            // Reset gameover count
            gameover_count = 0;

            // Increment overall life count
            overall_life_count = overall_life_count + 1;

            if (overall_life_count > 2)
            {
                levelspeed = levelspeed - 0.25f;
                overall_life_count = 0;
                // Debug.Log(levelspeed+" : levelspeed");
                player_level = (levelspeed - 0.5f) / 0.25f;
                level_playing = (int)(player_level + 1);
            }
            //// Transition to TargetAndPlayerCollided state
            GameStateMachine.Instance.TransitionToState(GameState.TargetAndPlayerCollided);
        }




    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.tag);
        if (collision.gameObject.tag == "PlayerFire")
        {
            Debug.Log("Collided with PlayerFire, ignoring collision.");
            return;
        }
        GameOver();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger with: " + other.gameObject.tag);
        if (other.gameObject.tag == "PlayerFire")
        {
            Debug.Log("Triggered with PlayerFire, ignoring trigger.");
            return;
        }
        GameOver();
    }



   

    public void StartNextlevel()
    {
        currentLevel++;

        // Update the LevelText to the new level value
        LevelText.text = "Level: " + currentLevel;

        //nextlevel = false;
        //CongratsCanvas.SetActive(false);
        duration = 60;
        //UpdateDuration();
        // Reset the lives and gameover count
        gameover_count = 0;
        imagelife1.SetActive(true);
        imagelife2.SetActive(true);
        imagelife3.SetActive(true);
        AdjustSpeedForNextLevel();

        Time.timeScale = 1;
        // Restart the wave coroutine
        wavecoRoutine = SpawnWaves();
        StartCoroutine(wavecoRoutine);
    }

   

    public void onclick_previouslevel()
    {
        // Decrease the level
        currentLevel--;

        // Update the LevelText to the new level value
        LevelText.text = "Level: " + currentLevel;
        nextlevel = false;
        CongratsCanvas.SetActive(false);
        duration = 60;
        // levelspeed = levelspeed-0.3f;
        levelspeed = 0.25f;
        player_level = (levelspeed - 0.5f) / 0.25f;
        level_playing = (int)(player_level + 1);

    }

    public void onclick_replay()
    {
        // Reset the level to 1
        currentLevel = 1;

        // Update the LevelText to the new level value
        LevelText.text = "Level: " + currentLevel;

        nextlevel = false;
        CongratsCanvas.SetActive(false);
        duration = 60;
    }

    public void onclick_Restart()
    {
        SceneManager.LoadScene("SpaceShooterDemo");
        EndCurrentGameSession();
    }

    public void doquit()
    {
        if (Time.timeScale == 0)
        {

        }
        else
        {
            EndCurrentGameSession();
        }
        
        SceneManager.LoadScene("ChooseGame");

        Debug.Log("Quit");

    }
    public void SetHazardDestroyedByBoundary(bool destroyedByBoundary)
    {
        hazardDestroyedByBoundary = destroyedByBoundary;
        Debug.Log("SetHazardDestroyedByBoundary called. Value: " + destroyedByBoundary);
    }
    public void ResetHazardDestroyedByBoundary()
    {
        hazardDestroyedByBoundary = false;
        Debug.Log("ResetHazardDestroyedByBoundary called.");
    }
    // New method to be called when a hazard is destroyed
    public void HazardDestroyed()
    {
        hazardsDestroyed++;

    }


    private void AdjustSpeedForNextLevel()
    {

        float destroyedPercentage = (float)hazardsDestroyed / hazardsSpawned;

        PlayerPrefs.SetFloat("spacegamepercentage", destroyedPercentage);
       Debug.Log("destroyed percentage"+  destroyedPercentage);

    }
    public void EndGame()
    {
        gameOver = true;
        //restartText.text = "Press 'R' to Restart";
        gameOverText.text = "Congrats!";
        GameOverCanvas.SetActive(true);
        AdjustSpeedForNextLevel();

        // Destroy the player if it exists
        if (Player != null)
        {
            Destroy(Player);
        }

        // Stop all coroutines to prevent further spawning
        StopAllCoroutines();

        // Display the total score on the Game Over screen
        totalScoreText.text = "Total Score: " + score;
        Time.timeScale = 0; // Pause the game


        //AutoData();


    }
    public void onclick_nextlevel()
    {
        currentLevel++;


       // LevelText.text = "Level: " + currentLevel;

        nextlevel = false;
        CongratsCanvas.SetActive(false);
        GameOverCanvas.SetActive(false);
        duration = 60;
        // Reset the lives and gameover count
        gameover_count = 0;
        imagelife1.SetActive(true);
        imagelife2.SetActive(true);
        imagelife3.SetActive(true);
        AdjustSpeedForNextLevel();

        Time.timeScale = 1;
        // Restart the wave coroutine
        wavecoRoutine = SpawnWaves();
        StartCoroutine(wavecoRoutine);
        InitializeGame();
        

    }
}


























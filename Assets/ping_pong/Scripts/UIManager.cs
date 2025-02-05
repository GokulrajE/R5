
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System;
using UnityEditor;
using NeuroRehabLibrary;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    GameObject[] pauseObjects, finishObjects;
    public BoundController rightBound;
    public BoundController leftBound;
    public bool isFinished;
    public bool playerWon, enemyWon;
    public AudioClip[] audioClips; // winlevel loose
    public int winScore = 6;
    public int win;

    public static string startdatetime;
    public static string enddatetime;
    public static string starttime;
    public static string endtime;
    public static string end_time;
    public static string datetime;
    public static string start_time;
    public static string gameend_time;


    //private SessionManager sessionManager;
    private GameSession currentGameSession;
    private string gameParameter = "";
    public static float gameparameterpong;
    public static float nextparameter;
    public static float paddlemove;
    public static float gameparametervalue;
    public static float pong_speed;
    public static float setpaddle;
    public static float playerMoveTime;



    void Start()
    {
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        finishObjects = GameObject.FindGameObjectsWithTag("ShowOnFinish");
        hideFinished();

        startdatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff");
        start_time = DateTime.Now.ToString("HH:mm:ss.fff");
        datetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff");
        Time.timeScale = 1; // Ensure the game starts unpaused
        playerMoveTime = 0;
        StartNewGameSession();
        RetrieveAndAdjustGameParameterFromCSV();
    }

    void StartNewGameSession()
    {
        currentGameSession = new GameSession
        {
            GameName = "PING-PONG",
            Assessment = 0 // Example assessment value, adjust as needed
        };

        SessionManager.Instance.StartGameSession(currentGameSession);
        Debug.Log($"Started new game session with session number: {currentGameSession.SessionNumber}");

        SetSessionDetails();
    }


    public void SetSessionDetails()
    {
        string device = "R5"; // Set the device name
        string assistMode = "Null"; // Set the assist mode
        string assistModeParameters = "Null"; // Set the assist mode parameters
        string deviceSetupLocation = "Null";

        string gameParameter = pong_speed.ToString();

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
        float ponggameplayPercentage = PlayerPrefs.GetFloat("ponggamepercentage"); // Assume this is tracked somewhere

        for (int i = csvLines.Length - 1; i > 0; i--)
        {
            string[] row = csvLines[i].Split(',');

            if (row[sessionIndex] == currentSessionNumber && row[gameNameIndex] == currentGameName)
            {
                lastGameParameterValue = row[gameParameterIndex];
                break;
            }
        }

        if (lastGameParameterValue != null)
        {
            gameparametervalue = float.Parse(lastGameParameterValue);
            if (ponggameplayPercentage >= 0.8f)
            {
                gameparametervalue = gameparametervalue - 0.2f; // Increase speed by 0.1


            }
            else 
            {
                gameparametervalue = gameparametervalue + 0.2f; // Decrease speed by 0.1


            }


            pong_speed = gameparametervalue; // Assign the adjusted value to levelspeed
            Debug.Log("Game parameter retrieved and adjusted from CSV: " + gameparametervalue);

            SetSessionDetails();
        }
        else
        {
            pong_speed = 1.0f; // Default speed for the first time
            Debug.Log("No matching session and game name found in the CSV file. Using default game parameter.");

            SetSessionDetails();
        }

    }
    void EndCurrentGameSession()
    {
        if (currentGameSession != null)
        {
            int moveTime = (int)playerMoveTime;
            string trialDataFileLocation = pongclass.relativepath;
            SessionManager.Instance.moveTime(moveTime.ToString(), currentGameSession);
            SessionManager.Instance.SetTrialDataFileLocation(trialDataFileLocation, currentGameSession);
            SessionManager.Instance.EndGameSession(currentGameSession);
        }
    }

    void Update()
    {
        if (rightBound.enemyScore >= winScore && !isFinished)
        {
            HandleGameEnd(false);
        }
        else if (leftBound.playerScore >= winScore && !isFinished)
        {
            HandleGameEnd(true);
        }

        if (isFinished)
        {
            showFinished();
            if (Input.anyKeyDown)
            {
                LoadScene("pong_menu");
            }
        }
        if (!isFinished && Time.timeScale == 1)
        {
            playerMoveTime += Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.P) && !isFinished)
        {
            pauseControl();
        }

        if (Time.timeScale == 0 && !isFinished)
        {
            foreach (GameObject g in pauseObjects)
            {
                if (g.name == "PauseText")
                {
                    g.SetActive(true);
                }
            }
        }
        else
        {
            foreach (GameObject g in pauseObjects)
            {
                if (g.name == "PauseText")
                {
                    g.SetActive(false);
                }
            }
        }

        if (scoreclass.playerpoint == 5 || scoreclass.enemypoint == 5) // Example winning condition
        {
            // Set the gameWon flag to true
            EndCurrentGameSession();
        }
    }


    public void percentagecal()
    {
        if (scoreclass.playerpoint > scoreclass.enemypoint)
        {
            setpaddle = 0.9f;
        }
        else
        {
            setpaddle = 0.4f;
        }
        Debug.Log("percentpaddle"+setpaddle);
        PlayerPrefs.SetFloat("ponggamepercentage", setpaddle);
    }


    void HandleGameEnd(bool playerWon)
    {
        Camera.main.GetComponent<AudioSource>().Stop();
        this.playerWon = playerWon;
        enemyWon = !playerWon;
        win = playerWon ? 1 : -1;
        isFinished = true;
        if (isFinished == true)
        {
            Time.timeScale = 0;
        }
        playAudio(playerWon ? 0 : 1);

        percentagecal();
        EndCurrentGameSession();


    }

    public void LoadScene(string sceneName)
    {
        percentagecal();
        EndCurrentGameSession();

        SceneManager.LoadScene(sceneName);

    }


    public void onclick_pongmenu()
    {
        percentagecal();
        EndCurrentGameSession();
        SceneManager.LoadScene("pong_menu");


    }
    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    void playAudio(int clipNumber)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = audioClips[clipNumber];
        audio.Play();
    }

    public void pauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            showPaused();
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            hidePaused();
        }
    }

    public void showPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
    }

    public void hidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    public void showFinished()
    {
        foreach (GameObject g in finishObjects)
        {
            g.SetActive(true);
        }
    }

    public void hideFinished()
    {
        foreach (GameObject g in finishObjects)
        {
            g.SetActive(false);
        }
    }


}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;
//using UnityEngine.Rendering.Universal;
using static Done_PlayerController;
using NeuroRehabLibrary;
using UnityEngine.Rendering;


public class Player : MonoBehaviour
{
    public static Player instance;
    public Done_Boundary boundary;
    public float movespeed = 5f;
    public float Xp;
    public float Yp;
    //public float l1 = 333;
    //public float l2 = 381;
    public static string Date;
    public static string filePath;
    public InputField Hos_;
    public string gameParameter;

    public static string csvFilePath;
    public string selectedComPort = "COM5"; // Default COM port
    public float x2;
    public float y2;
    public static JediSerialCom serReader;
    public float[] angXRange;
    public float[] angZRange;
    float enc_1, enc_2;
    float Rob_X, Rob_Y;
    string TargetPosx, TargetPosy, CurrentStat, PlayerPosx, PlayerPosy, CirclePositionX, CirclePositionY, targetRadii, circleRadiusStr;

    public static float timer_;

    private string welcompath = Staticvlass.FolderPath;

    public Rigidbody2D _rb;
    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning;
    private string latestData;
    public Text uiTextComponent;

    public static string datetime;
    public static string start_time;
    public static string gameend_time;


    private SessionManager sessionManager;
    private GameSession currentGameSession;


    // Variables to hold the first non-null CirclePosX and CirclePosY values
    private string firstNonNullCirclePosX = null;
    private string firstNonNullCirclePosY = null;
    private double x_c;
    private double y_c;
    public static class assessmentclass
    {
        public static string assessmentpath;
        public static string relativepath;
    }
    //Actual X and Y maximum and minimum position values of the robot (hand holder or the cone used to hold the robot
    float romMinY = 67f;
    float romMaxY = 17f;
    float romMinX = 53f;
    float romMaxX = -24f;
    // Game scene Player max and min value in x and y
    float xmax = 8.6f;
    float xmin = -8.5f;
    float ymax = 4.7f;
    float ymin = -4.6f;
   


    void Awake()
    {
       
        instance = this;
    }
    void Start()
    {
      
        start_time = DateTime.Now.ToString("HH:mm:ss.fff");
        datetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff");
       
        string assessfile = Path.Combine(welcompath, "assessment_Data");
        if (!Directory.Exists(assessfile))
        {
            Directory.CreateDirectory(assessfile);
        }
        //csvFilePath = Path.Combine(assessfile, "assessment_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".csv");
        // Base file path for the CSV file
        string baseFilePath = Path.Combine(assessfile, "assessment.csv");

        // Create a unique CSV file path for the new session
        csvFilePath = GetNewCsvFilePath(baseFilePath);
        assessmentclass.assessmentpath = csvFilePath;

        string fullFilePath = assessmentclass.assessmentpath;

        // Define the part of the path you want to store
        string partOfPath = @"Application.dataPath";

        // Use Path class to get the relative path
        string relativePath = Path.GetRelativePath(partOfPath, fullFilePath);
        assessmentclass.relativepath = relativePath;
        // Initialize the CSV file with the header
        SaveCSVData(csvFilePath, true, ref firstNonNullCirclePosX, ref firstNonNullCirclePosY);
        // Initialize the CSV file with the header
        //SaveCSVData(csvFilePath, true);





        StartNewGameSession();

        // Clear previous PlayerPrefs data
        
        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY");
        PlayerPrefs.DeleteKey("targetPos1");
        PlayerPrefs.DeleteKey("targetPos2");
        PlayerPrefs.DeleteKey("targetRadii");
        PlayerPrefs.DeleteKey("CirclePosX");
        PlayerPrefs.DeleteKey("CirclePosY");
        PlayerPrefs.DeleteKey("CircleRadius");
        PlayerPrefs.DeleteKey("Currentstat");
        PlayerPrefs.DeleteKey("Trials");
       
        // Add debug logs to identify null objects
        Debug.Log($"Cleared previous PlayerPrefs data");
        
    }
    public void StartNewGameSession()
    {
        currentGameSession = new GameSession
        {
            GameName = " REACHING ASSESSMENT",
            Assessment = 1 // Example assessment value, adjust as needed
        };

        SessionManager.Instance.StartGameSession(currentGameSession);
        Debug.Log($"Started new game session with session number: {currentGameSession.SessionNumber}");
        //SetSessionDetails();
    }

    public void SetSessionDetails()
    {
        

        string device = "R5"; // Set the device name
        string assistMode = "Null"; // Set the assist mode
        string assistModeParameters = "Null"; // Set the assist mode parameters
        string deviceSetupLocation = "Null"; // Set the device setup location
        string trialDataFileLocation = assessmentclass.relativepath; // Adjust path as needed
                                                                     //string gameParameter = RetrieveFirstCirclePositions(csvFilePath); // Retrieve the last CirclePosX and CirclePosY values
       
        SaveCSVData(assessmentclass.relativepath, false, ref firstNonNullCirclePosX, ref firstNonNullCirclePosY);

        if (string.IsNullOrEmpty(firstNonNullCirclePosX) || string.IsNullOrEmpty(firstNonNullCirclePosY))
        {
            Debug.LogWarning("Circle positions are not set yet. Skipping gameParameter setting.");
        }
        else
        {
            gameParameter = $"{firstNonNullCirclePosX}{firstNonNullCirclePosY}";
            Debug.Log($"firstNonNullCirclePosX: {firstNonNullCirclePosX}");
            Debug.Log($"firstNonNullCirclePosY: {firstNonNullCirclePosY}");
            Debug.Log($"gameParameter: {gameParameter}");

        }
          SessionManager.Instance.SetGameParameter(gameParameter, currentGameSession);
       
        // Set session details
        SessionManager.Instance.SetDevice(device, currentGameSession);
        Debug.Log($"gameParameter_New: {gameParameter}");
        SessionManager.Instance.SetAssistMode(assistMode, assistModeParameters, currentGameSession);
        SessionManager.Instance.SetDeviceSetupLocation(deviceSetupLocation, currentGameSession);
        SessionManager.Instance.SetTrialDataFileLocation(trialDataFileLocation, currentGameSession);
    }
    

    public static string RetrievePreviousSessionGameParameter(GameSession currentGameSession, string circlePath)
    {
        Debug.Log(currentGameSession);
        if (currentGameSession == null)
        {
            Debug.LogError("currentGameSession is null");
            return null;
        }

        if (string.IsNullOrEmpty(circlePath))
        {
            Debug.LogError("circlePath is null or empty");
            return null;
        }
        string csvFilePath = Path.Combine(circleclass.circlePath, "Sessions.csv");

        if (!File.Exists(csvFilePath))
        {
            Debug.LogError("CSV file not found at: " + csvFilePath);
            return null;
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
            return null;
        }

        //int currentSessionNumber = currentGameSession.SessionNumber;
        string currentSessionNumber = currentGameSession.SessionNumber.ToString();
        string currentGameName = currentGameSession.GameName;

        //string lastGameParameterValue = null;


        ////for (int i = 1; i < csvLines.Length; i++)
        //for (int i = csvLines.Length - 1; i > 0; i--)
        //{
        //    string[] row = csvLines[i].Split(',');
        //    if (string.Compare(row[sessionIndex], currentSessionNumber) < 0 &&
        //        row[gameNameIndex] == currentGameName)
        //    {
        //        string gameParameter = row[gameParameterIndex];
        //        //lastGameParameterValue = row[gameParameterIndex];
        //        //break;
        //        Debug.Log("Found gameParameter: " + row[gameParameterIndex]);
        //        // return row[gameParameterIndex];
        //        return gameParameter;
        //    }
        //}
        Debug.Log("No matching session and game name found in the CSV file.");
        return null;

    }
    void OnDisable()
    {
        EndCurrentGameSession();

    }
    void EndCurrentGameSession()
    {
        
        SetSessionDetails();
        SessionManager.Instance.moveTime("0", currentGameSession);
        SessionManager.Instance.EndGameSession(currentGameSession);
       
    }
   

    void Update()
    {
       
        timer_ += Time.deltaTime;
        
       gameend_time = DateTime.Now.ToString("HH:mm:ss.fff");
       
        robdata();
        
        SaveCSVData(csvFilePath, false, ref firstNonNullCirclePosX, ref firstNonNullCirclePosY);
        // Calculate target position
        //float x2 = -((R5comm.Xp * 10 - 150) / (262.5f + 450)) * 12f;
        //float y2 = -((R5comm.Yp * 10 - 460) / (470 * 2)) * 4.8f;
        float x2 = MapXpToScreenX(R5comm.Xp);
        float y2 =MapYpToScreenZ(R5comm.Yp);
        Vector3 targetPosition = new Vector3(x2, y2, 0);

        // Smooth transition
        float smoothSpeed = 5f; // Adjust for smoother/faster movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);


    }
    public float MapXpToScreenX(float xp)
    {
        float playSizeX = xmax - xmin;

        // Map xp to screen X range
        float screenX = xmin + ((xp - romMinX) / (romMaxX - romMinX)) * playSizeX;

        // Clamp to ensure it stays within valid range
        return Mathf.Clamp(screenX, xmin - 0.2f * playSizeX, xmax+ 0.2f * playSizeX);
    }

    public float MapYpToScreenZ(float yp)
    {
        float playSizeZ = ymax - ymin;

        // Map yp to screen Z range
        float screenZ = ymin + ((yp - romMinY) / (romMaxY - romMinY)) * playSizeZ;

        // Clamp to keep it within the range
        return Mathf.Clamp(screenZ, ymin - 0.2f * playSizeZ, ymax + 0.2f * playSizeZ);
    }

    public void robdata()
    {
            PlayerPrefs.SetFloat("Enc1", R5comm.Xp);
            PlayerPrefs.SetFloat("Enc2", R5comm.Yp);
            PlayerPrefs.SetFloat("Robx", transform.position.x);
            PlayerPrefs.SetFloat("Roby", transform.position.y);
        
    }
  
    private string GetNewCsvFilePath(string baseFilePath)
    {
        string directory = Path.GetDirectoryName(baseFilePath);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(baseFilePath);
        string extension = Path.GetExtension(baseFilePath);
        string newFileName = $"{fileNameWithoutExtension}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
        return Path.Combine(directory, newFileName);
    }

    public bool SaveCSVData(string baseFilePath, bool isNewSession, ref string firstNonNullCirclePosX, ref string firstNonNullCirclePosY)
    {
       

        string header = "Time,enc_1,enc_2,Trails,Robx,Roby,PlayerX,PlayerY,CirclePosX,CirclePosY,circleRadius,TargetPos1,TargetPos2,targetRadii,CurrentStat\n";
        DateTime currentDateTime = DateTime.Now;
        string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

        // Retrieve current PlayerPrefs values
        float enc1 = PlayerPrefs.GetFloat("Enc1");
        float enc2 = PlayerPrefs.GetFloat("Enc2");
        float robX = PlayerPrefs.GetFloat("Robx");
        float robY = PlayerPrefs.GetFloat("Roby");

        string targetPos1 = PlayerPrefs.GetString("targetPos1");
        string targetPos2 = PlayerPrefs.GetString("targetPos2");
        string targetRadii = PlayerPrefs.GetString("targetRadii");
        string PlayerPosx = PlayerPrefs.GetString("PlayerX");
        string PlayerPosy = PlayerPrefs.GetString("PlayerY");
        string circlePosX = PlayerPrefs.GetString("CirclePosX");
        string circlePosY = PlayerPrefs.GetString("CirclePosY");
        string circleRadius = PlayerPrefs.GetString("CircleRadius");

        string currentStat = PlayerPrefs.GetString("Currentstat");
        int assessmentRounds = PlayerPrefs.GetInt("Trials");
        // Save the first non-null CirclePosX and CirclePosY values
        if (string.IsNullOrEmpty(firstNonNullCirclePosX) && !string.IsNullOrEmpty(circlePosX))
        {
            firstNonNullCirclePosX = circlePosX;
            Debug.Log($"First non-null CirclePosX set: {firstNonNullCirclePosX}");
        }

        if (string.IsNullOrEmpty(firstNonNullCirclePosY) && !string.IsNullOrEmpty(circlePosY))
        {
            firstNonNullCirclePosY = circlePosY;
            Debug.Log($"First non-null CirclePosY set: {firstNonNullCirclePosY}");
        }
        // Format the data string
        string data = $"{formattedDateTime},{enc1},{enc2},{assessmentRounds},{robX},{robY},{PlayerPosx},{PlayerPosy},{circlePosX},{circlePosY},{circleRadius},{targetPos1},{targetPos2},{targetRadii},{currentStat}\n";

        try
        {
            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(baseFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Initialize the CSV file with the header if it does not exist
            if (isNewSession && !File.Exists(baseFilePath))
            {
                File.WriteAllText(baseFilePath, header);
            }

            // Append the data to the CSV file
            File.AppendAllText(baseFilePath, data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save CSV data: {ex.Message}");
            return false; // Indicate failure if an exception occurs
        }

        return true; 

    }

    public void AutoData()
    {

        string GameData_Bird = Application.dataPath;
        // Directory.CreateDirectory(GameData_Bird + "\\" + "Patient_Data" + "\\" + Welcome.p_hospno);
        string filepath_Bird = GameData_Bird + "\\" + "Patient_Data" + "\\" + Welcome.p_hospno + "\\" + "gamedata.csv";
        // string filepath_Bird =  gameclass.gamePath;
        if (IsCSVEmpty(filepath_Bird))
        {

        }
        else
        {

        }

    }
    private bool IsCSVEmpty(string filepath_Bird)
    {
        int session = GameDataManager.instance.GetSessionNumber();

        string currentTime = datetime;
        string device = "R2";
        string assessment = "1";
        string starttime = start_time;
        string endtime = gameend_time;
        string gamename = " ASSESSMENT";
        string datalocation = assessmentclass.relativepath;
        string devicesetup = "null";
        string assistmode = "null";
        string assistmodeparameter = " null";
        string gameparameter = "null";

        if (File.Exists(filepath_Bird))
        {
            string Position_Bird = "";
            //check the file is empty,write header
            if (new FileInfo(filepath_Bird).Length == 0)
            {
                string Endata_Bird = "sessionnumber,datetime,device,assessment,starttime,Stoptime,gamename,traildatafilelocation,devicesetupfile,assistmode,assistmodeparameter,gameparameter\n";
                File.WriteAllText(filepath_Bird, Endata_Bird);
                DateTime currentDateTime = DateTime.Now;
                //string Position_Space = currentDateTime + "," + AppData.plutoData.enc1 + "," + AppData.plutoData.enc2 + AppData.plutoData.enc3 + "," + AppData.plutoData.enc4 + "," + AppData.plutoData.torque1 + "," + AppData.plutoData.torque3 + '\n';

                Position_Bird = session + "," + currentTime + "," + device + "," + assessment + "," + starttime + "," + endtime + "," + gamename + "," + datalocation + "," + devicesetup + "," + assistmode + "," + assistmodeparameter + "," + gameparameter + '\n';
                return true;
            }

            else
            {

                //If the file is not empty,return false
                DateTime currentDateTime = DateTime.Now;
                //string Position_SpaceR = currentDateTime + "," + AppData.plutoData.enc1 + "," + AppData.plutoData.enc2 + AppData.plutoData.enc3 + "," + AppData.plutoData.enc4 + "," + AppData.plutoData.torque1 + "," + AppData.plutoData.torque3 + '\n';

                Position_Bird = session + "," + currentTime + "," + device + "," + assessment + "," + starttime + "," + endtime + "," + gamename + "," + datalocation + "," + devicesetup + "," + assistmode + "," + assistmodeparameter + "," + gameparameter + '\n';

                File.AppendAllText(filepath_Bird, Position_Bird);
                return true;
            }
        }
        else
        {
            string PositionBird = "";
            //If the file doesnt exist
            string DataPath_Bird = Application.dataPath;
            Directory.CreateDirectory(DataPath_Bird + "\\" + "Patient_Data" + "\\" + Welcome.p_hospno);
            string filepath_Endata1_Bird = DataPath_Bird + "\\" + "Patient_Data" + "\\" + Welcome.p_hospno + "\\" + "gamedata.csv";
            string Endata1_Bird = "sessionnumber,datetime,device,assessment,starttime,Stoptime,gamename,traildatafilelocation,devicesetupfile,assistmode,assistmodeparameter,gameparameter\n";
            File.WriteAllText(filepath_Endata1_Bird, Endata1_Bird);
            DateTime currentDateTime = DateTime.Now;
            //string Position = currentDateTime + "," + AppData.plutoData.enc1 + "," + AppData.plutoData.enc2 + AppData.plutoData.enc3 + "," + AppData.plutoData.enc4 + "," + AppData.plutoData.torque1 + "," + AppData.plutoData.torque3 + '\n';
            PositionBird = session + "," + currentTime + "," + device + "," + assessment + "," + starttime + "," + endtime + "," + gamename + "," + datalocation + "," + devicesetup + "," + assistmode + "," + assistmodeparameter + "," + gameparameter + '\n';

            File.AppendAllText(filepath_Endata1_Bird, PositionBird);
            return true;
        }
    }
}

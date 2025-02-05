
using UnityEngine;
using System.Collections.Generic;
using static ChooseGame;
using System.IO;
using System;
using NeuroRehabLibrary;

public class Drawlines : MonoBehaviour
{
    public float speed;
    public float tilt;
    public Done_Boundary boundary;
    public static Drawlines instance;

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    public float nextFire;

   
    public float fireStopClock;
    public float singleFireTime;

    float max_x_init;
    float min_x_init;
    float max_y_init;
    float min_y_init;
    float startWidth = 0.1f;
    float endWidth = 0.1f;
    double x_c;
    double y_c;

    double x_value;
    double y_value;
    double x_u;
    double y_u;
    public static float Xp;
    public static float Yp;
   
    public static LineRenderer lr;
    public static List<Vector3> paths_draw;
    public static List<Vector3> paths_pass;


    private string CalliPath;
    public static string start_time;
    public static string end_time;
    public static string datetime;
    public static string relativePath;
    public static string stopTime;
    public float[] Encoder = new float[6];
    public float[] ELValue = new float[6];




    
    private GameSession currentGameSession;

    public static class calliclass
    {
        public static string calli;

    }

    void Start()
    {
        //max_x_init = -591;
        //min_x_init = 91;
        //max_y_init = -575;
        //min_y_init = 75;
       

        paths_draw = new List<Vector3>();
        paths_pass = new List<Vector3>();
        lr = GetComponent<LineRenderer>();
        lr.startWidth = startWidth;
        lr.endWidth = endWidth;

        x_c = (max_x_init + min_x_init) / 2;
        y_c = (max_y_init + min_y_init) / 2;



        string welcompath = Staticvlass.FolderPath;
        string callifile = Path.Combine(welcompath, "calibration");
        if (!Directory.Exists(callifile))
        {
            Directory.CreateDirectory(callifile);
        }
        CalliPath = Path.Combine(callifile, "calibration_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".csv");
        calliclass.calli = CalliPath;
        string fullFilePath = calliclass.calli;

        // Define the part of the path you want to store
        string partOfPath = @"Application.dataPath";

        // Use Path class to get the relative path
        relativePath = Path.GetRelativePath(partOfPath, fullFilePath);



        start_time = DateTime.Now.ToString("HH:mm:ss.fff");
        datetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff");

        // Ensure the file is ready for writing
        PrepareCSVFile(CalliPath, "Time,Encoder1,Encoder2\n");

        // CalliData();

        StartNewGameSession();

    }


    void OnDisable()
    {
        EndCurrentGameSession();


    }
    void StartNewGameSession()
    {
        currentGameSession = new GameSession
        {
            GameName = "CALIBRATION",
            Assessment = 1 // Example assessment value, adjust as needed
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
        string gameParameter = "Null"; // Set the game parameter
        string trialDataFileLocation = relativePath;
        SessionManager.Instance.moveTime("0", currentGameSession);
        SessionManager.Instance.SetDevice(device, currentGameSession);
        SessionManager.Instance.SetAssistMode(assistMode, assistModeParameters, currentGameSession);
        SessionManager.Instance.SetDeviceSetupLocation(deviceSetupLocation, currentGameSession);
        SessionManager.Instance.SetGameParameter(gameParameter, currentGameSession);
        SessionManager.Instance.SetTrialDataFileLocation(trialDataFileLocation, currentGameSession);

    }

    void EndCurrentGameSession()
    {
        if (currentGameSession != null)
        {
           
            SessionManager.Instance.EndGameSession(currentGameSession);
        }
    }

    
    private void PrepareCSVFile(string path, string header)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, header);
        }
    }


   
    void Update()
    {
        Time.timeScale = 1;
        SensorValue();
        // Debug.Log(AppData.plutoData.motorCurrent1 == 0 ? "pREESSED" : "nOtPressed");
        end_time = DateTime.Now.ToString("HH:mm:ss.fff");
        drawLine();
    }


    private void SensorValue()
    {

        ELValue[0] = R5comm.x;
        ELValue[1] = R5comm.y;

        Encoder = new float[] { R5comm.Xp, R5comm.Yp };

        string data = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff},{Encoder[0]},{Encoder[1]}\n";
        File.AppendAllText(calliclass.calli, data);

    }

    void drawLine()
    {

        x_value = R5comm.Xp;
        y_value = R5comm.Yp;
        //Xp = (float.Parse(JediSerialPayload.data[0].ToString()));
        //Yp = (float.Parse(JediSerialPayload.data[1].ToString()));

        //x_value = Xp;
        //y_value = Yp;

        //Adjust the coordinate transformation unity draw area size size x is 8.6 cm and y is 4
        x_u = -(((x_value - 15) / 86) * 8.6 * 2); // Adjust the x transformation
        y_u = -(((y_value - 40) / 80) * 8f);  // Adjust the y transformation


        Debug.Log("x: pos" + x_value + "y:" + y_value);




        // Create the draw and pass vectors
        Vector3 to_draw_values = new Vector3((float)x_u, (float)y_u, 0.0f);
        Vector3 to_pass_values = new Vector3((float)x_value, (float)y_value, 0.0f);

        // Add the vectors to the lists
        paths_draw.Add(to_draw_values);
        paths_pass.Add(to_pass_values);

        // Update the LineRenderer
        lr.positionCount = paths_draw.Count;
        lr.SetPositions(paths_draw.ToArray());
        lr.useWorldSpace = true;



    }
//    void drawLine()
//    {
//        x_value = R5comm.Xp;
//        y_value = R5comm.Yp;

//        // Adjust coordinate transformation
//        x_u = -(((x_value - 15) / 86) * 8.6f * 2); // Adjust x transformation
//        y_u = -(((y_value - 40) / 80) * 8f);       // Adjust y transformation

//        Debug.Log("x: pos " + x_value + " y: " + y_value);

//        // Create smoothed interpolation for the draw path
//        Vector3 to_draw_values = new Vector3((float)x_u, (float)y_u, 0.0f);
//        Vector3 to_pass_values = new Vector3((float)x_value, (float)y_value, 0.0f);

//        // Ensure a smooth transition by interpolating
//        if (paths_draw.Count > 0)
//        {
//            Vector3 lastPoint = paths_draw[paths_draw.Count - 1];

//            // Add interpolated points for smoother movement
//            float smoothnessFactor = 10; // Number of interpolated points
//            for (int i = 1; i <= smoothnessFactor; i++)
//            {
//                float t = i / smoothnessFactor;
//                Vector3 interpolatedPoint = Vector3.Lerp(lastPoint, to_draw_values, t);
//                paths_draw.Add(interpolatedPoint);
//            }
//        }

//        // Add the latest point
//        paths_draw.Add(to_draw_values);
//        paths_pass.Add(to_pass_values);

//        // Update LineRenderer with the smoothed paths
//        lr.positionCount = paths_draw.Count;
//        lr.SetPositions(paths_draw.ToArray());
//        lr.useWorldSpace = true;

//        // Adjust line rendering properties for smoother drawing
//        lr.widthMultiplier = 0.1f; // Adjust thickness
//        lr.numCapVertices = 5; // Makes line ends rounded for smoother effect
//    }


}














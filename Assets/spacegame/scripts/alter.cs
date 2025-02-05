//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using System.IO.Ports;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Runtime;
//using System.IO;
//using System;
//using System.Diagnostics;
//using Debug = UnityEngine.Debug;
//using System.Text.RegularExpressions;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using Random = UnityEngine.Random;
//using UnityEngine.SceneManagement;

//public class alter : MonoBehaviour
//{
//    public float speed = 2f; // Speed of the player movement
//    public float boundaryXMin = -30f; // Minimum X boundary
//    public float boundaryXMax = 30f; // Maximum X boundary
//    public float boundaryZMin = -9f; // Minimum Z boundary
//    public float boundaryZMax = 9f; // Maximum Z boundary

//    public float externalToUnityScale = 1.0f;
    






//    public static R5comm instance;
//    public static JediSerialCom serReader;

//    public float theta1;
//    public float theta2;

//    public float l1 = 333;
//    public float l2 = 381;


//    [SerializeField]
//    public Text textResults;


//    public static string Date;
//    public static string name_sub_;
//    public static string date_;
//    public static string Path;
//    public static string filePath;
//    public static string folderpath;
//    public static float timer;
//    public string[] portnames;
//    public static string savepath;
//    public InputField Hos_;

//    public static float timer_;

//    // Start is called before the first frame update
//    void Start()
//    {


//        JediDataFormat.ReadSetJediDataFormat(AppData.jdfFilename);
//        serReader = new JediSerialCom("COM10");
//        serReader.ConnectToArduino();

//        Date = System.DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy HH-mm-ss");

//    }

   

//    // Update is called once per frame
//    void Update()
//    {
//        // Read theta1 and theta2 from external source
//        float theta1 = ReadTheta1FromExternalSource();
//        float theta2 = ReadTheta2FromExternalSource();

//        // Convert external coordinates to Unity game scene coordinates
//        Vector3 unityPosition = ConvertExternalToUnityCoordinates(theta1, theta2);

//        // Move the object to the calculated position
//        MoveObject(unityPosition);

//        // Handle player input for movement
//        HandlePlayerMovement();
//    }

//    float ReadTheta1FromExternalSource()
//    {

//        theta1 = (float.Parse(JediSerialPayload.data[0].ToString()));
//        // Implement the logic to read theta1 from the external source
//        // For example:
//        // return someValueFromExternalSource;
//        return 0f; // Placeholder value
//    }

//    float ReadTheta2FromExternalSource()
//    {


//        theta2 = (float.Parse(JediSerialPayload.data[1].ToString()));
//        // Implement the logic to read theta2 from the external source
//        // For example:
//        // return someValueFromExternalSource;
//        return 0f; // Placeholder value
//    }

//    Vector3 ConvertExternalToUnityCoordinates(float theta1, float theta2)
//    {
//        // Convert thetas to radians
//        float theta1Radians = theta1 * Mathf.Deg2Rad;
//        float theta2Radians = (theta1 + theta2) * Mathf.Deg2Rad;

//        // Calculate x and y using trigonometric functions
//        float x = Mathf.Cos(theta1Radians) * l1 + Mathf.Cos(theta2Radians) * l2;
//        float y = Mathf.Sin(theta1Radians) * l1 + Mathf.Sin(theta2Radians) * l2;

//        // Scale the coordinates according to externalToUnityScale
//        Vector3 externalCoordinates = new Vector3(x, y, 0f);
//        return externalCoordinates * externalToUnityScale;
//    }

//    void MoveObject(Vector3 newPosition)
//    {
//        // Move the object to the new position
//        transform.position = newPosition;

//        // Clamp the object's position within the boundaries
//        Vector3 clampedPosition = transform.position;
//        clampedPosition.x = Mathf.Clamp(clampedPosition.x, boundaryXMin, boundaryXMax);
//        clampedPosition.z = Mathf.Clamp(clampedPosition.z, boundaryZMin, boundaryZMax);
//        transform.position = clampedPosition;
//    }

//    void HandlePlayerMovement()
//    {
//        // Handle player movement input
//        float moveHorizontal = Input.GetAxis("Horizontal");
//        float moveVertical = Input.GetAxis("Vertical");

//        // Calculate movement
//        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
//        transform.Translate(movement * speed * Time.deltaTime);
//    }


//    public void start_data_log()
//    {
//        string Hosp_number = PlayerPrefs.GetString("Hospital Number");
//        string Game_name = PlayerPrefs.GetString("Game name");
//        string Mech_name = PlayerPrefs.GetString("Mechanism");
//        string pth = "D:\\HypercubeData\\";
//        string total_path = pth + Hosp_number + "-" + Game_name + "-" + Mech_name;
//        folderpath = total_path;
//        Directory.CreateDirectory(total_path);
//        string filename = total_path + "\\" + Hosp_number + Game_name + Mech_name + AppData.dataFolder + DateTime.UtcNow.ToLocalTime().ToString("yy-MM-dd-HH-mm-ss") +
//             "-" + ".csv";
//        savepath = filename;
//        PlayerPrefs.SetString("data", filename);
//        //saver(total_path);
//        AppData.dlogger = new DataLogger(filename, "");

//        if (!File.Exists(filename))
//        {
//            //Debug.Log(filename);
//            /*****header setting***/
//            string clientHeader = $"\"theta1\",\"theta1\",{Environment.NewLine}";



//            File.WriteAllText(filename, clientHeader);

//            //start.GetComponentInChildren<Text>().text = "STARTED";
//        }
//        //if (timer_>= 10f)
//        //{
//        //AppData.dlogger.stopDataLog();
//        //Debug.Log("dataStopped");
//        //}

//    }

//    public void Stop_data_log()
//    {
//        AppData.dlogger.stopDataLog();
//        Debug.Log("dataStopped");
//    }

//    public void DconnectToArduino()
//    {
//        serReader.DisconnectArduino();
//    }

//}

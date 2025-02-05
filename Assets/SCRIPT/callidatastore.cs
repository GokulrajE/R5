using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public static class calliclass
{
    public static string calli;

}
public class callidatastore : MonoBehaviour
{



    public static LineRenderer lr;
    public static List<Vector3> paths_draw;
    public static List<Vector3> paths_pass;
    private string CalliPath;
    public float[] Encoder = new float[6];
    public float[] ELValue = new float[6];
    public static string start_time;
    public static string end_time;
    public static string datetime;
    public static string relativePath;
    public static string stopTime;

    // Start is called before the first frame update
    void Start()
    {
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

        CalliData();
    }



    private void PrepareCSVFile(string path, string header)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, header);
        }
    }



    public void CalliData()
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

        string endtime = ChooseGame.end_time;


        //string endtime = end_time;
        string gamename = "CALIBRATION";
        string datalocation = relativePath;
        string devicesetup = "null";
        string assistmode = "null";
        string assistmodeparameter = "null";
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


    private void SensorValue()
    {


        //float l1 = 333;
        //float l2 = 381;

        float Xp = Drawlines.Xp;
        float Yp = Drawlines.Yp;


        //float thetaa = theta1 * Mathf.Deg2Rad;
        //float thetab = theta2 * Mathf.Deg2Rad;

        //float y1 = -(Mathf.Cos((thetaa)) * l1 + Mathf.Cos((thetaa + thetab)) * l2);
        //float x1 = -(Mathf.Sin((thetaa)) * l1 + Mathf.Sin((thetaa + thetab)) * l2);


        float x = ((Xp * 10) / (262.5f + 450) * 12f);
        float y = ((Yp * 10 + 350) / (500 * 2)) * 4.8f;
        //float x = ((x1) / (333 + 381) * 12f);
        //float y = ((y1 + 350) / (400 * 2)) * 4.8f;
        //ELValue[0] = R5comm.theta1;
        //ELValue[1] = R5comm.theta2;


        ELValue[0] = x;
        ELValue[1] = y;


        Encoder = new float[] { Xp, Yp };

        string data = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff},{Encoder[0]},{Encoder[1]}\n";
        File.AppendAllText(CalliPath, data);

    }
    // Update is called once per frame
    void Update()
    {
        SensorValue();
    }
}

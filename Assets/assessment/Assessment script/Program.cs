using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Program : MonoBehaviour
{
    
    float enc_1,enc_2;
    float Rob_X, Rob_Y;
    string TargetPos, CurrentStat;
    void Start()
    {

    }
    void Update()
    {
        enc_1 = PlayerPrefs.GetFloat("Enc1");
        enc_2 = PlayerPrefs.GetFloat("Enc2");
        Rob_X = PlayerPrefs.GetFloat("Robx");
        Rob_Y = PlayerPrefs.GetFloat("Roby");
        TargetPos = PlayerPrefs.GetString("targetPos");
        CurrentStat = PlayerPrefs.GetString("Currentstat");
        robot_data();
    }

    public void robot_data()
    {
        
        string DataPath = Application.dataPath;
        Directory.CreateDirectory(DataPath + "\\" + "Rob_Data");
        string filepath_Endata = DataPath + "\\" + "Rob_Data" + "\\" + "robo data.csv";
        if (IsCSVEmpty(filepath_Endata))
        {

        }
        else
        {

        }
    }

    private bool IsCSVEmpty(string filepath_Endata)
    {
       
        if (File.Exists(filepath_Endata))
        {
            //check the file is empty,write header
            if (new FileInfo(filepath_Endata).Length == 0)
            {
                string Endata = "Time,enc_1, enc_2,Rob_X,Rob_Y,TargetPos,CurrentStat\n";
                File.WriteAllText(filepath_Endata, Endata);
                DateTime currentDateTime = DateTime.Now;
                string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string data = $"{formattedDateTime},{enc_1},{enc_2},{Rob_X},{Rob_Y},{TargetPos},{CurrentStat}\n";
                return true;
            }
            else
            {
                //If the file is not empty,return false
                DateTime currentDateTime = DateTime.Now;
                string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string data = $"{formattedDateTime},{enc_1},{enc_2},{Rob_X},{Rob_Y},{TargetPos},{CurrentStat}\n";

                File.AppendAllText(filepath_Endata, data);
                return false;
            }
        }
        else
        {
            //If the file doesnt exist
            string DataPath = Application.dataPath;
            Directory.CreateDirectory(DataPath + "\\" + "Rob_data" + "\\");
            string filepath_Endata1 = DataPath + "\\" + "Rob_Data" + "\\" + "\\" + "robo data.csv";
            string Endata = "Time,enc_1, enc_2,Rob_X,Rob_Y,TargetPos,CurrentStat\n";
            File.WriteAllText(filepath_Endata, Endata);
            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string data = $"{formattedDateTime},{enc_1},{enc_2},{Rob_X},{Rob_Y},{TargetPos},{CurrentStat}\n";
            File.AppendAllText(filepath_Endata1, data);
            return true;
        }
    }
}

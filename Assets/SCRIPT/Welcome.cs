
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;
using System;
using NeuroRehabLibrary;

public class Welcome : MonoBehaviour
{
    public TMP_InputField hospno;
    public TMP_Text invalidText;
    public TMP_Text nameText;
    public TMP_Text timeRemainingText;
    public TMP_Text TodaysDateText;
    public GameObject nextButton;
    public static string p_hospno;
    public static string newDirPath;
    public static string finalpath;
    public GameObject welcomePanel;
    public GameObject loginPanel;
    public TextMeshProUGUI[] prevDays = new TextMeshProUGUI[7];
    public TextMeshProUGUI[] prevDates = new TextMeshProUGUI[7];
    public Image[] pies = new Image[7];
    public bool piChartUpdated = false;
    private DaySummary[] daySummaries;
    private SessionManager sessionManager;

    void Start()
    {
        invalidText.text = "";
        AppData.initializeRobot();
    }

    void Update() { 
    
      
    }

    public void signup()
    {
        SceneManager.LoadScene("Register");
    }

    public void onCLickQuit()
    {
        SceneManager.LoadScene("summaryScene");
    }
    public void next()
    {
       
        SceneManager.LoadScene("Calibration");
    }
    private void UpdatePieChart()
    {
        int N = daySummaries.Length;
        for (int i = 0; i < N; i++)
        {
            prevDays[i].text = daySummaries[i].Day;
            prevDates[i].text = daySummaries[i].Date;
            pies[i].fillAmount = daySummaries[i].MoveTime / AppData.PrescribedTime;
            pies[i].color = new Color32(148, 234, 107, 255);
        }
        piChartUpdated = true;
    }
    public void login()
    {
        p_hospno = hospno.text;
        bool hospno_check = string.IsNullOrEmpty(p_hospno);
        if (hospno_check == true )
        {
            Debug.Log("Empty hospno"+".. it is not connected with the robot");
            invalidText.text = "Enter Hos_NO..";
        }
        else
        {
            string path_to_data = Application.dataPath;

            if (!Directory.Exists(path_to_data + "\\" + p_hospno))
            {
                string patientDir = path_to_data + "\\" + "Patient_Data" + "\\" + p_hospno;

                circleclass.circlePath = patientDir;
                CheckForPatientDirectory(patientDir);
               
            }
        }
    }
    public void CheckForPatientDirectory(string patientDir)
    {
        if (Directory.Exists(patientDir))
        {
            // Read patient data
            string patientJson = File.ReadAllText(patientDir + "\\patient.json");
            var patient = JsonConvert.DeserializeObject<patient>(patientJson);
            Staticvlass.CrossSceneInformation = patient.name + "," + patient.hospno;

            string dateTimeNow = DateTime.Now.ToString("dd-MM-yyyy");
            string newDirPath = Path.Combine(patientDir, dateTimeNow);

            if (Directory.Exists(newDirPath))
            {
                Staticvlass.FolderPath = newDirPath;
            }
            else
            {
                Directory.CreateDirectory(newDirPath);
                Staticvlass.FolderPath = newDirPath;
            }
            loginPanel.SetActive(false);
            nameText.text = "--" + patient.name + "[" + patient.hospno + "]";
            afterLogin();

        }
        else
        {
            invalidText.text = "user Detials Not found";
            Debug.Log("Hospital Number Does not exist");
        }
    }
    public void afterLogin()
    {
      
        welcomePanel.SetActive(true);
        AppData.UserData.readAllUserData();
        daySummaries = AppData.UserData.daySummary;
        TodaysDateText.text = DateTime.Now.ToString("ddd, dd-MM-yyyy");
        timeRemainingText.text = (AppData.PrescribedTime - AppData.UserData.MovetimeCurr).ToString();
        UpdatePieChart();
        nextButton.SetActive(true);
        SessionManager.Instance.Login();
    }
}

public static class circleclass
{
    public static string circlePath;
    public static string sessionpath;
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using NeuroRehabLibrary;

public class ChooseGame : MonoBehaviour
{
    public static ChooseGame instance;
    public float[] Encoder = new float[6];
    public float[] ELValue = new float[6];
    public float max_x;
    public float min_x;
    public float max_y;
    public float min_y;
    int tocarry;
    int Asses;

    private SessionManager sessionManager;
    private GameSession currentGameSession;

    private string CalliPath;
    List<Vector3> paths;

    public static string end_time;

    // private string CalliPath;


    public static class calliclass
        {

          public static string callipath;
        }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
      
    }

    void Update()
    {
     
    }

    //Rest of the class...
    public void onclick_assessment()
    {
        SceneManager.LoadScene("Assessment");
       
    }
    public void onclick_SpaceShooter()
    {
        tocarry = 1;
    }

    public void onclick_Auto()
    {
        tocarry = 2;
    }

    public void onclick_Pingpong()
    {
        tocarry = 3;
    }

  
    public void onclick_game()
    {
        if (tocarry == 1)
        {
            SceneManager.LoadScene("SpaceShooterDemo");
            paths = Drawlines.paths_pass;

            max_x = (paths.Max(v => v.x));
            min_x = (paths.Min(v => v.x));
            max_y = (paths.Max(v => v.y));
            min_y = (paths.Min(v => v.y));
        }

        else if (tocarry == 2)
        {
            SceneManager.LoadScene("FlappyGame");

            paths = new List<Vector3>();
            paths = Drawlines.paths_pass;

            max_y = paths.Max(v => v.y);
            min_y = paths.Min(v => v.y);
            PlayerPrefs.SetFloat("y max", max_y);
            PlayerPrefs.SetFloat("y min", min_y);

        }
        else if (tocarry == 3)
        {


            SceneManager.LoadScene("pong_menu");
            paths = Drawlines.paths_pass;

            max_y = paths.Max(v => v.y);
            min_y = paths.Min(v => v.y);
            PlayerPrefs.SetFloat("y max", max_y);
            PlayerPrefs.SetFloat("y min", min_y);


        }

       

    }
   
    public void onclick_drawpath()
    {
        SceneManager.LoadScene("Calibration");
    }
   
    public void quit() {
        if(AppData.UserData.dTableSession != null)
        {
            SceneManager.LoadScene("summaryScene");
        }
        else
        {
            AppData.Quit();
        }
        
    }
   
  
}

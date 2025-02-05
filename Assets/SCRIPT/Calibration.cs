using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Calibration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void onclickChooseGame()
    {
        SceneManager.LoadScene("ChooseGame");
    }

    public void onclick_recalibrate()
    {
        SceneManager.LoadScene("Calibration");
        //StartNewGameSession();

    }
    public void quit()
    {
        // check for sessions Data To calculate the MoveTime
        if (AppData.UserData.dTableSession != null)
        {
            SceneManager.LoadScene("summaryScene");
        }
        else
        {
            AppData.Quit();
        }
    }

}

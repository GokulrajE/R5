using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime;
using System.IO;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class R5comm : MonoBehaviour
{

    public static R5comm instance;
    public static JediSerialCom serReader;

    public static float Xp, x;// XP - posisiton of x , x coordinate of the real board
    public static float Yp, y;// YP - posisiton of y , y coordinate of the real board

    public static string Date;
    public static float timer_;
  
    void Awake()
    {
        instance = this;
    }
    void Start()
    {

        Date = System.DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy HH-mm-ss");

    }

    void Update()
    {

        parseByteArray();
    }
    public  static void  parseByteArray()
    {
       
        timer_ += Time.deltaTime;
        if ((JediSerialPayload.data.Count == 2))
        {
            try
            {
                // torque = (float.Parse(JediSerialPayload.data[0].ToString()));
                Xp = (float.Parse(JediSerialPayload.data[0].ToString()));
                Yp = (float.Parse(JediSerialPayload.data[1].ToString()));
                // error = (float.Parse(JediSerialPayload.data[3].ToString()));

                Debug.Log(Xp+ "/ " + Yp +"pos");

                x = (float)-(((Xp - 15) / 86) * 8.6 * 2); // Adjust the x transformation
                y = -(((Yp - 40) / 80) * 8f);

            }
            catch (System.Exception)
            {

            }

        }

      

       
    }
  
   

}











































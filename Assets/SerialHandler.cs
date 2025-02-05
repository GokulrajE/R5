using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using PlutoDataStructures;
using System.IO.Ports;

public class SerialHandler : MonoBehaviour {

    //public Text statusText;
    //public Text sensDataText;
    //public Text errDataText;
    //public Text MechSelText;
    //public Dropdown mechList;
    //public Dropdown comPorts;
    //static public SerialPort serPort;

    private StringBuilder sb = new StringBuilder();
    //private int[] _count;
    //private int[] _Th = new int[] { 10, 50, };

    public float stopClock = 0;

    public int n = 0;

    float randomValue =-9;
    // Use this for initialization
    void Awake () {
       
    }
	
	// Update is called once per frame
	void Update () {
        stopClock += Time.deltaTime;


        }
       
        
    }

 

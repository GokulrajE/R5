
//NEW SCRIPT

using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO.Ports;
using System.Threading;

public class SerialPortManager : MonoBehaviour
{
    public GameObject buttonTemplate; // Reference to the Button template in the Content
    public Transform contentTransform; // Reference to the Content Transform

    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning;
    private string latestData;
    public static SerialPortManager Instance;

    void Awake()
    {
        // Singleton pattern to ensure only one instance of SerialPortManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Get the available serial COM ports
        string[] portNames = SerialPort.GetPortNames();

        // Check if there are any available COM ports
        if (portNames.Length > 0)
        {
            foreach (string port in portNames)
            {
                // Create a new Button element for each COM port
                GameObject buttonInstance = Instantiate(buttonTemplate, contentTransform);
                buttonInstance.GetComponentInChildren<Text>().text = port;
                buttonInstance.SetActive(true);

                // Add click event to the button
                Button button = buttonInstance.GetComponent<Button>();
                string selectedPort = port; // Local copy for the closure
                button.onClick.AddListener(() => OnComPortClick(selectedPort));
            }
        }
        else
        {
            // Create a new Button element indicating no COM ports are available
            GameObject buttonInstance = Instantiate(buttonTemplate, contentTransform);
            buttonInstance.GetComponentInChildren<Text>().text = "No COM ports available.";
            buttonInstance.GetComponent<Button>().interactable = false; // Disable button interaction
            buttonInstance.SetActive(true);
        }
    }

    void OnComPortClick(string port)
    {
        PlayerPrefs.SetString("SelectedCOMPort", port);
        Debug.Log("Selected COM Port: " + port);
        //InitializeSerialPort(port);
    }
}


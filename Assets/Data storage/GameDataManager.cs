using System;
using System.IO;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    private string sessionDirectory;
    private string sessionFilePath;
    public static GameDataManager instance;
    private bool sessionIncremented; // Flag to ensure session number is incremented only once per login

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Set the directory and file path
            sessionDirectory = circleclass.circlePath;
            sessionFilePath = Path.Combine(sessionDirectory, "sessiondata.txt");
            Debug.Log("Session file path: " + sessionFilePath);

            sessionIncremented = false; // Initialize the flag
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (IsSessionFileEmpty(sessionFilePath))
        {
            Debug.Log("Session file is empty.");
        }
        else
        {
            Debug.Log("Session file is not empty.");
        }
    }

    public void IncrementSessionNumber()
    {
        if (!sessionIncremented) // Check if the session number has already been incremented in this session
        {
            Debug.Log("Incrementing session number...");
            int session = GetSessionNumber();
            session++;
            SaveSessionNumber(session);
            sessionIncremented = true; // Set the flag to true to prevent further increments in this session
            Debug.Log("Incremented session number to: " + session);
        }
    }

    public int GetSessionNumber()
    {
        int session = 0;
        if (File.Exists(sessionFilePath))
        {
            Debug.Log("Session file exists.");
            string sessionStr = File.ReadAllText(sessionFilePath);
            if (int.TryParse(sessionStr, out int lastSession))
            {
                session = lastSession;
            }
            else
            {
                Debug.LogError("Failed to parse session number.");
            }
        }
        else
        {
            Debug.Log("Session file does not exist. Returning default session number 1.");
        }
        Debug.Log("Current session number: " + session);
        return session;
    }

    private void SaveSessionNumber(int session)
    {
        try
        {
            if (!Directory.Exists(sessionDirectory))
            {
                Debug.Log("Creating session directory: " + sessionDirectory);
                Directory.CreateDirectory(sessionDirectory);
            }

            File.WriteAllText(sessionFilePath, session.ToString());
            Debug.Log("Saved session number to file: " + sessionFilePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save session number: " + ex.Message);
        }
    }

    private bool IsSessionFileEmpty(string filePath)
    {
        if (File.Exists(filePath))
        {
            if (new FileInfo(filePath).Length == 0)
            {
                Debug.Log("Session file exists but is empty.");
                return true;
            }
            else
            {
                Debug.Log("Session file exists and is not empty.");
                return false;
            }
        }
        else
        {
            Debug.Log("Session file does not exist.");
            return true;
        }
    }
}

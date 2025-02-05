
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NeuroRehabLibrary
{
    public class SessionManager
    {
        private static SessionManager _instance;
        public static SessionManager Instance => _instance ??= new SessionManager(circleclass.circlePath);

        private int _currentSessionNumber;
        private bool _sessionStarted;
        private DateTime _sessionDateTime;
        private GameSession _currentSession;
        private readonly string _sessionFilePath;
        private bool _loginCalled; // Track if login has been called once
        private readonly string csvFilePath;
        private readonly Dictionary<int, string> _gameParameters = new Dictionary<int, string>();
        private SessionManager(string baseDirectory)
        {
            _sessionFilePath = Path.Combine(baseDirectory, "Sessions.csv");

            // Ensure the base directory exists
            Directory.CreateDirectory(baseDirectory);

            // Ensure the Sessions.csv file has headers if it doesn't exist
            csvFilePath = _sessionFilePath;
            if (!File.Exists(csvFilePath))
            {
                using (var writer = new StreamWriter(csvFilePath, false, Encoding.UTF8))
                {
                    writer.WriteLine("SessionNumber,DateTime,Device,Assessment,StartTime,StopTime,GameName,TrialDataFileLocation,DeviceSetupLocation,AssistMode,AssistModeParameters,GameParameter,MoveTime");
                }
                Debug.Log("Initialized SessionManager with session number: 0");
            }
            else
            {
                _currentSessionNumber = GetLastSessionNumber();
                Debug.Log($"Initialized SessionManager with session number: {_currentSessionNumber}");
            }

            _loginCalled = false; // Initialize login called flag
        }

        public void Login()
        {
            if (!_loginCalled)
            {
                _currentSessionNumber = GetLastSessionNumber() + 1;
                Debug.Log($"Session number incremented to: {_currentSessionNumber}");

                _loginCalled = true;
            }
            _sessionStarted = false;
        }

        public void StartGameSession(GameSession session)
        {
            if (!_sessionStarted)
            {
                _sessionStarted = true;
                _sessionDateTime = DateTime.Now;
            }

            session.SessionNumber = _currentSessionNumber;
            session.DateTime = _sessionDateTime;
            session.SetStartTime();
            _currentSession = session;

            Debug.Log($"Starting game session with session number: {session.SessionNumber}");

        }

        public void EndGameSession(GameSession session)
        {
            if (session != null && _sessionStarted)
            {
                session.SetStopTime();
                WriteSession(session);
                _sessionStarted = false; // End the current game session

                Debug.Log($"Ending game session with session number: {session.SessionNumber}");

            }
        }

        public void SetDevice(string device, GameSession session)
        {
            if (session != null)
            {
                session.Device = device;
            }
        }
        public void moveTime(string moveTime, GameSession session)
        {
            if (session != null)
            {
                session.moveTime = moveTime;
            }
        }

        public void SetAssistMode(string assistMode, string assistModeParameters, GameSession session)
        {
            if (session != null)
            {
                session.AssistMode = string.IsNullOrEmpty(assistMode) ? "None" : assistMode;
                session.AssistModeParameters = string.IsNullOrEmpty(assistModeParameters) ? "None" : assistModeParameters;
            }
        }

        public void SetDeviceSetupLocation(string location, GameSession session)
        {
            if (session != null)
            {
                session.DeviceSetupLocation = location;
            }
        }

        private int GetLastSessionNumber()
        {
            if (!File.Exists(csvFilePath))
            {
                return 0;
            }

            var lastLine = File.ReadLines(csvFilePath).LastOrDefault();
            if (lastLine == null || lastLine.StartsWith("SessionNumber"))
            {
                return 0;
            }

            var fields = lastLine.Split(',');
            return int.TryParse(fields[0], out var sessionNumber) ? sessionNumber : 0;
        }

        public void SetGameParameter(string gameParameter, GameSession session)
        {
            if (session != null)
            {
                //Debug.Log(typeof(gameParameter));
                session.GameParameter = string.IsNullOrEmpty(gameParameter) ? "None" : gameParameter;
                //_gameParameters[session.SessionNumber] = session.GameParameter;
                Debug.Log($"Set game parameter for session {session.SessionNumber}: {session.GameParameter}");
            }
        }
      
        public void SetTrialDataFileLocation(string path, GameSession session)
        {
            if (session != null)
            {
                session.TrialDataFileLocation = path;
            }
        }



        private void WriteSession(GameSession session)
        {
            using (var writer = new StreamWriter(csvFilePath, true, Encoding.UTF8))
            {
                var csvLine = string.Join(",",
                    session.SessionNumber,
                    session.DateTime.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    session.Device,
                    session.Assessment,
                    session.StartTime.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    session.StopTime?.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty,
                    session.GameName,
                    session.TrialDataFileLocation,
                    session.DeviceSetupLocation,
                    session.AssistMode,
                    session.AssistModeParameters,
                    session.GameParameter,
                    session.moveTime);

                writer.WriteLine(csvLine);
            }
        }
    }

    public class GameSession
    {
        public int SessionNumber { get; set; }
        public DateTime DateTime { get; set; }
        public string Device { get; set; }
        public int Assessment { get; set; }
        public DateTime StartTime { get; private set; }
        public DateTime? StopTime { get; private set; }
        public string GameName { get; set; }
        public string TrialDataFileLocation { get; set; }
        public string DeviceSetupLocation { get; set; }
        public string AssistMode { get; set; }
        public string AssistModeParameters { get; set; }
        public string GameParameter { get; set; }
        public string moveTime { get; set; }

        public void SetStartTime()
        {
            StartTime = DateTime.Now;
        }

        public void SetStopTime()
        {
            StopTime = DateTime.Now;
        }
    }
}

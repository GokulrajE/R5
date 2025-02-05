using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using Random = UnityEngine.Random;
using static UnityEngine.GraphicsBuffer;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.UI; // Add this line to access UI components
using NeuroRehabLibrary;
using TMPro;


public enum State
{
    // None = -1, // Placeholder state
    TargetAppeared = 0,
    reachingtarget = 1,//deactivating circle(circle disappears)
    targetreached = 2,
    homepositionappears = 3,//circle appears
    reachinghome = 4,//deactivating target
    reachedhome = 5,

}

public static class store
{
    public static string storex;
    public static string storey;
}

public class CircleColorChanger : MonoBehaviour
{
   
    public static CircleColorChanger instance;

    public Player playerScript; // Reference to the Player script
    public GameObject circlePrefab;
    public GameObject targetPrefab;
    public TMP_Text messageText;
    
    private GameSession currentGameSession;

    public Transform PlayerPosition;

    public KeyCode spawnKey = KeyCode.Space;

    public float delayInSeconds = 1f;
    public int numberOfTargets = 7;
    public float distancefromhome = 30f;
    public Color insideColor = Color.red;
    public Color leavingColor = Color.green;
    public Color stayingColor = Color.yellow;
    public float detectionradius = 0.55f;
    private GameObject spawnedCircle;
    private GameObject[] spawnedTargets;
    private int currentTargetIndex = 0;
    private bool isPlayerInsideCircle = false;
    private bool isPlayerInsideTarget = false;
    public float targetresttime = 1f;
    public float circleresttime = 3f;
    private float timeInside = 0f;

    private bool isSpawningTarget = false;
    private bool isSpawningCircle = false;
    private bool circleAppeared = false; // Track if the circle has appeared

    private State currentState;
    private bool shouldStopStateMachine = false; // Flag to indicate if the state machine should stop
    public Text moveBackText; // Reference to the UI text element
    float enc_1, enc_2;
    float Rob_X, Rob_Y;
    string TargetPosx, TargetPosy, CurrentStat, PlayerPosx, PlayerPosy, CirclePositionX, CirclePositionY, targetRadii, circleRadiusStr;
    // Add class variables to track time
    private float circleActivationTime;
    private float circleTotargetDuration;
    private float targetReachedTime;
    private float targetToCircleDuration;
    private int assessmentRounds = 0;
    int totalRounds = 3; // Total number of assessment rounds including the initial one
    public float delayBeforeNextAssessment = 6.0f; // Adjust as needed
    public bool isAssessmentRunning = false;
    public Text roundCompletionText;
    public float textDisplayDelay = 5f; // Delay in seconds before hiding the text
    public Text assessmentoverText;
    public AudioSource assessmentAudio; // Reference to the AudioSource component
    //public AudioSource moveBackAudio; // Reference to the AudioSource component for move back text
    public Text ssessmentoverText; // Reference to the UI Text element for "Assessment Over"
    // Define a List to store the positions of the targets spawned in the first round
    List<Vector2> targetPositionsFirstRound = new List<Vector2>();
    
    void Start()
    {

        // Get reference to PlayerScript
        playerScript = FindObjectOfType<Player>();

        if (playerScript == null)
        {
            Debug.LogError("PlayerScript not found!");
        }
        StartCoroutine(WaitForSKeyPress());
        //currentState = State.WaitingForInput;
        // Clear previous PlayerPrefs data
        
        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY");
        PlayerPrefs.DeleteKey("targetPos1");
        PlayerPrefs.DeleteKey("targetPos2");
        PlayerPrefs.DeleteKey("targetRadii");
        PlayerPrefs.DeleteKey("CirclePosX");
        PlayerPrefs.DeleteKey("CirclePosY");
        PlayerPrefs.DeleteKey("CircleRadius");
        PlayerPrefs.DeleteKey("Currentstat");
        PlayerPrefs.DeleteKey("Trials");
        //enc_1 = PlayerPrefs.GetFloat("Enc1");
        //enc_2 = PlayerPrefs.GetFloat("Enc2");
        //Rob_X = PlayerPrefs.GetFloat("Robx");
        //Rob_Y = PlayerPrefs.GetFloat("Roby");
        //TargetPosx = PlayerPrefs.GetString("targetPos1");
        //TargetPosy = PlayerPrefs.GetString("targetPos2");
        //CurrentStat = PlayerPrefs.GetString("Currentstat");
        //PlayerPosx = PlayerPrefs.GetString("PlayerX");
        //PlayerPosy = PlayerPrefs.GetString("PlayerY");
        //CirclePositionX = PlayerPrefs.GetString("CirclePosX");
        //CirclePositionY = PlayerPrefs.GetString("CirclePosY");
        //targetRadii = PlayerPrefs.GetString("targetRadii");
        //circleRadiusStr = PlayerPrefs.GetString("circleRadius");
        assessmentoverText.gameObject.SetActive(false); // Disable the "Assessment Over" text

        currentGameSession = new GameSession
        {
            GameName = " REACHING ASSESSMENT",
            Assessment = 1 // Example assessment value, adjust as needed
        };
       
        SessionManager.Instance.StartGameSession(currentGameSession);
        Debug.Log($"Started new game session with session number: {currentGameSession.SessionNumber}");
        //CallStartNewGameSession();
    }

    IEnumerator WaitForSKeyPress()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                messageText.text = "";
                //yield return new WaitForSeconds(delayInSeconds);
                StartAssessment(numberOfTargets, distancefromhome);
                assessmentAudio.Play(); // Play the audio when the 'S' key is pressed

            }
            yield return null;
        }
    }
    IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {

                case State.TargetAppeared:
                    Debug.Log("Current State: " + (int)currentState);
                    yield return TargetAppearedState();
                    break;
                case State.reachingtarget:
                    Debug.Log("Current State: " + (int)currentState);
                    yield return reachingtargetState();
                    break;
                case State.targetreached:
                    Debug.Log("Current State: " + (int)currentState);
                    yield return targetreachedState();
                    break;
                case State.homepositionappears:
                    Debug.Log("Current State: " + (int)currentState);
                    yield return homepositionappearsState();
                    break;
                case State.reachinghome:
                    Debug.Log("Current State: " + (int)currentState);
                    yield return reachinghomeState();
                    break;
                case State.reachedhome:
                    Debug.Log("Current State: " + (int)currentState);
                    yield return reachedhomeState();
                    break;

            }
            // After the state transition, save the state to CSV
            Data(currentState);
            // Ensure the state transition is properly handled
            yield return null; // Adjust if necessary
            //CallRobdataMethod();

        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnCircleAtPlayerPosition();
            StartCoroutine(StateMachine());
            messageText.text = "press 's' to start the Assessment";
           
        }
        

        if (spawnedCircle != null)
        {
           
            GameObject Player = GameObject.FindGameObjectWithTag("Player");

            if (Player != null)
            {
                // Get the player's position
                Vector2 PlayerPosition = Player.transform.position;

                // Check if the player is inside the circle
                bool isPlayerInside = IsPlayerInsideCircle(PlayerPosition);

                if (isPlayerInside)
                {
                    SetCircleColor(Color.red);
                    Debug.Log("player inside circle");
                }
                else
                {
                    SetCircleColor(Color.green);
                    Debug.Log("player outside circle");
                }
            }
            else
            {
                Debug.LogError("Player not found in the scene. Make sure the player has the 'Player' tag.");
            }
        }

        if (spawnedTargets != null && spawnedTargets.Length >= 0 && currentTargetIndex < spawnedTargets.Length)
        {
            foreach (GameObject Target in spawnedTargets)
            {

                GameObject Player = GameObject.FindGameObjectWithTag("Player");

                if (Player != null)
                {
                    // Get the player's position
                    Vector2 PlayerPosition = Player.transform.position;

                    //Vector2 playerPosition = transform.position;
                    bool wasPlayerInside = isPlayerInsideTarget;
                    if (currentTargetIndex < spawnedTargets.Length)
                    {
                        isPlayerInsideTarget = IsPlayerInsideTarget(PlayerPosition, spawnedTargets[currentTargetIndex]);

                        if (isPlayerInsideTarget)
                        {
                            SetTargetColor(insideColor, currentTargetIndex);

                            Debug.Log("player inside target");


                            if (!wasPlayerInside)
                            {
                                timeInside = 0f;


                            }

                            if (timeInside >= targetresttime)
                            {
                                SetTargetColor(stayingColor, currentTargetIndex);

                                Debug.Log("player staying inside target");

                            }

                            else
                            {
                                timeInside += Time.deltaTime;
                            }
                        }

                        else
                        {
                            SetTargetColor(leavingColor, currentTargetIndex);
                            Debug.Log("player outside target");

                            timeInside = 0f;
                        }




                    }
                }
            }

        }


    }
   
    public void SpawnCircleAtPlayerPosition()
    {
        if (currentGameSession == null)
        {
            Debug.LogError("currentGameSession is null. Ensure it's set before calling this method.");
            return;
        }

        Vector2 playerPosition;
        if (currentGameSession == null)
        {
            Debug.LogError("currentGameSession is null");
            return;
        }

        if (string.IsNullOrEmpty(circleclass.circlePath))
        {
            Debug.LogError("circleclass or circleclass.circlePath is null or empty");
            return;
        }
        // Check if there is a previous session and retrieve the gameParameter
        string lastGameParameterValue = Player.RetrievePreviousSessionGameParameter(currentGameSession, circleclass.circlePath);

        if (!string.IsNullOrEmpty(lastGameParameterValue))
        {
            Debug.Log("Retrieved gameParameter: " + lastGameParameterValue);
            string[] gameParameters = lastGameParameterValue.Split(';');

            if (gameParameters.Length >= 2 &&
                float.TryParse(gameParameters[0], out float posX) &&
                float.TryParse(gameParameters[1], out float posY))
            {
                playerPosition = new Vector2(posX, posY);
                Debug.Log($"Using retrieved circle position: ({posX}, {posY})");

            }
            else
            {
                Debug.LogError("Invalid gameParameter format, spawning at player position.");
                playerPosition = GetPlayerPosition();
            }
        }
        else
        {
            // Spawn at player position if no previous session data is found
            playerPosition = GetPlayerPosition();
        }
        // Spawn the circle at the determined position
        if (spawnedCircle != null)
        {
            Destroy(spawnedCircle); // Remove existing circle if present
        }

        // Spawn the circle at the determined position
        spawnedCircle = Instantiate(circlePrefab, playerPosition, Quaternion.identity);
        Collider2D circleCollider = spawnedCircle.AddComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        ((CircleCollider2D)circleCollider).radius = detectionradius;

        spawnedCircle.SetActive(true);

        // Record the activation time of the circle
        circleActivationTime = Time.time;
        Debug.Log($"Spawned circle at position: {spawnedCircle.transform.position}");
       
        // Get the position of the spawned circle
        Vector2 circlePosition = spawnedCircle.transform.position;
        Debug.Log($"Spawned circle position: ({circlePosition.x}, {circlePosition.y})");
        //string circlePositionx = $"{circlePosition.x};";
        //string circlePositiony = $"{circlePosition.y};";
        //string gameParameter = circlePositionx + " " + circlePositiony ;
        //Debug.Log($"Assigned gameParameter: {gameParameter}");
        //SessionManager.Instance.SetGameParameter(gameParameter, currentGameSession);
        //CallSetSessionDetails();
    }
    public void CallSetSessionDetails()
    {
        if (playerScript != null)
        {
            playerScript.SetSessionDetails();
        }
        else
        {
            Debug.LogError("PlayerScript reference is null!");
        }
    }
   
    private Vector2 GetPlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            return player.transform.position;
        }

        // Default position if player not found
        return Vector2.zero;
    }


    void StartAssessment(int numberOfTargets, float distancefromhome)
    {
        if (!isAssessmentRunning)
        {
            assessmentRounds++; // Increment the number of assessment rounds completed
            Debug.Log("Starting assessment round " + assessmentRounds);
            currentTargetIndex = 0;
            SpawnTargets(numberOfTargets, distancefromhome);
        }

    }

    void SpawnTargets(int numberOfTargets, float distanceFromHome)
    {
        Debug.Log("SpawnTargets method called");
        if (circlePrefab != null && targetPrefab != null)
        {
            Debug.Log("circlePrefab and targetPrefab are not null");
            Vector2 homePosition = new Vector2(spawnedCircle.transform.position.x, spawnedCircle.transform.position.y);
            //float angleStep = Mathf.PI / (numberOfTargets - 1); // Use Mathf.PI for a semicircle
            Debug.Log($"Home position: {homePosition}");                                                 //float angleStep = 360f / numberOfTargets;
            spawnedTargets = new GameObject[numberOfTargets];
            if (assessmentRounds == 1)
            {
                // First round, spawn targets normally and store their positions
                float currentAngle = 0f; // Starting angle
                float angleStep = (Mathf.PI) / (numberOfTargets - 1);
               
                for (int i = 0; i < numberOfTargets; i++)
                {
                    //float angle = i * angleStep;
                   // Angle increment for a semicircle
                  
                    float x = distanceFromHome * Mathf.Cos(currentAngle);
                    float y = distanceFromHome * Mathf.Sin(currentAngle);
                    Debug.Log(x + " /" + y+"dista");
                    Vector2 spawnPosition = new Vector2(homePosition.x + x, homePosition.y + y);
                    Debug.Log($"Target {i} should be at: {spawnPosition} | Distance: {Vector2.Distance(homePosition, spawnPosition)}");
                
                    spawnedTargets[i] = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
                    Debug.Log($"Target {i} spawned at position: {spawnPosition}");
                    targetPositionsFirstRound.Add(spawnPosition); // Store the position
                    Collider2D targetCollider = spawnedTargets[i].AddComponent<CircleCollider2D>(); // Use CircleCollider2D for 2D projects
                    targetCollider.isTrigger = true;
                    ((CircleCollider2D)targetCollider).radius = detectionradius;
                    spawnedTargets[i].SetActive(false);
                    // Update the angle for the next target
                    currentAngle += angleStep;
                }
            }
            else
            {
                // Subsequent rounds, shuffle and spawn targets at positions from the first round
                List<Vector2> shuffledPositions = new List<Vector2>(targetPositionsFirstRound);
                shuffledPositions.Shuffle(); // Shuffle the positions

                for (int i = 0; i < numberOfTargets; i++)
                {
                    Vector2 spawnPosition = shuffledPositions[i];
                    
                    spawnedTargets[i] = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
                    Collider2D targetCollider = spawnedTargets[i].AddComponent<CircleCollider2D>(); // Use CircleCollider2D for 2D projects
                    targetCollider.isTrigger = true;
                    ((CircleCollider2D)targetCollider).radius = detectionradius;
                    spawnedTargets[i].SetActive(false);
                }
            }
            // Check the distance from the home position to each target
            for (int i = 0; i < numberOfTargets; i++)
            {
               
                if (spawnedTargets[i] != null)
                {
                    float distance = Vector2.Distance(homePosition, spawnedTargets[i].transform.position);
                    Debug.Log($"Target {i + 1} distance from home position: {distance} units");
                }
                else
                {
                    Debug.LogWarning($"spawnedTargets[{i}] is null");
                }
            }
            
        }
        else
        {
            Debug.LogError("circlePrefab or targetPrefab is null");
        }
    
    }

       
    IEnumerator TargetAppearedState()
    {
        if (isSpawningCircle) yield break; // Avoid spawning a new circle if one is already being spawned

        isSpawningCircle = true;


        if (spawnedCircle != null)
        {
            bool playerInsideCircle = false; // Assuming player is initially inside the circle
            float timeInsideCircle = 0f; // Track the time the player spends inside the circle
            bool targetAppeared = false; // Track if the target has appeared

            while (!targetAppeared) // Loop until the target has appeared
            {
                yield return null; // Wait for the next frame

                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player != null)
                {
                    Vector2 playerPosition = player.transform.position;
                    playerInsideCircle = IsPlayerInsideCircle(playerPosition);

                    if (playerInsideCircle)
                    {
                        timeInsideCircle += Time.deltaTime; // Increment time spent inside the circle

                        if (timeInsideCircle >= circleresttime)
                        {
                            StartCoroutine(SpawnNextTargetactivate()); // Increment the target index to move to the next target
                            targetAppeared = true;
                            currentState = State.reachingtarget; // Update currentState to reachingtarget
                            Debug.Log("Target appeared: " + targetAppeared);
                            Debug.Log("Transitioning to State " + (int)currentState);
                        }
                    }
                    else
                    {
                        timeInsideCircle = 0f; // Reset time if player exits the circle
                        Debug.Log("Player moved out of the circle before the target appeared.");
                        ShowMoveBackText(); // Show the UI text
                    }
                }
            }
        }

        isSpawningCircle = false;
    }
    void ShowMoveBackText()
    {
        if (moveBackText != null)
        {
            moveBackText.gameObject.SetActive(true); // Enable the UI text
           // moveBackAudio.Play(); // Play the move back audio
            StartCoroutine(HideMoveBackTextAfterDelay()); // Hide the text after some delay
        }
    }
    IEnumerator HideMoveBackTextAfterDelay()
    {
        yield return new WaitForSeconds(2f); // Adjust the delay as needed
        if (moveBackText != null)
        {
            moveBackText.gameObject.SetActive(false); // Disable the UI text
        }
    }
    IEnumerator SpawnNextTargetactivate()
    {

        if (isSpawningTarget) yield break; // Avoid spawning a new target if one is already being spawned

        isSpawningTarget = true;
        //check if the next target index is within the bounds of spawnedTargets array
        if (spawnedTargets != null && spawnedTargets.Length >= 0 && currentTargetIndex < spawnedTargets.Length)
        {
            // Activate the target at the current index
            spawnedTargets[currentTargetIndex].SetActive(true);

            Debug.Log("Activating target: " + currentTargetIndex);

            bool playerInsideTarget = true;
            while (playerInsideTarget && currentTargetIndex < spawnedTargets.Length)
            {
                yield return null;

                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player != null)
                {
                    Vector2 playerPosition = player.transform.position;
                    playerInsideTarget = IsPlayerInsideTarget(playerPosition, spawnedTargets[currentTargetIndex]);
                }

            }

        }


        isSpawningTarget = false;
    }
    IEnumerator reachingtargetState()
    {
        if (isSpawningCircle) yield break; // Avoid spawning a new circle if one is already being spawned

        isSpawningCircle = true;


        if (spawnedCircle != null)
        {
            bool playerInsideCircle = true;
            bool targetAppeared = true;

            if (targetAppeared)
            {



                while (playerInsideCircle)
                {
                    yield return null;

                    GameObject player = GameObject.FindGameObjectWithTag("Player");

                    if (player != null)
                    {
                        Vector2 playerPosition = player.transform.position;
                        playerInsideCircle = IsPlayerInsideCircle(playerPosition);
                        Debug.Log("Player inside circle: " + playerInsideCircle);
                    }
                }

                // Player left the circle, deactivate it
                spawnedCircle.SetActive(false);
                // Player reached the target within 2 seconds
                // Calculate reaching duration
                circleTotargetDuration = Time.time - circleActivationTime;
                // Record the time when the target is reached
                targetReachedTime = Time.time;
                currentState = State.targetreached; // Update currentState to targetreached
                Debug.Log("Target reached: " + targetAppeared);
                Debug.Log("Transitioning to State " + (int)currentState);

            }


        }
        isSpawningCircle = false;
    }
    IEnumerator targetreachedState()
    {
        float elapsedTime = 0f; // Track elapsed time
        bool playerReachedTarget = false;

        while (elapsedTime < 3f)
        {
            // Check if the player is inside the target
            if (IsPlayerInsideTarget(PlayerPosition.position, spawnedTargets[currentTargetIndex]))
            {
                // Player has reached the target within 4 seconds
                playerReachedTarget = true;
                break;
            }

            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame before checking again
        }

        // If player reached the target within 4 seconds
        if (playerReachedTarget)
        {
            currentState = State.homepositionappears;
            Debug.Log("Player has reached the target within 4 seconds. Transitioning to homepositionappears state.");
            Debug.Log("Transitioning to State " + (int)currentState);
        }
        else
        {
            // Player didn't reach the target within 4 seconds
            // Make the target disappear
            MakeTargetDisappear();
            ActivateHomeposition();
            // Display assessment text
           // GetComponent<CircleColorChanger>().ShowAssessmentText("Assessment over. Keep trying ");
            Debug.Log("Player took more than 4 seconds to reach the target. Target disappeared. Transitioning to homepositionappears state.");

            Debug.Log("Transitioning to State " + (int)currentState);
            yield return new WaitForSeconds(1f);
            currentState = State.reachinghome;
        }
    }

    void MakeTargetDisappear()
    {
        // Code to make the target disappear
        // For example:
        spawnedTargets[currentTargetIndex].SetActive(false);
    }

    void ActivateHomeposition()
    {
        // Implement logic to make the homeposition appear here
        // For example:
        spawnedCircle.SetActive(true);
    }
    public void ShowAssessmentText(string text)
    {
        assessmentoverText.text = text;
        assessmentoverText.gameObject.SetActive(true);
      
    }

    IEnumerator homepositionappearsState()
    {
        if (isSpawningTarget) yield break;

        isSpawningTarget = true;

        if (spawnedTargets != null && spawnedTargets.Length >= 0 && currentTargetIndex < spawnedTargets.Length)
        {
            if (spawnedTargets[currentTargetIndex].activeSelf)
            {
                bool wasPlayerInside = isPlayerInsideTarget;
                bool playerInsideTarget = true;
                float timeInsideTarget = 0f;

                while (playerInsideTarget && currentTargetIndex < spawnedTargets.Length && timeInsideTarget < targetresttime)
                {
                    yield return null;

                    GameObject player = GameObject.FindGameObjectWithTag("Player");

                    if (player != null)
                    {
                        Vector2 playerPosition = player.transform.position;
                        playerInsideTarget = IsPlayerInsideTarget(playerPosition, spawnedTargets[currentTargetIndex]);
                        if (playerInsideTarget)
                        {
                            timeInsideTarget += Time.deltaTime; // Increment time spent inside the circle

                        }
                        else
                        {
                            timeInsideTarget = 0f; // Reset time if player exits the circle

                        }
                    }



                }
                if (timeInsideTarget >= targetresttime)
                {
                    StartCoroutine(SpawnNextCircleactivate());

                    circleAppeared = true;
                    currentState = State.reachinghome; // Update currentState to reachinghome

                    Debug.Log("circle appeared");
                    Debug.Log("Transitioning to State " + (int)currentState);


                }


            }
        }
        isSpawningTarget = false;


    }
    IEnumerator SpawnNextCircleactivate()
    {
        if (isSpawningCircle) yield break; // Avoid spawning a new circle if one is already being spawned

        isSpawningCircle = true;

        if (spawnedCircle != null)
        {
            spawnedCircle.SetActive(true);
            Debug.Log("Activating spawned circle");


            bool playerInsideCircle = true; // Assuming player is initially inside the circle

            while (playerInsideCircle)
            {
                yield return null; // Wait for the next frame

                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player != null)
                {
                    Vector2 playerPosition = player.transform.position;
                    playerInsideCircle = IsPlayerInsideCircle(playerPosition);
                }
            }

        }
        isSpawningCircle = false;
    }
    IEnumerator reachinghomeState() //deactivating target
    {
        // Continuously check if the player has left the target
        while (IsPlayerInsideTarget(PlayerPosition.position, spawnedTargets[currentTargetIndex]))
        {
            yield return null; // Wait for the next frame before checking again
        }

        // Player has left the target, deactivate it
        spawnedTargets[currentTargetIndex].SetActive(false);
        Debug.Log("Deactivating target: " + currentTargetIndex);

        // Transition to the next state
        currentState = State.reachedhome;
        Debug.Log("Transitioning to State " + (int)currentState);
        IncrementNextTarget();

    }
    IEnumerator reachedhomeState()
    {
        // Continuously check if the player is inside the home
        while (true)
        {
            // Check if the player is inside the home
            if (IsPlayerInsideCircle(PlayerPosition.position))
            {
                // Calculate reaching duration from target back to circle
                targetToCircleDuration = Time.time - targetReachedTime;
                // Player is inside the home, transition to State 0 (Target Appeared)
                currentState = State.TargetAppeared;
                Debug.Log("Player has reached home. Transitioning to State 0 (Target Appeared).");
                Debug.Log("Transitioning to State " + (int)currentState);
                yield break; // Exit the coroutine
            }

            yield return null; // Wait for the next frame before checking again

        }

    }



    void IncrementNextTarget()
    {
        currentTargetIndex++;
        if (currentTargetIndex >= numberOfTargets)
        {
            //currentTargetIndex = 0;
            shouldStopStateMachine = true; // Set the flag to stop the state machine
            AllTargetsVisitedState();
        }
    }
    void AllTargetsVisitedState()
    {
        Debug.Log("All targets visited!");

        if (assessmentRounds < totalRounds)
        {
            ResetAssessment();
        }
        else
        {
           
            Debug.Log("Assessment completed.");
            // You might want to perform any final actions here after completing all rounds.
        }
        DisplayRoundCompletionText();
       
    }
    void DisplayRoundCompletionText()
    {
        Debug.Log("DisplayRoundCompletionText called."); // Add this line
        // Determine the text to display based on the assessment round
        string completionText = (assessmentRounds <= totalRounds) ?
            "Round " + assessmentRounds + " completed!" : "Assessment completed!";

        // Set the text of the UI Text element
        roundCompletionText.text = completionText;

        // Activate the round completion text
        roundCompletionText.gameObject.SetActive(true);

        // Start coroutine to hide the text after a delay
        StartCoroutine(HideTextAfterDelay());
        // Check if the current round is the last round
        if (assessmentRounds >= totalRounds)
        {
            // Display the "Assessment Over" text
            assessmentoverText.gameObject.SetActive(true);
        }
    }

    IEnumerator HideTextAfterDelay()
    {
        Debug.Log("HideTextAfterDelay coroutine started."); // Add this line
        // Wait for the specified delay
        yield return new WaitForSeconds(textDisplayDelay);

        if (roundCompletionText != null)
        {
            roundCompletionText.gameObject.SetActive(false); // Disable the UI text
        }

    }
    void ResetAssessment()
    {
        // Reset any assessment-related variables or targets here.
        isAssessmentRunning = false;

        // Start a new assessment after a delay.
        StartCoroutine(WaitForNextAssessment());
    }

    IEnumerator WaitForNextAssessment()
    {
        // Wait for some time before starting the next assessment round.
        //yield return new WaitForSeconds(2.0f); // Adjust as needed

        yield return new WaitForSeconds(2.0f); // take break before starting next assessment round

        // Start the next round of assessment.
        StartAssessment(numberOfTargets, distancefromhome);
    }
    

    void SetCircleColor(Color color)
    {
        if (spawnedCircle != null)
        {
            Renderer circleRenderer = spawnedCircle.GetComponent<Renderer>();
            if (circleRenderer != null)
            {
                circleRenderer.material.color = color;
            }
        }
    }

    void SetTargetColor(Color color, int currentTargetIndex)
    {
        if (spawnedTargets != null && spawnedTargets.Length > 0 && currentTargetIndex < spawnedTargets.Length)
        {
            if (spawnedTargets[currentTargetIndex] != null)
            {
                Renderer targetRenderer = spawnedTargets[currentTargetIndex].GetComponent<Renderer>();
                if (targetRenderer != null)
                {
                    targetRenderer.material.color = color;
                }
            }
        }
    }

    bool IsPlayerInsideCircle(Vector2 playerPosition)
    {
        if (spawnedCircle != null)
        {
            //Vector2 circlePosition = new Vector2(spawnedCircle.transform.position.x, spawnedCircle.transform.position.y);
            Vector2 circlePosition = spawnedCircle.transform.position;
            float distance = (playerPosition - circlePosition).magnitude;
            //float distance = Vector2.Distance(circlePosition, playerPosition);
            return distance <= detectionradius;
        }
        return false;
    }
    bool IsPlayerOutsideCircle(Vector2 playerPosition)
    {
        if (spawnedCircle != null)
        {
            Vector2 circlePosition = spawnedCircle.transform.position;
            float distance = (playerPosition - circlePosition).magnitude;
            // Alternatively, you can use Vector2.Distance
            // float distance = Vector2.Distance(circlePosition, playerPosition);
            return distance > detectionradius;
        }
        return false;
    }

    bool IsPlayerInsideTarget(Vector2 playerPosition, GameObject target)
    {
        if (target != null)
        {
            Vector2 targetPosition = target.transform.position;
            float distance = (playerPosition - targetPosition).magnitude;
            return distance <= detectionradius;
        }
        return false;
        //float distance = Vector2.Distance(PlayerPosition, target.transform.position);
        //return distance <= detectionradius;
    }

    void Data(State state)
    {
        // Clear previous PlayerPrefs data
        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY");
        PlayerPrefs.DeleteKey("targetPos1");
        PlayerPrefs.DeleteKey("targetPos2");
        PlayerPrefs.DeleteKey("targetRadii");
        PlayerPrefs.DeleteKey("CirclePosX");
        PlayerPrefs.DeleteKey("CirclePosY");
        PlayerPrefs.DeleteKey("CircleRadius");
        PlayerPrefs.DeleteKey("Currentstat");
        PlayerPrefs.DeleteKey("Trials");
        // Add debug logs to identify null objects
        // Add debug logs to identify null objects
        Debug.Log($"Cleared previous PlayerPrefs data");

        //// Example of accessing an object in this method
        //if (currentGameSession == null)
        //{
        //    Debug.LogError("currentGameSession is null");
        //}



        // Now save the current data
        string currentStateStr = ((int)state).ToString();

        // Get the positions of all active targets
        string targetPositions1 = "";
        string targetPositions2 = "";
        string targetRadii = ""; // String to store target radii


        foreach (GameObject target in spawnedTargets)
        {
            if (target.activeSelf)
            {
                Vector2 position = target.transform.position;
                targetPositions1 += $"{position.x};";
                targetPositions2 += $"{position.y};";
                //Vector2 position = target.transform.position;
                //Vector2 scale = target.transform.localScale;
                //targetData += $"{position.x};{position.y};{scale.x};{scale.y};";
                // Get the target's radius
                CircleCollider2D collider = target.GetComponent<CircleCollider2D>();
                if (collider != null)
                {
                    float radius = collider.radius;
                    targetRadii += $"{radius};";
                }
            }
        }
         
        // Get the player's position
        //string Playerpositionx = "";
        //string Playerpositiony = "";
        Vector2 playerPosition = PlayerPosition.transform.position;
        String Playerpositionx = $"{playerPosition.x};";
        String Playerpositiony = $"{playerPosition.y};";    

        // Get the position of the spawned circle
        Vector2 circlePosition = spawnedCircle.transform.position;
        Debug.Log($"Spawned circle position: ({circlePosition.x}, {circlePosition.y})");
        string circlePositionx = $"{circlePosition.x};";
        string circlePositiony = $"{circlePosition.y};";

        Debug.Log($"Formatted circle position x: {circlePositionx}");
        Debug.Log($"Formatted circle position y: {circlePositiony}");
        //Debug.Log("Spawned circle position: " + spawnedCircle.transform.position);
        // Get the radius of the spawned circle
        float circleRadius = spawnedCircle.GetComponent<CircleCollider2D>().radius;
        Debug.Log("Circle Radius: " + circleRadius); // Add this line
        string circleRadiusStr = $"{circleRadius};";

        // Save the data to PlayerPrefs
        PlayerPrefs.SetString("PlayerX", Playerpositionx);
        PlayerPrefs.SetString("PlayerY", Playerpositiony);
        PlayerPrefs.SetString("targetPos1", targetPositions1);
        PlayerPrefs.SetString("targetPos2", targetPositions2);
        PlayerPrefs.SetString("targetRadii", targetRadii); // Save target radii

        PlayerPrefs.SetString("CirclePosX", circlePositionx); // Save circle position X
        PlayerPrefs.SetString("CirclePosY", circlePositiony); // Save circle position Y
        PlayerPrefs.SetString("CircleRadius", circleRadiusStr); // Save circle radius
        PlayerPrefs.SetString("Currentstat", currentStateStr);
        PlayerPrefs.SetInt("Trials", assessmentRounds);
        // Add debug logs to confirm data saving
        Debug.Log($"Saved current data to PlayerPrefs");
    }

}



//void SpawnTargets(int numberOfTargets, float distancefromhome)
//{
//    if (circlePrefab != null && targetPrefab != null)
//    {
//        Vector2 homePosition = new Vector2(spawnedCircle.transform.position.x, spawnedCircle.transform.position.y);
//        float angleStep = Mathf.PI / (numberOfTargets - 1); // Use Mathf.PI for a semicircle
//        //float angleStep = 360f / numberOfTargets;
//        spawnedTargets = new GameObject[numberOfTargets];
//        if (assessmentRounds == 1)
//        {

//            for (int i = 0; i < numberOfTargets; i++)
//            {
//                float angle = i * angleStep;
//                float x = homePosition.x + distancefromhome * Mathf.Cos(angle);
//                float y = homePosition.y + distancefromhome * Mathf.Sin(angle);
//                Vector2 spawnPosition = new Vector2(x, y);

//                spawnedTargets[i] = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
//                Collider2D targetCollider = spawnedTargets[i].AddComponent<CircleCollider2D>(); // Use CircleCollider2D for 2D projects
//                targetCollider.isTrigger = true;
//                ((CircleCollider2D)targetCollider).radius = detectionradius;
//                //((CircleCollider2D)targetCollider).radius = 1f;



//                spawnedTargets[i].SetActive(false);
//            }
//        }
//        else
//        {
//            // Ensure that all targets spawn at least once in the second round
//            int guaranteedSpawnCount = Mathf.Min(numberOfTargets, 1);
//            for (int i = 0; i < guaranteedSpawnCount; i++)
//            {
//                float angle = i * angleStep; // Adjust angle for the guaranteed spawns
//                float x = homePosition.x + distancefromhome * Mathf.Cos(angle);
//                float y = homePosition.y + distancefromhome * Mathf.Sin(angle);
//                Vector2 spawnPosition = new Vector2(x, y);

//                spawnedTargets[i] = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
//                Collider2D targetCollider = spawnedTargets[i].AddComponent<CircleCollider2D>(); // Use CircleCollider2D for 2D projects
//                targetCollider.isTrigger = true;
//                ((CircleCollider2D)targetCollider).radius = detectionradius;
//                spawnedTargets[i].SetActive(false);
//            }

//            // Randomly spawn the remaining targets
//            for (int i = guaranteedSpawnCount; i < numberOfTargets; i++)
//            {
//                float randomAngle = Random.Range(0f, Mathf.PI); // Random angle in radians
//                float x = homePosition.x + distancefromhome * Mathf.Cos(randomAngle);
//                float y = homePosition.y + distancefromhome * Mathf.Sin(randomAngle);
//                Vector2 spawnPosition = new Vector2(x, y);

//                spawnedTargets[i] = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
//                Collider2D targetCollider = spawnedTargets[i].AddComponent<CircleCollider2D>(); // Use CircleCollider2D for 2D projects
//                targetCollider.isTrigger = true;
//                ((CircleCollider2D)targetCollider).radius = detectionradius;
//                spawnedTargets[i].SetActive(false); // Deactivate the spawned target initially
//            }
//        }

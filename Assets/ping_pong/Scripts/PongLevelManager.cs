//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
////using PlutoDataStructures;


//public class PongLevelManager : MonoBehaviour

//{
//    public static PongLevelManager instance;
//    public int PongGameLevel;
//    GameObject rgbd;
//    public GameObject levelIncreaseObject;
//    bool showLevelIncrease;
//    int paddleSize;
//    float ballSpeed = 1f;
//    float enemSpeed = 1f;
//    // Start is called before the first frame update
//    BoundController boundController;
//    void Start()
//    {
//        showLevelIncrease = false;
//        // AppData.ReadGamePerformance();
//        PongGameLevel = myData.startGameLevelRom;

//        //SendToRobot.ControlParam(AppData.plutoData.mechs[AppData.plutoData.mechIndex], ControlType.TORQUE, false, false);
//        //// Set control parameters
//        //SendToRobot.ControlParam(AppData.plutoData.mechs[AppData.plutoData.mechIndex], ControlType.TORQUE, false, true);
//        //AppData.plutoData.desTorq = 0;

//        //PongGameLevel = GameDifficulty.level;
//        ballSpeed = 2f + 0.3f * myData.startGameLevelSpeed;
//        enemSpeed = 1.2f + 0.32f * myData.startGameLevelSpeed;
//        rgbd = GameObject.FindGameObjectWithTag("Player");
//        rgbd.transform.localScale = new Vector3(0.2f, Mathf.Clamp(3f - 2 * 0.3f, .8f, 3f), 1f);

//        EnemyController.speed = enemSpeed;
//        BallController.speed = ballSpeed;
//        // Find the BoundController instance
//        boundController = FindObjectOfType<BoundController>();

//    }

//    // Update is called once per frame
//    void Update()
//    {


//        if (Input.GetKey(KeyCode.LeftControl))
//        {
//            if (Input.GetKeyUp(KeyCode.L))
//            {
//                //showLevelIncrease = !showLevelIncrease;

//                ShowLevelControl();
//                //Code here
//                Debug.Log("skjn");
//            }

//        }
//    }

//    public void ShowLevelControl()
//    {
//        showLevelIncrease = !showLevelIncrease;
//        levelIncreaseObject.SetActive(showLevelIncrease);

//    }

//    public void OnSpeedUpButton()
//    {
//        myData.startGameLevelSpeed += 1;
//        Debug.Log("Manual Increase" + myData.startGameLevelSpeed);
//        //ballSpeed = 2f + 0.3f * myData.startGameLevelSpeed;
//        //enemSpeed = 1.2f + 0.32f * myData.startGameLevelSpeed;
//        //EnemyController.speed = enemSpeed;
//        //BallController.speed = ballSpeed;
//        UpdateSpeed();
//    }
//    public void OnSpeedDownButton()
//    {
//        myData.startGameLevelSpeed -= 1;
//        Debug.Log("Manual decrease" + myData.startGameLevelSpeed);
//        //ballSpeed = 2f + 0.3f * myData.startGameLevelSpeed;
//        //enemSpeed = 1.2f + 0.32f * myData.startGameLevelSpeed;
//        //EnemyController.speed = enemSpeed;
//        //BallController.speed = ballSpeed;
//        UpdateSpeed();
//    }
//    void UpdateSpeed()
//    {
//        ballSpeed = 2f + 0.3f * myData.startGameLevelSpeed;
//        enemSpeed = 1.2f + 0.32f * myData.startGameLevelSpeed;
//        EnemyController.speed = enemSpeed;
//        BallController.speed = ballSpeed;
//    }

//    public void OnClickNextLevel()
//    {
//        PongGameLevel = 2;
//        myData.startGameLevelSpeed = 2; // Assuming level 2 starts at speed 2

//        ballSpeed = 2f + 0.3f * myData.startGameLevelSpeed;
//        enemSpeed = 1.2f + 0.32f * myData.startGameLevelSpeed;
//        rgbd.transform.localScale = new Vector3(0.2f, Mathf.Clamp(3f - 2 * 0.3f, .8f, 3f), 1f);

//        EnemyController.speed = enemSpeed;
//        BallController.speed = ballSpeed;
//        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
//        if (ball != null)
//        {
//            Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
//            ballRb.velocity = Vector2.zero;
//            ball.transform.position = Vector3.zero; // Assuming the center of the play area
//            //Call AdjustSpeedForNextLevel method from BoundController
//            if (boundController != null)
//            {
//                boundController.AdjustSpeedForNextLevel(ballRb);
//            }
//        }

//        // Reset the enemy's position
//        // enemy.position = new Vector3(-6, 0, 0);

//        // Optionally, restart the game or show some transition effect
//        Time.timeScale = 1; // Ensure the game is running
//        Debug.Log("Level 2 started");

//    }
//}

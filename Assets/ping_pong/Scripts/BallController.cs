//using UnityEngine;
//using System.Collections;

//public class BallController : MonoBehaviour
//{

//    //speed of the ball
//    public static float speed = 3.5F;

//    //the initial direction of the ball
//    private Vector2 spawnDir;

//    //ball's components
//    Rigidbody2D rig2D;
//    // Use this for initialization

//    // Audio clip ballCollision,Win, Loose
//    public AudioClip[] audioClips;
//    int rand = 3;
//    float threshold = 2;

//    void Start()
//    {
//        //setting balls Rigidbody 2D
//        rig2D = this.gameObject.GetComponent<Rigidbody2D>();

//        //generating random number based on possible initial directions
//        int rand = Random.Range(1, 4);

//        //setting initial direction
//        if (rand == 1)
//        {
//            spawnDir = new Vector2(1, 1);
//        }
//        else if (rand == 2)
//        {
//            spawnDir = new Vector2(1, -1);
//        }
//        else if (rand == 3)
//        {
//            spawnDir = new Vector2(-1, -1);
//        }
//        else if (rand == 4)
//        {
//            spawnDir = new Vector2(-1, 1);
//        }

//        //moving ball in initial direction and adding speed
//        rig2D.velocity = (spawnDir * speed);

//    }

//    // Update is called once per frame
//    void Update()
//    {




//    }
//    void playAudio(int clipNumber)
//    {
//        AudioSource audio = GetComponent<AudioSource>();
//        audio.clip = audioClips[clipNumber];
//        audio.Play();

//    }

//    void OnCollisionEnter2D(Collision2D col)
//    {
//        //calculate enc1
//        playAudio(0);
//        //tag check
//        if (col.gameObject.tag == "Enemy")
//        {

//            float y = launchAngle(transform.position,
//                                col.transform.position,
//                                col.collider.bounds.size.y);

//            //set enc1 and speed
//            Vector2 d = new Vector2(1, y).normalized;
//            rig2D.velocity = d * speed * 1.5F;
//        }

//        if (col.gameObject.tag == "Player")
//        {
//            //calculate enc1
//            // playAudio(0);
//            float y = launchAngle(transform.position,
//                                col.transform.position,
//                                col.collider.bounds.size.y);

//            //set enc1 and speed
//            Vector2 d = new Vector2(-1, y).normalized;
//            rig2D.velocity = d * speed * 1.5F;
//        }
//    }

//    //calculates the enc1 the ball hits the paddle at
//    float launchAngle(Vector2 ballPos, Vector2 paddlePos,
//                    float paddleHeight)
//    {
//        return 0.2f * Mathf.Sign(ballPos.y - paddlePos.y) + (ballPos.y - paddlePos.y) / paddleHeight;
//    }


//}







using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    // Speed of the ball
    public  float speed = 1.5f;//3.5f;

    // The initial direction of the ball
    private Vector2 spawnDir;

    // Ball's components
    public Rigidbody2D rig2D;

    // Audio clips for collision, win, loose
    public AudioClip[] audioClips;
    // Counter for the number of times the player stops the ball
    private int playerStopCount = 0;

    // Threshold for increasing speed
    private const int stopThreshold = 2;
    private const float speedIncrement = 0.25f;
    public int PongGameLevel;
    BoundController boundController;
    private PongPlayerController pongPlayerController;
    private Vector2 initialVelocity;

    void Start()
    {
        // Setting ball's Rigidbody 2D
        rig2D = GetComponent<Rigidbody2D>();
        initialVelocity = rig2D.velocity;
        // Generating random number based on possible initial directions
        int rand = Random.Range(1, 5);

        // Setting initial direction
        switch (rand)
        {
            case 1:
                spawnDir = new Vector2(1, 1);
                break;
            case 2:
                spawnDir = new Vector2(1, -1);
                break;
            case 3:
                spawnDir = new Vector2(-1, -1);
                break;
            case 4:
                spawnDir = new Vector2(-1, 1);
                break;
        }

        // Adding speed to initial direction to move ball
        rig2D.velocity = spawnDir * speed;
        // Find the BoundController instance
        //boundController = FindObjectOfType<BoundController>();
        pongPlayerController = FindObjectOfType<PongPlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        // Keep the ball moving at the specified speed
        //rig2D.velocity = rig2D.velocity.normalized * speed;
        if (pongPlayerController.levelComplete)
        {
            rig2D.velocity = Vector2.zero; // Stop the ball's movement
        }
        //else if (rig2D.velocity == Vector2.zero)
        //{
        //    rig2D.velocity = initialVelocity; // Resume ball's movement if it was stopped
        //}


    }
    public void ResetBall()
    {
        //transform.position = Vector2.zero; // Reset the ball's position to the center
        rig2D.velocity = initialVelocity; // Reset the ball's velocity
        rig2D.velocity = spawnDir * speed;
    }
    void playAudio(int clipNumber)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = audioClips[clipNumber];
        audio.Play();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // Play collision audio
        playAudio(0); // Assuming 0 is the collision sound

        if (col.gameObject.CompareTag("Enemy"))
        {
            // Calculate the launch angle and set new velocity for the ball
            float y = launchAngle(transform.position, col.transform.position, col.collider.bounds.size.y);
            Vector2 d = new Vector2(1, y).normalized;
            rig2D.velocity = d * speed * 1.5f;
        }
        else if (col.gameObject.CompareTag("Player"))
        {
            // Calculate the launch angle and set new velocity for the ball
            float y = launchAngle(transform.position, col.transform.position, col.collider.bounds.size.y);
            Vector2 d = new Vector2(-1, y).normalized;
            rig2D.velocity = d * speed * 1.5f;
           
            //// Increment the player stop counter
            //playerStopCount++;

            //// Increase the speed if the player stops the ball more than 5 times
            //if (playerStopCount > stopThreshold)
            //{
            //    speed += speedIncrement;
            //    playerStopCount = 0; // Reset the counter after increasing the speed
            //}
        }
    }

    // Calculates the angle at which the ball hits the paddle
    float launchAngle(Vector2 ballPos, Vector2 paddlePos, float paddleHeight)
    {
        //return (ballPos.y - paddlePos.y) / paddleHeight;
        return 0.2f * Mathf.Sign(ballPos.y - paddlePos.y) + (ballPos.y - paddlePos.y) / paddleHeight;

    }
    //public void OnClickNextLevel()
    //{
    //    PongGameLevel = 2;
    //    myData.startGameLevelSpeed = 2; // Assuming level 2 starts at speed 2
    //    Time.timeScale = 1; // Ensure the game is running
    //    Debug.Log("Level 2 started");
        
            
    //}
}

//using UnityEngine;
//using System.Collections;

//public class Done_DestroyByContact : MonoBehaviour
//{
//	public GameObject explosion;
//	public GameObject playerExplosion;
//	public int scoreValue;
//	private Done_GameController gameController;

//	void Start ()
//	{
//		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
//		if (gameControllerObject != null)
//		{
//			gameController = gameControllerObject.GetComponent <Done_GameController>();
//		}
//		if (gameController == null)
//		{
//			Debug.Log ("Cannot find 'GameController' script");
//		}
//	}

//	void OnTriggerEnter (Collider other)
//	{
//		if (other.tag == "Boundary" || other.tag == "Enemy")
//		{
//			return;
//		}

//		if (explosion != null)
//		{
//			Instantiate(explosion, transform.position, transform.rotation);
//		}

//		if (other.tag == "Player")
//		{
//			Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
//			gameController.GameOver();
//		}


//		if (other.tag != "Player")
//		{
//			Debug.Log("here");
//			Destroy(other.gameObject);
//		}
//		else
//			other.gameObject.SetActive(false);
//		gameController.AddScore(scoreValue);
//		Destroy (gameObject);
//	}
//}
//using UnityEngine;
//using System.Collections;

//public class Done_DestroyByContact : MonoBehaviour
//{
//    public static Done_DestroyByContact instance;
//    public GameObject explosion;
//    public GameObject playerExplosion;
//    public int scoreValue;
//    private Done_GameController gameController;
//    private bool hazardDestroyedByBoundary;

//    void Start()
//    {
//        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
//        if (gameControllerObject != null)
//        {
//            gameController = gameControllerObject.GetComponent<Done_GameController>();
//        }
//        if (gameController == null)
//        {
//            Debug.Log("Cannot find 'GameController' script");
//        }

//    }

//    void OnTriggerEnter(Collider other)
//    {

//        if (other.tag == "Boundary")

//        {
//            return; // If it's a boundary, do nothing
//        }

//        //if (other.tag == "Enemy")

//        //{
//        //    Debug.Log("Collision with Enemy, handling target destruction.");
//        //    //HandlePlayerDestruction();
//        //    HandleTargetDestroyed();
//        //}
//        if (other.tag == "PlayerFire")

//        {
//            Debug.Log("Player's fire hit the target, handling target destruction.");
//            HandleTargetDestroyed();
//        }

//        if (other.tag == "Player")

//        {

//            Debug.Log("Collision with Player, handling player destruction.");
//            HandlePlayerCollision(other);
//        }


//        if (explosion != null)
//        {
//            Instantiate(explosion, transform.position, transform.rotation);
//        }

//        Destroy(gameObject);


//    }
//    public void HandlePlayerCollision(Collider Player)
//    {

//        //if (Player.CompareTag("PlayerProjectile"))
//        //{
//        //    Debug.Log("Player collided with their own fire. Player's life remains unaffected.");
//        //    return; // Skip reducing player's life if collided with own fire
//        //}
//        //Instantiate(playerExplosion, Player.transform.position, Player.transform.rotation);
//        //gameController.GameOver();

//        //Debug.Log("HandlePlayerCollision called with collider: " + Player.name + ", Tag: " + Player.tag);

//        // Check if the collision is with a player projectile
//        //if (Player.CompareTag("PlayerFire"))
//        //{
//        //    Debug.Log("Player collided with their own fire. Player's life remains unaffected.");
//        //    return; // Skip reducing player's life if collided with own fire
//        //}

//        if (playerExplosion != null)
//        {
//            Instantiate(playerExplosion, Player.transform.position, Player.transform.rotation);
//        }

//        if (gameController != null)
//        {
//            gameController.GameOver();
//        }
//        else
//        {
//            Debug.LogError("gameController is null. Cannot call GameOver().");
//        }

//    }
//    public void HandleTargetDestroyed()
//    {
//        if (gameController != null)
//        {
//            // Add score when the hazard is destroyed by the player
//            gameController.AddScore(scoreValue);
//            gameController.HazardDestroyed();
//            GameStateMachine.Instance.TransitionToState(GameState.TargetDestroyed);
//        }
//        else
//        {
//            Debug.LogError("gameController is null. Cannot handle target destroyed.");
//        }
//    }
//}
   

using UnityEngine;

public class Done_DestroyByContact : MonoBehaviour
{
    public static Done_DestroyByContact instance;
    public GameObject explosion;
    public GameObject playerExplosion;
    public int scoreValue;
    private Done_GameController gameController;
    private bool hazardDestroyedByBoundary;

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<Done_GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
        {
            return; // If it's a boundary, do nothing
        }

        if (other.tag == "PlayerFire")
        {
            Debug.Log("Player's fire hit the target, handling target destruction.");
            HandleTargetDestroyed();
        }

        if (other.tag == "Player")
        {
            Debug.Log("Collision with Player, checking if it's an enemy.");
            HandlePlayerCollision(other);
        }

        if (explosion != null && other.tag != "Player") // Prevent explosion when colliding with the player
        {
            Instantiate(explosion, transform.position, transform.rotation);
        }

        if (other.tag != "Player") // Prevent self-destruction when colliding with the player
        {
            Destroy(gameObject);
        }
    }

    public void HandlePlayerCollision(Collider player)
    {
        // Only handle player life decrease if collided with an enemy
        if (this.CompareTag("Enemy"))
        {
            if (playerExplosion != null)
            {
                Instantiate(playerExplosion, player.transform.position, player.transform.rotation);
            }

            if (gameController != null)
            {
                gameController.GameOver(); // Assuming there's a method to decrease player's life
            }
            else
            {
                Debug.LogError("gameController is null. Cannot decrease player's life.");
            }
            // Destroy the enemy (this game object) upon collision with the player
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Player collided with a non-enemy object. Player's life remains unaffected.");
        }
    }

    public void HandleTargetDestroyed()
    {
        if (gameController != null)
        {
            // Add score when the hazard is destroyed by the player
            gameController.AddScore(scoreValue);
            gameController.HazardDestroyed();
            GameStateMachine.Instance.TransitionToState(GameState.TargetDestroyed);
        }
        else
        {
            Debug.LogError("gameController is null. Cannot handle target destroyed.");
        }
    }
}
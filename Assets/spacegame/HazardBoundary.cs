using UnityEngine;


public class HazardBoundary : MonoBehaviour
{
    public static HazardBoundary instance;
    private Done_GameController gameController;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hazard entered boundary.");
    }
    void Start()
    {
        gameController = FindObjectOfType<Done_GameController>();
        if (gameController == null)
        {
            Debug.LogError("GameController not found in the scene!");
        }
    }
   
    //void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Hazard"))
    //    {
    //        //Done_GameController gameController = FindObjectOfType<Done_GameController>();
    //        //gameController.HazardExitedBoundary();
    //        Destroy(other.gameObject);

    //    }
    //}
    //void OnTriggerExit(Collider other)
    //{
    //    Debug.Log("Hazard exited boundary.");
    //    if (other.CompareTag("Enemy"))
    //    {
    //        //Done_GameController gameController = FindObjectOfType<Done_GameController>();
    //        //if (gameController != null)
    //        //{
    //        //    gameController.SetHazardDestroyedByBoundary(true);
    //        //    Debug.Log("Hazard destroyed by boundary. Set flag to true.");

    //        //}
    //        //Call HandleHazardExitingBoundary method from HazardStateManager

    //        Destroy(other.gameObject);
    //    }
    //}
    //public void HandleTargetExitedBoundary(Collider other)
    //{
    //    Debug.Log("Hazard exited boundary.");
    //    if (other.CompareTag("Enemy"))
    //    {
    //        //Done_GameController gameController = FindObjectOfType<Done_GameController>();
    //        //if (gameController != null)
    //        //{
    //        //    gameController.SetHazardDestroyedByBoundary(true);
    //        //    Debug.Log("Hazard destroyed by boundary. Set flag to true.");

    //        //}
    //        //Call HandleHazardExitingBoundary method from HazardStateManager

    //        Destroy(other.gameObject);
    //        GameStateMachine.Instance.TransitionToState(GameState.TargetExitedBoundary);

    //    }
    //}

}

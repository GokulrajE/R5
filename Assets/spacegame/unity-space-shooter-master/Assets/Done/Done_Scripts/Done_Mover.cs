using UnityEngine;
//using static Hazardstate;
//using static HazardStateManager;

public class Done_Mover : MonoBehaviour
{
    public static Done_Mover instance;
    public float speed;

    void Start()
    {
        instance = this;
        InitializeMovement();
    }
    public void InitializeMovement()
    {
        //if (GameStateMachine.Instance.CurrentState == GameState.TargetMoving)
        //{
        //    GetComponent<Rigidbody>().velocity = transform.forward * speed;
        //}
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        Debug.Log("Initialized movement with speed: " + speed);
    }
    void Update()
    {
        //if (GameStateMachine.Instance.CurrentState == GameState.TargetMoving)
        //{
        //    // Save hazard position in PlayerPrefs
        //    SavePosition();
        //}
        //GameStateMachine.Instance.TransitionToState(GameState.TargetMoving);
        SavePosition();
    }

    private void SavePosition()
    {
        // Save hazard position in PlayerPrefs
        PlayerPrefs.SetFloat("HazardX", transform.position.x);
        PlayerPrefs.SetFloat("HazardY", transform.position.z);
    }
    public void HandleTargetMoving()
    {
        // Logic to handle target movement
        Debug.Log("Handling target moving.");
        InitializeMovement();
        //GameStateMachine.Instance.TransitionToState(GameState.TargetMoving);
    }
}

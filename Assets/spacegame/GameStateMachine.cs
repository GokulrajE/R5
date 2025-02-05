using UnityEngine;
using System;
using System.IO;


public enum GameState
{
    TargetMoving = 1,
    TargetDestroyed = 2,
    TargetExitedBoundary = 3,
    TargetAndPlayerCollided = 4
}

public class GameStateMachine : MonoBehaviour
{
    public static GameStateMachine Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    private const string PlayerPrefsKey = "GameState";
    // string CurrentStat;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Load the last saved state or set to default
        CurrentState = (GameState)PlayerPrefs.GetInt(PlayerPrefsKey, (int)GameState.TargetMoving);

        HandleCurrentState();
        //CurrentStat = PlayerPrefs.GetString("Currentstat");
    }

    public void TransitionToState(GameState newState)
    {
        CurrentState = newState;  // Update the current state
        //Save the new state to PlayerPrefs
        PlayerPrefs.SetInt(PlayerPrefsKey, (int)newState);
        PlayerPrefs.Save(); // Explicitly save to disk if necessary
        // Notify Done_PlayerController to log the state change
        Done_PlayerController.instance?.LogGameState(CurrentState);
        HandleCurrentState();
    }

    private void HandleCurrentState()
    {
        switch (CurrentState)
        {
            case GameState.TargetMoving:
                HandleTargetMoving();
                break;
            case GameState.TargetDestroyed:
                HandleTargetDestroyed();
                break;
            case GameState.TargetExitedBoundary:
                HandleTargetExitedBoundary();
                break;
            case GameState.TargetAndPlayerCollided:
                HandleCollision();
                break;
        }
        //TransitionToState(CurrentState);
    }

    private void HandleTargetMoving()
    {
        Debug.Log("Target is moving.");
        // Done_Mover.instance?.InitializeMovement();
        if (Done_Mover.instance != null)
        {
            Done_Mover.instance.InitializeMovement();
        }

    }

    private void HandleTargetDestroyed()
    {
        Debug.Log("Target destroyed. Target continues moving.");

        // Done_DestroyByContact.instance?.HandleTargetDestroyed();
        if (Done_DestroyByContact.instance != null)
        {
            Done_DestroyByContact.instance.HandleTargetDestroyed();
        }

        TransitionToState(GameState.TargetMoving);
    }

    private void HandleTargetExitedBoundary()
    {
        Debug.Log("Target exited boundary.");
        //Done_GameController.instance?.HandleTargetExitedBoundary();
        if (Done_GameController.instance != null)
        {
            Done_GameController.instance.HandleTargetExitedBoundary();
        }
        TransitionToState(GameState.TargetMoving);
    }

    private void HandleCollision()
    {
        Debug.Log("Collision occurred. Handling collision event.");
        //Done_GameController.instance?.GameOver();
        if (Done_GameController.instance != null)
        {
            Done_GameController.instance.GameOver();
        }
        TransitionToState(GameState.TargetMoving);
    }
}
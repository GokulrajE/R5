using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private string[] monitoredScenes = { "SpaceShooterDemo", "FlappyGame", "pong_game", "DrawPath", "Assessment","level1" };

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("SceneController started and sceneLoaded event subscribed.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        foreach (string monitoredScene in monitoredScenes)
        {
            if (scene.name == monitoredScene)
            {
                Debug.Log("Monitored scene loaded: " + scene.name);
                if (GameDataManager.instance != null)
                {
                    GameDataManager.instance.IncrementSessionNumber();
                }
                else
                {
                    Debug.LogError("GameDataManager instance is null.");
                }
                break;
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("SceneController destroyed and sceneLoaded event unsubscribed.");
    }
}

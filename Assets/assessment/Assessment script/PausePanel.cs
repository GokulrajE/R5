using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    public static PausePanel instance;
    public GameObject pausePanel;
    public Button pauseButton;
    public Button resumeButton;

    private bool assessmentPaused = false;

    void Start()
    {
        // Assign click event handlers
        pauseButton.onClick.AddListener(PauseButtonClicked);
        resumeButton.onClick.AddListener(ResumeButtonClicked);
        // Deactivate the panel initially
        gameObject.SetActive(false);
    }

    public void PauseButtonClicked()
    {
        assessmentPaused = true;
        Time.timeScale = 0f; // Pause the game
        pausePanel.SetActive(false); // Hide the pause panel
    }

    public void ResumeButtonClicked()
    {
        assessmentPaused = false;
        Time.timeScale = 1f; // Resume the game
        pausePanel.SetActive(true); // Show the pause panel
    }

    public bool IsAssessmentPaused()
    {
        return assessmentPaused;
    }
}

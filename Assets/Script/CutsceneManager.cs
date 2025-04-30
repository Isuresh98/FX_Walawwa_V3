using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector cutsceneDirector;
    public Button collectButton;
    public Button skipButton; // Add this for skipping
    public float pauseTime = 5f;
    private bool isPaused = false;

    void Start()
    {
        collectButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(true); // Show skip button
        cutsceneDirector.Play();
        Invoke(nameof(PauseCutscene), pauseTime);
    }

    void PauseCutscene()
    {
        cutsceneDirector.Pause();
        isPaused = true;
        collectButton.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(false);
    }

    public void OnCollectButtonClicked()
    {
        if (isPaused)
        {
            collectButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
            cutsceneDirector.Play();
            isPaused = false;
        }
    }
    public void OnSkipButtonClicked()
    {
        CancelInvoke(nameof(PauseCutscene)); // 🚫 Cancel the scheduled pause
        cutsceneDirector.time = cutsceneDirector.duration; // Jump to end
        cutsceneDirector.Evaluate(); // Update immediately
        cutsceneDirector.Stop(); // Stop playback
        skipButton.gameObject.SetActive(false);
        collectButton.gameObject.SetActive(false); // Make sure collect doesn't reappear
        isPaused = false; // Reset pause state
    }
}
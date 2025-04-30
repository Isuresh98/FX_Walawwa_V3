using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector cutsceneDirector;
    public Button collectButton;
    public float pauseTime = 5f;
    private bool isPaused = false;

    void Start()
    {
        collectButton.gameObject.SetActive(false);
        cutsceneDirector.Play();
        Invoke(nameof(PauseCutscene), pauseTime);
    }

    void PauseCutscene()
    {
        cutsceneDirector.Pause();
        isPaused = true;
        collectButton.gameObject.SetActive(true);
    }

    public void OnCollectButtonClicked()
    {
        if (isPaused)
        {
            collectButton.gameObject.SetActive(false);
            cutsceneDirector.Play();
            isPaused = false;
        }
    }
}
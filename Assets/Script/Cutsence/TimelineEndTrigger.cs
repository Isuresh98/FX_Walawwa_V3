using UnityEngine;
using UnityEngine.Playables;

public class TimelineEndTrigger : MonoBehaviour
{
    public PlayableDirector timeline;

    void Start()
    {
        timeline.stopped += OnTimelineStopped;
    }

    void OnTimelineStopped(PlayableDirector pd)
    {
        if (pd == timeline)
        {
            // Your game function here
            Debug.Log("Timeline finished. Starting game function...");
            StartGameFunction();
        }
    }

    void StartGameFunction()
    {
        // Replace with your actual game function
        Debug.Log("Game function started!");
    }
}
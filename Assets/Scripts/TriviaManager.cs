using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriviaManager : MonoBehaviour
{
    public static TriviaManager Instance { get; private set; }
    private Queue<TriviaEvent> waitQueue = new Queue<TriviaEvent>();
    [SerializeField] int displayCount = 0, maxDisplayCount = 4;
    private float triviaDisplayTime = 3;

    private struct TriviaEvent
    {
        public Quarry quarry;
        public string trivia;
    }

    public void EnqueueTrivia(Quarry quarry, string trivia)
    {
        waitQueue.Enqueue(new TriviaEvent(){
            quarry=quarry,
            trivia=trivia,
        });
    }

    public void AdjustDisplayCountOnElimination(Quarry quarry)
    {
        if (quarry.triviaBubble.gameObject.activeInHierarchy)
        {
            displayCount--;
        }
    }

    private IEnumerator DisplayTrivia()
    {
        TriviaEvent triviaEvent = waitQueue.Dequeue();
        TriviaBubble bubble = triviaEvent.quarry.triviaBubble;
        if (bubble.transform.parent.gameObject.activeInHierarchy)
        {
            bubble.gameObject.SetActive(true);
            displayCount++;
            yield return StartCoroutine(bubble.DisplayTrivia(triviaEvent.trivia));
            StartCoroutine(EndTriviaDisplay(bubble, triviaDisplayTime));
        }
    }

    private IEnumerator EndTriviaDisplay(TriviaBubble bubble, float ttl)
    {
        yield return new WaitForSeconds(ttl);
        bubble.gameObject.SetActive(false);
        displayCount--;
    }

    private bool CanDisplay()
    {
        return displayCount < maxDisplayCount & waitQueue.Count > 0;
    }

    private IEnumerator DisplayLoop()
    {
        WaitUntil displayCondition = new WaitUntil(CanDisplay);
        while (true)
        {
            yield return displayCondition;
            StartCoroutine(DisplayTrivia());
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(DisplayLoop());
    }
}

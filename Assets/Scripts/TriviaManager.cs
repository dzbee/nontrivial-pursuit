using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriviaManager : MonoBehaviour
{
    public static TriviaManager Instance { get; private set; }
    private Queue<TriviaEvent> waitQueue = new Queue<TriviaEvent>();
    [SerializeField] int displayCount = 0, maxDisplayCount = 4;
    private float triviaDisplayTime = 2;

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

    private void DisplayTrivia()
    {
        TriviaEvent triviaEvent = waitQueue.Dequeue();
        TriviaBubble bubble = triviaEvent.quarry.triviaBubble;
        bubble.gameObject.SetActive(true);
        bubble.DisplayTrivia(triviaEvent.trivia);
        displayCount++;
        StartCoroutine(EndTriviaDisplay(bubble, triviaDisplayTime));
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
        while (true)
        {
            yield return new WaitUntil(CanDisplay);
            DisplayTrivia();
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

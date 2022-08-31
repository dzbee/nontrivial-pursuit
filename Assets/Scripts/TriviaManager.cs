using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriviaManager : MonoBehaviour
{
    public static TriviaManager Instance { get; private set; }
    private Queue<Quarry> quarryQueue = new Queue<Quarry>();
    private HashSet<Quarry> displaySet = new HashSet<Quarry>();
    [SerializeField] int maxDisplayCount = 4;
    private float triviaDisplayTime = 3, minTriviaRelaxation = 3, maxTriviaRelaxation = 10;
    private WaitForSeconds despawnWait;

    public void RegisterQuarry(Quarry quarry)
    {
        quarryQueue.Enqueue(quarry);
    }

    private Quarry SelectQuarry()
    {
        return quarryQueue.Dequeue();
    }

    public void AdjustDisplayCountOnElimination(Quarry quarry)
    {
        displaySet.Remove(quarry);
    }

    private IEnumerator DisplayTrivia(Quarry quarry)
    {
        if (quarry.gameObject.activeInHierarchy)
        {
            quarry.triviaBubble.gameObject.SetActive(true);
            displaySet.Add(quarry);
            yield return StartCoroutine(quarry.triviaBubble.DisplayTrivia(quarry.SelectUnusedTrivia()));
            StartCoroutine(EndTriviaDisplay(quarry, triviaDisplayTime));
        }
    }

    private IEnumerator EndTriviaDisplay(Quarry quarry, float ttl)
    {
        yield return new WaitForSeconds(ttl);
        quarry.triviaBubble.gameObject.SetActive(false);
        displaySet.Remove(quarry);
        yield return new WaitForSeconds(Random.Range(minTriviaRelaxation, maxTriviaRelaxation));
        quarryQueue.Enqueue(quarry);
    }

    private bool CanDisplay()
    {
        return displaySet.Count < maxDisplayCount && quarryQueue.Count > 0;
    }

    private IEnumerator DisplayLoop()
    {
        WaitUntil displayCondition = new WaitUntil(CanDisplay);
        while (true)
        {
            yield return displayCondition;
            Quarry quarry = SelectQuarry();
            StartCoroutine(DisplayTrivia(quarry));
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

        despawnWait = new WaitForSeconds(triviaDisplayTime);
    }

    private void Start()
    {
        StartCoroutine(DisplayLoop());
    }
}

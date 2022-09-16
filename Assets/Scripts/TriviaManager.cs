using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class TriviaManager : MonoBehaviour
{
    public static TriviaManager Instance { get; private set; }
    private Queue<Quarry> quarryQueue = new Queue<Quarry>();
    private HashSet<Quarry> displaySet = new HashSet<Quarry>();
    private int maxDisplayCount = 4;
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

    public void PositionBubbleInBounds(TriviaBubble bubble)
    {
        bubble.transform.position = GameManager.Instance.BoundPosition(
            bubble.transform.position,
            bubble.background.size.x / 2,
            bubble.background.size.y / 2
        );
    }

    private void VerticallyFlipPointer(TriviaBubble bubble)
    {
        Debug.Log($"flipping: {bubble.pointer.transform.localPosition.y}");
        bubble.pointer.flipY = !bubble.pointer.flipY;
        bubble.pointer.transform.localPosition = new Vector2(
            bubble.pointer.transform.localPosition.x, 
            -bubble.pointer.transform.localPosition.y
        );
    }

    private void RestoreDefaultPointerPosition(SpriteRenderer pointer)
    {
        if (pointer.flipY) {
            pointer.transform.localPosition = new Vector2(
                pointer.transform.localPosition.x, 
                -pointer.transform.localPosition.y
            );
            pointer.flipY = false;  
        }
    }

    public void PositionSpeechPointer(Quarry quarry)
    {
        //Determine if pointer needs vertical flip
        if (Mathf.Sign(quarry.transform.position.y - quarry.triviaBubble.transform.position.y) != Mathf.Sign(quarry.triviaBubble.pointer.transform.localPosition.y))
        {
            VerticallyFlipPointer(quarry.triviaBubble);
        }
    }

    private IEnumerator DisplayTrivia(Quarry quarry)
    {
        if (quarry.gameObject.activeInHierarchy)
        {
            string trivia = quarry.SelectUnusedTrivia();
            quarry.triviaBubble.gameObject.SetActive(true);
            quarry.triviaBubble.SetTriviaBackgroundSize(trivia);
            PositionBubbleInBounds(quarry.triviaBubble);
            PositionSpeechPointer(quarry);
            displaySet.Add(quarry);
            yield return StartCoroutine(quarry.triviaBubble.DisplayTrivia(trivia));
            StartCoroutine(EndTriviaDisplay(quarry, triviaDisplayTime));
        }
    }

    private IEnumerator EndTriviaDisplay(Quarry quarry, float ttl)
    {
        yield return new WaitForSeconds(ttl);
        RestoreDefaultPointerPosition(quarry.triviaBubble.pointer);
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

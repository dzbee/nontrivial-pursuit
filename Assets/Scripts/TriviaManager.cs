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
    private int maxDisplayCount = 3;
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

    public void PositionBubble(Quarry quarry)
    {
        TriviaBubble bubble = quarry.triviaBubble;

        //Iterate over other bubbles
        //For each overlap, move bubble to not overlap (via vertical flip or horizontal slide)
        foreach (Quarry other in displaySet)
        {
            if (!object.ReferenceEquals(quarry, other))
            {
                if (!bubble.background.bounds.Intersects(other.triviaBubble.background.bounds))
                {
                    continue;
                }
                Vector2 overlaps = GetOverlaps(
                    quarry.triviaBubble.background.bounds,
                    other.triviaBubble.background.bounds
                );
                if (Mathf.Abs(overlaps.y) >= Mathf.Abs(overlaps.x))
                {
                    bubble.transform.localPosition = new Vector2(bubble.transform.localPosition.x, -bubble.transform.localPosition.y);
                }
                else
                {
                    bubble.transform.localPosition = new Vector2(bubble.transform.localPosition.x + overlaps.x, bubble.transform.localPosition.y);
                }
            }
        }

        //Flip bubble over sprite if bubble reaches out of vertical bounds
        if (
            bubble.background.bounds.center.y + bubble.background.bounds.extents.y > GameManager.Instance.arenaHeight ||
            bubble.background.bounds.center.y - bubble.background.bounds.extents.y < -GameManager.Instance.arenaHeight
        )
        {
            bubble.transform.localPosition = new Vector2(bubble.transform.localPosition.x, -bubble.transform.localPosition.y);
        }

        bubble.transform.position = GameManager.Instance.BoundPosition(
            bubble.transform.position,
            bubble.background.size.x / 2,
            bubble.background.size.y / 2
        );

        //Despawn trivia bubble if trivia bubble escapes quarry
        if (Mathf.Abs(quarry.transform.position.x - quarry.triviaBubble.transform.position.x) > (quarry.triviaBubble.background.size.x / 2 + quarry.GetComponent<SpriteRenderer>().bounds.extents.x))
        {
            StartCoroutine(EndTriviaDisplay(quarry, 0));
            return;
        }

        //Iterate over bubbles
        //If bubbles overlap, despawn current bubble
        /*
        foreach (Quarry other in displaySet)
        {
            if (!object.ReferenceEquals(quarry, other))
            {
                if (bubble.background.bounds.Intersects(other.triviaBubble.background.bounds))
                {
                    StartCoroutine(EndTriviaDisplay(quarry, 0));
                }
            }
        }
        */
    }

    private Vector2 GetOverlaps(Bounds self, Bounds other)
    {
        Vector2 diff = other.center - self.center;
        Vector2 sum = other.extents + self.extents;

        return new Vector2(
            Mathf.Sign(diff.x) * (Mathf.Abs(diff.x) - sum.x),
            Mathf.Sign(diff.y) * (Mathf.Abs(diff.y) - sum.y)
        );
    }

    private void VerticallyFlipPointer(TriviaBubble bubble)
    {
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

        //Get quarry sprite extent (plus some padding)
        float quarrySize = quarry.GetComponent<SpriteRenderer>().bounds.extents.x * 1.25f;
        //Slide pointer horizontally to offset local bubble positioning
        //Flip pointer if needed
        if (quarry.triviaBubble.transform.localPosition.x < 0)
        {
            quarry.triviaBubble.pointer.transform.localPosition = new Vector2(
                -quarry.triviaBubble.transform.localPosition.x - quarrySize, 
                quarry.triviaBubble.pointer.transform.localPosition.y
            );
            quarry.triviaBubble.pointer.flipX = true;
        }
        else
        {
            quarry.triviaBubble.pointer.transform.localPosition = new Vector2(
                -quarry.triviaBubble.transform.localPosition.x + quarrySize,
                quarry.triviaBubble.pointer.transform.localPosition.y
            );
            quarry.triviaBubble.pointer.flipX = false;
        }
    }

    private IEnumerator DisplayTrivia(Quarry quarry)
    {
        if (quarry.gameObject.activeInHierarchy)
        {
            string trivia = quarry.SelectUnusedTrivia();
            quarry.triviaBubble.gameObject.SetActive(true);
            quarry.triviaBubble.SetTriviaBackgroundSize(trivia);
            PositionBubble(quarry);
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

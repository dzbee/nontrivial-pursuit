using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quarry : MonoBehaviour
{
    public TriviaBubble triviaBubble { get; protected set; }
    private enum Direction{Up, Down, Right, Left};
    private float arenaWidth = 11, arenaHeight = 5;
    [SerializeField] private float maxIdle = 1;
    [SerializeField] private float movementTime = 0.5f;
    [SerializeField] protected string[] trivia;
    protected HashSet<int> usedTrivia = new HashSet<int>();
    [SerializeField] private float triviaMaxWaitTime = 5;

    private IEnumerator MovementLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0, maxIdle));
            yield return StartCoroutine(Move());
        }
    }

    private IEnumerator Move()
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = currentPosition + (Vector3)GetMovementUpdate();
        float timeElapsed = 0;
        while (timeElapsed <= movementTime)
        {
            transform.position = Vector3.Lerp(currentPosition, targetPosition, timeElapsed / movementTime);
            yield return new WaitForFixedUpdate();
            timeElapsed += Time.fixedDeltaTime;
        }
        transform.position = Vector3.Lerp(currentPosition, targetPosition, 1);
    }

    private Vector2 GetMovementUpdate()
    {
        Direction directionCode = (Direction)Random.Range(0, 4);
        Vector2 move = Vector2.zero;
        switch (directionCode) {
            case Direction.Up:
                move = new Vector2(0, 1);
                break;
            case Direction.Down:
                move = new Vector2(0, -1);
                break;
            case Direction.Right:
                move = new Vector2(1, 0);
                break;
            case Direction.Left:
                move = new Vector2(-1, 0);
                break;
        }
        if (!IsMoveInBounds(move))
        {
            move = -move;
        }
        return move;
    }

    private bool IsMoveInBounds(Vector2 move)
    {
        Vector2 updatedPosition = new Vector2(transform.position.x, transform.position.y) + move;
        if (
            updatedPosition.x - transform.localScale.x / 2 < -arenaWidth ||
            updatedPosition.x + transform.localScale.x / 2 > arenaWidth ||
            updatedPosition.y - transform.localScale.y / 2 < -arenaHeight ||
            updatedPosition.y + transform.localScale.y / 2 > arenaHeight
        )
        {
            return false;
        }
        return true;
    }

    private IEnumerator TriviaLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0, triviaMaxWaitTime));
            EnqueueTrivia();
        }
    }

    private string SelectUnusedTrivia()
    {
        HashSet<int> available = Enumerable.Range(0, trivia.Length).ToHashSet<int>();
        available.ExceptWith(usedTrivia);
        if (available.Count == 0)
        {
            usedTrivia = new HashSet<int>();
            return SelectUnusedTrivia();
        }
        int selectIndex = Random.Range(0, available.Count);
        int triviaIndex = available.ToList<int>()[selectIndex];
        usedTrivia.Add(triviaIndex);
        return trivia[triviaIndex].Replace("\\n", "\n");
    }

    private void EnqueueTrivia()
    {
        TriviaManager.Instance.EnqueueTrivia(this, SelectUnusedTrivia());
    }

    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        triviaBubble = gameObject.GetComponentInChildren<TriviaBubble>(true);
        StartCoroutine(MovementLoop());
        StartCoroutine(TriviaLoop());
    }
}

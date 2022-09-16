using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quarry : MonoBehaviour
{
    [Serializable]
    protected class SpriteSet {
        public Sprite up, down, left, right;
    }

    public TriviaBubble triviaBubble { get; protected set; }
    protected enum Direction{Up, Down, Right, Left};
    protected float maxIdle = 0.5f, movementTime = 0.25f;
    [SerializeField] protected string[] trivia;
    [SerializeField] protected SpriteSet[] spriteSets;
    private SpriteSet spriteSet;
    protected HashSet<int> usedTrivia = new HashSet<int>();

    protected IEnumerator MovementLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0, maxIdle));
            Vector2 currentPosition = transform.position;
            Vector2 targetPosition = GetMovementUpdate();
            yield return StartCoroutine(Move(currentPosition, targetPosition));
        }
    }

    protected virtual IEnumerator Move(Vector2 currentPosition, Vector2 targetPosition)
    {
        float timeElapsed = 0;
        while (timeElapsed <= movementTime)
        {
            transform.position = Vector3.Lerp(currentPosition, targetPosition, timeElapsed / movementTime);
            yield return new WaitForFixedUpdate();
            timeElapsed += Time.fixedDeltaTime;
        }
        transform.position = Vector3.Lerp(currentPosition, targetPosition, 1);
    }

    protected Vector2 GetMovementUpdate()
    {
        Direction directionCode = (Direction)UnityEngine.Random.Range(0, 4);
        UpdateSpriteDirection(directionCode);
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
        
        return BoundMove(move);
    }

    protected Vector2 BoundMove(Vector2 move)
    {
        return new Vector2(
            Mathf.Clamp(transform.position.x + move.x, -GameManager.Instance.arenaWidth, GameManager.Instance.arenaWidth), 
            Mathf.Clamp(transform.position.y + move.y, -GameManager.Instance.arenaHeight, GameManager.Instance.arenaHeight)
        );
    }

    public string SelectUnusedTrivia()
    {
        HashSet<int> available = Enumerable.Range(0, trivia.Length).ToHashSet<int>();
        available.ExceptWith(usedTrivia);
        if (available.Count == 0)
        {
            usedTrivia = new HashSet<int>();
            return SelectUnusedTrivia();
        }
        int selectIndex = UnityEngine.Random.Range(0, available.Count);
        int triviaIndex = available.ToList<int>()[selectIndex];
        usedTrivia.Add(triviaIndex);
        return trivia[triviaIndex];
    }

    protected void UpdateSpriteDirection(Direction direction)
    {
        switch (direction) {
            case Direction.Up:
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteSet.up;
                return;
            case Direction.Down:
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteSet.down;
                return;
            case Direction.Left:
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteSet.left;
                return;
            case Direction.Right:
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteSet.right;
                return;
        }
    }

    private void Start()
    {
        spriteSet = spriteSets[UnityEngine.Random.Range(0, spriteSets.Length)];
        UpdateSpriteDirection(Direction.Down);
        triviaBubble = gameObject.GetComponentInChildren<TriviaBubble>(true);
        TriviaManager.Instance.RegisterQuarry(this);
        StartCoroutine(MovementLoop());
    }
}

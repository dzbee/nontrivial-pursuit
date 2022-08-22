using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quarry : MonoBehaviour
{
    private enum Direction{Up, Down, Right, Left};
    private float arenaWidth = 11, arenaHeight = 5;
    private float maxIdle = 1;
    private float movementTime = 0.5f;

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

    private void Start()
    {
        StartCoroutine(MovementLoop());
    }

    private void OnMouseDown()
    {
        Destroy(gameObject);
    }
}

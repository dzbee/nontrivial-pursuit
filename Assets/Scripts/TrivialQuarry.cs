using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrivialQuarry : Quarry
{
    private void OnMouseDown()
    {
        GameManager.Instance.GameOver();
    }

    protected override IEnumerator Move(Vector2 currentPosition, Vector2 targetPosition)
    {
        if (MovementManager.Instance.RegisterMove(targetPosition, currentPosition))
        {
            yield return base.Move(currentPosition, targetPosition);
        }
    }
}

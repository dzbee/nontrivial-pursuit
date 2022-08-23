using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrivialQuarry : Quarry
{
    private void OnMouseDown()
    {
        GameManager.Instance.GameOver();
    }
}

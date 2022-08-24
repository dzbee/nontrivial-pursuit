using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NontrivialQuarry : Quarry
{
    private void OnMouseDown()
    {
        GameManager.Instance.EliminateNontrivialQuarry(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NontrivialQuarry : Quarry
{
    private void OnMouseDown()
    {
        gameObject.SetActive(false);
    }
}

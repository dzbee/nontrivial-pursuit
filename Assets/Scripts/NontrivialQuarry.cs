using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NontrivialQuarry : Quarry
{
    private void Awake()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void OnMouseDown()
    {
        Destroy(gameObject);
    }
}

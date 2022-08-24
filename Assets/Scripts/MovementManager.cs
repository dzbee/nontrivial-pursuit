using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager Instance { get; private set; }
    private HashSet<Vector2> destinations = new HashSet<Vector2>();

    public bool RegisterMove(Vector2 destination, Vector2 currentPosition)
    {
        if (destinations.Contains(destination))
        {
            return false;
        }
        destinations.Add(destination);
        destinations.Remove(currentPosition);
        return true;
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
    }

}

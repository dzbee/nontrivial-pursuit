using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject quarryPrefab;
    [SerializeField] private int nQuarries = 13;
    [SerializeField] private int spawnWidth = 10, spawnHeight = -4;

    void Awake()
    {
        for (int i = 0; i < nQuarries; i++)
        {
            int spawnX = Random.Range(-spawnWidth, spawnWidth);
            int spawnY = Random.Range(-spawnHeight, spawnHeight);
            Instantiate(quarryPrefab, new Vector3(spawnX, spawnY, 0), quarryPrefab.transform.rotation);
        }
    }
}

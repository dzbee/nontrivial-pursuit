using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private TrivialQuarry trivialQuarryPrefab;
    [SerializeField] private NontrivialQuarry nontrivialQuarryPrefab;
    [SerializeField] private int nQuarries = 10, nNontrivialQuarries = 0;
    [SerializeField] private int spawnWidth = 10, spawnHeight = -4;
    [SerializeField] private GameObject gameOverPanel;

    private void SpawnQuarries()
    {
        int spawnX = Random.Range(-spawnWidth, spawnWidth);
        int spawnY = Random.Range(-spawnHeight, spawnHeight);
        for (int i = 0; i < nQuarries; i++)
        {
            spawnX = Random.Range(-spawnWidth, spawnWidth);
            spawnY = Random.Range(-spawnHeight, spawnHeight);
            Instantiate(trivialQuarryPrefab, new Vector3(spawnX, spawnY, 0), trivialQuarryPrefab.transform.rotation);
        }
        for (int i = 0; i < nNontrivialQuarries; i++)
        {
            spawnX = Random.Range(-spawnWidth, spawnWidth);
            spawnY = Random.Range(-spawnHeight, spawnHeight);
            Instantiate(nontrivialQuarryPrefab, new Vector3(spawnX, spawnY, 0), nontrivialQuarryPrefab.transform.rotation);
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
    }

    public void NewGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
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

        SpawnQuarries();
    }
}

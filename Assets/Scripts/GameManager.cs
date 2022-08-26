using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] UIManager uiManager;
    [SerializeField] private TrivialQuarry trivialQuarryPrefab;
    [SerializeField] private NontrivialQuarry nontrivialQuarryPrefab;
    private enum GameState{Active, Inactive};
    private GameState gameState;
    [SerializeField] private int nQuarries = 15, nNontrivialQuarries = 5;
    [SerializeField] private int lives = 3;
    [SerializeField] private int spawnWidth = 10, spawnHeight = -4;
    public int timeElapsed = 0;

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

    public void EliminateNontrivialQuarry(GameObject quarry)
    {
        if (gameState == GameState.Active)
        {
            quarry.SetActive(false);
            nNontrivialQuarries--;
            uiManager.ScorePoint();
            if (nNontrivialQuarries < 1)
            {
                GameOver();
                uiManager.GameOver(true);
            }
        }
    }

    public void EliminateTrivialQuarry(GameObject quarry)
    {
        if (gameState == GameState.Active)
        {
            quarry.SetActive(false);
            lives--;
            uiManager.LoseLife();
            if (lives < 1)
            {
                GameOver();
                uiManager.GameOver(false);
            }
        }
    }

    private void GameOver()
    {
        Time.timeScale = 0;
        gameState = GameState.Inactive;
    }

    public void NewGame()
    {
        Time.timeScale = 1;
        gameState = GameState.Active;
        SceneManager.LoadScene("Game");
    }

    private IEnumerator RunGameClock()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timeElapsed++;
            uiManager.UpdateClock(timeElapsed);
        }
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
        StartCoroutine(RunGameClock());
    }
}

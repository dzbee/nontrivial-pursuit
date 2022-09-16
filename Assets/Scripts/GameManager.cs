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
    [HideInInspector] public int arenaWidth = 10, arenaHeight = 5;
    private HashSet<Vector2> spawns = new HashSet<Vector2>();
    public int timeElapsed = 0;

    private void SpawnQuarry(Quarry prefab)
    {
        Vector2 spawnPos = new Vector2(Random.Range(-arenaWidth, arenaWidth), Random.Range(-arenaHeight, arenaHeight));
        while (spawns.Contains(spawnPos))
        {
                spawnPos = new Vector2(Random.Range(-arenaWidth, arenaWidth), Random.Range(-arenaHeight, arenaHeight));
        }
        spawns.Add(spawnPos);
        Instantiate(prefab, spawnPos, prefab.transform.rotation);
    }

    private void SpawnQuarries()
    {
        for (int i = 0; i < nQuarries; i++)
        {
            SpawnQuarry(trivialQuarryPrefab);
        }
        for (int i = 0; i < nNontrivialQuarries; i++)
        {
            SpawnQuarry(nontrivialQuarryPrefab);
        }
    }

    public void EliminateNontrivialQuarry(GameObject quarry)
    {
        if (gameState == GameState.Active)
        {
            TriviaManager.Instance.AdjustDisplayCountOnElimination(quarry.GetComponent<NontrivialQuarry>());
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
            TriviaManager.Instance.AdjustDisplayCountOnElimination(quarry.GetComponent<TrivialQuarry>());
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

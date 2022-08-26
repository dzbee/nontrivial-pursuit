using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private GameObject[] lives;
    [SerializeField] private Image[] pointImages;
    [SerializeField] private Sprite lightbulbOff, lightbulbOn;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;


    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadRules()
    {
        SceneManager.LoadScene("Rules");
    }

    public void LoseLife()
    {
        for (int i=lives.Length-1; i >= 0; i--)
        {
            if (lives[i].activeInHierarchy)
            {
                lives[i].SetActive(false);
                break;
            }
        }
    }

    public void ScorePoint()
    {
        for (int i=0; i<pointImages.Length; i++)
        {
            if (pointImages[i].sprite == lightbulbOff)
            {
                pointImages[i].sprite = lightbulbOn;
                break;
            }
        }
    }

    public void GameOver(bool victory)
    {
        if (victory)
        {
            gameOverText.text = $"SUCCESS\nTime: {secondsToTime(GameManager.Instance.timeElapsed)}";
        }
        else
        {
            gameOverText.text = $"GAME OVER\nTry to avoid clicking on true facts!";
        }
        gameOverPanel.SetActive(true);
    }

    public void UpdateClock(int seconds)
    {
        clockText.text = secondsToTime(seconds);
    }

    private string secondsToTime(int seconds)
    {
        int minutes = seconds / 60;
        int remainderSeconds = seconds % 60;
        string timeStr = $"{minutes}:{remainderSeconds}";
        if (remainderSeconds < 10)
        {
            timeStr = $"{minutes}:0{remainderSeconds}";
        }
        return timeStr;
    }
}

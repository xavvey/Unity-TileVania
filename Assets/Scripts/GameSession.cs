using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerHealth = 2;
    [SerializeField] int currentScore = 0;
    // [SerializeField][Range(0f, 1f)] float deathDelay = 1f;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI scoreText;

    // Singleton pattern
    void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        healthText.text = playerHealth.ToString();
        scoreText.text = "Score: " + currentScore.ToString();
    }

    public void ProcessPointsCount(int score)
    {
        currentScore += score;
        scoreText.text = "Score: " + currentScore.ToString();
    }

    public void ProcesPlayerDeath()
    {
        if (playerHealth > 1)
        {
            
            // StartCoroutine(TakeLife());
            TakeLife();
        }
        else
        {
            // StartCoroutine(ResetGameSession());
            ResetGameSession();
        }
    }

    // IEnumerator 
    void TakeLife()
    {
        // coroutine gives bug with lvl load -> subtracts more lives or reloads lvl when not needed
        // yield return new WaitForSecondsRealtime(deathDelay); 
        playerHealth--;
        healthText.text = playerHealth.ToString();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    // IEnumerator
    void ResetGameSession()
    {
        // yield return new WaitForSecondsRealtime(deathDelay);
        SceneManager.LoadScene(0);
        Destroy(gameObject);
        FindObjectOfType<ScenePersist>().ResetScenePersist();
    }
}

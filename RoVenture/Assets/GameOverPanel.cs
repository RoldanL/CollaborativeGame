using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] GameObject gameOver;

    public void GameOver()
    {
        if (gameOver != null)
        {
            gameOver.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOver GameObject is not assigned in the Inspector.");
        }
    }
}

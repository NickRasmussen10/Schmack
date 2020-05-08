using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] Text highscoreText;

    private void OnLevelWasLoaded(int level)
    {
        if (scoreText)
        {
            Debug.Log(Score.score);
            scoreText.text = "Score: " + Score.score;
        }
        if (highscoreText)
        {
            highscoreText.text = "Highscore: " + Score.highScore;
        }
    }
}

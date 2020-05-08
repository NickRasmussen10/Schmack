using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score
{
    public static int score;
    public static int highScore;

    public static void SetScore(int newScore)
    {
        score = newScore;
        if (score > highScore) highScore = newScore;
    }

    public static int GetScore() { return score; }
    public static int GetHighScore() { return highScore; }
}

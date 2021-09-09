using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;
    private float scoreMultiplier = 1;
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int scoreToAdd)
    {
        score += Mathf.CeilToInt(scoreToAdd * scoreMultiplier);
    }

    public void RemoveScore(int scoreToAdd)
    {
        score += Mathf.CeilToInt(scoreToAdd * scoreMultiplier);
    }



    public void SetScoreMultiplier(float newValue)
    {
        scoreMultiplier = newValue;
    }

    public int GetScore()
    {
        return score;
    }
}

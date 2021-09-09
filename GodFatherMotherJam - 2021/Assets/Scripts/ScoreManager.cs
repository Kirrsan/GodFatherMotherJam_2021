using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    public Animator scoreAnimator;

    private int score = 0;
    private float scoreMultiplier = 1;

    [Header("TimeToChangeScore")]
    public float timerMax = 0;
    public AnimationCurve ScoreTextCurve;
    private float timer = 0;
    private float ratio = 0;
    private float ratioCurve = 0;

    private int startValue = 0;
    private int endValue = 0;
    private int oldValue = 0;
    public int stepToPlaySoudAgain = 300;

    private bool shouldLerp = false;

    [SerializeField] private AudioClip scoreWinAudio = null;
    private AudioSource scoreWin_AudioSource;

    private void Start()
    {
        scoreText.text = "0";
        scoreWin_AudioSource = GetComponent<AudioSource>();
        oldValue =  stepToPlaySoudAgain;
    }

    // Update is called once per frame
    void Update()
    {
        if (!shouldLerp) return;

        LerpText();
    }

    public void AddScore(int scoreToAdd)
    {
        startValue = score;

        if (shouldLerp)
        {
            endValue += Mathf.CeilToInt(scoreToAdd * scoreMultiplier);
        }
        else
        {
            endValue = score + Mathf.CeilToInt(scoreToAdd * scoreMultiplier);
            shouldLerp = true;
        }
        scoreAnimator.SetTrigger("AddScore");

        if(endValue >= oldValue )
        {
            oldValue = oldValue + stepToPlaySoudAgain;
            scoreWin_AudioSource.PlayOneShot(scoreWinAudio);
        }
    }

    public void RemoveScore(int scoreToRemove)
    {
        startValue = score;

        if(shouldLerp)
        {
            endValue -= Mathf.CeilToInt(scoreToRemove * scoreMultiplier);
        }
        else
        {
            endValue = score - Mathf.CeilToInt(scoreToRemove * scoreMultiplier);
            shouldLerp = true;
        }

        if(endValue < 0)
        {
            endValue = 0;
        }

        scoreAnimator.SetTrigger("RemoveScore");
    }

    private void LerpText()
    {
        timer += Time.deltaTime;

        if(ratio <= 1)
        {
            ratio = timer / timerMax;
            ratioCurve = ScoreTextCurve.Evaluate(ratio);

            score = (int)Mathf.Lerp(startValue, endValue, ratioCurve);
        }
        else
        {
            timer = 0;
            ratio = 0;
            score = endValue;
            endValue = 0;
            shouldLerp = false;
        }

        scoreText.text = score.ToString();
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

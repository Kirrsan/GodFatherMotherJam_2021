using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreParticlesScript : MonoBehaviour
{
    public float timeToFade = 1.5f;
    public Color WrongColor;
    public Color RightColor;
    private Color endColor = new Color();
    private Color startColor = new Color();

    public bool right = false;


    private float timer = 0;
    private float ratio = 0;


    private TextMeshProUGUI FlyingScoreText;
    private Animator FlyingScoreAnimator;


    public void Setup(bool tempRight, int scoreToAdd)
    {
        FlyingScoreText = GetComponent<TextMeshProUGUI>();
        FlyingScoreAnimator = GetComponent<Animator>();
        right = tempRight;

        FlyingScoreText.text = scoreToAdd.ToString();

        if (right)
        {
            endColor = new Color(RightColor.r, RightColor.g, RightColor.b, 0);
            startColor = RightColor;
            FlyingScoreText.color = startColor;
            FlyingScoreAnimator.SetTrigger("Right");
        }
        else
        {
            endColor = new Color(WrongColor.r, WrongColor.g, WrongColor.b, 0);
            startColor = WrongColor;
            FlyingScoreText.color = startColor;
            FlyingScoreAnimator.SetTrigger("Wrong");
        }

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        ratio = timer / timeToFade;

        if (ratio <= 1)
        {
            FlyingScoreText.color = Color.Lerp(startColor, endColor, ratio);
        }
        else
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}

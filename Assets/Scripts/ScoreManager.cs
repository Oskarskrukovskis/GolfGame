using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    //Components
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    //Gameplay variables
    public float scoreCount = 0;
    public float highScoreCount = 0;


    private static ScoreManager instance;
    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType(typeof(ScoreManager)) as ScoreManager;

            return instance;
        }
        set
        {
            instance = value;
        }
    }

    private void Start()
    {
        scoreCount = 0;
        UpdateScoreUi();
    }

    //Updates current score and changes the high score, if it has been reached.
    public void UpdateScoreUi()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + scoreCount;
        }
        if (scoreCount >= highScoreCount)
        {
            highScoreCount = scoreCount;
            highScoreText.text = "High: " + highScoreCount;
        }
    }
}

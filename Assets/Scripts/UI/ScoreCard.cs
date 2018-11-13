using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Score card data class that will be serialized to get previous game scores.
/// </summary>
[System.Serializable]
public class ScoreCardData : IComparable
{
    public int rank=-1;
    public string name;
    public float score;

    public ScoreCardData(int r,string n,float s)
    {
        rank = r;
        name = n;
        score = s;
    }

    public int CompareTo(object obj)
    {
        ScoreCardData other = obj as ScoreCardData;
        float f = score - other.score;
        if (f > 0)
            return 1;
        else if (f < 0)
            return -1;
        return 0;
    }
}

/// <summary>
/// Score card script that lets us to easily set necessary value to UI elements.
/// </summary>
public class ScoreCard : MonoBehaviour {
    public Text rankText;
    public Text playerNameText;
    public Text scoreText;

    /// <summary>
    /// Color of Background of bgImage that will be used if this scorecard represents score from current game.
    /// <para>This distinguishes scores so player can clearly see if current score is good enough.
    /// </summary>
    public Color currentScoreColor;

    public Image bgImage;

    void Start()
    {
        if(bgImage==null)
            bgImage = gameObject.GetComponent<Image>();
    }

    public void setRankText(int number)
    {
        rankText.text = number.ToString();
    }

    public void setPlayerName(string name)
    {
        playerNameText.text = name;
    }

    public void setScoreText(float score)
    {
        scoreText.text = score.ToString("0.00");
    }

    public void setAsCurrentScore()
    {
        bgImage.color = currentScoreColor;
    }

    public void setScoreCard(ScoreCardData data)
    {
        setRankText(data.rank);
        setScoreText(data.score);
        setPlayerName(data.name);
    }
}

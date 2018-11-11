using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

public class ScoreCard : MonoBehaviour {
    public Text rankText;
    public Text playerNameText;
    public Text scoreText;

    public Color currentScoreColor;

    public Image bgImage;

    void Start()
    {
        if(bgImage!=null)
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

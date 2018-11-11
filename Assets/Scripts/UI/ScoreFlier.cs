using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreFlier : MonoBehaviour {


    public Text textBox;

    public Color positiveColor;
    public Color negativeColor;

    public void setScoreValue(float score)
    {
        textBox.text = score.ToString("0.00");
        if (score<0)
        {
            textBox.color = negativeColor;
        }else if(score>0)
        {
            textBox.color = positiveColor;
        }
    }
}

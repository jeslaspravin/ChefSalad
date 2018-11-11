using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultDisplay : MonoBehaviour {

    public Button restartButton;

    public Text resultMessageText;

    public GameObject scoreCardPrefab;

    public Transform highscorePanel;


    public void refreshData(string playerName,float score,ref List<ScoreCardData> scoreCardData)
    {
        string resultText = string.Format("Player {0} won with score {1}", playerName, score.ToString("0.00"));
        resultMessageText.text = resultText;

        while(highscorePanel.childCount>0)
        {
            highscorePanel.GetChild(0).SetParent(null);
            Destroy(highscorePanel.GetChild(0).gameObject);
        }

        for(int idx=0;idx<scoreCardData.Count;idx++)
        {
            ScoreCardData scoreCard = scoreCardData[idx];
            GameObject go = Instantiate(scoreCardPrefab);
            ScoreCard sc = go.GetComponent<ScoreCard>();
            if(scoreCard.rank ==-1)// Means the score is current game score so highlight it
            {
                sc.setAsCurrentScore();
            }
            scoreCard.rank = idx + 1;
            sc.setScoreCard(scoreCard);
            go.transform.SetParent(highscorePanel);
            scoreCardData[idx] = scoreCard;
        }
    }
}

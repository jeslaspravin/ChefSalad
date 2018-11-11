using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour {
    // TODO : Change to generic thread safe Async system if time permits
    //In seconds
    public float initialTimerDuration;

    public float initialMovementSpeed;

    public float speedBuffSaturationPt;

    private float timeConsumed = 0;

    private float currentMovementSpeed;

    private float currentSpeedBuff = 0;

    private float playerScore = 0;

    struct SpeedBuff
    {
        public float buffVal;
        public float timeLeft;

        public SpeedBuff(float buff,float time)
        {
            buffVal = buff;
            timeLeft = time;
        }
    }

    private List<SpeedBuff> buffs = new List<SpeedBuff>();

    public int TimeLeft
    {
        get
        {
            int val=(int)(initialTimerDuration - timeConsumed);
            return val>0?val:0;
        }
    }

    public float CurrentMovementSpeed
    {
        get { return currentMovementSpeed+currentSpeedBuff; }
    }

    public float PlayerScore
    {
        get { return playerScore; }
    }

    void Awake()
    {
        currentMovementSpeed = initialMovementSpeed;
    }

	// Update is called once per frame
	void Update () {
        timeConsumed += Time.deltaTime;
        for(int i =0;i<buffs.Count;i++)
        {
            SpeedBuff sb = buffs[i];
            sb.timeLeft -= Time.deltaTime;
            buffs[i] = sb;
        }
        buffs.RemoveAll((SpeedBuff buff) => {
            if(buff.timeLeft <= 0)
            {
                currentSpeedBuff -= buff.buffVal;
                return true;
            }
            return false;
        });
    }
    
    public void addTime(float timeToAdd)
    {
        timeConsumed -= timeToAdd;
    }

    public void addScore(float scoreToAdd)
    {
        playerScore += scoreToAdd;
    }

    public void addSpeedBuff(float buff,float duration)
    {
        buffs.Add(new SpeedBuff(buff, duration));
        currentSpeedBuff = Mathf.Clamp(currentSpeedBuff+buff, -speedBuffSaturationPt, speedBuffSaturationPt);
    }

}

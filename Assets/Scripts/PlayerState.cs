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

    private int playerScore = 0;

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
            return Mathf.Clamp(val, 0, val);
        }
    }

    public float CurrentMovementSpeed
    {
        get { return currentMovementSpeed+getSpeedBuffAmt(); }
    }

    private float getSpeedBuffAmt()
    {
        float buffVal = 0;
        foreach (SpeedBuff buff in buffs)
        {
            buffVal += buff.timeLeft>0?buff.buffVal:0;
        }
        return Mathf.Clamp(buffVal, -speedBuffSaturationPt, speedBuffSaturationPt);
    }

    public int PlayerScore
    {
        get { return playerScore; }
    }

    void Awake()
    {
        currentMovementSpeed = initialMovementSpeed;
    }

    // Use this for initialization
    void Start () {
        InvokeRepeating("cleanBuffs", 5, 5);
	}

    protected void cleanBuffs()
    {
        buffs.RemoveAll((SpeedBuff buff) => { return buff.timeLeft <= 0; });
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
    }
    
    public void addTime(float timeToAdd)
    {
        timeConsumed -= timeToAdd;
    }

    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
    }

    public void addSpeedBuff(float buff,float duration)
    {
        buffs.Add(new SpeedBuff(buff, duration));
    }

}

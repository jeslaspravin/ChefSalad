using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that holds player's state and will be available to be accessed from PlayerController only.
/// </summary>
public class PlayerState : MonoBehaviour {
    // TODO : Change to generic thread safe Async system if time permits

    /// <summary>
    /// <para>Initial Time for the player at start of level</para>
    /// <para>In seconds</para>
    /// </summary>
    public float initialTimerDuration;

    /// <summary>
    /// <para>Initial Movement Speed for the player at start of level</para>
    /// </summary>
    public float initialMovementSpeed;

    /// <summary>
    /// Saturation(Maximum extend) point in both positive and negative extreme from zero beyond which the speed of 
    /// player cannot be increased.
    /// </summary>
    public float speedBuffSaturationPt;

    /// <summary>
    /// Total time consumed by the player(We can use Time but still having something separate from engine Time is better)
    /// </summary>
    private float timeConsumed = 0;

    /// <summary>
    /// Current speed of player movement.
    /// </summary>
    private float currentMovementSpeed;

    /// <summary>
    /// Current speed buff that caches speed buffer for this frame so that it don't have to be recalculated 
    /// every time from player controller call
    /// <para>This gets changed every frame along with other calculation to avoid repeated calculations</para>
    /// </summary>
    private float currentSpeedBuff = 0;
    // Number of Speed buffs that are ready to be killed,Once reached 3 update removes expired buffers.
    private int numOfBuffTimedOut = 0;

    /// <summary>
    /// Score of player
    /// </summary>
    private float playerScore = 0;

    /// <summary>
    /// Delegate used by score/state publisher event
    /// </summary>
    /// <param name="value">Value that needs to be published</param>
    /// <param name="playerState">PlayerState object</param>
    public delegate void PlayerStateDelegates(float value, PlayerState playerState);

    /// <summary>
    /// Event that gets invoked every time score gets changed
    /// </summary>
    public event PlayerStateDelegates onScoreChanged;

    /// <summary>
    /// Speed Buff Data struct
    /// </summary>
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

    /// <summary>
    /// Getter for Time left for player
    /// </summary>
    public int TimeLeft
    {
        get
        {
            int val=(int)(initialTimerDuration - timeConsumed);
            return val>0?val:0;
        }
    }

    /// <summary>
    /// Getter that provides combined speed of player actual speed and its speed buff.
    /// </summary>
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
        float totalSpeedBuff = 0;
        numOfBuffTimedOut = 0;
        // Update the current speed buff value
        for(int i =0;i<buffs.Count;i++)
        {
            SpeedBuff sb = buffs[i];
            sb.timeLeft -= Time.deltaTime;
            buffs[i] = sb;
            if (sb.timeLeft > 0)
                totalSpeedBuff += sb.buffVal;
            else
                numOfBuffTimedOut += 1;
        }
        currentSpeedBuff = Mathf.Clamp(totalSpeedBuff, -speedBuffSaturationPt, speedBuffSaturationPt);
        // 3 is magic value of speed buff remove buffer
        if(numOfBuffTimedOut>3)
        {
            buffs.RemoveAll((SpeedBuff buff) => {
                if (buff.timeLeft <= 0)
                {
                    return true;
                }
                return false;
            });
        }
    }
    
    public void addTime(float timeToAdd)
    {
        timeConsumed -= timeToAdd;
    }

    public void addScore(float scoreToAdd)
    {
        if (onScoreChanged != null)
            onScoreChanged.Invoke(scoreToAdd, this);
        playerScore += scoreToAdd;
    }

    public void addSpeedBuff(float buff,float duration)
    {
        buffs.Add(new SpeedBuff(buff, duration));
    }

}

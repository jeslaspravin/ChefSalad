using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Speed buff collectible class
/// <seealso cref="CollectibleItem"/>
/// </summary>
public class SpeedCollectible : CollectibleItem {

    public float minDuration = 15;
    public float maxDuration = 40;
    public float minSpeedChange = 5;
    public float maxSpeedChange = 10;


    protected override void action(PlayerController pc)
    {
        base.action(pc);
        pc.PlayerState.addSpeedBuff(Random.Range(minSpeedChange, maxSpeedChange), Random.Range(minDuration, maxDuration));
    }
}

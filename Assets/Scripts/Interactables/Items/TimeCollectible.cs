using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Time collectible class
/// <seealso cref="CollectibleItem"/>
/// </summary>
public class TimeCollectible : CollectibleItem {

    public int minTimeToAdd=20;
    public int maxTimeToAdd=40;

    protected override void action(PlayerController pc)
    {
        base.action(pc);
        pc.PlayerState.addTime(Random.Range(minTimeToAdd, maxTimeToAdd));
    }
}

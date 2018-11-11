using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCollectible : CollectibleItem {

    public int minScoreToAdd=10;
    public int maxScoreToAdd=20;

    protected override void action(PlayerController pc)
    {
        base.action(pc);
        pc.PlayerState.addScore(Random.Range(minScoreToAdd, maxScoreToAdd));
    }
}

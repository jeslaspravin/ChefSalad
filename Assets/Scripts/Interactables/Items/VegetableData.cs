using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Vegetable", menuName = "Inventory/Vegetables", order = 10)]
public class VegetableData : ScriptableObject
{
    public Vegies vegetableMask;
    public Sprite vegTexture;
    public float preparationTime;
    // Ratio of score reward that this vegetable costs on dumping/failure in serving
    public float scoreCostFactor;
    // Multiplier for preparation time to give reward
    public float scoreReward;

    public float Reward
    {
        get { return preparationTime * scoreReward; }
    }

    public float Penalty
    {
        get { return Reward * scoreCostFactor; }
    }
}
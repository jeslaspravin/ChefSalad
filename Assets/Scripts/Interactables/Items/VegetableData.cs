using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Data Asset that will be used to hold unique vegetable data
/// </summary>
[CreateAssetMenu(fileName = "Vegetable", menuName = "Inventory/Vegetables", order = 10)]
public class VegetableData : ScriptableObject
{
    // Vegetable that this asset represents
    public Vegies vegetableMask;

    // Sprite sheet for this vegetable
    public Sprite vegTexture;

    // Time it takes to chop this vegetable to add it to salad.
    public float preparationTime;

    // Ratio of score reward that this vegetable costs on dumping/failure in serving
    public float scoreCostFactor;

    // Multiplier for preparation time to give reward
    public float scoreReward;

    // Getter to easily get proper reward value for this vegetable
    public float Reward
    {
        get { return preparationTime * scoreReward; }
    }

    // Getter to easily get proper penalty value for this vegetable
    public float Penalty
    {
        get { return Reward * scoreCostFactor; }
    }
}
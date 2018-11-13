using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>Enum that represent each vegetable
/// none is never used(Not mostly,but is default for editor)</para>
/// 
/// <para>Vegetable int values are chosen such that all those vegetable data can be encoded into same integer</para>
/// 
/// </summary>
public enum Vegies
{
    none = 0,
    count = 6,// Total number of vegetables
    carrot = 1,
    tomato = 2,
    potato = 4,
    onion = 8,
    beet = 16,
    chilli = 32,
    oneItemSaladHandle=64// Handling to ensure that salad with one vegetable is considered as salad,Not vegie
}

/// <summary>
/// Vegetable interactable object that lets user to carry vegetables around in inventory
/// </summary>
public class Vegetables : CarriableItem {

    // Vegetable this item points to useful in determining the VegetableData and used for storing in inventory.
    public Vegies vegetable=Vegies.none;
    
    // Interact override for Vegetable
    public override void interact(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        ((PlayerController)player.controller).PlayerInventory.addItem((int)vegetable);
    }

    /// <summary>
    /// helper method that checks if vegetable is vegetable or salad
    /// </summary>
    /// <param name="mask">Vegetable's/Salad's integer equivalent representation</param>
    /// <returns>True if mask is vegetable and false otherwise</returns>
    public static bool isRawVegetable(int mask)
    {
        float log2Val = Mathf.Log(mask, 2);
        // Since raw vegies are always in power of 2
        return Mathf.Ceil(log2Val) == Mathf.Floor(log2Val);
    }
}

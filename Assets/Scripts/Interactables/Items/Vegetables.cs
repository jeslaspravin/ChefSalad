using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Vegies
{
    none = 0,
    count = 6,
    carrot = 1,
    tomato = 2,
    potato = 4,
    onion = 8,
    beet = 16,
    chilli = 32,
    oneItemSaladHandle=64// Handling to ensure that salad with one vegetable is considered as salad,Not vegie
}

public class Vegetables : CarriableItem {

    public Vegies vegetable=Vegies.none;
    
    public override void interact(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        ((PlayerController)player.controller).PlayerInventory.addItem((int)vegetable);
    }

    public static bool isRawVegetable(int mask)
    {
        float log2Val = Mathf.Log(mask, 2);
        // Since raw vegies are always in power of 2
        return Mathf.Ceil(log2Val) == Mathf.Floor(log2Val);
    }
}

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/Vegetables", order = 1)]
public class VegetableData : ScriptableObject
{
    public Vegies vegetableMask;
    public Sprite vegTexture;
    public float preparationTime;
}
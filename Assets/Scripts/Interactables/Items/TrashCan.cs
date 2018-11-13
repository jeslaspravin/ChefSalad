using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trashcan interactable item
/// </summary>
public class TrashCan : UsableItem
{
    // Delegate for On item trash event.
    public delegate void ItemTrashedDelegate(Guid guid,float cost);

    /// <summary>
    /// Event that gets invoked when item gets trashed by any of player so listeners can process with those data.
    /// </summary>
    public event ItemTrashedDelegate onItemTrashed;

    public override bool canInteract(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        return player.PlayerInventory.hasAnyItem();
    }

    public override void interact(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        int item=player.PlayerInventory.getNextItem();
        // if is vegetable then cost can be determined from vegetable data
        if(Vegetables.isRawVegetable(item))
        {
            VegetableData vegData = ChefSaladManager.getVegetableData(item);
            if(onItemTrashed!=null)
            {
                onItemTrashed.Invoke(player.GetId, vegData.Penalty);
            }
        }else // if it is salad we get each individual vegetable by bit shifting vegetable integer and finding its vegetable data
        {
            float cost = 0;
            int itemMask = item;
            for (int i = 0; i < (int)Vegies.count; i++)
            {
                if ((itemMask & 1) > 0)
                {
                    int veg = (int)Mathf.Pow(2, i);
                    VegetableData vegData = ChefSaladManager.getVegetableData(veg);
                    cost += vegData.Penalty;
                }
                itemMask >>= 1;
            }
            if (onItemTrashed != null)
            {
                onItemTrashed.Invoke(player.GetId, cost);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : UsableItem
{

    public delegate void ItemTrashedDelegate(Guid guid,float cost);

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
        if(Vegetables.isRawVegetable(item))
        {
            VegetableData vegData = GameManager.getVegetableData(item);
            if(onItemTrashed!=null)
            {
                onItemTrashed.Invoke(player.GetId, vegData.Penalty);
            }
        }else
        {
            float cost = 0;
            int itemMask = item;
            for (int i = 0; i < (int)Vegies.count; i++)
            {
                if ((itemMask & 1) > 0)
                {
                    int veg = (int)Mathf.Pow(2, i);
                    VegetableData vegData = GameManager.getVegetableData(veg);
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

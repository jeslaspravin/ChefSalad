using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictedUsageItem : UsableItem {

    public Guid userId;
    
    public override bool canInteract(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        return player != null && userId.Equals(player.GetId) && player.PlayerInventory.hasAnyItem();
    }

    public override void interact(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        ((PlayerController)player.controller).setInputActive(false);
    }
}

public class Stove : RestrictedUsageItem
{
    private int currentStack=0;

    public override void interact(GameObject interactor)
    {
        base.interact(interactor);
        Player player = interactor.GetComponent<Player>();
        int veg = player.PlayerInventory.getNextItem();
    }
}

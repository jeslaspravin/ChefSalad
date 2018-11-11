using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : RestrictedUsageItem {
    public override void interact(GameObject interactor)
    {
        Player player=interactor.GetComponent<Player>();
        action((PlayerController)player.controller);
        Destroy(gameObject);
    }

    protected virtual void action(PlayerController pc)
    {
#if GAME_DEBUG
        Debug.Log("Collected by " + pc.name);
#endif
        // Empty in base
    }
}

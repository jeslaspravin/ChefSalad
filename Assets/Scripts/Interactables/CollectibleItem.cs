using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collectible Items that will be spawned as reward for serving well to a player.
/// <para>Collectible spawned for a player will not be able to be picked up by any other player.</para>
/// </summary>
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

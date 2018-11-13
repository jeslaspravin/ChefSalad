using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Item that can be taken and carried by player by interacting
/// </summary>
public class CarriableItem : BasicItem {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool canInteract(GameObject interactor)
    {
        Player player=interactor.GetComponent<Player>();
        return player.PlayerInventory.canAddItem();// Only interact if player inventory accepts any more item.
    }

}

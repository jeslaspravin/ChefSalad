using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        return player.PlayerInventory.canAddItem();
    }

}

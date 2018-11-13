using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dish object which will hold one vegetable for a player and each dish that belongs to a player cannot be accessed by other player.
/// </summary>
public class Dish : RestrictedUsageItem {

    /// <summary>
    /// Vegetable that this dish holds
    /// </summary>
    private int vegetable = 0;

    /// <summary>
    /// Vegetable held indicator sprite,This sprite gets update based on vegetable is held by this dish.
    /// </summary>
    public SpriteRenderer vegetableSprite;

    void Start()
    {
        if(vegetableSprite == null)
        {
            vegetableSprite=GetComponentInChildren<SpriteRenderer>();
        }
    }

    public override bool canInteract(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        // Only interact if dish belongs to player and player has items in inventory or if there is vegetable 
        // in dish and player has free slot to pick the vegetable in dish
        return base.canInteract(interactor) && ((vegetable == 0 && player.PlayerInventory.hasAnyItem()) || 
            (vegetable > 0 && player.PlayerInventory.canAddItem()));
    }

    public override void interact(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        if (vegetable == 0)// If dish is empty try get first vegetable from player inventory and store it here.
        {
            int veg = player.PlayerInventory.getFirstItem(true);
            if (veg != 0)
            {
                vegetable = veg;
                vegetableSprite.sprite = ChefSaladManager.getVegetableData(veg).vegTexture;
            }
        }
        else// If dish has vegetable add it to player inventory.
        {
            player.PlayerInventory.addItem(vegetable);
            vegetable = 0;
            vegetableSprite.sprite = null;
        }
    }
}

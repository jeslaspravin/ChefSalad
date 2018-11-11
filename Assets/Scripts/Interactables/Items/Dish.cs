using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : RestrictedUsageItem {

    private int vegetable = 0;

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
        return base.canInteract(interactor) && ((vegetable == 0 && player.PlayerInventory.hasAnyItem()) || 
            (vegetable > 0 && player.PlayerInventory.canAddItem()));
    }

    public override void interact(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        if (vegetable == 0)
        {
            int veg = player.PlayerInventory.getFirstItem(true);
            if(veg != 0)
            {
                vegetable = veg;
                vegetableSprite.sprite = ChefSaladManager.getVegetableData(veg).vegTexture;
            }
        }
        else
        {
            player.PlayerInventory.addItem(vegetable);
            vegetable = 0;
            vegetableSprite.sprite = null;
        }
    }
}

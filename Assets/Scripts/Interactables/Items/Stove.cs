using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictedUsageItem : UsableItem {

    public Guid userId;
    
    public override bool canInteract(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        return player != null && userId.Equals(player.GetId);
    }

    public override void interact(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        ((PlayerController)player.controller).setInputActive(false);
    }
}

public class Stove : RestrictedUsageItem
{
    public delegate void IngredientPreparedDelegate(int prepdVeg, int saladMask);

    public event IngredientPreparedDelegate ingredientPreparedEvent;

    public delegate void SaladDelegate(int saladMask);

    public event SaladDelegate saladPickedUpEvent;

    public Transform stoveStandingSpot;

    private int currentStack=0;

    private float prepareTimeLeft=0;

    private VegetableData preparingVegData;

    private Player currentLockedPlayer;

    private void resetData()
    {
        prepareTimeLeft = 0;
        preparingVegData = null;
        ((PlayerController)currentLockedPlayer.controller).setInputActive(true);
        currentLockedPlayer = null;
    }

    public override bool canInteract(GameObject interactor)
    {
        return base.canInteract(interactor) && (currentStack != 0 || interactor.GetComponent<Player>().PlayerInventory.hasAnyItem());
    }

    public override void interact(GameObject interactor)
    {
        base.interact(interactor);
        Player player = interactor.GetComponent<Player>();
        currentLockedPlayer = player;

        if (currentLockedPlayer.PlayerInventory.hasAnyItem())
            StartCoroutine(startCooking());
        else
        {
            currentLockedPlayer.PlayerInventory.addItem(currentStack);
            saladPickedUp();
            currentStack = 0;
            resetData();
        }
    }

    IEnumerator startCooking()
    {
        int veg = currentLockedPlayer.PlayerInventory.peekNextItem();
        if (!Vegetables.isRawVegetable(veg))
        {
            resetData();
            yield break;
        }

        veg = currentLockedPlayer.PlayerInventory.getNextItem();
        preparingVegData = GameManager.getVegetableData(veg);
        prepareTimeLeft = preparingVegData.preparationTime;
                    
        yield return new WaitForEndOfFrame();

        while (true)
        {
            if ((stoveStandingSpot.position - currentLockedPlayer.transform.position).magnitude > 0.02)
            {
                currentLockedPlayer.transform.position += (stoveStandingSpot.position - currentLockedPlayer.transform.position) 
                    * Time.deltaTime;
                currentLockedPlayer.transform.rotation = stoveStandingSpot.rotation;
            }
            prepareTimeLeft -= Time.deltaTime;
            if(prepareTimeLeft <=0)
            {
                ingredientPrepared(veg);
                if (currentLockedPlayer.PlayerInventory.hasAnyItem())
                {
                    veg = currentLockedPlayer.PlayerInventory.getNextItem();
                    preparingVegData = GameManager.getVegetableData(veg);
                    prepareTimeLeft = preparingVegData.preparationTime;                    
                }
                else
                    break;
            }
            yield return new WaitForEndOfFrame();
        }
        resetData();
    }

    public float getCurrentProgress()
    {
        return preparingVegData != null ? 1 - (prepareTimeLeft / preparingVegData.preparationTime) : 0;
    }

    private void ingredientPrepared(int vegMask)
    {
        currentStack |= vegMask;
        Debug.Log("Chopped Vegetable " + vegMask + " Salad " + currentStack);
        if(ingredientPreparedEvent!= null)
            ingredientPreparedEvent.Invoke(vegMask, currentStack);
    }

    private void saladPickedUp()
    {
        Debug.Log("Picked up salad " + currentStack);
        if (saladPickedUpEvent != null)
            saladPickedUpEvent.Invoke(currentStack);
    }
}

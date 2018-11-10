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

    public SaladCanvasScript saladCanvas;

    private int currentStack=0;

    private float prepareTimeLeft=0;

    private VegetableData preparingVegData;

    private Player currentLockedPlayer;

    void Start()
    {
        if (saladCanvas == null)
            throw new Exception("Missing Salad Canvas!");

        saladCanvas.gameObject.SetActive(false);
        saladCanvas.IsProgressVisible = new SaladCanvasScript.ProgressVisibilityDelegate(isSaladBeingPrepared);
        saladCanvas.ProgressionRatio = new SaladCanvasScript.ProgressionDelegate(getCurrentProgress);
    }

    private void resetData()
    {
        prepareTimeLeft = 0;
        preparingVegData = null;
        ((PlayerController)currentLockedPlayer.controller).setInputActive(true);
        currentLockedPlayer = null;
    }

    public override bool canInteract(GameObject interactor)
    {
        if (!base.canInteract(interactor))
            return false;
        Player player = interactor.GetComponent<Player>();
        if (player.PlayerInventory.hasAnyItem())
        {
            int veg = player.PlayerInventory.peekNextItem();
            // Only if non repeating ingredients are allowed
#if GAME_DEBUG
            Debug.Log("Checking if chef is adding same ingredient to salad");
#endif
            return (currentStack & veg) == 0;
        }
        else
            return currentStack != 0;
    }

    public override void interact(GameObject interactor)
    {
        base.interact(interactor);
        Player player = interactor.GetComponent<Player>();
        currentLockedPlayer = player;

        if (currentLockedPlayer.PlayerInventory.hasAnyItem())
        {
            StartCoroutine(startCooking());
        }
        else
        {
            currentLockedPlayer.PlayerInventory.addItem(currentStack);
            saladPickedUp();
            currentStack = 0;
            resetData();
        }
    }

    public bool isSaladBeingPrepared()
    {
        return currentLockedPlayer != null;
    }

    IEnumerator startCooking()
    {
        int veg = currentLockedPlayer.PlayerInventory.peekNextItem();
        if (!Vegetables.isRawVegetable(veg))
        {
            resetData();
            yield break;
        }

        if (!saladCanvas.gameObject.activeSelf)
        {
            saladCanvas.gameObject.SetActive(true);
        }

        veg = currentLockedPlayer.PlayerInventory.getNextItem();
        currentStack |= (int)Vegies.oneItemSaladHandle;
        preparingVegData = GameManager.getVegetableData(veg);
        prepareTimeLeft = preparingVegData.preparationTime;
                    
        yield return new WaitForEndOfFrame();

        while (true)
        {
            if ((stoveStandingSpot.position - currentLockedPlayer.transform.position).magnitude > 0.02)
            {
                currentLockedPlayer.transform.position += 2*(stoveStandingSpot.position - currentLockedPlayer.transform.position) 
                    * Time.deltaTime;
                currentLockedPlayer.transform.rotation = stoveStandingSpot.rotation;
            }
            prepareTimeLeft -= Time.deltaTime;
            if(prepareTimeLeft <=0)
            {
                ingredientPrepared(veg);
                if (currentLockedPlayer.PlayerInventory.hasAnyItem())
                {
                    veg = currentLockedPlayer.PlayerInventory.peekNextItem();

                    // if next ingredient is already added then stop cooking
                    if ((currentStack & veg) != 0)
                    {
                        break;
                    }
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
#if GAME_DEBUG
        Debug.Log("Chopped Vegetable " + vegMask + " Salad " + currentStack);
#endif
        if(ingredientPreparedEvent!= null)
            ingredientPreparedEvent.Invoke(vegMask, currentStack);

        saladCanvas.addItem(GameManager.getVegetableData(vegMask));
    }

    private void saladPickedUp()
    {
#if GAME_DEBUG
        Debug.Log("Picked up salad " + currentStack);
#endif
        if (saladPickedUpEvent != null)
            saladPickedUpEvent.Invoke(currentStack);

        saladCanvas.clearSalad();
        saladCanvas.gameObject.SetActive(false);
    }
}

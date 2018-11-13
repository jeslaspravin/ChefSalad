using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item that can only be used by the owner player of that item
/// </summary>
public class RestrictedUsageItem : UsableItem {

    /// <summary>
    /// Unique ID of player who can use this item.
    /// </summary>
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

/// <summary>
/// Stove where player prepares and mixes the salad together.
/// </summary>
public class Stove : RestrictedUsageItem
{
    /// <summary>
    /// Delegate Type that will be used to send ingredient prepared events.
    /// </summary>
    /// <param name="prepdVeg">Prepared vegetable equivalent integer.</param>
    /// <param name="saladMask">Salad equivalent integer value.</param>
    public delegate void IngredientPreparedDelegate(int prepdVeg, int saladMask);

    /// <summary>
    /// Event that gets triggered when an ingredient is prepared in this stove by player.
    /// </summary>
    public event IngredientPreparedDelegate ingredientPreparedEvent;

    /// <summary>
    /// Delegate Type that will be used to send salad related events
    /// </summary>
    /// <param name="saladMask"></param>
    public delegate void SaladDelegate(int saladMask);

    /// <summary>
    /// Event that gets triggered when player picks up the prepared salad.
    /// </summary>
    public event SaladDelegate saladPickedUpEvent;

    public Transform stoveStandingSpot;

    /// <summary>
    /// UI script that displays currently cooking/cooked salad information.
    /// </summary>
    public SaladCanvasScript saladCanvas;

    /// <summary>
    /// Currently Preparing/Prepared salad data
    /// </summary>
    private int currentStack=0;
    /// <summary>
    /// Current ingredient count in the salad.
    /// </summary>
    private int currentIngredientCount = 0;

    /// <summary>
    /// Time left to prepare currently preparing ingredient for salad.
    /// </summary>
    private float prepareTimeLeft=0;

    /// <summary>
    /// Currently preparing vegetable data for salad.
    /// </summary>
    private VegetableData preparingVegData;

    /// <summary>
    /// Locked player reference
    /// </summary>
    private Player currentLockedPlayer;

    /// <summary>
    /// Flag to ensure that player will not interact in same frame causing undesirable cases.
    /// </summary>
    private bool isBeingUsed = false;

    void Start()
    {
        if (saladCanvas == null)
            throw new Exception("Missing Salad Canvas!");

        saladCanvas.gameObject.SetActive(false);
        saladCanvas.IsProgressVisible = new SaladCanvasScript.ProgressVisibilityDelegate(isSaladBeingPrepared);
        saladCanvas.ProgressionRatio = new SaladCanvasScript.ProgressionDelegate(getCurrentProgress);
    }

    /// <summary>
    /// Resets stove data this is usually done when player finishes interaction with stove and unlocks player controller
    /// </summary>
    private void resetData()
    {
        prepareTimeLeft = 0;
        preparingVegData = null;
        ((PlayerController)currentLockedPlayer.controller).setInputActive(true);
        currentLockedPlayer = null;
        isBeingUsed = false;
    }

    public override bool canInteract(GameObject interactor)
    {
        if (!base.canInteract(interactor) || isBeingUsed)
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
        isBeingUsed = true;
        base.interact(interactor);
        Player player = interactor.GetComponent<Player>();
        currentLockedPlayer = player;

        /*
         *If current Ingredient count is less than max ingredient allowed count and player has vegetable then
         *start cooking else pickup the salad
         */
        if (currentLockedPlayer.PlayerInventory.hasAnyItem() && currentIngredientCount < ChefSaladManager.MAX_INGREDIENT_COUNT)
        {
            StartCoroutine(startCooking());
        }
        else// If player has no vegetable to be added to salad then let him pick the prepared one in stove.
        {
            currentLockedPlayer.PlayerInventory.addItem(currentStack);
            saladPickedUp();
            currentStack = 0;
            currentIngredientCount = 0;
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
        if (!Vegetables.isRawVegetable(veg))// if only it is vegetable then allow cooking.
        {
            resetData();
            yield break;
        }

        // Enable salad canvas
        if (!saladCanvas.gameObject.activeSelf)
        {
            saladCanvas.gameObject.SetActive(true);
        }

        veg = currentLockedPlayer.PlayerInventory.getNextItem();
        currentStack |= (int)Vegies.oneItemSaladHandle;// Adding salad handler enum value
        preparingVegData = ChefSaladManager.getVegetableData(veg);
        prepareTimeLeft = preparingVegData.preparationTime;
        
        // Wait and pass on this frame so calculation start from next frame.
        yield return new WaitForEndOfFrame();

        while (true)
        {
            // Moving player to stove's preparation point
            if ((stoveStandingSpot.position - currentLockedPlayer.transform.position).magnitude > 0.02)
            {
                currentLockedPlayer.transform.position += 2 * (stoveStandingSpot.position - currentLockedPlayer.transform.position)
                    * Time.deltaTime;
                currentLockedPlayer.transform.rotation = stoveStandingSpot.rotation;
            }
            prepareTimeLeft -= Time.deltaTime;
            if(prepareTimeLeft <=0)// if preparation time ends it means vegetable is prepared.
            {
                ingredientPrepared(veg);
                // Check for if player has more vegetable to add if yes then continue with next item he has.
                if (currentLockedPlayer.PlayerInventory.hasAnyItem())
                {
                    veg = currentLockedPlayer.PlayerInventory.peekNextItem();

                    // if next ingredient is already added then stop cooking or if salad still has space to add ingredient
                    if ((currentStack & veg) != 0 || currentIngredientCount >= ChefSaladManager.MAX_INGREDIENT_COUNT)
                    {
                        break;
                    }
                    veg = currentLockedPlayer.PlayerInventory.getNextItem();
                    preparingVegData = ChefSaladManager.getVegetableData(veg);
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
        // Adding the vegetable to salad and triggering the event.
        currentStack |= vegMask;
        currentIngredientCount++;
#if GAME_DEBUG
        Debug.Log("Chopped Vegetable " + vegMask + " Salad " + currentStack);
#endif
        if(ingredientPreparedEvent!= null)
            ingredientPreparedEvent.Invoke(vegMask, currentStack);

        saladCanvas.addItem(ChefSaladManager.getVegetableData(vegMask));
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

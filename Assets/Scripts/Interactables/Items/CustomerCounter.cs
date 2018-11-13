using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Counter in which customer comes in and orders the salad.
/// </summary>
public class CustomerCounter : UsableItem {

    /// <summary>
    /// Reference to waiting customer NPC controller
    /// </summary>
    private NpcController waitingController=null;

    /// <summary>
    /// Time waited so far by customer.
    /// </summary>
    private float timeConsumed = 0;

    /// <summary>
    /// Cached penalty score at time of customer placing order
    /// </summary>
    private float penaltyOnFailure = 0;

    /// <summary>
    /// Cached reward score at time of customer placing order
    /// </summary>
    private float scoreOnSuccess = 0;

    /// <summary>
    /// Multiplier that determines the rate at which the waiting time of player runs out,Angry customer will have this >1
    /// </summary>
    private float timeMultiplier = 1;

    /// <summary>
    /// Salad display script that displays salad asked by customer in counter.
    /// </summary>
    public SaladCanvasScript saladCanvas;

    /// <summary>
    /// Customer patience/wait time progress bar.
    /// </summary>
    public Slider progressionBar;

    /// <summary>
    /// Slider fill image to get control to color of sprite.
    /// </summary>
    public Image sliderFillImage;

    /// <summary>
    /// Temporary holder for list of player Ids that gets affected by score resulting from customer either reward or penalty.
    /// </summary>
    private List<Guid> playerIds=new List<Guid>();

    /// <summary>
    /// Delegate type that will be used to send customer leaving counter event.
    /// </summary>
    /// <param name="score">Score that customer gives to player</param>
    /// <param name="timeRatio">Ratio of time consumed to Maximum wait time of customer</param>
    /// <param name="playerIds">List of Player IDs that gets affected by this score</param>
    /// <param name="controller">NPC controller that is controlling the NPC</param>
    public delegate void CustomerLeavingDelegate(float score, float timeRatio, List<Guid> playerIds,NpcController controller);

    /// <summary>
    /// Customer leaving counter event
    /// </summary>
    public event CustomerLeavingDelegate onCustomerLeaving;

    public bool isCounterFree()
    {
        return waitingController == null;
    }

    /// <summary>
    /// Resets data of this counter.Called when customer is leaving and when new customer comes in.
    /// </summary>
    private void resetData()
    {
        timeConsumed = 0;
        penaltyOnFailure = 0;
        scoreOnSuccess = 0;
        timeMultiplier = 1;
        saladCanvas.clearSalad();
        playerIds.Clear();
    }

	// Use this for initialization
	void Start () {
        progressionBar.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        // Just updating customer waiting patience bar and time customer has waited.
		if(waitingController != null)
        {
            timeConsumed += timeMultiplier*Time.deltaTime;

            float progressRatio = Mathf.Clamp01(timeConsumed / waitingController.attributes.maxWaitTime);
            sliderFillImage.color = new Color(progressRatio, 1 - progressRatio, 0, 1);
            progressionBar.value = progressRatio;

            if ((waitingController.attributes.maxWaitTime-timeConsumed)<=0)
            {
                waitOver(-penaltyOnFailure);
            }
        }
	}

    public override bool canInteract(GameObject interactor)
    {
        BasicPawn pawn = interactor.GetComponent<BasicPawn>();
        if(pawn.controller is NpcController) // If is customer check only if another customer is already in counter.
        {
#if GAME_DEBUG
            Debug.Log("Checking NpcController");
#endif
            return waitingController == null;
        }else // If Player then check if there is any customer,the customer is still waiting and Player has salad.
        {
#if GAME_DEBUG
            Debug.Log("Checking PlayerController and Inventory");
#endif
            Player player = (Player)pawn;
            return waitingController != null && (waitingController.attributes.maxWaitTime-timeConsumed)>0 && 
                player.PlayerInventory.hasAnySalad();
        }
    }

    public void waitOver(float score)
    {
        if (onCustomerLeaving != null)
            onCustomerLeaving.Invoke(score, timeConsumed / waitingController.attributes.maxWaitTime, playerIds,waitingController);
        progressionBar.gameObject.SetActive(false);
        waitingController.IsMobile = true;
        waitingController = null;
        resetData();
    }

    public override void interact(GameObject interactor)
    {
        BasicPawn pawn = interactor.GetComponent<BasicPawn>();
        if (pawn.controller is NpcController) // If is NPC then set all data that customer is bringing in to counter.
        {
            resetData();
            waitingController = (NpcController)pawn.controller;

            progressionBar.gameObject.SetActive(true);

            // Make controller stop
            waitingController.IsMobile = false;
            // Hard coded value to make npc go off screen after waiting
            waitingController.MoveTo = new Vector3(transform.parent.position.x, transform.parent.position.y + 7,
                waitingController.GetControlledPawn.transform.position.z);

            int itemMask = waitingController.attributes.salad;
            for (int i = 0; i < (int)Vegies.count; i++)// Using bit shifting to find ingredients.
            {
                if ((itemMask & 1) > 0)
                {
                    int veg = (int)Mathf.Pow(2, i);
                    VegetableData vegData = ChefSaladManager.getVegetableData(veg);
                    scoreOnSuccess += vegData.Reward;
                    penaltyOnFailure += vegData.Penalty;
                    saladCanvas.addItem(vegData);
                }
                itemMask >>= 1;
            }
            scoreOnSuccess += scoreOnSuccess*waitingController.attributes.scoreMultiplier;
            penaltyOnFailure += waitingController.attributes.scoreMultiplier;
        }
        else //If it is player then get salad and do the processing.
        {
            Player player = (Player)pawn;
            int salad=player.PlayerInventory.getFirstItem(false) & (~(int)Vegies.oneItemSaladHandle);
#if GAME_DEBUG
            Debug.Log("Player Serving salad "+salad+" Customer requested salad "+waitingController.attributes.salad);
#endif
            if((salad ^ waitingController.attributes.salad)==0) // If salad ingredient match required ingredient do below
            {
                playerIds.Clear();
                playerIds.Add(player.GetId);
                waitOver(scoreOnSuccess);
            }else// If not matching then add the player to player ids that needs to be affected by penalty.
            {
                playerIds.Add(player.GetId);
                if (timeMultiplier == 1)// Only increase time multiplier once rest of failure does not affect
                    timeMultiplier *= waitingController.attributes.failureTimeMultiplier;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerCounter : UsableItem {

    private NpcController waitingController=null;

    private float timeConsumed = 0;

    private float penaltyOnFailure = 0;

    private float scoreOnSuccess = 0;

    private float timeMultiplier = 1;

    public SaladCanvasScript saladCanvas;

    public Slider progressionBar;
    public Image sliderFillImage;

    private List<Guid> playerIds=new List<Guid>();

    public delegate void CustomerLeavingDelegate(float score, float timeRatio, List<Guid> playerIds,NpcController controller);

    public event CustomerLeavingDelegate onCustomerLeaving;

    public bool isCounterFree()
    {
        return waitingController == null;
    }

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
        if(pawn.controller is NpcController)
        {
#if GAME_DEBUG
            Debug.Log("Checking NpcController");
#endif
            return waitingController == null;
        }else
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
        if (pawn.controller is NpcController)
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
            for (int i = 0; i < (int)Vegies.count; i++)
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
        else
        {
            Player player = (Player)pawn;
            int salad=player.PlayerInventory.getFirstItem(false) & (~(int)Vegies.oneItemSaladHandle);
#if GAME_DEBUG
            Debug.Log("Player Serving salad "+salad+" Customer requested salad "+waitingController.attributes.salad);
#endif
            if((salad ^ waitingController.attributes.salad)==0)
            {
                playerIds.Clear();
                playerIds.Add(player.GetId);
                waitOver(scoreOnSuccess);
            }else if(timeMultiplier == 1)
            {
                playerIds.Add(player.GetId);
                timeMultiplier *= waitingController.attributes.failureTimeMultiplier;
            }
        }
    }
}

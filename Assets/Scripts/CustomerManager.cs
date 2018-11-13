using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Customer manager controls how often customers will be spawned and on which counter to spawn.
/// </summary>
public class CustomerManager : MonoBehaviour {

    /// <summary>
    /// Pooled NPC pawns,That are ready to be controlled.
    /// </summary>
    private Queue<NpcPawn> npcsAvailable=new Queue<NpcPawn>();

    /// <summary>
    /// NPC Controllers that are returning out from counter.
    /// </summary>
    private List<NpcController> returningNpcControllers = new List<NpcController>();

    /// <summary>
    /// NPC Pawn prefab.
    /// </summary>
    public GameObject npcPrefab;
    
    /// <summary>
    /// Minimum and maximum score multiplier that this customer provides to vegetables on top of its original score.
    /// </summary>
    public float minScoreMultiplier=0.3f;
    public float maxScoreMultiplier=0.75f;

    /// <summary>
    ///  Minimum and maximum rate of patience/timer drop multiplier of this customer upon being served with incorrect salad.
    /// </summary>
    public float minFailureTimeMultiplier=1.2f;
    public float maxFailureTimeMultiplier=1.8f;

    // In seconds
    public float spawnNpcInterval=7;    

	// Use this for initialization
	void Start () {
        spawnNpcPawns();
        InvokeRepeating("createCustomer", 3, spawnNpcInterval);
	}
	
	// Update is called once per frame
	void Update () {
        returningNpcControllers.RemoveAll((NpcController controller) =>
        {
            if((controller.MoveTo-controller.GetControlledPawn.transform.position).magnitude<0.1f)// If returning NPC has reached 
            // location off screen they releases controlled pawn.
            {
                npcsAvailable.Enqueue((NpcPawn)controller.GetControlledPawn);
                controller.releasePawn();
                controller.IsMobile = false;
                return true;
            }
            return false;
        });
	}

    /// <summary>
    /// Method that gets invoked from Customer counter when NPC leaves the counter.
    /// </summary>
    /// <param name="score">Score that customer gives to player</param>
    /// <param name="timeRatio">Ratio of time consumed to Maximum wait time of customer</param>
    /// <param name="playerIds">List of Player IDs that gets affected by this score</param>
    /// <param name="npcController">NPC controller that is controlling the NPC</param>
    private void onNpcReturning(float score, float timeRatio, List<Guid> playerIds, NpcController npcController)
    {
        npcController.counterTransform.gameObject.GetComponentInChildren<CustomerCounter>().onCustomerLeaving -= onNpcReturning;
        returningNpcControllers.Add(npcController);
    }

    /// <summary>
    /// Creates new NPC Attributes and sends the customer to chosen counter.
    /// </summary>
    private void createCustomer()
    {
        int index = -1;
        GameObject counter=ChefSaladManager.selectRandomUsableCounter(out index);// Select random available counter.
        if (counter == null)
            return;

        // Register to customer leaving that counter event of the counter.
        counter.GetComponentInChildren<CustomerCounter>().onCustomerLeaving += onNpcReturning;

        // Get NPC Controller of the counter.
        NpcController controller=(NpcController)ChefSaladManager.getNpcController(index);

        // Get Next NPC pawn from queue.
        NpcPawn pawn = npcsAvailable.Dequeue();

        // Set Pawn's start position.
        pawn.transform.position = new Vector3(counter.transform.position.x, counter.transform.position.y + 7, pawn.transform.position.z);

        // Set controller variables and attributes.
        controller.MoveTo = new Vector3(counter.transform.position.x, counter.transform.position.y, pawn.transform.position.z);
        controller.IsMobile = true;
        controller.counterTransform = counter.transform;
        controller.attributes.failureTimeMultiplier = UnityEngine.Random.Range(minFailureTimeMultiplier, maxFailureTimeMultiplier);
        controller.attributes.scoreMultiplier = UnityEngine.Random.Range(minScoreMultiplier, maxScoreMultiplier);
        controller.attributes.maxWaitTime = 0;

        // Chose random number of ingredients.
        int noOfIngredient = UnityEngine.Random.Range(1, ChefSaladManager.MAX_INGREDIENT_COUNT);
        int salad = 0;
        // Selecting vegetable that the salad needs to be made of randomly and not repeating.
        for(int i = 0,trial = 0;i<noOfIngredient;i++,trial=0)
        {
            int choosen = 0;
            int pow = 0;
            VegetableData vegData = null;
            while (trial < 3 && choosen==0)
            {
                pow = UnityEngine.Random.Range(0, (int)Vegies.count);
                choosen = (int)Mathf.Pow(2, pow);
                if((choosen & salad) == 0)
                {
                    salad |= choosen;
                    vegData=ChefSaladManager.getVegetableData(choosen);
                    controller.attributes.maxWaitTime += (int)(vegData.preparationTime + (vegData.preparationTime * 4f));
                }
                else
                {
                    choosen = 0;
                    trial++;
                }
            }

            if (choosen != 0)
                continue;

            pow = UnityEngine.Random.Range(0, (int)Vegies.count);
            choosen = (int)Mathf.Pow(2, pow);

            while((choosen & salad) != 0)
            {
                pow++;
                choosen= (int)Mathf.Pow(2, pow % (int)Vegies.count);
            }
            vegData = ChefSaladManager.getVegetableData(choosen);
            controller.attributes.maxWaitTime += (int)(vegData.preparationTime + (vegData.preparationTime * 4f));
            salad |= choosen;
        }
        controller.attributes.salad = salad;

        // Starting to control the selected pawn from selected controller.
        controller.controlPawn(pawn);
    }

    private void spawnNpcPawns()
    {
        Vector3 location = new Vector3(1000, 1000, -2);
        for (int i = 0; i < 10; i++)
        {
            GameObject go = Instantiate(npcPrefab, location,Quaternion.identity);
            go.name = "NPC" + (i + 1);
            npcsAvailable.Enqueue(go.GetComponent<NpcPawn>());
        }
    }


}

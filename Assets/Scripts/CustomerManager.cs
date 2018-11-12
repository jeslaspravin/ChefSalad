﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour {

    private Queue<NpcPawn> npcsAvailable=new Queue<NpcPawn>();

    private List<NpcController> returningNpcControllers = new List<NpcController>();

    public GameObject npcPrefab;
    
    public float minScoreMultiplier=0.3f;
    public float maxScoreMultiplier=0.75f;

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
            if((controller.MoveTo-controller.GetControlledPawn.transform.position).magnitude<0.1f)
            {
                npcsAvailable.Enqueue((NpcPawn)controller.GetControlledPawn);
                controller.releasePawn();
                controller.IsMobile = false;
                return true;
            }
            return false;
        });
	}

    private void onNpcReturning(float score, float timeRatio, List<Guid> playerIds, NpcController npcController)
    {
        npcController.counterTransform.gameObject.GetComponentInChildren<CustomerCounter>().onCustomerLeaving -= onNpcReturning;
        returningNpcControllers.Add(npcController);
    }

    private void createCustomer()
    {
        int index = -1;
        GameObject counter=ChefSaladManager.selectRandomUsableCounter(out index);
        if (counter == null)
            return;


        counter.GetComponentInChildren<CustomerCounter>().onCustomerLeaving += onNpcReturning;

        NpcController controller=(NpcController)ChefSaladManager.getNpcController(index);

        NpcPawn pawn = npcsAvailable.Dequeue();

        pawn.transform.position = new Vector3(counter.transform.position.x, counter.transform.position.y + 7, pawn.transform.position.z);

        controller.MoveTo = new Vector3(counter.transform.position.x, counter.transform.position.y, pawn.transform.position.z);
        controller.IsMobile = true;
        controller.counterTransform = counter.transform;
        controller.attributes.failureTimeMultiplier = UnityEngine.Random.Range(minFailureTimeMultiplier, maxFailureTimeMultiplier);
        controller.attributes.scoreMultiplier = UnityEngine.Random.Range(minScoreMultiplier, maxScoreMultiplier);
        controller.attributes.maxWaitTime = 0;

        int noOfIngredient = UnityEngine.Random.Range(1, ChefSaladManager.MAX_INGREDIENT_COUNT);
        int salad = 0;
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

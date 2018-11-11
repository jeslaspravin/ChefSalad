using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Prefab that will be used to spawn player
    public GameObject playerPrefab;
    // Prefab used to spawn player controller
    public GameObject playerControllerPrefab;

    public GameObject npcControllerPrefab;

    public List<VegetableData> vegetableDataAssets;

    public TrashCan trashCan;

    public CollectibleSpawner collectibleSpawner;

    public static GameManager manager;
    
    private Dictionary<Guid, BasicController> playersList=new Dictionary<Guid, BasicController>();

    [System.Serializable]
    public struct PlayerSpawnData
    {
        public string playerName;
        public string playerInputPrefix;
        public Color spriteColor;

        public RestrictedUsageItem stove;
        public RestrictedUsageItem dish;
        public Transform spawnTransform;
        public PlayerHudScript playerHud;
    }

    public List<PlayerSpawnData> playersToSpawn;

    public List<GameObject> customerCounters;
    private List<BasicController> npcControllers=new List<BasicController>();

    private void Awake()
    {
        if (manager != null)
        {
            Destroy(gameObject);
            return;
        }

        manager = this;

        trashCan.onItemTrashed += itemTrashed;

        foreach(PlayerSpawnData psd in playersToSpawn)
        {
            BasicPawn pawn = ((GameObject)Instantiate(playerPrefab,psd.spawnTransform.position,Quaternion.identity)).GetComponent<BasicPawn>();
            if (!pawn)
                throw new Exception("Add proper Pawn Prefab in game manager");
            pawn.name = psd.playerName;
            pawn.GetComponent<SpriteRenderer>().color = psd.spriteColor;

            PlayerController controller = ((GameObject)Instantiate(playerControllerPrefab)).GetComponent<PlayerController>();
            if (!controller)
                throw new Exception("Add proper Pawn Controller Prefab in game manager");
            controller.name = psd.playerName+"Controller";

            InputManager inputManager = new GameObject(controller.name + "InputManager").AddComponent<InputManager>();

            playersList.Add(controller.GetID, controller);

            controller.InputPrefix = psd.playerInputPrefix;
            controller.playerName = psd.playerName;
            controller.controlPawn(pawn);
            controller.setupInputs(inputManager);

            psd.playerHud.PlayerInventory = controller.PlayerInventory;
            psd.playerHud.PlayerState = controller.PlayerState;

            psd.stove.userId = psd.dish.userId = controller.GetID;
            
        }

        foreach(GameObject counterObj in customerCounters)
        {
            CustomerCounter counter = counterObj.GetComponentInChildren<CustomerCounter>();
            GameObject go = Instantiate(npcControllerPrefab);
            go.name = counter.name + "NpcController";
            BasicController controller = go.GetComponent<BasicController>();
            npcControllers.Add(controller);
            counter.onCustomerLeaving += customerLeft;
        }
    }

    public static VegetableData getVegetableData(int vegetable)
    {
        return manager.vegetableDataAssets.Find((VegetableData data) => { return data.vegetableMask == (Vegies)vegetable; });
    }

    private void itemTrashed(Guid guid,float cost)
    {
        float newCost = -cost;
#if GAME_DEBUG
        Debug.Log(guid.ToString() + " Cost "+ newCost);
#endif
        PlayerController controller = (PlayerController)(playersList[guid]);
        controller.PlayerState.addScore(newCost>0?0:newCost);
    }

    private void customerLeft(float score,float timeRatio,List<Guid> playerIds, NpcController npcController)
    {
        if(playerIds.Count==0)
        {
            // Customer left after time out case
            foreach(KeyValuePair<Guid,BasicController> entry in playersList)
            {
                pushScoreToPlayer(score, entry.Value);
            }
        }else if(score>0)
        {
            // Successfully served case
            BasicController controller = playersList[playerIds[0]];
            if(timeRatio<=0.7f)
            {
                collectibleSpawner.spawnCollectible(playerIds[0]);
            }
            pushScoreToPlayer(score, controller);
        }else
        {
            // Failed with wrong combination,2x penalty
            foreach(Guid guid in playerIds)
            {
                BasicController controller = playersList[guid];
                pushScoreToPlayer(2 * score, controller);
            }
        }
    }

    private void pushScoreToPlayer(float score,BasicController controller)
    {
        PlayerController playerController = (PlayerController)controller;
        playerController.PlayerState.addScore(score);
        // TODO : Spawn score pop in player location
    }

    public static GameObject selectRandomUsableCounter(out int counterIndex)
    {
        int rem = manager.customerCounters.Count;
        int currentIndex=0;
        while (rem > 0)
        {
            currentIndex += UnityEngine.Random.Range(0, rem);
            int index = currentIndex % manager.customerCounters.Count;
            CustomerCounter counter = manager.customerCounters[index].GetComponentInChildren<CustomerCounter>();
            if(counter.isCounterFree() && !manager.npcControllers[index].IsControllingPawn)
            {
                counterIndex = index;
                return manager.customerCounters[index];
            }
            rem--;
        }
        counterIndex = -1;
        return null;
    }

    public static BasicController getNpcController(int index)
    {
        return manager.npcControllers.Count > index ? manager.npcControllers[index] : null;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDestroy()
    {
        if (trashCan != null)
            trashCan.onItemTrashed -= itemTrashed;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Prefab that will be used to spawn player
    public GameObject playerPrefab;
    // Prefab used to spawn player controller
    public GameObject playerControllerPrefab;

    public List<VegetableData> vegetableDataAssets;

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

    private void Awake()
    {
        if (manager != null)
            Destroy(this);

        manager = this;

        foreach(PlayerSpawnData psd in playersToSpawn)
        {
            // TODO : Change it to spawn at spawn locations
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
    }

    public static VegetableData getVegetableData(int vegetable)
    {
        return manager.vegetableDataAssets.Find((VegetableData data) => { return data.vegetableMask == (Vegies)vegetable; });
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

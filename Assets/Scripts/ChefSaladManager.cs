using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChefSaladManager : MonoBehaviour {

    // Prefab that will be used to spawn player
    public GameObject playerPrefab;
    // Prefab used to spawn player controller
    public GameObject playerControllerPrefab;

    public GameObject npcControllerPrefab;

    public GameObject scoreFlierPrefab;

    public List<VegetableData> vegetableDataAssets;

    public TrashCan trashCan;

    public CollectibleSpawner collectibleSpawner;

    public GameObject hudCanvas;
    public GameObject gameEndCanvas;
    private ResultDisplay resultScreen;

    public static ChefSaladManager manager;
    
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

    private List<ScoreCardData> scordCards = null;

    private string HIGHSCORE_SAVE_PATH;

    private void Awake()
    {
        if (manager != null)
        {
            Destroy(gameObject);
            return;
        }

        HIGHSCORE_SAVE_PATH = Path.Combine(Application.persistentDataPath, "highscores.sav");

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
            controller.PlayerState.onScoreChanged += scoredChangedFor;

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

        resultScreen=gameEndCanvas.GetComponentInChildren<ResultDisplay>();
        resultScreen.restartButton.onClick.AddListener(restartLevel);
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
        spawnScoreFlier(score, playerController.GetControlledPawn.transform.position);
    }

    private void scoredChangedFor(float score,PlayerState playerState)
    {
        BasicController controller = playerState.gameObject.GetComponent<BasicController>();
        spawnScoreFlier(score, controller.GetControlledPawn.transform.position);
    }

    private void spawnScoreFlier(float score,Vector3 location)
    {
        GameObject go = Instantiate(scoreFlierPrefab, location, Quaternion.identity);
        go.GetComponent<ScoreFlier>().setScoreValue(score);
        Destroy(go, 3);
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
    void Start()
    {
        scordCards = loadData<List<ScoreCardData>>(HIGHSCORE_SAVE_PATH);
        if (scordCards == null)
            scordCards = new List<ScoreCardData>();
        unpauseGame();
    }
    // Update is called once per frame
    void Update () {
        bool timedOut = true;
		foreach(KeyValuePair<Guid,BasicController> entry in playersList)
        {
            PlayerController pc = ((PlayerController)entry.Value);
            if (pc.PlayerState.TimeLeft>0)
            {
                timedOut = false;
            }else
            {
                pc.setInputActive(false);
            }
        }
        if(timedOut && Time.timeScale>0)
        {
            onModeTimedOut();
        }
	}

    void OnDestroy()
    {
        if (trashCan != null)
            trashCan.onItemTrashed -= itemTrashed;
    }

    private void onModeTimedOut()
    {
        pauseGame();
#if GAME_DEBUG
        Debug.Log("Game Timed out");
#endif
        hudCanvas.SetActive(false);

        string playerWon="";
        float score=float.MinValue;
        foreach (KeyValuePair<Guid, BasicController> entry in playersList)
        {
            float playerScore = ((PlayerController)entry.Value).PlayerState.PlayerScore;
            if (playerScore > score)
            {
                score = playerScore;
                playerWon = entry.Value.GetControlledPawn.name;
            }
            scordCards.Add(new ScoreCardData(-1, entry.Value.GetControlledPawn.name, playerScore));
        }
        scordCards.Sort((x, y) => { return -1 * x.CompareTo(y); });
        if (scordCards.Count > 10)
            scordCards.RemoveRange(10, scordCards.Count - 10);
        gameEndCanvas.SetActive(true);
        resultScreen.refreshData(playerWon, score, ref scordCards);
        saveData(HIGHSCORE_SAVE_PATH, scordCards);
    }

    private void pauseGame()
    {
        Time.timeScale = 0;
    }

    private void unpauseGame()
    {
        Time.timeScale = 1;
    }
    private void restartLevel()
    {
        manager = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex,LoadSceneMode.Single);
    }

    private void saveData<T>(string path,T data)
    {
        FileStream stream = File.Create(path);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, data);
        stream.Close();
    }

    private T loadData<T>(string path)
    {
        if (!File.Exists(path))
        {
            Debug.Log("No Save");
            return default(T);
        }
        FileStream stream = File.Open(path, FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();
        T data = (T)formatter.Deserialize(stream);
        stream.Close();
        return data;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// This is the main class where all the game mode of this chef salad is done
/// </summary>
public class ChefSaladManager : MonoBehaviour {

    // Prefab that will be used to spawn player
    public GameObject playerPrefab;
    // Prefab used to spawn player controller
    public GameObject playerControllerPrefab;

    /// <summary>
    /// Prefab used to spawn NPC controller
    /// One NPC controller is spawned for each Customer counter.
    /// So one can only be there at any given time at any counter
    /// </summary>
    public GameObject npcControllerPrefab;

    // Prefab used to spawn score flier
    public GameObject scoreFlierPrefab;

    // Scriptable object asset that contains all unique data of each vegetable.
    public List<VegetableData> vegetableDataAssets;

    // Reference to trashcan,holding reference here since it is common for all player in this level and as it has gameplay impact.
    public TrashCan trashCan;

    // Reference to collectible spawner that spawns 
    public CollectibleSpawner collectibleSpawner;

    // HUD Canvas object used for switching to result screen.
    public GameObject hudCanvas;
    // Result screen or game end high score displaying screen.
    public GameObject gameEndCanvas;
    // Result screen script that will be useful for setting correct values in UI.
    private ResultDisplay resultScreen;

    // Singleton reference to itself.
    public static ChefSaladManager manager;
    
    /// <summary>
    /// List of player mapped by their unique ID.
    /// </summary>
    private Dictionary<Guid, BasicController> playersList=new Dictionary<Guid, BasicController>();

    /// <summary>
    /// Spawn data for player this struct values will be altered in editor.
    /// </summary>
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

    /// <summary>
    /// List of players and their spawn data,Will be edited and serialized from editor.
    /// </summary>
    public List<PlayerSpawnData> playersToSpawn;

    /// <summary>
    /// List of reference to Customer counters in level.
    /// </summary>
    public List<GameObject> customerCounters;

    /// <summary>
    /// List of reference to NPC Controllers spawned for each customer counter.
    /// <para>These controller will take control of different NPC pawns as necessary.</para>
    /// </summary>
    private List<BasicController> npcControllers=new List<BasicController>();

    /// <summary>
    /// High score data that will be serialize and deserialized at runtime to provide user with proper high scores at result screen.
    /// </summary>
    private List<ScoreCardData> scordCards = null;

    /// <summary>
    /// Path in which high score data will be saved to.
    /// </summary>
    private string HIGHSCORE_SAVE_PATH;

    /// <summary>
    /// Assumed constant to maximum allowed ingredient in each salad.
    /// </summary>
    public const int MAX_INGREDIENT_COUNT = 3;

    private void Awake()
    {
        // Destroy in case more object exists as it is singleton.
        if (manager != null)
        {
            Destroy(gameObject);
            return;
        }

        HIGHSCORE_SAVE_PATH = Path.Combine(Application.persistentDataPath, "highscores.sav");

        manager = this;
        
        trashCan.onItemTrashed += itemTrashed;

        // Creating all Player ,Player controller,Player input manager combination and setting necessary data for them at start of game.
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
            controller.PlayerState.onScoreChanged += scoreChangedFor;

            psd.playerHud.PlayerInventory = controller.PlayerInventory;
            psd.playerHud.PlayerState = controller.PlayerState;

            psd.stove.userId = psd.dish.userId = controller.GetID;
            
        }

        // Creating all NPC controller needed,One for each counter.
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

    /// <summary>
    /// Helper method that gets and gives vegetable data for requested vegetable.
    /// </summary>
    /// <param name="vegetable">Vegetable for which data is needed</param>
    /// <returns>VegetableData</returns>
    public static VegetableData getVegetableData(int vegetable)
    {
        return manager.vegetableDataAssets.Find((VegetableData data) => { return data.vegetableMask == (Vegies)vegetable; });
    }

    /// <summary>
    /// Function that gets called when and Item gets trashed and this function in turn affects score of corresponding player.
    /// </summary>
    /// <param name="guid">GUID of player who trashed the item</param>
    /// <param name="cost">Cost of trashed item,This negates score from player state</param>
    private void itemTrashed(Guid guid,float cost)
    {
        float newCost = -cost;
#if GAME_DEBUG
        Debug.Log(guid.ToString() + " Cost "+ newCost);
#endif
        PlayerController controller = (PlayerController)(playersList[guid]);
        controller.PlayerState.addScore(newCost>0?0:newCost);
    }

    /// <summary>
    /// Function that gets called when customer leaves the counter.
    /// </summary>
    /// <param name="score">Score that customer gives to player</param>
    /// <param name="timeRatio">Ratio of time consumed to Maximum wait time of customer</param>
    /// <param name="playerIds">List of Player IDs that gets affected by this score</param>
    /// <param name="npcController">NPC controller that is controlling the NPC</param>
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

    /// <summary>
    /// Adds the provided score to player state and also spawns a flier that give player visual indication of score change
    /// </summary>
    /// <param name="score">Score of add</param>
    /// <param name="controller">Player Controller to which score needs to be added.</param>
    private void pushScoreToPlayer(float score,BasicController controller)
    {
        PlayerController playerController = (PlayerController)controller;
        playerController.PlayerState.addScore(score);
        spawnScoreFlier(score, playerController.GetControlledPawn.transform.position);
    }

    private void scoreChangedFor(float score,PlayerState playerState)
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

    /// <summary>
    /// Helper method to select and give free counter for customer to use.
    /// </summary>
    /// <param name="counterIndex">out counter index chosen</param>
    /// <returns>Counter Game object if found else null</returns>
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

    /// <summary>
    /// Gets NPC controller for provided Customer Counter Index.
    /// </summary>
    /// <param name="index">Index of Customer counter.</param>
    /// <returns>NPC Controller.</returns>
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
        // If player timed out disable their input controls
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
        if(timedOut && Time.timeScale>0) // If both player timed out start game end flow.
        {
            onModeTimedOut();
        }
	}

    void OnDestroy()
    {
        if (trashCan != null)
            trashCan.onItemTrashed -= itemTrashed;
    }

    /// <summary>
    /// On Timed out this method gets invoked to handle end game.
    /// </summary>
    private void onModeTimedOut()
    {
        pauseGame();
#if GAME_DEBUG
        Debug.Log("Game Timed out");
#endif
        hudCanvas.SetActive(false);// Hide HUD.

        string playerWon="";
        float score=float.MinValue;
        // Finding player with highest score.
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
        // Sorting all highscores in descending order.
        scordCards.Sort((x, y) => { return -1 * x.CompareTo(y); });
        if (scordCards.Count > 10)
            scordCards.RemoveRange(10, scordCards.Count - 10);
        // Enabling game end UI.
        gameEndCanvas.SetActive(true);
        // Pushing highscore and game end result to display script.
        resultScreen.refreshData(playerWon, score, ref scordCards);
        // Saving the new highscore list out to disk
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

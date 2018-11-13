using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player's unique data
/// </summary>
/// <remarks>
/// Not used in any of implementations.
/// </remarks>
public struct PlayerUData
{
    public string playerName;
    public Guid guid;

    public PlayerUData(string name,Guid id)
    {
        playerName = name;
        guid=id;
    }
}

/// <summary>
/// Player Controller
/// </summary>
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(PlayerState))]
public class PlayerController : BasicController {

    /// <summary>
    /// Input manager of this controller.
    /// <para>Each player controller has one input manager</para>
    /// </summary>
    private InputManager inputMngr;

    /// <summary>
    /// Inventory of Player
    /// </summary>
    private Inventory playerInventory;

    /// <summary>
    /// State of player.
    /// </summary>
    private PlayerState playerState;

    public PlayerState PlayerState
    {
        get { return playerState; }
    }
    
    public InputManager GetInputManager
    {
        get { return inputMngr; }
    }

    public Inventory PlayerInventory
    {
        get { return playerInventory; }
    }

    /// <summary>
    /// Name of player
    /// </summary>
    public string playerName;

    /// <summary>
    /// Prefix that needs to be attached to input event name to listen to whether registering input to input manager.
    /// <para>This is needed as name of input mapping will be same for all player from player pawn,this prefix
    /// is the one that differentiate them in case of couch co-op</para>
    /// </summary>
    private string playerInputPrefix;



    public string InputPrefix
    {
        set { playerInputPrefix = value; }
    }

    /// <summary>
    /// Unique ID of this player controller or player
    /// </summary>
    private Guid guid;

    public Guid GetID
    {
        get { return guid; }
    }

    public PlayerUData GetPlayerUniqueData
    {
        get { return new PlayerUData(playerName, guid); }
    }

    private void Awake()
    {
        guid = Guid.NewGuid();
        playerInventory = GetComponent<Inventory>();
        playerState = GetComponent<PlayerState>();
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	public override void Update () {
    }

    public override float getMovementSpeed()
    {
        return playerState.CurrentMovementSpeed;
    }

    private void OnDestroy()
    {
        inputMngr.onInputProcessed -= processMovement;
    }

    public override void controlPawn(BasicPawn pawn)
    {
        base.controlPawn(pawn);
    }

    public override void releasePawn()
    {
        if (inputMngr)
            inputMngr.stopListening(guid.ToString());
        base.releasePawn();
    }

    /// <summary>
    /// Method that need to be called after taking control of pawn to start listening to controls of that pawn from input manager
    /// <para>will be useful in later stage of project</para>
    /// </summary>
    /// <param name="inputManager">Input manager to pass in</param>
    public void setupInputs(InputManager inputManager)
    {
        if (inputMngr == null)
            inputMngr = inputManager;
        inputMngr.onInputProcessed += processMovement;
        Dictionary<string, Action<float>> inputMap = new Dictionary<string, Action<float>>();
        ((Player)controlledPawn).setupInputs(ref inputMap);

        foreach (KeyValuePair<string, Action<float>> entry in inputMap)
        {
            inputMngr.addToListenerSet(guid.ToString(), playerInputPrefix+entry.Key, entry.Value);
        }
    }

    public void setInputActive(bool isActive)
    {
        string guidStr = guid.ToString();
        if (isActive)
            inputMngr.resumeInputSet(guidStr);
        else
            inputMngr.pauseInputSet(guidStr);
    }


}

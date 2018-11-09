using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


public class PlayerController : BasicController {

    private InputManager inputMngr;

    private Inventory playerInventory;
    
    public InputManager GetInputManager
    {
        get { return inputMngr; }
    }

    public Inventory PlayerInventory
    {
        get { return playerInventory; }
    }

    public string playerName;
    private string playerInputPrefix;



    public string InputPrefix
    {
        set { playerInputPrefix = value; }
    }

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
    }

    // Use this for initialization
    void Start () {
        playerInventory = GetComponent<Inventory>();
	}
	
	// Update is called once per frame
	public override void Update () {
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

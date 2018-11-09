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
    
    public InputManager GetInputManager
    {
        get { return inputMngr; }
    }

    public string playerName;
    private string playerInputPrefix;


    private Vector3 currentMovementVelocity = Vector3.zero;
    private Vector3 currentPendingRotation = Vector3.zero;
    private int moveRefCount = 0, rotRefCount = 0;

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
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
    }

    private void OnDestroy()
    {
        inputMngr.onInputProcessed -= processMovement;
    }

    public void processMovement()
    {
        if (controlledPawn != null)
        {
            transform.position = controlledPawn.transform.position;
            Rigidbody2D rigidBody = controlledPawn.GetComponent<Rigidbody2D>();
            currentMovementVelocity.Normalize();
            rigidBody.velocity = currentMovementVelocity * movementSpeed;
            moveRefCount = 0;
            if (alwaysFaceMovingDirection)
            {
                float angleToRot = rigidBody.velocity.magnitude > 0 ? -90+Vector3.SignedAngle(controlledPawn.transform.right, rigidBody.velocity, Vector3.forward)
                    : 0;
                controlledPawn.transform.Rotate(Vector3.forward, angleToRot);
                transform.Rotate(Vector3.forward, angleToRot);
            }
            else
            {
                transform.Rotate(currentPendingRotation, Space.Self);
                controlledPawn.transform.Rotate(Vector3.forward, currentPendingRotation.z);
            }
            rotRefCount = 0;
        }
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


    public virtual void addMovementInput(Vector3 velocity)
    {
        if (moveRefCount == 0)
            currentMovementVelocity = velocity;
        else
        {
            currentMovementVelocity += velocity;
        }
        ++moveRefCount;
    }

    public virtual void addRotation(Vector3 rotation)
    {
        if (rotRefCount == 0)
            currentPendingRotation = rotation;
        else
        {
            currentPendingRotation += rotation;
        }
        ++rotRefCount;
    }

}

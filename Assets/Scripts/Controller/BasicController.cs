using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicController : MonoBehaviour {

    protected BasicPawn controlledPawn;

    public float movementSpeed;

    private Vector3 currentMovementVelocity = Vector3.zero;
    private Vector3 currentPendingRotation = Vector3.zero;
    private int moveRefCount = 0, rotRefCount = 0;

    public BasicPawn GetControlledPawn
    {
        get { return controlledPawn; }
    }


    public bool alwaysFaceMovingDirection;

    public virtual void controlPawn(BasicPawn pawn)
    {
        if(pawn.controller!=null)
        {
            pawn.controller.releasePawn();
        }
        pawn.controller = this;
        controlledPawn = pawn;
    }

    public virtual void releasePawn()
    {
        controlledPawn.controller = null;
        controlledPawn = null;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
        processMovement();
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
                float angleToRot = rigidBody.velocity.magnitude > 0 ? -90 + Vector3.SignedAngle(controlledPawn.transform.right, rigidBody.velocity, Vector3.forward)
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

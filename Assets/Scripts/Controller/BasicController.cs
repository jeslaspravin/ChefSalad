using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic Controller used to extend both NPC controller and player controller
/// </summary>
public class BasicController : MonoBehaviour {

    /// <summary>
    /// Pawn that this controller is controlling
    /// </summary>
    protected BasicPawn controlledPawn;

    /// <summary>
    /// Speed of Pawn controlled by this controller
    /// <para>Will be used by NPC controller only</para>
    /// </summary>
    public float movementSpeed;

    /// <summary>
    /// Current frame's movement velocity to be added to controlling pawn.
    /// <seealso cref="processMovement"/>
    /// </summary>
    private Vector3 currentMovementVelocity = Vector3.zero;
    /// <summary>
    /// Current frame's rotation to be added to controlling pawn.
    /// <seealso cref="processMovement"/>
    /// </summary>
    private Vector3 currentPendingRotation = Vector3.zero;

    /// <summary>
    /// Reference counting is used so that if multiple input is received for same frame and all needs to be pushed into processMovement.
    /// </summary>
    private int moveRefCount = 0, rotRefCount = 0;

    /// <summary>
    /// Whether Pawn should be always facing in direction that it moves.
    /// </summary>
    public bool alwaysFaceMovingDirection;

    public BasicPawn GetControlledPawn
    {
        get { return controlledPawn; }
    }

    public bool IsControllingPawn
    {
        get { return controlledPawn != null; }
    }

    /// <summary>
    /// Method used to take control of a pawn
    /// <para>Don't forget to register for input event in case of PlayerController</para>
    /// </summary>
    /// <param name="pawn">Pawn that will be controlled</param>
    public virtual void controlPawn(BasicPawn pawn)
    {
        if(pawn.controller!=null)
        {
            pawn.controller.releasePawn();
        }
        pawn.controller = this;
        controlledPawn = pawn;
    }

    /// <summary>
    /// Releases currently controlled pawn.
    /// </summary>
    public virtual void releasePawn()
    {
        // Reset the last set simulation velocity before releasing
        currentMovementVelocity = Vector3.zero;
        processMovement();

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

    /// <summary>
    /// Method that provides Movement speed for currently controlling pawn.Can be overridden to change behavior.
    /// </summary>
    /// <returns>Movement speed</returns>
    public virtual float getMovementSpeed()
    {
        return movementSpeed;
    }

    /// <summary>
    /// Processes whatever movements that are pending for current frame
    /// </summary>
    public void processMovement()
    {
        if (controlledPawn != null)
        {
            transform.position = controlledPawn.transform.position;
            Rigidbody2D rigidBody = controlledPawn.GetComponent<Rigidbody2D>();
            currentMovementVelocity.Normalize();
            rigidBody.velocity = currentMovementVelocity * getMovementSpeed();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicController : MonoBehaviour {

    protected BasicPawn controlledPawn;

    public float movementSpeed;

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
        
    }
}

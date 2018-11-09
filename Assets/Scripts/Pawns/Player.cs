using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BasicPawn {

    public Inventory PlayerInventory
    {
        get { return ((PlayerController)controller).PlayerInventory; }
    }

    public Guid GetId
    {
        get { return ((PlayerController)controller).GetID; }
    }

	// Use this for initialization
	protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void setupInputs(ref Dictionary<string,Action<float>> inputMap)
    {
        inputMap.Add("Horizontal", horizontalMovement);
        inputMap.Add("Vertical", verticalMovement);
    }

    public void horizontalMovement(float value)
    {
        ((PlayerController)controller).addMovementInput(value*Vector3.right);
    }

    public void verticalMovement(float value)
    {
        ((PlayerController)controller).addMovementInput(value*Vector3.up);
    }    
}

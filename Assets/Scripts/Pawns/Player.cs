using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Pawn class that will be the body of player and gets controlled by PlayerController
/// </summary>
public class Player : BasicPawn {

    public Inventory PlayerInventory
    {
        get { return ((PlayerController)controller).PlayerInventory; }
    }

    /// <summary>
    /// Provides unique ID of the player
    /// </summary>
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

    /// <summary>
    /// Maps the input listeners to functions ,This method gets called from controller and get further processed in controller
    /// <seealso cref="PlayerController"/>
    /// </summary>
    /// <param name="inputMap">Map that will be filled with input events to listen to with method</param>
    public virtual void setupInputs(ref Dictionary<string,Action<float>> inputMap)
    {
        inputMap.Add("Horizontal", horizontalMovement);
        inputMap.Add("Vertical", verticalMovement);
    }


    /// <summary>
    ///  Provides horizontal movement to the character by feeding necessary data to controller.
    /// </summary>
    /// <param name="value">Input axis value</param>
    public void horizontalMovement(float value)
    {
        ((PlayerController)controller).addMovementInput(value*Vector3.right);
    }


    /// <summary>
    /// Provides vertical movement to the character by feeding necessary data to controller.
    /// </summary>
    /// <param name="value">Input axis value</param>
    public void verticalMovement(float value)
    {
        ((PlayerController)controller).addMovementInput(value*Vector3.up);
    }    
}

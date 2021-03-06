﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC's attribute will be set by Customer Manager
/// </summary>
[System.Serializable]
public struct NpcAttributes
{
    public int salad;
    public int maxWaitTime;
    // Multiplier that will be multiplied to score calculated from salad recipe
    public float scoreMultiplier;
    // Multiplier to rate at which time drops when incorrect salad is served
    public float failureTimeMultiplier;
}

/// <summary>
/// NPC controller for this game mode
/// </summary>
public class NpcController : BasicController {

    /// <summary>
    /// Counter that this Controller belongs to.
    /// </summary>
    public Transform counterTransform;

    /// <summary>
    /// Target location that controller will focus on moving the NPC pawn to.
    /// </summary>
    private Vector3 moveToLocation;

    /// <summary>
    /// Determine whether NPC controller should move the NPC pawn.
    /// </summary>
    private bool bIsMobile=false;

    public bool IsMobile
    {
        get { return bIsMobile; }
        set { bIsMobile = value; }
    }
    public Vector3 MoveTo
    {
        get { return moveToLocation; }
        set { moveToLocation = value; }
    }

    /// <summary>
    /// NPC's salad requirement and waiting info.
    /// </summary>
    public NpcAttributes attributes;

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public override void Update () {
        // Adding movement to NPC.
        if(controlledPawn != null)
        {
            Vector3 direction = (moveToLocation - controlledPawn.transform.position).normalized;
            addMovementInput(direction * (bIsMobile ? 1 : 0));
            base.Update();
        }
	}
}

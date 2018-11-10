using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : BasicController {

    public Transform counterTransform;

    private Vector3 moveToLocation;

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

    public int salad=0;
    public int maxWaitTime = 0;

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public override void Update () {
        if(controlledPawn != null)
        {
            Vector3 direction = (moveToLocation - controlledPawn.transform.position).normalized;
            addMovementInput(direction * (bIsMobile ? 1 : 0));
            base.Update();
        }
	}
}

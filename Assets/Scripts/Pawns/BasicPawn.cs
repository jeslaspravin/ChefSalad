using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

/// <summary>
/// Base class for Player and NPC pawns
/// </summary>
[assembly: InternalsVisibleTo("BasicController")]
[assembly: InternalsVisibleTo("BasicItem")]
public class BasicPawn : MonoBehaviour {

    /// <summary>
    /// Reference to controller that controls this pawn
    /// </summary>
    internal BasicController controller;

    /// <summary>
    /// Interactable Event trigger object that triggers event with an Interactable object enters its trigger 
    /// and not trigger for anything else
    /// <seealso cref="InteractableTriggerEvent"/>
    /// </summary>
    private InteractableTriggerEvent interactableTrigger;

    /// <summary>
    /// Reference to any interactable that enters into the player interactable event trigger.
    /// </summary>
    private InteractableInterface currentInteractable;

	// Use this for initialization
	protected virtual void Start () {
        interactableTrigger = GetComponentInChildren<Collider2D>().gameObject.AddComponent<InteractableTriggerEvent>();
        interactableTrigger.onTriggered += onInteractableInRange;
        interactableTrigger.onTriggerExited += onInteractableOffRange;
    }

    protected virtual void OnDestroy()
    {
        interactableTrigger.onTriggered -= onInteractableInRange;
        interactableTrigger.onTriggerExited -= onInteractableOffRange;
    }

    // Update is called once per frame
    void Update () {

	}

    protected virtual void onInteractableInRange(InteractableInterface interactable,Collider2D collider)
    {
        // if the current interactable is null and new interactable is interactable do interact the interactable object
        // Note : currently we are not using currentInteractable so it does not matters if we check null on that.
        if (currentInteractable == null && interactable.canInteract(gameObject))
        {
            interactable.interact(gameObject);
        }
    }
    protected virtual void onInteractableOffRange(InteractableInterface interactable, Collider2D collider)
    {
        // Reset current interactable
        // Note : Does not matters too as not being used.
        if(currentInteractable != null)
        {
            currentInteractable = null;
        }
    }
}

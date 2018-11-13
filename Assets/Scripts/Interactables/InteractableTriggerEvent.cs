using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object that triggers event when an interactable object enters its trigger to intimidate the listener about it.
/// </summary>
public class InteractableTriggerEvent : MonoBehaviour {

    /// <summary>
    /// Delegate that will be used to create event for this class
    /// </summary>
    /// <param name="interactable">Interactable that entered the trigger</param>
    /// <param name="self">Trigger that triggered the event</param>
    public delegate void InteractableTriggerDelegate(InteractableInterface interactable, Collider2D self);

    /// <summary>
    /// Event that gets triggered when an interactable object enters its trigger
    /// </summary>
    public event InteractableTriggerDelegate onTriggered;

    /// <summary>
    /// Event that gets triggered when an interactable object leaves its trigger
    /// </summary>
    public event InteractableTriggerDelegate onTriggerExited;

    private Collider2D myCollider;

	// Use this for initialization
	void Start () {
        myCollider = GetComponent<Collider2D>();	
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        InteractableInterface interactable=collision.gameObject.GetComponent<InteractableInterface>();
        if(interactable != null)
        {
            onTriggered.Invoke(interactable, myCollider);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        InteractableInterface interactable = collision.gameObject.GetComponent<InteractableInterface>();
        if (interactable != null)
        {
            onTriggerExited.Invoke(interactable, myCollider);
        }
    }

}

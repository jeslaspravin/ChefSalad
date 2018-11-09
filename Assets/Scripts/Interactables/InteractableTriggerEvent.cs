using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTriggerEvent : MonoBehaviour {

    public delegate void InteractableTriggerDelegate(InteractableInterface interactable, Collider2D self);

    public event InteractableTriggerDelegate onTriggered;

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

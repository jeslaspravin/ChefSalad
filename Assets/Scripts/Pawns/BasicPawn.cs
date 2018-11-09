using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[assembly: InternalsVisibleTo("BasicController")]
[assembly: InternalsVisibleTo("BasicItem")]
public class BasicPawn : MonoBehaviour {

    internal BasicController controller;

    private InteractableTriggerEvent interactableTrigger;

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
        if(currentInteractable == null && interactable.canInteract(gameObject))
        {
            interactable.interact(gameObject);
        }
    }
    protected virtual void onInteractableOffRange(InteractableInterface interactable, Collider2D collider)
    {
        if(currentInteractable != null)
        {
            currentInteractable = null;
        }
    }
}

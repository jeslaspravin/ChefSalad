using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactable Interface that will be implemented by objects that needs to be interactable by player or NPC
/// </summary>
public interface InteractableInterface{

    void interact(GameObject interactor);
    bool canInteract(GameObject interactor);

}

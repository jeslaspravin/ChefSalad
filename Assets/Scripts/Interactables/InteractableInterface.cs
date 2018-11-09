using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InteractableInterface{

    void interact(GameObject interactor);
    bool canInteract(GameObject interactor);

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicItem : MonoBehaviour,InteractableInterface {

    public Color normalColor;
    public Color highlightColor;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual bool canInteract(GameObject interactor)
    {
        throw new System.NotImplementedException();
    }

    public virtual void interact(GameObject interactor)
    {
        throw new System.NotImplementedException();
    }

}

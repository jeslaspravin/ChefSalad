using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaladIngredientCanvasScript : MonoBehaviour {

    public Image itemImageSprite;

	// Use this for initialization
	void Start () {
        if (itemImageSprite == null)
            itemImageSprite = GetComponent<Image>();
	}
}

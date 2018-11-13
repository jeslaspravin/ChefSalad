using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ingredient Image script reference holder
/// <para>Created this as seperate class just in case it needs to be extended in future.</para>
/// </summary>
public class SaladIngredientCanvasScript : MonoBehaviour {

    public Image itemImageSprite;

	// Use this for initialization
	void Start () {
        if (itemImageSprite == null)
            itemImageSprite = GetComponent<Image>();
	}
}

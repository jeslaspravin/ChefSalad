using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHudScript : MonoBehaviour {

    private PlayerState playerState;

    private Inventory playerInventory;

    public GameObject saladItemIconPrefab;
    public GameObject vegItemIconPrefab;

    public GameObject inventoryPanel;
    public Text scoreTextBox;
    public Text timeTextBox;

    public bool reverseInventory=false;

    public PlayerState PlayerState
    {
        set
        {
            playerState = value;
        }
    }

    public Inventory PlayerInventory
    {
        set
        {
            playerInventory = value;
            playerInventory.onInventoryChanged += inventoryRefresh;
        }
    }
	// Use this for initialization
	void Start () {
        scoreTextBox.text = "0";
        timeTextBox.text = "0";
	}
	
	// Update is called once per frame
	void Update () {
		if(playerState != null)
        {
            scoreTextBox.text = playerState.PlayerScore.ToString("0.00");
            timeTextBox.text = playerState.TimeLeft.ToString();
        }
	}
    void OnDestroy()
    {
        if(playerInventory!= null)
        {
            playerInventory.onInventoryChanged -= inventoryRefresh;
        }
    }
    public void inventoryRefresh()
    {
        if (playerInventory == null)
            return;
        int[] items = playerInventory.getItemsInArray();
        while(inventoryPanel.transform.childCount !=0)
        {
            GameObject go = inventoryPanel.transform.GetChild(0).gameObject;
            go.transform.SetParent(null);
            Destroy(go);
        }

        if(reverseInventory)
            Array.Reverse(items);

        foreach(int item in items)
        {            
            if (Vegetables.isRawVegetable(item))
            {
                GameObject go = Instantiate(vegItemIconPrefab);
                SaladIngredientCanvasScript saladIngredient = go.GetComponent<SaladIngredientCanvasScript>();
                saladIngredient.itemImageSprite.sprite = ChefSaladManager.getVegetableData(item).vegTexture;
                go.transform.SetParent(inventoryPanel.transform);
            }
            else
            {
                GameObject go = Instantiate(saladItemIconPrefab);
                SaladCanvasScript saladCanvas = go.GetComponent<SaladCanvasScript>();
                go.transform.SetParent(inventoryPanel.transform);
                int itemMask = item;
                for(int i=0;i<(int)Vegies.count;i++)
                {
                    if((itemMask & 1)>0)
                    {
                        int veg = (int)Mathf.Pow(2, i);
                        saladCanvas.addItem(ChefSaladManager.getVegetableData(veg));
                    }
                    itemMask >>= 1;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaladCanvasScript : MonoBehaviour {
    public GameObject itemsContainerWidget;

    public Slider vegChoppingProgressWidget;

    public GameObject itemWidgetPrefab;

    private Queue<SaladIngredientCanvasScript> items=new Queue<SaladIngredientCanvasScript>();

    public delegate float ProgressionDelegate();

    public delegate bool ProgressVisibilityDelegate();

    private ProgressionDelegate progressionRatio;
    private ProgressVisibilityDelegate isProgressVisible;

    public ProgressionDelegate ProgressionRatio
    {
        set { progressionRatio = value; }
    }

    public ProgressVisibilityDelegate IsProgressVisible
    {
        set { isProgressVisible = value; }
    }

    void Start()
    {
        fillQueue();
    }

    private void fillQueue()
    {
        if (items.Count > 0 || (itemsContainerWidget != null && itemsContainerWidget.transform.childCount > 0))
            return;
        for (int i = 0; i < 3; i++)
        {
            GameObject go = Instantiate(itemWidgetPrefab);
            items.Enqueue(go.GetComponent<SaladIngredientCanvasScript>());
        }
    }

    void Update()
    {
        if(vegChoppingProgressWidget != null)
        {
            if (isProgressVisible != null && isProgressVisible())
            {
                if (!vegChoppingProgressWidget.gameObject.activeInHierarchy)
                    vegChoppingProgressWidget.gameObject.SetActive(true);
                if (progressionRatio != null)
                {
                    vegChoppingProgressWidget.value = progressionRatio();
                }
            }
            else if (vegChoppingProgressWidget.gameObject.activeInHierarchy)
            {
                vegChoppingProgressWidget.gameObject.SetActive(false);
            }
        }
    }

    public bool addItem(VegetableData vegData)
    {
        if (items.Count == 0)
        {
            // If Queued Item icons are used already then return,else fill it manually
            if (itemsContainerWidget.transform.childCount > 0)
                return false;
            else
                fillQueue();
        }
        SaladIngredientCanvasScript item = items.Dequeue();
        item.itemImageSprite.sprite = vegData.vegTexture;
        item.transform.SetParent(itemsContainerWidget.transform);
        return true;
    }

    public void clearSalad()
    {
        for(int i=0;i< itemsContainerWidget.transform.childCount;i++)
        {
            GameObject go=itemsContainerWidget.transform.GetChild(i).gameObject;
            go.transform.SetParent(null);
            items.Enqueue(go.GetComponent<SaladIngredientCanvasScript>());
        }
    }
}

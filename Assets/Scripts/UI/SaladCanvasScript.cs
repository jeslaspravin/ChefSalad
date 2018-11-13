using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Salad Ingredient showing UI script ,This will help in showing what vegetable the salad is made of to player.
/// </summary>
public class SaladCanvasScript : MonoBehaviour {

    /// <summary>
    /// Game object transform to which vegetable image will be childed.
    /// </summary>
    public GameObject itemsContainerWidget;

    /// <summary>
    /// Slider to show current progress in preparing vegetable.
    /// </summary>
    public Slider vegChoppingProgressWidget;

    /// <summary>
    /// Ingredient game object prefab that will be spawned at start and Pooled for rest of level.
    /// </summary>
    public GameObject itemWidgetPrefab;

    /// <summary>
    /// Queue of cached/pooled ingredient script and its game object.
    /// </summary>
    private Queue<SaladIngredientCanvasScript> items=new Queue<SaladIngredientCanvasScript>();

    /// <summary>
    /// Progression value getter delegate type.
    /// </summary>
    /// <returns>Givens amount of progression that has been made in preparing a vegetable.</returns>
    public delegate float ProgressionDelegate();

    /// <summary>
    /// Whether progression slider has to be displayed delegate type.
    /// </summary>
    /// <returns>Whether </returns>
    public delegate bool ProgressVisibilityDelegate();

    /// <summary>
    /// Progression value getter delegate.
    /// </summary>
    private ProgressionDelegate progressionRatio;
    /// <summary>
    /// Whether progression slider has to be displayed delegate.
    /// </summary>
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
        // Fill pooled object queue
        fillQueue();
    }

    private void fillQueue()
    {
        // Only if already there is no pooled object either in queue or already in use create and add pooled object.
        if (items.Count > 0 || (itemsContainerWidget != null && itemsContainerWidget.transform.childCount > 0))
            return;
        for (int i = 0; i < ChefSaladManager.MAX_INGREDIENT_COUNT; i++)
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

    void OnDestroy()
    {
        while(items !=null && items.Count>0)
        {
            SaladIngredientCanvasScript ingredient= items.Dequeue();
            if(ingredient != null)
            {
                Destroy(ingredient.gameObject);
            }
        }
    }

    /// <summary>
    /// Adds a vegetable to salad
    /// </summary>
    /// <param name="vegData">Vegetable data of vegetable being added</param>
    /// <returns>true if added successfully.</returns>
    public bool addItem(VegetableData vegData)
    {
        if (items.Count == 0)
        {
            // If Queued Item icons are used already then return as this means ingredient count reached max(this case won't happen)
            // ,else fill it manually
            if (itemsContainerWidget.transform.childCount > 0)
                return false;
            else
                // This needs to be done as in some cases after spawn start gets executed only in 
                // next frame and we need it in same frame.
                fillQueue();
                
        }
        SaladIngredientCanvasScript item = items.Dequeue();
        item.itemImageSprite.sprite = vegData.vegTexture;
        item.transform.SetParent(itemsContainerWidget.transform);
        return true;
    }

    /// <summary>
    /// Clears all salad images in the salad currently
    /// </summary>
    public void clearSalad()
    {
        while(itemsContainerWidget.transform.childCount>0)
        {
            GameObject go=itemsContainerWidget.transform.GetChild(0).gameObject;
            go.transform.SetParent(null);
            items.Enqueue(go.GetComponent<SaladIngredientCanvasScript>());
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventory of the player
/// </summary>
public class Inventory : MonoBehaviour {


    /// <summary>
    /// Maximum size of inventory
    /// </summary>
    public int stackLimit = 2;

    /// <summary>
    /// <para>Inventory data structure.Since we can encode all the vegetables in an integer, All we need is int for each item player holds.</para>
    /// <para>Queue of Vegetable/Salad(Multiple vegetable) and Queue is used because first picked will be used first</para>
    /// </summary>
    private Queue<int> stackItems= new Queue<int>();

    /// <summary>
    /// Inventory change delegate 
    /// </summary>
    public delegate void InventoryDelegate();

    /// <summary>
    /// Event that gets triggered when inventory item gets added or dropped,Used by objects like User Interface to refresh and more.
    /// </summary>
    public event InventoryDelegate onInventoryChanged;

    /// <summary>
    /// Checks whether any more item can be added to player's inventory.
    /// </summary>
    /// <returns>Boolean</returns>
    public bool canAddItem()
    {
        return stackItems.Count < 2;
    }
    
    /// <summary>
    /// Gets the item in front of queue from inventory and returns.
    /// </summary>
    /// <returns>Queue front Item's integer value vegetable or salad can be derived from that value.</returns>
    public int getNextItem()
    {
        int item=stackItems.Dequeue();
        if (onInventoryChanged != null)
            onInventoryChanged.Invoke();
        return item;
    }

    /// <summary>
    /// Just take a peek of what next item will be if we get it without taking item out of inventory.
    /// </summary>
    /// <returns>Queue front Item's integer value vegetable or salad can be derived from that value.</returns>
    public int peekNextItem()
    {
        return stackItems.Peek();
    }

    public bool hasAnyItem()
    {
        return stackItems.Count > 0;
    }

    /// <summary>
    /// Adds the item to inventory if we can add and invokes inventory changed event if added successfully
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <returns>true if addition is successful</returns>
    public bool addItem(int item)
    {
        if (canAddItem())
        {
            stackItems.Enqueue(item);
            if (onInventoryChanged != null)
                onInventoryChanged.Invoke();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns all items in inventory in array without removing anything from inventory.
    /// </summary>
    /// <returns>Array of Items in inventory</returns>
    public int[] getItemsInArray()
    {
        return stackItems.ToArray();
    }

    /// <summary>
    /// Checks if any item in inventory is a vegetables
    /// </summary>
    /// <returns>true if Inventory has any vegetables.</returns>
    public bool hasAnyVegetables()
    {
        if (!hasAnyItem())
            return false;
        if (Vegetables.isRawVegetable(peekNextItem()))
            return true;
        foreach(int item in getItemsInArray())
        {
            if (Vegetables.isRawVegetable(item))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if any item in inventory is a salad
    /// </summary>
    /// <returns>true if Inventory has any salad.</returns>
    public bool hasAnySalad()
    {
        if (!hasAnyItem())
            return false;
        if (!Vegetables.isRawVegetable(peekNextItem()))
            return true;
        foreach (int item in getItemsInArray())
        {
            if (!Vegetables.isRawVegetable(item))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Gets first item that is either vegetable or salad based on boolean passed in.
    /// </summary>
    /// <param name="isVegetable">True if need to find first vegetable else for salad</param>
    /// <returns>Found first item else will be zero(none)</returns>
    public int getFirstItem(bool isVegetable)
    {
        List<int> dataToAdd = new List<int>();
        int[] items = getItemsInArray();
        int retItem = 0;
        foreach(int item in items)
        {
            if (!(isVegetable ^ Vegetables.isRawVegetable(item)) && retItem == 0)
                retItem = item;
            else
                dataToAdd.Add(item);
        }
        stackItems.Clear();
        foreach (int item in dataToAdd)
        {
            stackItems.Enqueue(item);
        }

        if (retItem != 0 && onInventoryChanged != null)
            onInventoryChanged.Invoke();

        return retItem;
    }
}

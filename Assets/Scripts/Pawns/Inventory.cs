using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public int stackLimit = 2;

    private Queue<int> stackItems= new Queue<int>();

    public delegate void InventoryDelegate();

    public event InventoryDelegate onInventoryChanged;

    public bool canAddItem()
    {
        return stackItems.Count < 2;
    }

    public int getNextItem()
    {
        int item=stackItems.Dequeue();
        if (onInventoryChanged != null)
            onInventoryChanged.Invoke();
        return item;
    }
    public int peekNextItem()
    {
        return stackItems.Peek();
    }

    public bool hasAnyItem()
    {
        return stackItems.Count > 0;
    }

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

    public int[] getItemsInArray()
    {
        return stackItems.ToArray();
    }

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

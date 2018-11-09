using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public int stackLimit = 2;

    private Queue<int> stackItems= new Queue<int>();

    public bool canAddItem()
    {
        return stackItems.Count < 2;
    }

    public int getNextItem()
    {
        return stackItems.Dequeue();
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
            return true;
        }
        return false;
    }

    public int[] getItemsInArray()
    {
        return stackItems.ToArray();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackItem
{
    public BackpackItem(Item item)
    {
        this.item = item;
        numberOfItems = item.numberOfItems;
    }

    public void IncrementNumberOfItems(int numberToIncrement)
    {
        numberOfItems += numberToIncrement;
    }

    public Item item;
    public int numberOfItems;
}

public class BackpackController : MonoBehaviour
{
    [SerializeField] private Dictionary<uint, BackpackItem> items = new();

    public void ReceiveItem(Item itemReceived)
    {
        if(itemReceived.id == 0)
        {
            Debug.LogError("Backpack: " + "Item's id cannot be zero");
            return;
        }
        else if (IsContain(itemReceived))
        {
            items[itemReceived.id].IncrementNumberOfItems(itemReceived.numberOfItems);
            Debug.Log("Backpack: " + "the number of " + itemReceived.name + " was incremented to " + items[itemReceived.id].numberOfItems);
            return;
        }

        items.Add(itemReceived.id, new BackpackItem(itemReceived));
    }

    public List<BackpackItem> GetAllBackpackItems()
    {
        List<BackpackItem> backpackItems = new();
        foreach (BackpackItem backpackItem in items.Values)
        {
            backpackItems.Add(backpackItem);
        }
        return backpackItems;
    }

    public List<Item> GetAllItems()
    {
        List<Item> items = new();
        foreach(BackpackItem backpackItem in this.items.Values)
        {
            items.Add(backpackItem.item);
        }
        return items;
    }

    public bool IsContain(Item itemToCompare)
    {
        return items.TryGetValue(itemToCompare.id, out _);
    }
}

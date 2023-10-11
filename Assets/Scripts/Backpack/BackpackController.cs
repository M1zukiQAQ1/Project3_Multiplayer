using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BackpackItem
{
    public Item item;
    public int numberOfItems;

    public BackpackItem(Item item, int numberOfItems)
    {
        this.item = item;
        this.numberOfItems = numberOfItems;
    }


    public void IncrementNumberOfItems(int delta)
    {
        numberOfItems += delta;
    }
}


// ToDo: Add UI to recipe
// ToDo: Implement hint of consuming attribute, Add UI to attributes

public class BackpackController : MonoBehaviour
{
    [SerializeField] private Dictionary<uint, BackpackItem> items = new();

    public void ReceiveItem(Item itemReceived, int numberOfItems)
    {
        if (itemReceived.id == 0)
        {
            Debug.LogError("Backpack: " + "Item's id cannot be zero");
            return;
        }
        else if (IsContain(itemReceived.id))
        {
            items[itemReceived.id].IncrementNumberOfItems(numberOfItems);
            Debug.Log("Backpack: " + "the number of " + itemReceived.name + " was incremented to " + items[itemReceived.id].numberOfItems);
            return;
        }

        Debug.Log($"Backpack: Add new item {itemReceived.id}");
        items.Add(itemReceived.id, new BackpackItem(itemReceived, numberOfItems));
    }

    public void ChangeNumberOfItem(uint id, int delta)
    {
        items.TryGetValue(id, out var itemToChange);
        if(itemToChange == null)
        {
            Debug.LogError($"BackpackController: item {id} doesnt exist in this backpack");
            return;
        }
        items[id].numberOfItems += delta;
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
        foreach (BackpackItem backpackItem in this.items.Values)
        {
            items.Add(backpackItem.item);
        }
        return items;
    }

    public bool IsContain(uint id) => items.TryGetValue(id, out _);

    public bool IsContain(uint id, int numberOfItems) => items.TryGetValue(id, out var item) && item.numberOfItems > numberOfItems;

}

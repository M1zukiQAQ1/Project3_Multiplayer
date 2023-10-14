using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

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

public class BackpackController : NetworkBehaviour
{
    [SerializeField] private Dictionary<uint, BackpackItem> items = new();

    public event Action<Item, int> OnItemReceived;
    public event Action<Item, int> OnItemUsed;

    private void Start()
    {
        if (!IsOwner) return;

        OnItemReceived += ReceiveItemInternal;
        OnItemReceived += DisplayHintTextOnUIInternalForReceive;
        OnItemReceived += RefreshBackpackPanelInternal;

        OnItemUsed += UseItemInternal;
        OnItemUsed += DisplayHintTextOnUIInternalForUse;
        OnItemUsed += RefreshBackpackPanelInternal;
    }

    private void DisplayHintTextOnUIInternalForReceive(Item itemReceived, int numberOfItems)
    {
        ClientUIController.instance.indicationTextManager.DisplayHintTextOnUI($"Added {numberOfItems} {itemReceived.displayName}");
    }

    private void DisplayHintTextOnUIInternalForUse (Item itemUsed, int numberOfItems)
    {
        ClientUIController.instance.indicationTextManager.DisplayHintTextOnUI($"Removed {numberOfItems} {itemUsed.displayName}");
    }

    private void RefreshBackpackPanelInternal (Item arg1, int arg2)
    {
        ClientUIController.instance.RefreshBackpackPanel();
    }

    private void ReceiveItemInternal(Item itemReceived, int numberOfItems)
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

    public void ReceiveItem(Item itemReceived, int numberOfItems)
    {
        OnItemReceived?.Invoke(itemReceived, numberOfItems);
    }

    private void UseItemInternal (Item itemUsed, int delta)
    {
        // Use event
        items.TryGetValue(itemUsed.id, out var itemToChange);
        if(itemToChange == null)
        {
            Debug.LogError($"BackpackController: item {itemUsed.id} doesnt exist in this backpack");
            return;
        }
        items[itemUsed.id].numberOfItems -= delta;
        if(items[itemUsed.id].numberOfItems <= 0)
        {
            items.Remove(itemUsed.id);
        }
    }

    public void UseItem (Item itemUsed, int delta) {
        OnItemUsed?.Invoke(itemUsed, delta);
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

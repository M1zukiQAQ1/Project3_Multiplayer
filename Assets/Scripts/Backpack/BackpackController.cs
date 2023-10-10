using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackItem
{
    public Item item;
    public int numberOfItems;

    public BackpackItem(Item item)
    {
        this.item = item;
        numberOfItems = item.numberOfItems;
    }

    public void IncrementNumberOfItems(int numberToIncrement)
    {
        numberOfItems += numberToIncrement;
    }
}


// ToDo: Add UI to recipe
// ToDo: Implement hint of consuming attribute, Add UI to attributes
public class Recipe : ScriptableObject
{
    private List<BackpackItem> ingredients;
    private string recipeName;
    private string description;

    public Recipe(List<BackpackItem> ingredients, string recipeName, string description = "default description")
    {
        this.ingredients = ingredients;
        this.recipeName = recipeName;
        this.description = description;
    }

    public bool CapableOfProducing(BackpackController backpack)
    {
        if (ingredients == null || ingredients.Count == 0)
        {
            Debug.Log("Recipe can't be null");
            return false;
        }

        foreach (BackpackItem ingredient in ingredients)
        {
            if (!backpack.IsContain(ingredient.item.id, ingredient.numberOfItems))
                return false;
        }

        return true;
    }
}

public class BackpackController : MonoBehaviour
{
    [SerializeField] private Dictionary<uint, BackpackItem> items = new();

    public void ReceiveItem(Item itemReceived)
    {
        if (itemReceived.id == 0)
        {
            Debug.LogError("Backpack: " + "Item's id cannot be zero");
            return;
        }
        else if (IsContain(itemReceived.id))
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
        foreach (BackpackItem backpackItem in this.items.Values)
        {
            items.Add(backpackItem.item);
        }
        return items;
    }

    public bool IsContain(uint id) => items.TryGetValue(id, out _);

    public bool IsContain(uint id, int numberOfItems) => items.TryGetValue(id, out var item) && item.numberOfItems > numberOfItems;

}

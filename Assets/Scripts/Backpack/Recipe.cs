using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Backpack/Recipe")]
public class Recipe : Item
{
    [Header("Content of Recipe")]
    [SerializeField] private List<BackpackItem> ingredients;
    [SerializeField] private Item targetItem;
    [SerializeField] private int numberOfItemToProduce;

    public Recipe(List<BackpackItem> ingredients, Item targetItem)
    {
        this.ingredients = ingredients;
        this.targetItem = targetItem;
    }

    public List<BackpackItem> GetIngredients() => ingredients;

    public bool Produce(BackpackController backpack)
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

        foreach (BackpackItem ingredient in ingredients)
        {
            backpack.ChangeNumberOfItem(ingredient.item.id, ingredient.numberOfItems);
        }
        backpack.ReceiveItem(targetItem, numberOfItemToProduce);
        return true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeDescriptionController : MonoBehaviour
{
    public static RecipeDescriptionController instance;

    [Header("UI: Backpack System -> Recipe Panel -> Recipe Production")]
    [SerializeField] private TMP_Text recipeItemText;
    [SerializeField] private Image recipeItemImage;
    [SerializeField] private RectTransform recipeIngredientsGrid;
    [SerializeField] private RectTransform recipeIngredientStackPrefab;
    [SerializeField] private HoldButton recipeProduceButton;

    private void Start()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    public void RefreshRecipeDescriptionPanel(Recipe recipe)
    {
        recipeItemText.text = recipe.displayName;
        recipeItemImage.sprite = recipe.itemSprite;

        for(int i = recipeIngredientsGrid.childCount - 1; i >= 0; i--)
            Destroy(recipeIngredientsGrid.GetChild(i).gameObject);

        foreach (BackpackItem ingredient in recipe.GetIngredients())
        {
            var newRecipeStack = Instantiate(recipeIngredientStackPrefab, recipeIngredientsGrid);
            Debug.Log($"UI: New recipe stack instantiated: {newRecipeStack}");
            newRecipeStack.GetComponentInChildren<TMP_Text>().text = $"{ingredient.numberOfItems} of {ingredient.item.displayName}";
            newRecipeStack.GetComponentInChildren<Image>().sprite = ingredient.item.itemSprite;
        }
            
        recipeProduceButton.onLongClick.AddListener(() => ProduceItem(recipe));
    }

    private void ProduceItem(Recipe currentRecipe)
    {
        if (!currentRecipe.Produce(GameManager.instance.GetPlayerOwnedByClient().backpack))
            return;
        else
        {
            Debug.Log($"UI: item produced successfully");
        }
    }
}

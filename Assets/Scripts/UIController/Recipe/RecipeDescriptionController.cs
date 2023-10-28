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

    private void Start()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        DisableUIElements();
    }

    private void OnDisable()
    {
        DisableUIElements();
    }

    private void DisableUIElements()
    {
        recipeItemText.gameObject.SetActive(false);
        recipeIngredientsGrid.gameObject.SetActive(false);
        recipeItemImage.gameObject.SetActive(false);
    }

    private void EnableUIElements()
    {
        recipeItemText.gameObject.SetActive(true);
        recipeIngredientsGrid.gameObject.SetActive(true);
        recipeItemImage.gameObject.SetActive(true);
    }

    public void RefreshRecipeDescriptionPanel(Recipe recipe)
    {
        EnableUIElements();

        recipeItemText.text = recipe.displayName;

        recipeItemImage.sprite = recipe.itemSprite;
        recipeItemImage.GetComponent<ItemOnHoverForRecipe>().Initialize(new BackpackItem(recipe));

        for(int i = recipeIngredientsGrid.childCount - 1; i >= 0; i--)
            Destroy(recipeIngredientsGrid.GetChild(i).gameObject);

        foreach (BackpackItem ingredient in recipe.GetIngredients())
        {
            var newRecipeStack = Instantiate(recipeIngredientStackPrefab, recipeIngredientsGrid);
            Debug.Log($"UI: New recipe stack instantiated: {newRecipeStack}");
            newRecipeStack.GetComponentInChildren<TMP_Text>().text = $"{ingredient.numberOfItems} {ingredient.item.displayName}";
            newRecipeStack.GetComponentInChildren<Image>().sprite = ingredient.item.itemSprite;
        }
    }
}

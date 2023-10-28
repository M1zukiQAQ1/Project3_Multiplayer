using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemOnHoverForRecipe : ItemOnHoverBase
{
    private Recipe slotRecipe;

    public override void Initialize(BackpackItem slotItem)
    {
        base.Initialize(slotItem);
        slotRecipe = this.slotItem.item as Recipe;
        Debug.Log($"ItemOnHoverForRecipe: Initialized. Backpack item is {slotItem.item.displayName}, {slotItem.item is Recipe}");

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(!slotRecipe.Produce(GameManager.instance.GetPlayerOwnedByClient().backpack)){
            ClientUIController.instance.indicationTextManager.DisplayHintTextOnUI($"Ingredients lacks to produce");
        }
        else{
            ClientUIController.instance.indicationTextManager.DisplayHintTextOnUI($"Production completed successfully");
        }
    }

    protected override string GetDialogBoxDisplayText() => $"{slotRecipe.displayName}\r\nLeft Click to Craft";
}

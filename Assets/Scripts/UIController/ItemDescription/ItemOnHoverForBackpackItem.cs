using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class ItemOnHoverForBackpackItem : ItemOnHoverBase
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (slotItem.item.isUsable)
        {
            var targetPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>();
            slotItem.item.Use(targetPlayer);
            Destroy(currentDialogBox);
        }
    }

    protected override string GetDialogBoxDisplayText()
    {
        var displayText = $"{slotItem.numberOfItems} {slotItem.item.displayName}(s)\r\n{slotItem.item.itemDescription}";
        if (slotItem.item.isUsable)
        {
            displayText += "\r\nLeft Click to Use";
        }

        return displayText;
    }
}

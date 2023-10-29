using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CraftingTableInteractable : Interactable
{
    public override string GetHintText() => "Press F to open Crafting Table";
    
    [ServerRpc(RequireOwnership = false)]
    public override void InteractServerRpc(ulong clientId)
    {
        OpenRecipePanelClientRpc(clientId);        
    }

    [ClientRpc]
    private void OpenRecipePanelClientRpc(ulong targetClientId){
        if(targetClientId != NetworkManager.LocalClientId) return;
        ClientUIController.instance.OpenRecipePanel();
    }

}

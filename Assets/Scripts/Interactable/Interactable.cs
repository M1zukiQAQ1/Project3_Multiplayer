using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Interactable : NetworkBehaviour, IHintDisplayable
{
    public string interactableName;
    public bool isRequireItemToInteract = false;

    public bool isRequireHoldToInteract = false;
    private NetworkVariable<bool> isInteracting = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public Item requiredItem;

    public bool GetIsInteracting() => isInteracting.Value;

    public virtual string GetHintText() => "Press F to Interact";

    public void DisplayHintText()
    {
        ClientUIController.instance.DisplayHintText(GetHintText(), transform);
    }

    [ServerRpc]
    public virtual void InteractServerRpc(ulong clientId)
    {
        isInteracting.Value = true;
        Debug.Log("Object Interacted");
    }

    [ServerRpc]
    public virtual void StopInteractServerRpc(ulong clientId)
    {
        isInteracting.Value = false;
        Debug.Log("Object stop being interacted");
    }

    protected bool CheckIfRequirementFullfilled(PlayerController playerInteracted) => !isRequireItemToInteract || playerInteracted.backpack.IsContain(requiredItem);
}

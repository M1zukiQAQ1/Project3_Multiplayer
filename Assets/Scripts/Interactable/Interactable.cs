using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : NetworkBehaviour, IHintDisplayable
{
    [Header("Interactable Modifier: Require Item")]
    public bool isRequireItemToInteract = false;
    public Item requiredItem;

    [Header("Interactable Modifier: Require Attribute")]
    public bool isRequirePlayersAttributesToInteract = false;
    public PlayerController.AttributesOfPlayer.AttributeType attributeType;
    public float requiredValue;

    [Header("Interactable Modifier: Require Hold")]
    public bool isRequireHoldToInteract = false;

    private NetworkVariable<bool> isInteracting = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

    protected bool CheckIfRequirementFullfilled(PlayerController playerInteracted)
    {
        var fullfilledItemRequirement = !isRequireItemToInteract || playerInteracted.backpack.IsContain(requiredItem.id);
        Debug.Log($"Interactable: fullfilled item requirement {fullfilledItemRequirement}");
        var fullfilledAttributeRequirement = !isRequirePlayersAttributesToInteract || playerInteracted.attributes.CapableOf(attributeType, requiredValue);
        Debug.Log($"Interactable: fullfilled attributes requirement {fullfilledAttributeRequirement}");
        return fullfilledAttributeRequirement && fullfilledItemRequirement;
    }
}

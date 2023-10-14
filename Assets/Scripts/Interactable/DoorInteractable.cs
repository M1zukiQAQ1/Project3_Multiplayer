using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class DoorInteractable : Interactable
{
    [Header("New Attributes")]
    [SerializeField] private NetworkAnimator doorAnimator;

    //Requiring server to trigger door's animation

    public override string GetHintText() => "Hold F to open this door";

    [ServerRpc(RequireOwnership = false)]
    public override void InteractServerRpc(ulong clientId)
    {
        // Get player interacted with the door
        PlayerController playerInteracted = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();
        if (CheckIfRequirementFullfilled(playerInteracted) && !doorAnimator.GetComponent<DoorController>().isOpened)
        {
            Debug.Log("Opening!");
            doorAnimator.SetTrigger("Open");
            doorAnimator.GetComponent<DoorController>().isOpened = true;
        }

        base.InteractServerRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public override void StopInteractServerRpc(ulong clientId)
    {
        if (doorAnimator.GetComponent<DoorController>().isOpened)
        {
            doorAnimator.SetTrigger("Close");
            doorAnimator.GetComponent<DoorController>().isOpened = false;
        }

        base.StopInteractServerRpc(clientId);
    }


}

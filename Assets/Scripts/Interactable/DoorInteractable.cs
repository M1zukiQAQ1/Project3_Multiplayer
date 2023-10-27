using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DoorInteractable : Interactable
{
    [Header("New Attributes")]
    [SerializeField] private DoorController_ControllerByScripts doorController;

    //Requiring server to trigger door's animation

    public override string GetHintText() => "Hold F to open this door";

    [ServerRpc(RequireOwnership = false)]
    public override void InteractServerRpc(ulong clientId)
    {
        // Get player interacted with the door
        PlayerController playerInteracted = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();
        if (CheckIfRequirementFullfilled(playerInteracted))
        {
            doorController.OpenDoor();
        }

        base.InteractServerRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public override void StopInteractServerRpc(ulong clientId)
    {
        doorController.CloseDoor();
        base.StopInteractServerRpc(clientId);
    }


}

using Unity.Netcode;
using UnityEngine;

public class ElevatorInteractable : Interactable {
    public string teleportLocationName = "2F";
    public Vector3 teleportPosition;
    public override string GetHintText() => $"Press F to teleport to {teleportLocationName}";

    [ServerRpc(RequireOwnership = false)]
    public override void InteractServerRpc(ulong clientId)
    {
        var player = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();
        player.transform.position = teleportPosition;
    }
}
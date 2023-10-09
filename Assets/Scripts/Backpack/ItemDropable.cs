using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Collider))]
public class ItemDropable : NetworkBehaviour, IHintDisplayable
{
    public Item itemContained = null;

    public string GetHintText() => itemContained.displayName;

    public void DisplayHintText()
    {
        ClientUIController.instance.DisplayHintText(GetHintText(), transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
 //           GetComponent<NetworkObject>().ChangeOwnership(NetworkManager.Singleton.LocalClientId);
            other.GetComponentInParent<BackpackController>().ReceiveItem(itemContained);
            ClientUIController.instance.RefreshBackpackPanel();
            GameManager.instance.DestroyNetworkObjectServerRpc(GetComponent<NetworkObject>());
        }
    }
}

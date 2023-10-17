using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class ChangeMonitorCamInteractable : Interactable
{
    [SerializeField] private MonitorController targetMonitor;
    [SerializeField] private enum ChangeType{
        NEXT, PRIOR
    };
    [SerializeField] private ChangeType changeType;

    public override string GetHintText() => $"Press F to Display {changeType.ToString().ToLowerInvariant()} Camera";
    
    [ServerRpc]
    public override void InteractServerRpc(ulong clientId)
    {
        Debug.Log($"MonitorInteractable: target monitor {targetMonitor}");
        if(changeType == ChangeType.NEXT){
            targetMonitor.DisplayNextCamera();
        } 
        if(changeType == ChangeType.PRIOR){
            targetMonitor.DisplayPriorCamera();
        }
        // base.InteractServerRpc(clientId);
    }
}

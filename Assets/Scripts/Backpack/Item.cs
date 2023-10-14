using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "New Item", menuName = "Backpack/Item")]
public class Item : ScriptableObject
{
    public string displayName;
    public string itemDescription;
    public uint id;
    public bool isUsable = false;

    public virtual void Use(PlayerController targetPlayer) 
    {
        targetPlayer.backpack.UseItem(this, 1);
    }

    public Sprite itemSprite;
}

public class ItemReference : INetworkSerializable
{
    public uint id;

    public ItemReference() { }

    public ItemReference(Item item)
    {
        id = item.id;
    }

    public bool TryResolve(out Item item)
    {
        return ResourceManager.instance.TryGetItem(id, out item);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
    }
}

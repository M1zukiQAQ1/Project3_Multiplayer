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

    // To change number of items in the backpack, directly change it in the backpack. Do NOT change this.
    public readonly int numberOfItems = 1;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public const string FOLDER_ITEMS = "Items";
    private readonly Dictionary<uint, Item> itemDictionary = new();

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        Item[] itemSo = Resources.LoadAll<Item>(FOLDER_ITEMS);

        if(itemSo == null || itemSo.Length == 0)
        {
            Debug.LogWarning("Resource Manager: Unable to find items, check if path is correct");
        }

        foreach (Item item in itemSo)
        {
            uint id = item.id;
            if (itemDictionary.ContainsKey(id))
            {
                Debug.LogError("Resource Manager: Duplicate item id found!");
                continue;
            }
            Debug.Log("Resource Manager: Loading item " + id);
            itemDictionary.Add(id, item);
        }
    }

    public bool TryGetItem(uint id, out Item item)
    {
        return itemDictionary.TryGetValue(id, out item);
    }
}

using Inventory;
using Items.Collectables;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NonRespawnableItemsRemover
{
    private static NonRespawnableItemsRemover instance = null;
    private static readonly object instanceLock = new object(); //thread-safe for co-routines

    public static NonRespawnableItemsRemover Instance
    {
        get
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = new NonRespawnableItemsRemover();
                }
                return instance;
            }
        }
    }

    public void RemoveNonRespawnableItems()
    {
        var inventory = PlayerInventory.Instance.ItemDictionary;
        var worldItems = GameObject.FindObjectsByType<AffectInventoryWhenCollect>(FindObjectsSortMode.None).ToList<AffectInventoryWhenCollect>();

        foreach (var item in inventory.Keys)
        {
            if(!item.isRespawnable)
            {
                var itemInstances = from AffectInventoryWhenCollect instance in worldItems
                                    where instance.item.itemID == item.itemID
                                    select instance;
                foreach(var instance in itemInstances)
                {
                    Object.Destroy(instance.gameObject);
                }
            }
        }
    }
}

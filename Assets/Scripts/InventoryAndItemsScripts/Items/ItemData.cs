using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
    [Serializable]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public string itemDesc;
        public string itemID;
        public Sprite sprite; //can be used for UI graphic
        public GameObject itemPrefab;
        public int price = 0; // item price for shop, 0 by default
        public bool isEquippable;
        public bool isRespawnable = false;
    }
}
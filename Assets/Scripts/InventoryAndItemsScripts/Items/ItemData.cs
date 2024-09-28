using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public string itemDesc;
        public Sprite sprite; //can be used for UI graphic
        public GameObject itemPrefab;
        public bool isEquippable;
    }
}


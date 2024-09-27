using Inventory;
using Items;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The keybind that controls opening and closing the inventory.")]
    InputAction InventoryAction;

    [SerializeField]
    private GameObject inventory; // canvas game object

    private GameObject invPanel; // main inventory panel within canvas
    private GameObject coins; // coin counter and icon
    private GameObject health; // health icon
    private GameObject itemDesc; // item name & description

    private PlayerInventory pInv;

    private void Start()
    {
        pInv = PlayerInventory.Instance;
        invPanel = inventory.transform.GetChild(0).gameObject;
        coins = inventory.transform.GetChild(1).gameObject;
        health = inventory.transform.GetChild(2).gameObject;
        itemDesc = inventory.transform.GetChild(3).gameObject;
        Debug.Log(coins.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text);
    }

    private void OnEnable()
    {
        InventoryAction.performed += OpenInventory;
        InventoryAction.Enable();
    }

    private void OnDisable()
    {
        InventoryAction.performed -= OpenInventory;
        InventoryAction.Disable();
    }

    void OpenInventory(CallbackContext c)
    {
        if (inventory.activeSelf)
        {
            inventory.SetActive(false);
            PauseSystem.ResumeGame();
        } 
        else
        {
            // make sure something else isn't already pausing the game
            if (!PauseSystem.isPaused) {
                inventory.SetActive(true);
                PauseSystem.PauseGame();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log(pInv.PrintInventory());
        }

        List<ItemData> keysToRemove = new List<ItemData>();
        foreach (var item in pInv.ItemDictionary)
        {
            if (item.Key.itemName.Contains("Coin"))
            {
                int coinCount = int.Parse(coins.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text);
                coins.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = (coinCount + 1).ToString();
                keysToRemove.Add(item.Key);
            }
        }

        // Remove keys
        foreach (var key in keysToRemove)
        {
            pInv.ItemDictionary.Remove(key);
        }
    }
}

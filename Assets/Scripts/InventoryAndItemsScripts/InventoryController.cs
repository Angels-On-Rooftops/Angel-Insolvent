using Inventory;
using Items;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The keybind that controls opening and closing the inventory.")]
    InputAction InventoryAction;

    // Keybinds that control navigating the items within the inventory.
    [SerializeField]
    [Tooltip("The keybind that controls moving left in the inventory.")]
    InputAction NavLeft;

    [SerializeField]
    [Tooltip("The keybind that controls moving right in the inventory.")]
    InputAction NavRight;

    [SerializeField]
    [Tooltip("The keybind that controls moving up in the inventory.")]
    InputAction NavUp;

    [SerializeField]
    [Tooltip("The keybind that controls moving down in the inventory.")]
    InputAction NavDown;

    [SerializeField]
    private GameObject inventory; // canvas game object

    private GameObject invPanel; // main inventory panel within canvas
    private GameObject coins; // coin counter and icon
    private GameObject health; // health icon
    private GameObject itemDesc; // item name & description
    private PlayerInventory pInv;

    private List<GameObject> inventorySlots; // slots in the inventory panel
    private List<ItemData> itemsInInventory; // list of items currently held in inventory
    private int inventoryIndex = 0;
    private UnityEngine.UI.Image selectedSlot;

    private void Start()
    {
        pInv = PlayerInventory.Instance;
        invPanel = inventory.transform.GetChild(0).gameObject;
        coins = inventory.transform.GetChild(1).gameObject;
        health = inventory.transform.GetChild(2).gameObject;
        itemDesc = inventory.transform.GetChild(3).gameObject;

        inventorySlots = new List<GameObject>();
        itemsInInventory = new List<ItemData>();

        foreach (Transform child in invPanel.transform)
        {
            inventorySlots.Add(child.gameObject);
        }

        selectedSlot = invPanel.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();
    }

    private void OnEnable()
    {
        InventoryAction.performed += OpenInventory;
        InventoryAction.Enable();

        // Navigating inventory
        NavLeft.performed += NavInvLeft;
        NavRight.performed += NavInvRight;
        NavUp.performed += NavInvUp;
        NavDown.performed += NavInvDown;

        NavLeft.Enable();
        NavRight.Enable();
        NavUp.Enable();
        NavDown.Enable();

    }

    private void OnDisable()
    {
        InventoryAction.performed -= OpenInventory;
        InventoryAction.Disable();

        // Navigating inventory
        NavLeft.performed -= NavInvLeft;
        NavRight.performed -= NavInvRight;
        NavUp.performed -= NavInvUp;
        NavDown.performed -= NavInvDown;

        NavLeft.Disable();
        NavRight.Disable();
        NavUp.Disable();
        NavDown.Disable();
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

    void NavInvLeft(CallbackContext c)
    {
        if (inventory.activeSelf)
        {
            if (inventoryIndex > 0 && inventoryIndex % 6 != 0) 
            {
                inventoryIndex--;
                selectedSlot.transform.SetParent(invPanel.transform.GetChild(inventoryIndex));
                selectedSlot.transform.position = invPanel.transform.GetChild(inventoryIndex).position;
                Debug.Log(inventoryIndex);
            }
        }
    }

    void NavInvRight(CallbackContext c)
    {
        if (inventory.activeSelf)
        {
            if (inventoryIndex < inventorySlots.Count && inventoryIndex % 6 != 5)
            {
                inventoryIndex++;
                selectedSlot.transform.SetParent(invPanel.transform.GetChild(inventoryIndex));
                selectedSlot.transform.position = invPanel.transform.GetChild(inventoryIndex).position;
                Debug.Log(inventoryIndex);
            }
        }
    }

    void NavInvUp(CallbackContext c)
    {
        if (inventory.activeSelf)
        {
            if (inventoryIndex > 5) inventoryIndex -= 6;
            selectedSlot.transform.SetParent(invPanel.transform.GetChild(inventoryIndex));
            selectedSlot.transform.position = invPanel.transform.GetChild(inventoryIndex).position;
            Debug.Log(inventoryIndex);
        }
    }

    void NavInvDown(CallbackContext c)
    {
        if (inventory.activeSelf)
        {
            if (inventoryIndex < inventorySlots.Count - 6) inventoryIndex += 6;
            selectedSlot.transform.SetParent(invPanel.transform.GetChild(inventoryIndex));
            selectedSlot.transform.position = invPanel.transform.GetChild(inventoryIndex).position;
            Debug.Log(inventoryIndex);
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
            } else if (!itemsInInventory.Contains(item.Key))
            {
                itemsInInventory.Add(item.Key);
                var slot = inventorySlots[itemsInInventory.Count-1];
                var img = item.Key.sprite;
                slot.GetComponent<UnityEngine.UI.Image>().sprite = img;


                // var title = itemDesc.transform.GetChild(0).gameObject;
                // title.GetComponent<TextMeshProUGUI>().text = item.Key.itemName;
                // var desc = itemDesc.transform.GetChild(1).gameObject;
                // desc.GetComponent<TextMeshProUGUI>().text = item.Key.itemDesc;
            }
        }

        // Remove keys
        foreach (var key in keysToRemove)
        {
            pInv.ItemDictionary.Remove(key);
        }

        var title = itemDesc.transform.GetChild(0).gameObject;
        var desc = itemDesc.transform.GetChild(1).gameObject;
        if (itemsInInventory.Count > inventoryIndex)
        {
            itemDesc.SetActive(true);
            title.GetComponent<TextMeshProUGUI>().text = itemsInInventory[inventoryIndex].itemName;
            desc.GetComponent<TextMeshProUGUI>().text = itemsInInventory[inventoryIndex].itemDesc;
        }
        else
        {
            itemDesc.SetActive(false);
        }
    }
}

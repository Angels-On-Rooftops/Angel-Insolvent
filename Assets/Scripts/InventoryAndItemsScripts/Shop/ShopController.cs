using GameStateManagement;
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

public class ShopController : MonoBehaviour
{
    public List<ItemData> shopItems; // place items to be displayed in shop

    [SerializeField]
    [Tooltip("The keybind that controls opening and closing the inventory.")]
    InputAction InventoryAction;

    [SerializeField]
    [Tooltip("The keybind for interacting with items in the inventory.")]
    InputAction SelectItem;

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
    private GameObject shopInv; // canvas game object

    private GameObject invPanel; // main inventory panel within canvas
    private GameObject coins; // Player coins
    private GameObject itemInfo; // item name & description parent (holds the two fields below)
    private GameObject itemName;
    private GameObject itemDescription;

    private PlayerInventory pInv;

    private List<GameObject> inventorySlots; // slots in the inventory 
    private List<ItemData> itemsInInventory; // list of items currently held in inventory
    private int inventoryIndex = 0;
    private int inventoryRowSize;
    private UnityEngine.UI.Image selectedSlot;

    public GameObject GetShopInventoryPanel() { return shopInv; }

    private void Start()
    {
        pInv = PlayerInventory.Instance;
        invPanel = shopInv.transform.GetChild(0).gameObject;
        coins = shopInv.transform.GetChild(1).gameObject;
        itemInfo = shopInv.transform.GetChild(2).gameObject;

        shopInv.SetActive(true);
        itemName = GameObject.FindWithTag("InventoryItemName");
        itemDescription = GameObject.FindWithTag("InventoryItemDescription");
        shopInv.SetActive(false);

        inventorySlots = new List<GameObject>();
        itemsInInventory = new List<ItemData>();

        foreach (Transform child in invPanel.transform)
        {
            inventorySlots.Add(child.gameObject);
        }

        inventoryRowSize = invPanel.GetComponent<GridLayoutGroup>().constraintCount;
        selectedSlot = invPanel.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();
    }

    private void OnEnable()
    {
        InventoryAction.performed += OpenInventory;
        InventoryAction.Enable();
        SelectItem.performed += BuyItem;
        SelectItem.Enable();

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
        SelectItem.performed -= BuyItem;
        SelectItem.Disable();

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
        if (shopInv.activeSelf)
        {
            GameStateManager.Instance.SetState(new PlayingState());
        }
        else
        {
            // make sure something else isn't already pausing the game
            var state = GameStateManager.Instance.CurrentState;
            if (state is not PauseState && state is not InventoryState) {
                GameStateManager.Instance.SetState(new GameStateManagement.ShopState(this));
            }
        }
    }

    #region Inventory Navigation
    void NavInvLeft(CallbackContext c)
    {
        if (shopInv.activeSelf)
        {
            if (inventoryIndex > 0 && inventoryIndex % inventoryRowSize != 0) 
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
        if (shopInv.activeSelf)
        {
            if (inventoryIndex < inventorySlots.Count && inventoryIndex % inventoryRowSize != inventoryRowSize - 1)
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
        if (shopInv.activeSelf)
        {
            if (inventoryIndex > inventoryRowSize - 1) inventoryIndex -= inventoryRowSize;
            selectedSlot.transform.SetParent(invPanel.transform.GetChild(inventoryIndex));
            selectedSlot.transform.position = invPanel.transform.GetChild(inventoryIndex).position;
            Debug.Log(inventoryIndex);
        }
    }

    void NavInvDown(CallbackContext c)
    {
        if (shopInv.activeSelf)
        {
            if (inventoryIndex < inventorySlots.Count - inventoryRowSize) inventoryIndex += inventoryRowSize;
            selectedSlot.transform.SetParent(invPanel.transform.GetChild(inventoryIndex));
            selectedSlot.transform.position = invPanel.transform.GetChild(inventoryIndex).position;
            Debug.Log(inventoryIndex);
        }
    }
    #endregion
    
    void BuyItem(CallbackContext c)
    {
        // TODO
        // if player has enough coins, add item to inventory,
        // subtract amt. of coins from player's coins.
        // maybe add a confirmation box before purchasing
    }
    
    private void Update()
    {
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
            }
        }

        // Remove keys
        foreach (var key in keysToRemove)
        {
            pInv.ItemDictionary.Remove(key);
        }

        if (itemsInInventory.Count > inventoryIndex)
        {
            itemInfo.SetActive(true);
            itemName.GetComponent<TextMeshProUGUI>().text = itemsInInventory[inventoryIndex].itemName;
            itemDescription.GetComponent<TextMeshProUGUI>().text = itemsInInventory[inventoryIndex].itemDesc;
        }
        else
        {
            itemInfo.SetActive(false);
        }
    }
}

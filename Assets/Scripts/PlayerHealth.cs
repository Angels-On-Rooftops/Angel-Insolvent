using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Items.Collectables;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // TODO inventory should read player health, the player health shouldn't set the inventory
    //[SerializeField]
    //private GameObject inventory;
    //private TextMeshProUGUI healthUI;

    [SerializeField]
    private int Health = 4;

    //void Start()
    //{
    //    inventory.SetActive(true);
    //    healthUI = GameObject.FindWithTag("InventoryHealth").GetComponent<TextMeshProUGUI>();
    //    inventory.SetActive(false);
    //}

    public void IncreaseHealth(int amount = 1)
    {
        Health += amount;
    }

    public void DecreaseHealth(int amount = 1)
    {
        Health = Mathf.Max(Health - amount, 0);
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    healthUI.text = Health.ToString();
    //}
}

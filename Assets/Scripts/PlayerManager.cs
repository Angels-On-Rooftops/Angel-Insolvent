using Items.Collectables;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject inventory;

    private int health;
    private TextMeshProUGUI healthUI;

    
    void Start()
    {
        health = 4;
        inventory.SetActive(true);
        healthUI = GameObject.FindWithTag("InventoryHealth").GetComponent<TextMeshProUGUI>();
        inventory.SetActive(false);
    }

    public void IncreaseHealth(int amount = 1)
    {
        health += amount;
    }

    public void DecreaseHealth(int amount = 1)
    {
        health = Mathf.Max(health - amount, 0);
    }

    // Update is called once per frame
    void Update()
    {
        healthUI.text = health.ToString();
    }
}

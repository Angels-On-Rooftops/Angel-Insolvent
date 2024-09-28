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
        healthUI = inventory.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void IncreaseHealth(int amount = 1)
    {
        health += amount;
    }

    public void DecreaseHealth(int amount = 1)
    {
        if (health > 0) health -= amount;
        if (health < 0) health = 0; // in the event that the health decrease amount brings the health to negative
    }

    // Update is called once per frame
    void Update()
    {
        healthUI.text = health.ToString();
    }
}

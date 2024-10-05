using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    public int Health = 4;

    public void IncreaseHealth(int amount = 1)
    {
        Health += amount;
    }

    public void DecreaseHealth(int amount = 1)
    {
        Health = Mathf.Max(Health - amount, 0);
    }
}

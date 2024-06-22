using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;

    public void SetValue(int val) {
        value = val;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController target = collision.GetComponent<PlayerController>();
        if (target != null) {
            Inventory.AddCoins(value);
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChanger : MonoBehaviour
{
    [SerializeField] string Direction = "T";

    public void OnTriggerEnter2D(Collider2D collision) {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null) {
            DungeonManager dm = GameObject.Find("DungeonManager").GetComponent<DungeonManager>();
            dm.ChangeRoom(Direction);
        }
    }
}

using UnityEngine;

public class HealPoint : MonoBehaviour
{
    public Playerstatus player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        player.HealFull();

        Debug.Log("HP‘S‰ñ•œI");
    }
}
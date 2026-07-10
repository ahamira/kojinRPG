using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public BattleManager battleManager;
    public UnitData bossData;

    private bool defeated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (defeated) return;

        if (other.CompareTag("Player"))
        {
            battleManager.isBossBattle = true;
            battleManager.bossData = bossData;

            battleManager.EncounterEnemy();
        }
    }
}
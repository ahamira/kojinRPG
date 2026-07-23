using UnityEngine;
using System.Collections.Generic;

public class AreaEnemySet : MonoBehaviour
{
    public List<UnitData> areaEnemies;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        BattleManager battleManager =
            FindFirstObjectByType<BattleManager>();

        if (battleManager != null)
        {
            battleManager.enemyDatas = areaEnemies;
        }
    }
}
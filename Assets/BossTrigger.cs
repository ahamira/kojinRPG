using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public BattleManager battleManager;
    public UnitData bossData;

    public GameObject caveWall;

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
    public void BossDefeated()
    {
        defeated = true;

        if (caveWall != null)
            caveWall.SetActive(false);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = false;
        }
        Debug.Log("âΩÇ©Ç™è¡Ç¶ÇΩ");
    }
}
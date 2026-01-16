using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BattleState { Start, PlayerTurn, EnemyTurn, Win, Lose, Busy }

public class BattleManager : MonoBehaviour
{
    public BattleState state;
    [Header("参加者")]
    public BattleUnit playerUnit;
    public GameObject enemyPrefab;

    [Header("配置・UI")]
    public Transform[] spawnPoints;   // 敵の出現場所（3つ）
    public RectTransform selectionArrow; // 指アイコンUI

    private List<BattleUnit> activeEnemies = new List<BattleUnit>();
    private int selectedEnemyIndex = 0;

    void Start()
    {
        state = BattleState.Start;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        // 1~3匹の敵をランダム生成
        int count = Random.Range(1, 4);
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);
            BattleUnit unit = obj.GetComponent<BattleUnit>();
            unit.Setup();
            activeEnemies.Add(unit);
        }

        Debug.Log("魔物たちが あらわれた！");
        yield return new WaitForSeconds(1f);
        PlayerTurn();
    }

    void PlayerTurn()
    {
        state = BattleState.PlayerTurn;
        selectedEnemyIndex = 0;
        UpdateArrow();
        Debug.Log("どうする？ (左右で選択、Spaceで攻撃)");
    }

    void Update()
    {
        if (state != BattleState.PlayerTurn) return;

        // ターゲット選択
        if (Input.GetKeyDown(KeyCode.RightArrow)) { ChangeTarget(1); }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { ChangeTarget(-1); }

        // 決定
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(PlayerAttack());
        }
    }

    void ChangeTarget(int direction)
    {
        selectedEnemyIndex = (selectedEnemyIndex + direction + activeEnemies.Count) % activeEnemies.Count;
        UpdateArrow();
    }

    void UpdateArrow()
    {
        selectionArrow.gameObject.SetActive(true);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(activeEnemies[selectedEnemyIndex].transform.position);
        selectionArrow.position = screenPos + new Vector3(0, 120, 0); // 敵の少し上に表示
    }

    IEnumerator PlayerAttack()
    {
        state = BattleState.Busy;
        selectionArrow.gameObject.SetActive(false);

        BattleUnit target = activeEnemies[selectedEnemyIndex];
        Debug.Log($"{playerUnit.data.unitName}の攻撃！");
        target.TakeDamage(playerUnit.data.attack);

        yield return new WaitForSeconds(1f);

        if (target.isDead)
        {
            Debug.Log($"{target.data.unitName}を たおした！");
            Destroy(target.gameObject);
            activeEnemies.Remove(target);
        }

        if (activeEnemies.Count <= 0) { state = BattleState.Win; Debug.Log("戦いに勝利した！"); }
        else { StartCoroutine(EnemyAttack()); }
    }

    IEnumerator EnemyAttack()
    {
        state = BattleState.EnemyTurn;
        foreach (var enemy in activeEnemies)
        {
            Debug.Log($"{enemy.data.unitName}の攻撃！");
            playerUnit.TakeDamage(enemy.data.attack);
            yield return new WaitForSeconds(1f);
            if (playerUnit.isDead) break;
        }

        if (playerUnit.isDead) { state = BattleState.Lose; Debug.Log("全滅した..."); }
        else { PlayerTurn(); }
    }
}
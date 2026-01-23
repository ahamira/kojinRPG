using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BattleState { Start, PlayerTurn, EnemyTurn, Win, Lose, Busy }

public class BattleManager : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject commandPanel;
    public BattleState state;
    [Header("参加者")]
    public BattleUnit playerUnit;
    public GameObject enemyPrefab;

    [Header("配置・UI")]
    public Transform[] spawnPoints;  
    public RectTransform selectionArrow; 

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
        commandPanel.SetActive(true);
        selectionArrow.gameObject.SetActive(true);
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
        state = BattleState.Busy;
        commandPanel.SetActive(false);

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
    
    public void OnAttackButton()
    {
        if (state != BattleState.PlayerTurn) return;
        commandPanel.SetActive(false);
        selectionArrow.gameObject.SetActive(false);
        StartCoroutine(PlayerAttack());
    }

    public void OnDefendButton()
    {
        if (state != BattleState.PlayerTurn) return;
        StartCoroutine(PlayerDefend());
    }

    public void OnEscapeButton()
    {
        if (state != BattleState.PlayerTurn) return;
        StartCoroutine(PlayerEscape());
    }

    IEnumerator PlayerDefend()
    {
        state = BattleState.Busy;
        Debug.Log($"{playerUnit.data.unitName}は 身をまもっている！");
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyAttack());
    }

    IEnumerator PlayerEscape()
    {
        state = BattleState.Busy;
        Debug.Log("逃げ出した！");
        yield return new WaitForSeconds(1f);

        if (Random.value > 0.2f) 
        {
            Debug.Log("逃げれた");
        }
        else
        {
            Debug.Log("逃げられなかった！");
            StartCoroutine(EnemyAttack());
        }
    }
}
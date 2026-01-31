using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public enum BattleState { Start, PlayerTurn, EnemyTurn, Win, Lose, Busy }

public class BattleManager : MonoBehaviour
{
    [Header("Player UI")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerHPText;
    public Slider hpSlider;
    [Header("UI Panel")]
    public GameObject commandPanel;
    public BattleState state;
    [Header("参加者")]
    public BattleUnit playerUnit;
    public GameObject enemyPrefab;
    [Header("出現可能な敵のリスト")]
    public List<UnitData> enemyDatas;
    [Header("配置・UI")]
    public Transform[] spawnPoints;  
    public RectTransform selectionArrow;
    public Transform enemyField;
    private List<BattleUnit> activeEnemies = new List<BattleUnit>();
    private int selectedEnemyIndex = 0;
    public List<UnitData> possibleEnemies;
    public GameObject battleUI;
    [Header("UI Groups")]
    public CanvasGroup battleCanvasGroup;
    void Start()
    {
        state = BattleState.Start;
        StartCoroutine(SetupBattle());
        battleUI.SetActive(false);
    }

    IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        UpdatePlayerUI();
        int count = Random.Range(1, 4);
        for (int i = 0; i < count; i++)
        {
            UpdatePlayerUI();
            GameObject obj = Instantiate(enemyPrefab, enemyField);
            BattleUnit unit = obj.GetComponent<BattleUnit>();
            int randomIndex = Random.Range(0, enemyDatas.Count);
            unit.data = enemyDatas[randomIndex];
            unit.Setup();
            activeEnemies.Add(unit);
            unit.gameObject.name = unit.data.unitName + " " + (char)('A' + i);
        }

        Debug.Log("魔物たちが あらわれた！");
        yield return new WaitForSeconds(1f);
        PlayerTurn();
    }
    void UpdatePlayerUI()
    {
        playerNameText.text = playerUnit.data.unitName;
        playerHPText.text = $"HP {playerUnit.currentHp} / {playerUnit.data.maxHp}";
        hpSlider.maxValue = playerUnit.data.maxHp;
        hpSlider.value = playerUnit.currentHp;
        Image fillImage = hpSlider.fillRect.GetComponent<Image>();
        float hpPercent = (float)playerUnit.currentHp / playerUnit.data.maxHp;

        if (hpPercent <= 0.2f)
        {
            fillImage.color = Color.red;   
        }
        else if (hpPercent <= 0.5f)
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.green;  
        }
    }
    public void EncounterEnemy()
    {
        battleUI.SetActive(true);
       if (playerUnit != null)
       {
                idou moveScript = playerUnit.GetComponent<idou>();
                if (moveScript != null) moveScript.enabled = false;
       }
        StartCoroutine(SetupBattle());
    }
    void PlayerTurn()
    {
        state = BattleState.PlayerTurn;
        commandPanel.SetActive(true);
        selectionArrow.gameObject.SetActive(true);
        state = BattleState.PlayerTurn;
        selectedEnemyIndex = 0;
        UpdateArrow();
        Debug.Log("どうする？");
    }

    void Update()
    {
        if (state != BattleState.PlayerTurn) return;
        if (Input.GetKeyDown(KeyCode.RightArrow)) { ChangeTarget(1); }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { ChangeTarget(-1); }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(PlayerAttack());
        }
        if (hpSlider != null)
        {
            hpSlider.value = Mathf.Lerp(hpSlider.value, playerUnit.currentHp, Time.deltaTime * 5f);
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
        selectionArrow.position = activeEnemies[selectedEnemyIndex].transform.position + new Vector3(0, 150, 0);
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
            UpdatePlayerUI();
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
    public void EndBattle()
    {
        battleUI.SetActive(false);
        idou playerMovement = playerUnit.GetComponent<idou>();
        if (playerMovement != null) playerMovement.enabled = true;

        state = BattleState.Busy;
    }
}
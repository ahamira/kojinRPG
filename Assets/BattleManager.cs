using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Cainos.PixelArtTopDown_Basic;

public enum BattleState { Start, PlayerTurn, EnemyTurn, Win, Lose, Busy }

public class BattleManager : MonoBehaviour
{
    private int totalExpGained = 0;
    public bool isBossBattle = false;
    public UnitData bossData;

    [Header("Player UI")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerHPText;
    public Slider hpSlider;

    [Header("UI Panel")]
    public GameObject commandPanel;
    public BattleState state;

    [Header("参加者")]
    public Playerstatus fieldPlayerStatus; 
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

    [Header("フィールドUIのマネージャー")]
    public FieldUIManager fieldUIManager;

    void Start()
    {
        state = BattleState.Start;
        if (battleUI != null) battleUI.SetActive(false); 
    }
    public void RefreshPlayerUI()
    {
        UpdatePlayerUI();
    }
    public void EncounterEnemy()
    {
        if (battleUI != null) battleUI.SetActive(true);

        TopDownCharacterController[] moveScripts = Object.FindObjectsByType<TopDownCharacterController>(FindObjectsSortMode.None);
        foreach (var script in moveScripts)
        {
            if (script == null) continue;
            script.enabled = false;

            if (script.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = Vector2.zero;
            }
            if (script.TryGetComponent<Animator>(out var anim))
            {
                anim.SetBool("IsMoving", false);
            }
        }

        idou[] encounterScripts = Object.FindObjectsByType<idou>(FindObjectsSortMode.None);
        foreach (var script in encounterScripts)
        {
            if (script != null) script.enabled = false;
        }

        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        totalExpGained = 0;
        selectedEnemyIndex = 0;

        if (activeEnemies != null && activeEnemies.Count > 0)
        {
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                if (activeEnemies[i] != null)
                {
                    Destroy(activeEnemies[i].gameObject);
                }
            }
            activeEnemies.Clear();
        }
        if (fieldPlayerStatus != null && playerUnit != null)
        {
            playerUnit.Setup(fieldPlayerStatus);
        }
        UpdatePlayerUI();

        if (enemyPrefab != null && enemyDatas != null && enemyDatas.Count > 0)
        {
            int count = Random.Range(1, 4);
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(enemyPrefab, enemyField);

                if (spawnPoints != null && spawnPoints.Length > i && spawnPoints[i] != null)
                {
                    obj.transform.position = spawnPoints[i].position;
                }

                BattleUnit unit = obj.GetComponent<BattleUnit>();
                if (unit != null)
                {
                    int randomIndex = Random.Range(0, enemyDatas.Count);
                    unit.SetupEnemy(enemyDatas[randomIndex]);
                    activeEnemies.Add(unit);
                    unit.gameObject.name = enemyDatas[randomIndex].unitName + " " + (char)('A' + i);
                }
            }
        }
        Debug.Log("魔物たちが あらわれた！");
        yield return new WaitForSeconds(1f);
        PlayerTurn();
    }

    void UpdatePlayerUI()
    {
        if (playerUnit == null || playerUnit.data == null) return;

        int maxHp = (fieldPlayerStatus != null) ? fieldPlayerStatus.MaxHp : playerUnit.data.maxHp;

        if (playerNameText != null) playerNameText.text = playerUnit.data.unitName;
        if (playerHPText != null) playerHPText.text = $"HP {playerUnit.currentHp} / {maxHp}";

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = playerUnit.currentHp;

            Image fillImage = hpSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                float hpPercent = (float)playerUnit.currentHp / maxHp;
                if (hpPercent <= 0.2f) fillImage.color = Color.red;
                else if (hpPercent <= 0.5f) fillImage.color = Color.yellow;
                else fillImage.color = Color.green;
            }
        }
    }

    void PlayerTurn()
    {
        state = BattleState.PlayerTurn;
        if (playerUnit != null) playerUnit.isDefending = false;

        if (commandPanel != null) commandPanel.SetActive(true);
        if (selectionArrow != null) selectionArrow.gameObject.SetActive(true);

        if (selectedEnemyIndex >= activeEnemies.Count) selectedEnemyIndex = 0;
        UpdateArrow();
        Debug.Log("どうする？");
    }

    void Update()
    {
        if (hpSlider != null && playerUnit != null)
        {
            hpSlider.value = Mathf.Lerp(hpSlider.value, playerUnit.currentHp, Time.deltaTime * 5f);
        }

        if (state != BattleState.PlayerTurn) return;

        if (Input.GetKeyDown(KeyCode.RightArrow)) { ChangeTarget(1); }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { ChangeTarget(-1); }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (commandPanel != null) commandPanel.SetActive(false);
            if (selectionArrow != null) selectionArrow.gameObject.SetActive(false);
            StartCoroutine(PlayerAttack());
        }
    }

    void ChangeTarget(int direction)
    {
        if (activeEnemies.Count == 0) return;
        selectedEnemyIndex = (selectedEnemyIndex + direction + activeEnemies.Count) % activeEnemies.Count;
        UpdateArrow();
    }

    void UpdateArrow()
    {
        if (activeEnemies.Count == 0 || selectedEnemyIndex >= activeEnemies.Count) return;
        if (selectionArrow == null) return;

        selectionArrow.gameObject.SetActive(true);
        selectionArrow.position = activeEnemies[selectedEnemyIndex].transform.position + new Vector3(0, 1.5f, 0);
    }

    IEnumerator PlayerAttack()
    {
        state = BattleState.Busy;
        if (selectionArrow != null) selectionArrow.gameObject.SetActive(false);
        if (commandPanel != null) commandPanel.SetActive(false);

        BattleUnit target = activeEnemies[selectedEnemyIndex];
        Debug.Log($"{playerUnit.data.unitName}の攻撃！");

        int currentAtk = (fieldPlayerStatus != null) ? fieldPlayerStatus.Attack : playerUnit.data.attack;
        target.TakeDamage(currentAtk);

        yield return new WaitForSeconds(1f);

        if (target.isDead)
        {
            Debug.Log($"{target.data.unitName}を たおした！");
            if (target.data != null)
            {
                totalExpGained += target.data.exp;
            }
            activeEnemies.Remove(target);
            Destroy(target.gameObject);
        }

        if (activeEnemies.Count <= 0)
        {
            state = BattleState.Win;
            Debug.Log($"戦いに勝利した！合計 {totalExpGained} EXP 獲得！");

            if (fieldPlayerStatus != null)
            {
                int prevLevel = fieldPlayerStatus.level;
                fieldPlayerStatus.GainExp(totalExpGained);

                if (fieldPlayerStatus.level == prevLevel)
                {
                    fieldPlayerStatus.currentHp = Mathf.Clamp(playerUnit.currentHp, 0, fieldPlayerStatus.MaxHp);
                }
            }

            yield return new WaitForSeconds(1.5f);
            EndBattle();
        }
        else
        {
            if (selectedEnemyIndex >= activeEnemies.Count) selectedEnemyIndex = Mathf.Max(0, activeEnemies.Count - 1);
            StartCoroutine(EnemyAttack());
        }
    }

    IEnumerator EnemyAttack()
    {
        state = BattleState.EnemyTurn;
        foreach (var enemy in activeEnemies)
        {
            if (enemy == null || enemy.isDead) continue;

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
        if (commandPanel != null) commandPanel.SetActive(false);
        if (selectionArrow != null) selectionArrow.gameObject.SetActive(false);
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
        if (commandPanel != null) commandPanel.SetActive(false);
        playerUnit.isDefending = true;
        Debug.Log($"{playerUnit.data.unitName}は 身をまもっている！");
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyAttack());
    }

    IEnumerator PlayerEscape()
    {
        state = BattleState.Busy;
        Debug.Log("逃げ出した！");
        yield return new WaitForSeconds(1f);

        if (Random.value > 0.5f)
        {
            Debug.Log("逃げれた");
            EndBattle();
        }
        else
        {
            Debug.Log("逃げられなかった！");
            StartCoroutine(EnemyAttack());
        }
    }

    public void EndBattle()
    {
        if (battleUI != null) battleUI.SetActive(false);

        TopDownCharacterController[] moveScripts = Object.FindObjectsByType<TopDownCharacterController>(FindObjectsSortMode.None);
        foreach (var script in moveScripts)
        {
            if (script != null) script.enabled = true;
        }

        idou[] encounterScripts = Object.FindObjectsByType<idou>(FindObjectsSortMode.None);
        foreach (var script in encounterScripts)
        {
            if (script != null)
            {
                script.enabled = true;
                script.WarpReset();
            }
        }

        foreach (var enemy in activeEnemies)
        {
            if (enemy != null) Destroy(enemy.gameObject);
        }
        activeEnemies.Clear();

        if (fieldUIManager != null)
        {
            fieldUIManager.UpdateFieldUI();
        }
        else
        {
            FieldUIManager fieldUI = Object.FindFirstObjectByType<FieldUIManager>();
            if (fieldUI != null) fieldUI.UpdateFieldUI();
        }
    }
}
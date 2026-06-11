using UnityEngine;

public class idou : MonoBehaviour
{
    [Header("エンカウントするまでの必要歩数（距離）")]
    public float encounterThreshold = 5f;
    private float currentEncounterMeter = 0f;

    private Vector3 lastPosition;
    public bool canEncounter = true;

    private BattleManager battleManager;

    void Start()
    {
        FieldUIManager fieldUI = Object.FindFirstObjectByType<FieldUIManager>();

        battleManager = Object.FindFirstObjectByType<BattleManager>();

        if (battleManager == null)
        {
            Debug.LogError("シーン内に BattleManager が見つかりません！ヒエラルキーに配置されているか確認してください。");
        }

        lastPosition = transform.position;
    }

    void OnEnable()
    {
        WarpReset();
    }

    void Update()
    {
        if (!canEncounter) return;

        if (battleManager != null && battleManager.battleUI != null && battleManager.battleUI.activeSelf)
        {
            return;
        }

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved > 0)
        {
            currentEncounterMeter += distanceMoved * Random.Range(0.5f, 1.5f);
            lastPosition = transform.position;
        }

        if (currentEncounterMeter >= encounterThreshold)
        {
            StartBattle();
        }
    }

    void StartBattle()
    {
        currentEncounterMeter = 0;
        if (battleManager != null)
        {
            battleManager.EncounterEnemy();
        }
    }

    void OnDisable()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("IsMoving", false);
        }
    }

    public void WarpReset()
    {
        lastPosition = transform.position;
        currentEncounterMeter = 0f;
    }
}
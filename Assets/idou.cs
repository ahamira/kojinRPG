using UnityEngine;

public class idou : MonoBehaviour
{
    public float encounterThreshold = 5f; 
    private float currentEncounterMeter = 0f;

    private Vector3 lastPosition;
    public bool canEncounter = true; 
    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (!canEncounter) return;
        BattleManager bm = FindObjectOfType<BattleManager>();
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        if (bm != null && bm.battleUI.activeSelf) 
    {
        return; 
    }
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
        BattleManager bm = FindObjectOfType<BattleManager>();
        if (bm != null)
        {
            bm.EncounterEnemy();
        }
    }
    void OnDisable()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; 
        }
    }
}
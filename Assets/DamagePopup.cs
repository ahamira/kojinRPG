using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float moveSpeed = 150f;
    [SerializeField] private float lifeTime = 0.4f;

    private float timer;

    public void Setup(int damage)
    {
        if (text == null)
            text = GetComponent<TMP_Text>();

        text.text = damage.ToString();

        timer = lifeTime;
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
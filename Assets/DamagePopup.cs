using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public void Setup(int damage)
    {
        GetComponent<TMP_Text>().text = damage.ToString();
        Destroy(gameObject, 0.4f); 
    }

    void Update()
    {
        transform.position += new Vector3(0, 0.3f, 0);
    }
}
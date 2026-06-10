using UnityEngine;
using UnityEngine.UI; // UI（SliderやText）を使うために必須

public class FieldUIManager : MonoBehaviour
{
    public Playerstatus playerStatus; // 歩き回るプレイヤーのステータス

    [Header("フィールド画面のUIパーツ")]
    public Slider hpSlider;     
    public Text hpText;       
    public Text atkText;         
    public Text levelText;     

    public void UpdateFieldUI()
    {
        if (playerStatus == null) return;

        if (levelText != null)
        {
            levelText.text = "LV: " + playerStatus.level;
        }
        if (atkText != null)
        {
            atkText.text = "ATK: " + playerStatus.Attack;
        }
        if (hpSlider != null)
        {
            hpSlider.maxValue = playerStatus.MaxHp;     
            hpSlider.value = playerStatus.currentHp;   
        }

        if (hpText != null)
        {
            hpText.text = $"{playerStatus.currentHp} / {playerStatus.MaxHp}";
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    [Header("UI 引用 (在面板拖入)")]
    public Image iconImage;
    public Image cooldownMask;
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI levelText; // 显示等级

    [Header("默认占位图 (可选)")]
    public Sprite emptySlotSprite;

    private Color originalIconColor;
    public int SlotIndex { get; private set; }

    public void Setup(SkillData data, int index, string keyBinding, int level)
    {
        SlotIndex = index;

        // 防止 iconImage 没拖报错
        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(true);

            if (data != null)
            {
                if (data.icon != null)
                {
                    iconImage.sprite = data.icon;
                    iconImage.color = Color.white;
                }
                else
                {
                    iconImage.sprite = emptySlotSprite;
                    iconImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
            }
            else
            {
                iconImage.sprite = emptySlotSprite;
                iconImage.color = new Color(1f, 1f, 1f, 0.3f);
            }
            originalIconColor = iconImage.color;
        }

        // 防止 cooldownMask 没拖报错
        if (cooldownMask != null)
        {
            cooldownMask.fillAmount = 0;
        }

        // 防止 keyText 没拖报错
        if (keyText != null)
        {
            keyText.text = keyBinding;
        }

        // 显示等级
        if (levelText != null)
        {
            if (data != null) levelText.text = "Lv." + level;
            else levelText.text = "";
        }
    }

    public void UpdateCooldown(float current, float max)
    {
        if (cooldownMask == null || iconImage == null) return; // 防止报错

        if (current > 0 && max > 0)
        {
            cooldownMask.fillAmount = current / max;
            iconImage.color = originalIconColor * 0.3f;
        }
        else
        {
            cooldownMask.fillAmount = 0;
            iconImage.color = originalIconColor;
        }
    }
}
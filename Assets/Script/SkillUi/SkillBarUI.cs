using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBarUI : MonoBehaviour
{
    [Header("引用配置")]
    public Transform slotContainer;
    public GameObject skillSlotPrefab;
    public SkillManager skillManager;

    private List<SkillSlotUI> slotUIs = new List<SkillSlotUI>();
    private string[] keyBindings = { "auto", "auto", "auto", "auto", "auto" };

    void Start()
    {
        if (skillManager != null)
        {
            // 订阅技能变化事件
            skillManager.OnSkillBarChanged += RefreshSkillBar;

            // 游戏开始时刷新一次
            RefreshSkillBar();
        }
    }

    private void OnDestroy()
    {
        // 取消订阅，防止内存泄漏
        if (skillManager != null)
        {
            skillManager.OnSkillBarChanged -= RefreshSkillBar;
        }
    }

    private void RefreshSkillBar()
    {
        foreach (var ui in slotUIs) Destroy(ui.gameObject);
        slotUIs.Clear();

        for (int i = 0; i < skillManager.skillSlots.Count; i++)
        {
            SkillData data = skillManager.skillSlots[i];
            if (data != null)
            {
                GameObject slotObj = Instantiate(skillSlotPrefab, slotContainer);
                SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();

                string keyText = i < keyBindings.Length ? keyBindings[i] : "auto";

                // 【修改点】：获取当前等级并传入
                int level = skillManager.GetSkillLevel(data);
                slotUI.Setup(data, i, keyText, level);

                slotUIs.Add(slotUI);
                StartCoroutine(PopUpAnimation(slotObj.transform));
            }
        }
    }

    void Update()
    {
        if (skillManager == null) return;

        // 遍历当前界面上存在的 UI，通过 SlotIndex 对应数据
        foreach (var slotUI in slotUIs)
        {
            SkillData data = skillManager[slotUI.SlotIndex];
            if (data != null)
            {
                float currentCd = skillManager.GetCooldownRemaining(slotUI.SlotIndex);

                // 【修复点】：用 slotUI.SlotIndex 替换 i
                float maxCd = skillManager.GetCurrentCooldownTime(slotUI.SlotIndex);
                slotUI.UpdateCooldown(currentCd, maxCd);
            }
        }
    }

    private IEnumerator PopUpAnimation(Transform target)
    {
        target.localScale = Vector3.zero;
        float duration = 0.2f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(0f, 1.2f, timer / duration);
            if (timer / duration > 0.7f)
            {
                scale = Mathf.Lerp(1.2f, 1f, (timer / duration - 0.7f) / 0.3f);
            }
            target.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
        target.localScale = Vector3.one;
    }
}
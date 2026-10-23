using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic; // 引入泛型，需要用到 List

public class DebugPanel : MonoBehaviour
{
    [Header("面板控制")]
    public GameObject panelRoot;
    public GameObject dimMask;

    [Header("经济输入与按钮")]
    public TMP_InputField inputAmount;
    public Button btnAddGold;
    public Button btnAddExp;

    [Header("战斗与技能按钮")]
    public Button btnHealPlayer;
    public Button btnTriggerHextech; // 触发三选一
    public Button btnUpgradeAllSkills;
    public Button btnClearSkills;

    [Header("数据引用")]
    public SkillManager skillManager;
    public PlayerHealth playerHealth;

    // 【修改点】用最新的 HextechUI 替换旧的 HextechManager
    public HextechUI hextechUI;

    private float previousTimeScale = 1f;

    void Start()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        if (dimMask != null) dimMask.SetActive(false);

        if (btnAddGold != null) btnAddGold.onClick.AddListener(OnAddGoldClicked);
        if (btnAddExp != null) btnAddExp.onClick.AddListener(OnAddExpClicked);
        if (btnHealPlayer != null) btnHealPlayer.onClick.AddListener(OnHealClicked);
        if (btnTriggerHextech != null) btnTriggerHextech.onClick.AddListener(OnHextechClicked);
        if (btnUpgradeAllSkills != null) btnUpgradeAllSkills.onClick.AddListener(OnUpgradeAllClicked);
        if (btnClearSkills != null) btnClearSkills.onClick.AddListener(OnClearClicked);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (panelRoot != null)
            {
                bool isActive = !panelRoot.activeSelf;

                panelRoot.SetActive(isActive);
                if (dimMask != null) dimMask.SetActive(isActive);

                if (isActive)
                {
                    previousTimeScale = Time.timeScale;
                    Time.timeScale = 0f;
                }
                else
                {
                    Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f;
                    ClearInputFocus();
                }
            }
        }

        if (panelRoot != null && panelRoot.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                ClearInputFocus();
            }
        }
    }

    private void ClearInputFocus()
    {
        if (inputAmount != null)
        {
            inputAmount.DeactivateInputField();
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private int GetInputValue()
    {
        int amount = 100;
        if (inputAmount != null && !string.IsNullOrEmpty(inputAmount.text))
        {
            int.TryParse(inputAmount.text, out amount);
        }
        ClearInputFocus();
        return amount;
    }

    private void OnAddGoldClicked()
    {
        int amount = GetInputValue();
        PlayerStats.AddGold(amount);
    }

    private void OnAddExpClicked()
    {
        int amount = GetInputValue();
        PlayerStats.AddExp(amount);
        Debug.Log($"调试：经验增加 {amount}");
    }

    private void OnHealClicked()
    {
        if (playerHealth != null) playerHealth.HealToFull();
    }

    // 【核心修改】触发最新的三选一卡池
    private void OnHextechClicked()
    {
        if (UpgradeManager.Instance != null && hextechUI != null)
        {
            // 关闭调试面板的暂停状态，防止冲突（如果三选一自带暂停的话）
            if (panelRoot != null) panelRoot.SetActive(false);
            if (dimMask != null) dimMask.SetActive(false);
            Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f;

            // 1. 抽卡
            List<UpgradeData> choices = UpgradeManager.Instance.GetThreeRandomUpgrades();

            // 2. 弹出 UI
            hextechUI.ShowChoices(choices, (selectedUpgrade) =>
            {
                // 3. 选完直接执行
                selectedUpgrade.Apply();
                Debug.Log($"调试面板触发并选择了：{selectedUpgrade.upgradeName}");
            });

            Debug.Log("调试：触发了三选一！");
        }
        else
        {
            Debug.LogError("无法触发三选一，请检查 UpgradeManager.Instance 或 HextechUI 引用是否为空！");
        }
    }

    private void OnUpgradeAllClicked()
    {
        if (skillManager != null)
        {
            for (int i = 0; i < skillManager.skillSlots.Count; i++)
            {
                if (skillManager.skillSlots[i] != null)
                {
                    skillManager.UnlockSkill(skillManager.skillSlots[i]);
                }
            }
            Debug.Log("调试：所有技能升 1 级");
        }
    }

    private void OnClearClicked()
    {
        if (skillManager != null)
        {
            skillManager.ClearAllSkills();
        }
    }
}
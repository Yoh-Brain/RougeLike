using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DebugPanel : MonoBehaviour
{
    [Header("面板控制")]
    public GameObject panelRoot;         // 拖入调试面板本身
    public GameObject dimMask;           // 【新增】拖入全屏黑色遮罩

    [Header("经济输入与按钮")]
    public TMP_InputField inputAmount;
    public Button btnAddGold;
    public Button btnAddExp;

    [Header("战斗与技能按钮")]
    public Button btnHealPlayer;
    public Button btnTriggerHextech;
    public Button btnUpgradeAllSkills;
    public Button btnClearSkills;

    [Header("数据引用")]
    public SkillManager skillManager;
    public HextechManager hextechManager;
    public PlayerHealth playerHealth;

    private float previousTimeScale = 1f; // 记录打开面板前的时间流速

    void Start()
    {
        // 初始隐藏
        if (panelRoot != null) panelRoot.SetActive(false);
        if (dimMask != null) dimMask.SetActive(false);

        // 绑定按钮事件
        if (btnAddGold != null) btnAddGold.onClick.AddListener(OnAddGoldClicked);
        if (btnAddExp != null) btnAddExp.onClick.AddListener(OnAddExpClicked);
        if (btnHealPlayer != null) btnHealPlayer.onClick.AddListener(OnHealClicked);
        if (btnTriggerHextech != null) btnTriggerHextech.onClick.AddListener(OnHextechClicked);
        if (btnUpgradeAllSkills != null) btnUpgradeAllSkills.onClick.AddListener(OnUpgradeAllClicked);
        if (btnClearSkills != null) btnClearSkills.onClick.AddListener(OnClearClicked);
    }

    void Update()
    {
        // 1. 按 P 键 或 ~ 键开关面板
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (panelRoot != null)
            {
                bool isActive = !panelRoot.activeSelf;

                // 控制面板和遮罩的显隐
                panelRoot.SetActive(isActive);
                if (dimMask != null) dimMask.SetActive(isActive);

                // 【核心】控制游戏时间暂停与恢复
                if (isActive)
                {
                    previousTimeScale = Time.timeScale;
                    Time.timeScale = 0f; // 暂停游戏
                }
                else
                {
                    Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f; // 恢复游戏
                    ClearInputFocus();
                }
            }
        }

        // 2. 防误触：如果按了 WASD，强制取消输入框的焦点
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
        Debug.Log($"调试：金币增加 {amount}");
    }

    private void OnAddExpClicked()
    {
        int amount = GetInputValue();
        Debug.Log($"调试：经验增加 {amount}（待接入经验系统）");
    }

    private void OnHealClicked()
    {
        if (playerHealth != null) playerHealth.HealToFull();
    }

    private void OnHextechClicked()
    {
        if (hextechManager != null) hextechManager.TriggerHextech();
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
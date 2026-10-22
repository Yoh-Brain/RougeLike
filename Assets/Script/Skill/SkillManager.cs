using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("发射点引用")]
    public Transform firePoint;

    [Header("已解锁技能栏 (初始全空)")]
    public List<SkillData> skillSlots = new List<SkillData>();

    [Header("自动发射设置")]
    public bool autoFire = true;
    public bool allowManualFire = false;

    [Header("方向控制")]
    public Camera aimCamera;

    public event Action OnSkillBarChanged;
    public event Action<int> OnSkillUsed;

    private Dictionary<SkillData, float> cooldownTimers = new Dictionary<SkillData, float>();
    private Dictionary<SkillData, GameObject> activeOrbitWeapons = new Dictionary<SkillData, GameObject>();

    // 记录技能等级
    private Dictionary<SkillData, int> skillLevels = new Dictionary<SkillData, int>();

    public SkillData this[int index]
    {
        get
        {
            if (index >= 0 && index < skillSlots.Count) return skillSlots[index];
            return null;
        }
    }

    // 获取技能的当前等级，UI 显示和海克斯判断都要用
    public int GetSkillLevel(SkillData skill)
    {
        if (skill == null) return 0;
        if (skillLevels.ContainsKey(skill)) return skillLevels[skill];
        return 0; // 0 代表未拥有
    }

    // 获取当前槽位技能的最终冷却时间（供 UI 调用）
    public float GetCurrentCooldownTime(int index)
    {
        SkillData skill = this[index];
        if (skill == null) return 1f;
        int lv = GetSkillLevel(skill);
        return skill.GetFinalCooldown(lv);
    }

    // 解锁或升级技能
    public bool UnlockSkill(SkillData newSkill)
    {
        if (newSkill == null) return false;

        // 1. 如果已经有这个技能，尝试升级
        if (skillLevels.ContainsKey(newSkill))
        {
            int currentLv = skillLevels[newSkill];
            if (currentLv >= newSkill.maxLevel)
            {
                Debug.Log($"技能 {newSkill.skillName} 已达最高等级！");
                return false;
            }

            skillLevels[newSkill] = currentLv + 1;
            Debug.Log($"技能 {newSkill.skillName} 升级到了 {skillLevels[newSkill]} 级！");
            OnSkillBarChanged?.Invoke();
            return true;
        }

        // 2. 如果是新技能，找一个空槽位填入
        for (int i = 0; i < skillSlots.Count; i++)
        {
            if (skillSlots[i] == null)
            {
                skillSlots[i] = newSkill;
                skillLevels[newSkill] = 1; // 初始设为 1 级
                Debug.Log($"在槽位 {i} 解锁了新技能：{newSkill.skillName} Lv.1");
                OnSkillBarChanged?.Invoke();
                return true;
            }
        }

        Debug.LogWarning("技能槽已满，无法装备新技能！");
        return false;
    }

    // 【新增】提供给外部（如调试面板）调用的清空技能方法
    public void ClearAllSkills()
    {
        // 1. 清空技能槽数据
        for (int i = 0; i < skillSlots.Count; i++)
        {
            skillSlots[i] = null;
        }

        // 2. 清空等级数据
        skillLevels.Clear();

        // 3. 清理场上的环绕武器（比如镰刀）
        foreach (var weapon in activeOrbitWeapons.Values)
        {
            if (weapon != null) Destroy(weapon);
        }
        activeOrbitWeapons.Clear();

        // 4. 清空所有冷却计时
        cooldownTimers.Clear();

        // 5. 在类内部触发事件，通知 UI 刷新
        OnSkillBarChanged?.Invoke();

        Debug.Log("【调试】：玩家技能已全部清空重置！");
    }

    private void Update()
    {
        List<SkillData> keys = new List<SkillData>(cooldownTimers.Keys);
        foreach (var key in keys)
        {
            if (cooldownTimers[key] > 0)
            {
                cooldownTimers[key] -= Time.deltaTime;
            }
        }

        if (autoFire)
        {
            AutoFireAllSlots();
        }
    }

    private void AutoFireAllSlots()
    {
        Vector3 aimPos = GetMouseWorldPosition();

        for (int i = 0; i < skillSlots.Count; i++)
        {
            SkillData skill = this[i];
            if (skill == null) continue;

            if (skill.skillType == SkillType.OrbitMelee &&
                activeOrbitWeapons.TryGetValue(skill, out GameObject existing) &&
                existing != null)
            {
                continue;
            }

            bool isReady = !cooldownTimers.ContainsKey(skill) || cooldownTimers[skill] <= 0f;
            if (isReady)
            {
                TryUseSkill(i, aimPos, isAuto: true);
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Camera cam = aimCamera != null ? aimCamera : Camera.main;
        if (cam == null)
        {
            return firePoint.position + firePoint.right;
        }

        Vector3 screenPos = Input.mousePosition;
        float planeDistance = Mathf.Abs(cam.transform.position.z - firePoint.position.z);
        screenPos.z = planeDistance;

        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;
        return worldPos;
    }

    public bool TryUseSkill(int index, Vector3 mousePos, bool isAuto = false)
    {
        if (!allowManualFire && !isAuto) return false;

        SkillData skill = this[index];
        if (skill == null) return false;

        if (cooldownTimers.ContainsKey(skill) && cooldownTimers[skill] > 0)
        {
            Debug.Log($"[{skill.skillName}] 冷却中，剩余 {cooldownTimers[skill]:F1} 秒");
            return false;
        }

        int currentLv = GetSkillLevel(skill);

        if (!ExecuteSkill(skill, mousePos, currentLv)) return false;

        // 使用计算后的冷却时间
        cooldownTimers[skill] = skill.GetFinalCooldown(currentLv);
        OnSkillUsed?.Invoke(index);
        return true;
    }

    public float GetCooldownRemaining(int index)
    {
        SkillData skill = this[index];
        if (skill != null && cooldownTimers.ContainsKey(skill))
        {
            return cooldownTimers[skill];
        }
        return 0f;
    }

    private bool ExecuteSkill(SkillData skill, Vector3 mousePos, int level)
    {
        switch (skill.skillType)
        {
            case SkillType.Projectile:
                ExecuteShooting(skill, mousePos, level);
                return true;

            case SkillType.OrbitMelee:
                return SpawnOrbitWeapon(skill, level);

            default:
                return false;
        }
    }

    private bool SpawnOrbitWeapon(SkillData skill, int level)
    {
        if (skill.orbitWeaponPrefab == null)
        {
            Debug.LogWarning($"[{skill.skillName}] 没有配置 orbitWeaponPrefab");
            return false;
        }

        if (activeOrbitWeapons.TryGetValue(skill, out GameObject existing) && existing != null)
        {
            return false;
        }

        GameObject obj = Instantiate(skill.orbitWeaponPrefab, firePoint.position, Quaternion.identity);
        ScytheController scythe = obj.GetComponent<ScytheController>();
        if (scythe != null)
        {
            scythe.Init(
                transform,
                skill.GetFinalDamage(level), // 传入成长后的伤害
                skill.orbitDamageInterval,
                skill.orbitDuration,
                skill.enemyLayer
            );
        }

        activeOrbitWeapons[skill] = obj;
        return true;
    }

    private void ExecuteShooting(SkillData skill, Vector3 mousePos, int level)
    {
        mousePos.z = 0;

        Vector3 spawnPosition = firePoint.position;
        spawnPosition.z = 0;

        Vector2 shootDirection = (mousePos - spawnPosition).normalized;
        float centerAngle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        // 获取成长后的数值
        float actualDamage = skill.GetFinalDamage(level);
        int actualArrows = skill.GetFinalArrows(level);

        if (actualArrows <= 1)
        {
            GameObject arrowObj = Instantiate(skill.arrowPrefab, spawnPosition, Quaternion.Euler(0, 0, centerAngle));
            arrowObj.GetComponent<Arrow>().Setup(actualDamage);
            return;
        }

        float angleSpread = skill.totalSpreadAngle / (actualArrows - 1);
        float startAngle = centerAngle - (skill.totalSpreadAngle / 2f);

        for (int i = 0; i < actualArrows; i++)
        {
            float currentAngle = startAngle + (angleSpread * i);
            GameObject arrowObj = Instantiate(skill.arrowPrefab, spawnPosition, Quaternion.Euler(0, 0, currentAngle));
            arrowObj.GetComponent<Arrow>().Setup(actualDamage);
        }
    }
}
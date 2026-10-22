using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    // 只需要拿到管理器的引用
    private SkillManager skillManager;

    void Start()
    {
        // 获取同物体上的 SkillManager 组件
        skillManager = GetComponent<SkillManager>();
    }

    void Update()
    {
        // 获取鼠标位置
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 按鼠标左键，使用技能栏的第 0 号技能（普攻）
        if (Input.GetMouseButtonDown(0))
        {
            skillManager.TryUseSkill(0, mousePos);
        }

        // 【新增】按鼠标右键：释放索引 1（火球）
        if (Input.GetMouseButtonDown(1))
        {
            skillManager.TryUseSkill(1, mousePos);
        }
        // 如果以后你解锁了第 1 号技能，按键盘 1 释放：
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     skillManager.TryUseSkill(1, mousePos);
        // }
    }
}
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 游戏是否已开始
    public bool IsGameStarted { get; private set; } = false;

    // 游戏运行时间
    public float GameTime { get; private set; } = 0f;

    private void Awake()
    {
        // 单例模式，保证全局只有一个 GameManager
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 游戏刚开始时，暂停时间（可选，如果你希望菜单背景有动画，就不要设为 0）
        Time.timeScale = 0f;
    }

    private void Update()
    {
        // 只有当游戏正式开始后，才计算时间
        if (IsGameStarted)
        {
            // 使用 unscaledDeltaTime 以防有暂停菜单影响
            GameTime += Time.deltaTime;
        }
    }

    // 由开始菜单按钮调用
    public void StartGame()
    {
        IsGameStarted = true;
        Time.timeScale = 1f; // 恢复游戏正常时间流速
        Debug.Log("游戏开始！计时器启动。");
    }
}
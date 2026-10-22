using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject mainMenuPanel;
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;

    void Start()
    {
        // 绑定事件
        startButton.onClick.AddListener(OnStartClicked);
        settingsButton.onClick.AddListener(() => Debug.Log("设置面板暂未实现"));
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnStartClicked()
    {
        mainMenuPanel.SetActive(false);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            Debug.LogError("找不到 GameManager！请检查场景中是否挂载了 GameManager 脚本。");
        }
    }

    private void OnQuitClicked()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
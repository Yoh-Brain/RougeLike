using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("刷怪基础设置")]
    public GameObject enemyPrefab;
    public float spawnMargin = 1.5f;

    [Header("动态数量配置")]
    public int initialMaxEnemies = 5;
    public int maxEnemiesIncrease = 5;
    public float enemyMaxIncreaseInterval = 15f;

    [Header("动态血量配置")]
    public float baseHealth = 30f;
    public float healthIncreaseRate = 0.3f;
    public float waveInterval = 30f;

    public int currentMaxEnemies;
    public int aliveEnemiesCount = 0;
    private float currentHealthMultiplier = 1f;

    private Transform playerTransform;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        currentMaxEnemies = initialMaxEnemies;
        aliveEnemiesCount = 0;

        // 【修改】订阅新的 EnemyHealth 事件
        EnemyHealth.OnEnemyDied += HandleEnemyDied;

        for (int i = 0; i < currentMaxEnemies; i++)
        {
            SpawnEnemy();
        }

        StartCoroutine(MaxEnemyLimitRoutine());
        StartCoroutine(HealthGrowthRoutine());
    }

    void OnDestroy()
    {
        EnemyHealth.OnEnemyDied -= HandleEnemyDied;
    }

    private IEnumerator MaxEnemyLimitRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(enemyMaxIncreaseInterval);
            currentMaxEnemies += maxEnemiesIncrease;
            Debug.Log($"<color=cyan>时间到！场上数量上限提升至：{currentMaxEnemies}</color>");
        }
    }

    private IEnumerator HealthGrowthRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveInterval);
            currentHealthMultiplier += healthIncreaseRate;
            Debug.Log($"<color=orange>新的一波！怪物血量倍率：{currentHealthMultiplier:F2}</color>");
        }
    }

    private void HandleEnemyDied()
    {
        aliveEnemiesCount--;
        Debug.Log($"<color=red>【生成器】收到死亡通知，当前存活：{aliveEnemiesCount} / 上限：{currentMaxEnemies}</color>");

        if (aliveEnemiesCount < currentMaxEnemies)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || playerTransform == null || mainCam == null) return;

        Vector2 spawnPos = GetRandomSpawnPositionOutsideCamera();
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // 【修改】通过 EnemyHealth 组件初始化血量
        EnemyHealth healthScript = newEnemy.GetComponent<EnemyHealth>();
        if (healthScript != null)
        {
            float finalHealth = baseHealth * currentHealthMultiplier;
            healthScript.InitHealth(finalHealth);
        }

        aliveEnemiesCount++;
        Debug.Log($"<color=green>生成怪物，当前数量：{aliveEnemiesCount}</color>");
    }

    private Vector2 GetRandomSpawnPositionOutsideCamera()
    {
        int side = Random.Range(0, 4);
        float randomValue = Random.Range(0f, 1f);

        float viewX = 0f;
        float viewY = 0f;
        float offset = spawnMargin / mainCam.orthographicSize;

        switch (side)
        {
            case 0: viewX = randomValue; viewY = 1 + offset; break;
            case 1: viewX = randomValue; viewY = 0 - offset; break;
            case 2: viewX = 0 - offset * (mainCam.aspect); viewY = randomValue; break;
            case 3: viewX = 1 + offset * (mainCam.aspect); viewY = randomValue; break;
        }

        Vector3 worldPos = mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, 10f));
        worldPos.z = 0f;
        return worldPos;
    }
}
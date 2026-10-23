using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnEntry
    {
        public GameObject enemyPrefab;
        [Tooltip("生成权重，数值越大抽中概率越高")]
        public float weight = 1f;
    }

    [System.Serializable]
    public class WaveConfig
    {
        public string waveName = "第一波";
        public float duration = 30f;      // 该波持续时间
        public int maxAliveEnemies = 5;   // 该波场上最大存活数
        public float healthMultiplier = 1f; // 该波生成怪物的血量倍率
    }

    [Header("怪物池配置")]
    public List<EnemySpawnEntry> enemyPool;

    [Header("波次配置")]
    public List<WaveConfig> waves;
    [Tooltip("如果波次用完了，是否无限循环使用最后一波")]
    public bool loopLastWave = true;

    [Header("刷怪环境配置")]
    public float spawnMargin = 1.5f;

    // --- 运行时状态 ---
    private int currentWaveIndex = -1;
    private WaveConfig currentWave;
    private int currentAlive = 0;
    private float totalWeight = 0f;

    private Transform playerTransform;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        // 计算总权重
        foreach (var entry in enemyPool) totalWeight += entry.weight;

        EnemyHealth.OnEnemyDied += HandleEnemyDied;

        // 启动波次循环
        StartCoroutine(WaveLoopRoutine());
    }

    void OnDestroy()
    {
        EnemyHealth.OnEnemyDied -= HandleEnemyDied;
    }

    private IEnumerator WaveLoopRoutine()
    {
        while (true)
        {
            // 切换到下一波
            currentWaveIndex++;

            // 如果波次用完了，处理溢出逻辑
            if (currentWaveIndex >= waves.Count)
            {
                if (loopLastWave)
                {
                    currentWaveIndex = waves.Count - 1; // 停留在最后一波无限刷
                }
                else
                {
                    Debug.Log("所有波次已结束！");
                    yield break; // 结束协程
                }
            }

            currentWave = waves[currentWaveIndex];
            Debug.Log($"<color=cyan>进入新波次: {currentWave.waveName}，最大存活数: {currentWave.maxAliveEnemies}，血量倍率: {currentWave.healthMultiplier}</color>");

            // 【核心补齐机制】切波瞬间，立刻补齐缺口
            int gap = currentWave.maxAliveEnemies - currentAlive;
            if (gap > 0)
            {
                Debug.Log($"<color=orange>切波瞬间补齐缺口，生成 {gap} 只怪！</color>");
                for (int i = 0; i < gap; i++)
                {
                    SpawnOneEnemy();
                }
            }

            // 等待这一波的时间（不间断，直接进入下一波）
            yield return new WaitForSeconds(currentWave.duration);
        }
    }

    private void HandleEnemyDied()
    {
        currentAlive--;

        // 只有当场上数量低于当前波次上限时，才补怪
        if (currentAlive < currentWave.maxAliveEnemies)
        {
            SpawnOneEnemy();
        }
    }

    private void SpawnOneEnemy()
    {
        if (enemyPool.Count == 0 || playerTransform == null || mainCam == null) return;

        // 1. 权重随机算法，抽出一个怪物
        GameObject selectedPrefab = GetRandomEnemyByWeight();
        if (selectedPrefab == null) return;

        // 2. 获取屏幕外坐标
        Vector2 spawnPos = GetRandomSpawnPositionOutsideCamera();

        // 3. 生成怪物
        GameObject newEnemy = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);

        // 4. 配置血量（通过 EnemyHealth 初始化）
        EnemyHealth healthScript = newEnemy.GetComponent<EnemyHealth>();
        if (healthScript != null)
        {
            // 读取该怪物原本的基础血量，然后乘以波次的血量倍率
            float finalHealth = healthScript.maxHealth * currentWave.healthMultiplier;
            healthScript.InitHealth(finalHealth);
        }

        currentAlive++;
    }

    // 轮盘赌权重随机
    private GameObject GetRandomEnemyByWeight()
    {
        if (totalWeight <= 0) return enemyPool[0].enemyPrefab;

        float randomVal = Random.Range(0f, totalWeight);
        float currentSum = 0f;

        foreach (var entry in enemyPool)
        {
            currentSum += entry.weight;
            if (randomVal <= currentSum)
            {
                return entry.enemyPrefab;
            }
        }
        return enemyPool[0].enemyPrefab;
    }

    private Vector2 GetRandomSpawnPositionOutsideCamera()
    {
        int side = Random.Range(0, 4);
        float randomValue = Random.Range(0f, 1f);
        float viewX = 0f, viewY = 0f;
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
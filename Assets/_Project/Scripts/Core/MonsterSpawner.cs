using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(100)]
public class MonsterSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Transform player;
    [SerializeField] private MonsterSpawnTableSO spawnTable;
    [SerializeField] private PlayerStat playerStat;

    [Header("Spawn Range")]
    [SerializeField] private float minSpawnRadius = 7f;
    [SerializeField] private float maxSpawnRadius = 15f;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int spawnCountPerInterval = 2;
    [SerializeField] private int maxMonsterCount = 20;
    [SerializeField] private int maxSpawnAttempts = 10;

    private readonly List<GameObject> _activeMonsters = new();
    private float _elapsedTime;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        if (tilemap == null || player == null || spawnTable == null)
        {
            Debug.LogError("[MonsterSpawner] 필수 참조가 누락되었습니다.");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return null; // GroundSetting.Start() 완료 대기

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            _elapsedTime += spawnInterval;

            _activeMonsters.RemoveAll(m => m == null);

            var available = spawnTable.GetAvailableEntries((int)_elapsedTime);
            if (available.Count == 0) continue;

            // 플레이 시간 1분당 스폰량 2배 (1분 -> x2, 2분 -> x4, ...)
            int elapsedMinutes = playerStat != null ? (int)playerStat.playTime.TotalMinutes : 0;
            int spawnCount = spawnCountPerInterval * (int)Mathf.Pow(2, elapsedMinutes);

            int toSpawn = Mathf.Min(Mathf.Max(spawnCount, spawnCountPerInterval), maxMonsterCount - _activeMonsters.Count);
            for (int i = 0; i < toSpawn; i++)
                TrySpawnOne(available);
        }
    }

    void TrySpawnOne(List<SpawnEntry> available)
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist  = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector2 candidate = (Vector2)player.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            if (tilemap.HasTile(tilemap.WorldToCell(candidate)))
            {
                var prefab = spawnTable.PickRandom(available);
                _activeMonsters.Add(Instantiate(prefab, candidate, Quaternion.identity));
                return;
            }
        }
    }
}

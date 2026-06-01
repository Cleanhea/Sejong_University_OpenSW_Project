using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterSpawnTable", menuName = "Game/Monster Spawn Table")]
public class MonsterSpawnTableSO : ScriptableObject
{
    public List<SpawnEntry> entries = new();

    public List<SpawnEntry> GetAvailableEntries(int elapsedSeconds)
    {
        return entries.FindAll(e => e.monsterPrefab != null && e.spawnWeight > 0f && e.unlockTime <= elapsedSeconds);
    }

    public GameObject PickRandom(List<SpawnEntry> available)
    {
        float total = 0f;
        foreach (var e in available) total += e.spawnWeight;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;
        foreach (var e in available)
        {
            cumulative += e.spawnWeight;
            if (roll <= cumulative) return e.monsterPrefab;
        }
        return available[^1].monsterPrefab;
    }
}

[System.Serializable]
public class SpawnEntry
{
    public GameObject monsterPrefab;

    [Range(0f, 1f)]
    public float spawnWeight = 1f;

    [Tooltip("게임 시작 후 몇 초 뒤부터 스폰 풀에 추가되는지")]
    public int unlockTime = 0;
}

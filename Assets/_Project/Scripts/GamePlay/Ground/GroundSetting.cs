using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundSetting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase groundTile;
    [SerializeField] private Transform player;

    [Header("Map Settings")]
    [SerializeField] private int mapRadius = 20;
    [SerializeField] private int preplacedRadius = 3;
    [SerializeField] private float ringInterval = 0.06f;
    [SerializeField] private float playerCheckInterval = 0.2f;

    [Header("Drop Animation")]
    [SerializeField] private float dropHeight = 4f; // 타일이 떨어질 시작 높이 (월드 단위) - 타일이 생성될 때 이 높이에서 시작하여 땅으로 떨어짐
    [SerializeField] private float dropDuration = 0.3f; // 타일이 떨어지는 데 걸리는 시간 (초) - 타일이 시작 높이에서 땅까지 떨어지는 데 걸리는 시간

    private readonly HashSet<Vector3Int> placedCells = new HashSet<Vector3Int>();
    private readonly HashSet<Vector3Int> pendingCells = new HashSet<Vector3Int>();
    private Vector3Int lastPlayerCell;
    private bool isInitialSpawnDone = false;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();

        if (player == null || tilemap == null || groundTile == null)
        {
            Debug.LogError("[GroundSetting] 필수 참조가 누락되었습니다.");
            return;
        }

        lastPlayerCell = tilemap.WorldToCell(player.position);
        PlaceImmediate(lastPlayerCell);
        StartCoroutine(InitialSpread(lastPlayerCell));
    }

    IEnumerator InitialSpread(Vector3Int center)
    {
        yield return StartCoroutine(SpreadCircleAnimated(center));
        isInitialSpawnDone = true;
        StartCoroutine(TrackPlayerMovement());
    }

    IEnumerator TrackPlayerMovement()
    {
        while (true)
        {
            yield return new WaitForSeconds(playerCheckInterval);

            Vector3Int currentCell = tilemap.WorldToCell(player.position);
            if (currentCell != lastPlayerCell)
            {
                lastPlayerCell = currentCell;
                SpreadCircleImmediate(currentCell);
            }
        }
    }

    // 초기 생성: 링 순서대로 드롭 애니메이션과 함께 퍼뜨림
    IEnumerator SpreadCircleAnimated(Vector3Int center)
    {
        var cells = GetSortedCircleCells(center);

        int currentRing = -1;
        foreach (var (cell, ring) in cells)
        {
            if (ring != currentRing)
            {
                if (currentRing >= 0)
                    yield return new WaitForSeconds(ringInterval);
                currentRing = ring;
            }
            TryQueueDropTile(cell);
        }
    }

    // 이동 후 생성: 애니메이션 없이 즉시 배치
    void SpreadCircleImmediate(Vector3Int center)
    {
        var cells = GetSortedCircleCells(center);
        foreach (var (cell, _) in cells)
        {
            if (placedCells.Contains(cell) || pendingCells.Contains(cell)) continue;
            tilemap.SetTile(cell, groundTile);
            placedCells.Add(cell);
        }
    }

    void PlaceImmediate(Vector3Int center)
    {
        for (int x = -preplacedRadius; x <= preplacedRadius; x++)
        {
            for (int y = -preplacedRadius; y <= preplacedRadius; y++)
            {
                if (Mathf.Sqrt(x * x + y * y) > preplacedRadius) continue;

                var cell = new Vector3Int(center.x + x, center.y + y, center.z);
                tilemap.SetTile(cell, groundTile);
                placedCells.Add(cell);
            }
        }
    }

    bool TryQueueDropTile(Vector3Int cell)
    {
        if (placedCells.Contains(cell) || pendingCells.Contains(cell))
            return false;

        pendingCells.Add(cell);
        StartCoroutine(DropTile(cell));
        return true;
    }

    IEnumerator DropTile(Vector3Int cellPos)
    {
        Vector3 landPos = tilemap.GetCellCenterWorld(cellPos);
        Vector3 startPos = landPos + Vector3.down * dropHeight;

        GameObject go = new GameObject("FallingTile");
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();

        if (groundTile is Tile tile)
            sr.sprite = tile.sprite;

        TilemapRenderer tmr = tilemap.GetComponent<TilemapRenderer>();
        sr.sortingLayerName = tmr.sortingLayerName;
        sr.sortingOrder = tmr.sortingOrder - 1;
        go.transform.position = startPos;

        float elapsed = 0f;
        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dropDuration);
            go.transform.position = Vector3.Lerp(startPos, landPos, t * t); // 중력 가속
            yield return null;
        }

        tilemap.SetTile(cellPos, groundTile);
        placedCells.Add(cellPos);
        pendingCells.Remove(cellPos);
        Destroy(go);
    }

    // 유클리드 거리 기반으로 원형 내 셀을 링 순서로 정렬하여 반환
    List<(Vector3Int cell, int ring)> GetSortedCircleCells(Vector3Int center)
    {
        var result = new List<(Vector3Int, int)>();

        for (int x = -mapRadius; x <= mapRadius; x++)
        {
            for (int y = -mapRadius; y <= mapRadius; y++)
            {
                float dist = Mathf.Sqrt(x * x + y * y);
                if (dist > mapRadius) continue;

                var cell = new Vector3Int(center.x + x, center.y + y, center.z);
                if (!placedCells.Contains(cell) && !pendingCells.Contains(cell))
                    result.Add((cell, Mathf.RoundToInt(dist)));
            }
        }

        result.Sort((a, b) => a.Item2.CompareTo(b.Item2));
        return result;
    }
}

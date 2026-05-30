using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundSetting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase groundTile;

    [Header("Map Settings")]
    [SerializeField] private int worldRadius = 500;
    [SerializeField] private int safeRadius = 5;
    [SerializeField] private int seed = 0;

    [Header("Cellular Automata")]
    [SerializeField] [Range(0f, 1f)] private float initialFillRate = 0.48f;
    [SerializeField] private int smoothIterations = 5;
    [SerializeField] private int birthThreshold = 5;
    [SerializeField] private int surviveThreshold = 4;
    [SerializeField] [Range(0f, 1f)] private float edgeFadeStart = 0.75f;

    void Start()
    {
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();
        if (tilemap == null || groundTile == null)
        {
            Debug.LogError("[GroundSetting] 필수 참조가 누락되었습니다.");
            return;
        }

        var origin = tilemap.WorldToCell(transform.position);
        var rng = seed == 0 ? new System.Random() : new System.Random(seed);
        PlaceMap(origin, rng);
    }

    void PlaceMap(Vector3Int center, System.Random rng)
    {
        int size = worldRadius * 2 + 1;
        long wr2 = (long)worldRadius * worldRadius;
        long safeR2 = (long)safeRadius * safeRadius;

        bool[,] grid = InitializeGrid(size, wr2, safeR2, rng);

        for (int iter = 0; iter < smoothIterations; iter++)
            grid = ApplyCA(grid, size, wr2, safeR2);

        PlaceTiles(grid, center, wr2);
    }

    bool[,] InitializeGrid(int size, long wr2, long safeR2, System.Random rng)
    {
        bool[,] grid = new bool[size, size];
        int r = worldRadius;

        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                long d2 = (long)x * x + (long)y * y;
                if (d2 > wr2) continue;

                int gx = x + r;
                int gy = y + r;

                if (d2 <= safeR2)
                {
                    grid[gx, gy] = true;
                    continue;
                }

                // 가장자리로 갈수록 초기 육지 확률을 줄여 자연스러운 해안선 형성
                float t = (float)System.Math.Sqrt(d2) / r;
                float fill = initialFillRate * (1f - Mathf.SmoothStep(edgeFadeStart, 1f, t));
                grid[gx, gy] = rng.NextDouble() < fill;
            }
        }
        return grid;
    }

    bool[,] ApplyCA(bool[,] grid, int size, long wr2, long safeR2)
    {
        bool[,] next = new bool[size, size];
        int r = worldRadius;

        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                long d2 = (long)x * x + (long)y * y;
                if (d2 > wr2) continue;

                int gx = x + r;
                int gy = y + r;

                if (d2 <= safeR2)
                {
                    next[gx, gy] = true;
                    continue;
                }

                int neighbors = CountNeighbors(grid, gx, gy, size, x, y, wr2);
                next[gx, gy] = grid[gx, gy] ? neighbors >= surviveThreshold : neighbors >= birthThreshold;
            }
        }
        return next;
    }

    int CountNeighbors(bool[,] grid, int gx, int gy, int size, int wx, int wy, long wr2)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = gx + dx;
                int ny = gy + dy;
                if (nx < 0 || nx >= size || ny < 0 || ny >= size) continue;

                // 원 밖 셀은 바다로 처리 (카운트 제외)
                long nd2 = (long)(wx + dx) * (wx + dx) + (long)(wy + dy) * (wy + dy);
                if (nd2 > wr2) continue;

                if (grid[nx, ny]) count++;
            }
        }
        return count;
    }

    void PlaceTiles(bool[,] grid, Vector3Int center, long wr2)
    {
        int r = worldRadius;
        var positions = new List<Vector3Int>();
        var tiles = new List<TileBase>();

        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if ((long)x * x + (long)y * y > wr2) continue;
                if (grid[x + r, y + r])
                {
                    positions.Add(new Vector3Int(center.x + x, center.y + y, center.z));
                    tiles.Add(groundTile);
                }
            }
        }

        tilemap.SetTiles(positions.ToArray(), tiles.ToArray());
    }
}

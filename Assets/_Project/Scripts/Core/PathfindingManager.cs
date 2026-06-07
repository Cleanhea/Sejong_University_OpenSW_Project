using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance { get; private set; }

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private int maxNodes = 1500;

    // 8방향 오프셋 (상하좌우 + 대각선)
    private static readonly Vector3Int[] Directions =
    {
        new( 1,  0, 0), new(-1,  0, 0),
        new( 0,  1, 0), new( 0, -1, 0),
        new( 1,  1, 0), new(-1,  1, 0),
        new( 1, -1, 0), new(-1, -1, 0),
    };

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// startWorld에서 endWorld까지의 경로를 World 좌표 리스트로 반환.
    /// 경로를 찾지 못하면 null 반환.
    /// </summary>
    public List<Vector2> FindPath(Vector2 startWorld, Vector2 endWorld)
    {
        Vector3Int startCell = tilemap.WorldToCell(startWorld);
        Vector3Int endCell   = tilemap.WorldToCell(endWorld);

        if (!tilemap.HasTile(endCell)) return null;
        if (startCell == endCell)      return new List<Vector2> { endWorld };

        var openSet  = new MinHeap();

        var closed   = new HashSet<Vector3Int>();
        var nodeMap  = new Dictionary<Vector3Int, Node>();

        var startNode = new Node(startCell, 0f, Heuristic(startCell, endCell), null);
        openSet.Push(startNode);
        nodeMap[startCell] = startNode;

        int processed = 0;
        while (openSet.Count > 0 && processed < maxNodes)
        {
            Node current = openSet.Pop();
            if (current.cell == endCell)
                return BuildPath(current, endWorld, tilemap);

            closed.Add(current.cell);
            processed++;

            foreach (var dir in Directions)
            {
                Vector3Int neighborCell = current.cell + dir;

                if (closed.Contains(neighborCell))       continue;
                if (!tilemap.HasTile(neighborCell))      continue;

                // 대각 이동 시 양쪽 측면이 막혀 있으면 코너 끼임 방지
                if (dir.x != 0 && dir.y != 0)
                {
                    if (!tilemap.HasTile(current.cell + new Vector3Int(dir.x, 0, 0))) continue;
                    if (!tilemap.HasTile(current.cell + new Vector3Int(0, dir.y, 0))) continue;
                }

                float moveCost = (dir.x != 0 && dir.y != 0) ? 1.414f : 1f;
                float newG = current.gCost + moveCost;

                if (nodeMap.TryGetValue(neighborCell, out Node existing))
                {
                    if (newG < existing.gCost)
                    {
                        existing.gCost  = newG;
                        existing.parent = current;
                        openSet.UpdatePriority(existing);
                    }
                }
                else
                {
                    var node = new Node(neighborCell, newG, Heuristic(neighborCell, endCell), current);
                    openSet.Push(node);
                    nodeMap[neighborCell] = node;
                }
            }
        }

        return null;
    }

    static List<Vector2> BuildPath(Node endNode, Vector2 endWorld, Tilemap tilemap)
    {
        var path = new List<Vector2>();
        Node current = endNode;
        while (current != null)
        {
            path.Add(tilemap.GetCellCenterWorld(current.cell));
            current = current.parent;
        }
        path.Reverse();
        path[^1] = endWorld;
        return path;
    }

    // Chebyshev distance — 8방향 이동에 최적화된 휴리스틱
    static float Heuristic(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return Mathf.Max(dx, dy);
    }

    // ──────────────────────────────────────────────
    // Node
    // ──────────────────────────────────────────────
    class Node
    {
        public Vector3Int cell;
        public float      gCost, hCost;
        public float      fCost => gCost + hCost;
        public Node       parent;
        public int        heapIndex;

        public Node(Vector3Int cell, float g, float h, Node parent)
        {
            this.cell = cell; gCost = g; hCost = h; this.parent = parent;
        }
    }

    // ──────────────────────────────────────────────
    // MinHeap (binary heap, fCost 기준 최솟값 우선)
    // ──────────────────────────────────────────────
    class MinHeap
    {
        readonly List<Node> _items = new();
        public int Count => _items.Count;

        public void Push(Node node)
        {
            node.heapIndex = _items.Count;
            _items.Add(node);
            BubbleUp(node.heapIndex);
        }

        public Node Pop()
        {
            Node top  = _items[0];
            int  last = _items.Count - 1;
            _items[0] = _items[last];
            _items[0].heapIndex = 0;
            _items.RemoveAt(last);
            if (_items.Count > 0) BubbleDown(0);
            return top;
        }

        public void UpdatePriority(Node node) => BubbleUp(node.heapIndex);

        void BubbleUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (_items[parent].fCost <= _items[i].fCost) break;
                Swap(i, parent);
                i = parent;
            }
        }

        void BubbleDown(int i)
        {
            int count = _items.Count;
            while (true)
            {
                int left = i * 2 + 1, right = i * 2 + 2, smallest = i;
                if (left  < count && _items[left].fCost  < _items[smallest].fCost) smallest = left;
                if (right < count && _items[right].fCost < _items[smallest].fCost) smallest = right;
                if (smallest == i) break;
                Swap(i, smallest);
                i = smallest;
            }
        }

        void Swap(int a, int b)
        {
            (_items[a], _items[b]) = (_items[b], _items[a]);
            _items[a].heapIndex = a;
            _items[b].heapIndex = b;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public int cost = int.MaxValue;
    public Vector2 direction;

    public readonly Vector2 position;
    public Grid(Vector2 _position) => position = _position;
}

public class FlowFieldManager : MonoBehaviour
{
    [SerializeField, Header("맵 사이즈")] private Vector2 size;
    public static FlowFieldManager Instance { get; private set; }
    public Dictionary<Vector2, Grid> grid { get; private set; } = new();

    private Queue<Grid> queue = new();
    private HashSet<Grid> hash = new();

    private Vector2[] checkNodePos =
    {
        new Vector2(0, -1),
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),

        new Vector2(-1, -1),
        new Vector2(-1, 1),
        new Vector2(1, 1),
        new Vector2(1, -1),
    };

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        size = new Vector2((int)size.x, (int)size.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, size);
    }
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SetGrid();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            DestroyImmediate(Instance);
        }
    }

    private void SetGrid()
    {
        var mapLayer = LayerMask.GetMask("Map");
        var originPos = size * -0.5f;

        originPos.x += 0.5f;
        originPos.y += 0.5f;

        var spawnPos = originPos;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var spanwPos = new Vector2(spawnPos.x, spawnPos.y);

                if (!Physics2D.Raycast(spanwPos, Vector2.up, 0.3f, mapLayer))
                {
                    grid.Add(spanwPos, new Grid(spanwPos));
                }

                spawnPos.y++;
            }

            spawnPos.x++;
            spawnPos.y = originPos.y;
        }
    }

    public void SetTarget(Vector2 _targetPos)
    {
        _targetPos.x = Mathf.Floor(_targetPos.x) + 0.5f;
        _targetPos.y = Mathf.Floor(_targetPos.y) + 0.5f;
        if (!grid.ContainsKey(_targetPos)) return;

        hash.Clear();
        queue.Clear();

        grid[_targetPos].cost = 0;
        grid[_targetPos].direction = Vector2.zero;

        hash.Add(grid[_targetPos]);
        queue.Enqueue(grid[_targetPos]);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            for (int i = 0; i < checkNodePos.Length; i++)
            {
                var nextNode = node.position + checkNodePos[i];
                if (!grid.ContainsKey(nextNode)) continue;

                else if (!hash.Contains(grid[nextNode]))
                {
                    grid[nextNode].cost = node.cost + 1;
                    queue.Enqueue(grid[nextNode]);
                    hash.Add(grid[nextNode]);
                }

                if (grid[nextNode].cost < node.cost)
                {
                    node.direction = (grid[nextNode].position - node.position).normalized;
                }
            }
        }
    }
}
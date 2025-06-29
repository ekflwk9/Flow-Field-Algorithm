using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TestGrid
{
    public int cost = int.MaxValue;

    public Vector2 direction;
    public Vector2 position;
    //public Grid(Vector2 _gridPos) => position = _gridPos;

    //test
    public TMP_Text text;
    public TestGrid(Vector2 _gridPos, TMP_Text _text)
    {
        position = _gridPos;
        text = _text;
    }
}

public class TestFlowFieldManager : MonoBehaviour
{
    [SerializeField, Header("맵 사이즈")] private Vector2 size;
    [SerializeField, Header("모든 그리드 정보")] private TestGrid[] gridInfo; //시각화를 위한 변수

    public static TestFlowFieldManager Instance { get; private set; }
    public Dictionary<Vector2, TestGrid> grid { get; private set; } = new();

    private Queue<TestGrid> queue = new();
    private HashSet<TestGrid> hash = new();

    private Vector2[] checkNodePos =
    {
        new Vector2(1, 0),
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(0, -1),
        new Vector2(-1, -1),
        new Vector2(-1, 1),
        new Vector2(1, -1),
        new Vector2(1, 1),
    };

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
        var tempList = new List<TestGrid>();//
        var arrow = Resources.Load<GameObject>("Grid");//
        var parent = new GameObject("AllGrid");//

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
                    //==========================
                    var spawnArrow = Instantiate(arrow);
                    spawnArrow.transform.position = spanwPos;
                    spawnArrow.transform.SetParent(parent.transform);
                    spawnArrow.name = $"{arrow.name} {tempList.Count}";
                    var text = spawnArrow.GetComponent<TMP_Text>();
                    text.text = "X";
                    var gridNode = new TestGrid(spanwPos, text);
                    tempList.Add(gridNode);
                    grid.Add(spanwPos, gridNode);
                    //========================test

                    //grid.Add(spanwPos, new Grid(spanwPos));
                }

                spawnPos.y++;
            }

            spawnPos.x++;
            spawnPos.y = originPos.y;
        }

        gridInfo = tempList.ToArray();//
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        size = new Vector2((int)size.x, (int)size.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, size);
    }
#endif

    public void UpdateGrid(Vector2 _targetPos)
    {
        _targetPos.x = Mathf.Floor(_targetPos.x) + 0.5f;
        _targetPos.y = Mathf.Floor(_targetPos.y) + 0.5f;
        if (!grid.ContainsKey(_targetPos)) return;

        hash.Clear();
        queue.Clear();

        grid[_targetPos].cost = 0;
        hash.Add(grid[_targetPos]);
        queue.Enqueue(grid[_targetPos]);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            for (int i = 0; i < checkNodePos.Length; i++)
            {
                var nextNode = node.position + checkNodePos[i];

                if (!grid.ContainsKey(nextNode)) continue;
                else if (hash.Contains(grid[nextNode])) continue;

                grid[nextNode].cost = node.cost + 1;
                queue.Enqueue(grid[nextNode]);
                hash.Add(grid[nextNode]);

                grid[nextNode].text.text = grid[nextNode].cost.ToString(); //test;
            }
        }
    }
}
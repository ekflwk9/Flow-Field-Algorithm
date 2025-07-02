using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class ViewGrid
{
    public int cost = int.MaxValue;

    public Vector2 direction;
    public Vector2 position;
    //public Grid(Vector2 _gridPos) => position = _gridPos;

    //test
    public SpriteRenderer render;
    public TMP_Text text;
    public ViewGrid(Vector2 _gridPos, SpriteRenderer _render, TMP_Text _text)
    {
        position = _gridPos;
        render = _render;
        text = _text;
    }
}

public class ViewFlowFieldManager : MonoBehaviour
{
    [SerializeField, Header("맵 사이즈")] private Vector2 size;
    [SerializeField, Header("모든 그리드 정보")] private ViewGrid[] gridInfo; //시각화를 위한 변수

    public static ViewFlowFieldManager Instance { get; private set; }

    private Dictionary<Vector2, ViewGrid> grid = new();
    private Queue<ViewGrid> searchGrid = new();
    private HashSet<ViewGrid> setCost = new();

    private Vector2[] checkPos =
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
        var tempList = new List<ViewGrid>();//
        var arrow = Resources.Load<GameObject>("Grid");//
        var parent = new GameObject("AllGrid");//
        var sprite = Resources.Load<Sprite>("Direction");//

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
                    var render = spawnArrow.GetComponent<SpriteRenderer>();
                    render.sprite = sprite;
                    var text = render.transform.GetChild(0).GetComponent<TMP_Text>();
                    var gridNode = new ViewGrid(spanwPos, render, text);
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

    public void SetTarget(Vector2 _targetPos)
    {
        _targetPos.x = Mathf.Floor(_targetPos.x) + 0.5f;
        _targetPos.y = Mathf.Floor(_targetPos.y) + 0.5f;
        if (!grid.ContainsKey(_targetPos)) return;

        setCost.Clear();
        searchGrid.Clear();

        grid[_targetPos].cost = 0;
        grid[_targetPos].direction = Vector2.zero;

        setCost.Add(grid[_targetPos]);
        searchGrid.Enqueue(grid[_targetPos]);

        while (searchGrid.Count > 0)
        {
            var thisGrid = searchGrid.Dequeue();

            for (int i = 0; i < checkPos.Length; i++)
            {
                var nextGrid = thisGrid.position + checkPos[i];
                if (!grid.ContainsKey(nextGrid)) continue;

                else if (!setCost.Contains(grid[nextGrid]))
                {
                    if (checkPos[i].x == 0 || checkPos[i].y == 0)
                    {
                        setCost.Add(grid[nextGrid]);
                        searchGrid.Enqueue(grid[nextGrid]);
                        grid[nextGrid].cost = thisGrid.cost + 1;
                    }

                    else
                    {
                        grid[nextGrid].cost = thisGrid.cost + 2;
                    }
                }

                if (grid[nextGrid].cost < thisGrid.cost)
                {
                    thisGrid.direction = (grid[nextGrid].position - thisGrid.position).normalized;
                }
            }
        }

        //test;
        SetView();
    }

    public ViewGrid GetGrid(Vector2 _position)
    {
        _position.x = Mathf.Floor(_position.x) + 0.5f;
        _position.y = Mathf.Floor(_position.y) + 0.5f;

        if (!grid.ContainsKey(_position)) return null;
        return grid[_position];
    }

    private void SetView()
    {
        //test전용 메서드
        for (int i = 0; i < gridInfo.Length; i++)
        {
            gridInfo[i].direction = grid[gridInfo[i].position].direction;

            if (gridInfo[i].direction == Vector2.zero) gridInfo[i].render.gameObject.SetActive(false);
            else if (!gridInfo[i].render.gameObject.activeSelf) gridInfo[i].render.gameObject.SetActive(true);

            var direction = gridInfo[i].direction;

            var atan = math.atan2(direction.y, direction.x);
            var deg = math.degrees(atan);

            gridInfo[i].render.transform.rotation = Quaternion.Euler(0, 0, deg);
            gridInfo[i].text.transform.rotation = Quaternion.identity;
            gridInfo[i].text.text = $"{gridInfo[i].cost}";
        }
    }
}
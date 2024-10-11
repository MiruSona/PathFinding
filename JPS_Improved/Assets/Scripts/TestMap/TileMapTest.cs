using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(TileMapTest))]
public class TileMapTest_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("타일 생성"))
        {
            var tileMap = (TileMapTest)target;

            tileMap.ClearTiles();
            tileMap.CreateTiles();
        }
    }
}

public class TileMapTest : MonoBehaviour
{
    private GridLayoutGroup _gridLayout;
    private TileTest _tileTest;
    private List<TileTest> _tiles = new List<TileTest>();

    public int Row = 10;
    public int Col = 10;
    public float TileWidth = 10.0f;
    public float TileHeight = 10.0f;

    private bool _isInit = false;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (_isInit) return;

        _gridLayout = transform.GetComponent<GridLayoutGroup>();

        _tileTest = transform.GetChild(0).GetComponent<TileTest>();
        _tileTest.gameObject.SetActive(false);

        CreateTiles();

        _isInit = true;
    }

    public void CreateTiles()
    {
        _gridLayout.cellSize = new Vector2(TileWidth, TileHeight);
        _gridLayout.constraintCount = Col;

        for (int row = 0; row < Row; row++)
        {
            for (int col = 0; col < Col; col++)
            {
                var newTile = Instantiate(_tileTest);
                newTile.Init();
                newTile.transform.SetParent(transform);
                newTile.gameObject.SetActive(true);

                _tiles.Add(newTile);
            }
        }
    }

    public void ClearTiles()
    {
        for(int i = _tiles.Count - 1; i >= 0; i--)
        {
            Destroy(_tiles[i].gameObject);
        }
        _tiles.Clear();
    }
}

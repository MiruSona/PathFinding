#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PathFindingTest
{
    [CustomEditor(typeof(TileMapEditor))]
    public class TileMapEditor_Inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var targetComp = (TileMapEditor)this.target;

            if (GUILayout.Button("Create Tiles"))
            {
                targetComp.CreateTileMap();
                EditorUtility.SetDirty(target);
            }
        }
    }

    public class TileMapEditor : MonoBehaviour
    {
        [SerializeField]
        private Vector2 MapSize = Vector2.one;
        [SerializeField]
        private float TileSize = 1f;
        [SerializeField]
        private float SearchDelay = 0.1f;

        private TileMap _tileMap;
        private PathFinding _pathFinding;

        private Camera _cam;
        [SerializeField]
        private float CamMoveSpeed = 1.0f;
        [SerializeField]
        private float CamMoveMax = 80f;
        [SerializeField]
        private float CamMoveMin = 10f;

        private Coroutine _currentPathFinding = null;

        public void OnEnable()
        {
            CreateTileMap();
            _cam = Camera.main;
        }

        public void CreateTileMap()
        {
            _tileMap?.Dispose();
            _tileMap = null;
            _tileMap = new TileMap(MapSize, transform.position, TileSize);

            _pathFinding = null;
            _pathFinding = new PathFinding(_tileMap);
        }

        private void Update()
        {
            if (_tileMap == null) return;

            _pathFinding.WaitTime = SearchDelay;

            if (Input.GetMouseButtonDown(0))
            {
                var isHit = GetMouseWorldPos(out var mouseWorldPos);
                if (isHit)
                {
                    var occupiedTiles = _tileMap.GetOccipiedTiles(OccupyFlag.Ally);
                    if (occupiedTiles != null && occupiedTiles.Count > 0)
                    {
                        foreach (var occupiedTile in occupiedTiles)
                        {
                            occupiedTile.ClearOccupy();
                        }
                    }

                    var tile = _tileMap.GetTileByWorldPosition(mouseWorldPos);
                    tile.SetOccupy(OccupyFlag.Ally);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                var isHit = GetMouseWorldPos(out var mouseWorldPos);
                if (isHit)
                {
                    var occupiedTiles = _tileMap.GetOccipiedTiles(OccupyFlag.Enemy);
                    if (occupiedTiles != null && occupiedTiles.Count > 0)
                    {
                        foreach (var occupiedTile in occupiedTiles)
                        {
                            occupiedTile.ClearOccupy();
                        }
                    }

                    var tile = _tileMap.GetTileByWorldPosition(mouseWorldPos);
                    tile.SetOccupy(OccupyFlag.Enemy);
                }
            }

            if (Input.GetMouseButton(2))
            {
                var isHit = GetMouseWorldPos(out var mouseWorldPos);
                if (isHit)
                {
                    var tile = _tileMap.GetTileByWorldPosition(mouseWorldPos);
                    if (tile.IsOccupied(OccupyFlag.Obstacle) == false)
                    {
                        tile.ClearOccupy();
                        tile.SetOccupy(OccupyFlag.Obstacle);
                    }                    
                }
            }

            var wheelMove = Input.GetAxis("Mouse ScrollWheel");
            if(wheelMove > 0)
            {
                var pos = _cam.transform.position;
                if (pos.y < CamMoveMin)
                    pos.y = CamMoveMin;
                else
                    pos.y -= CamMoveSpeed;

                _cam.transform.position = pos;
            }
            else if (wheelMove < 0)
            {
                var pos = _cam.transform.position;
                if (pos.y > CamMoveMax)
                    pos.y = CamMoveMax;
                else
                    pos.y += CamMoveSpeed;

                _cam.transform.position = pos;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(_currentPathFinding != null) StopCoroutine(_currentPathFinding);
                _tileMap.ClearAllTileVisited();
                _currentPathFinding = StartCoroutine(_pathFinding.ProcessPathFinding());
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (_currentPathFinding != null) StopCoroutine(_currentPathFinding);
                _tileMap.ClearAllTileOccupy();
            }
        }

        private bool GetMouseWorldPos(out Vector3 mouseWorldPos)
        {
            mouseWorldPos = Vector3.zero;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var isHit = Physics.Raycast(ray, out var hitResult);

            if (isHit == false)
                return false;
            else
            {
                mouseWorldPos = hitResult.point;
                return true;
            }
        }

        private void OnDrawGizmos()
        {
            var mapSize = new Vector3(MapSize.x, 0f, MapSize.y);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, mapSize);

            if (_tileMap == null) return;
            foreach (var tile in _tileMap.GetTiles())
            {
                Color outlineColor = Color.black;

                Gizmos.color = tile.TileColor;
                Gizmos.DrawCube(tile.WorldPosition, tile.WorldSize);
                Gizmos.color = outlineColor;
                Gizmos.DrawWireCube(tile.WorldPosition, tile.WorldSize);
            }
        }
    }
}
#endif
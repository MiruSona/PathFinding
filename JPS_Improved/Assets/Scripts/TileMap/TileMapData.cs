using FastPathFinder.Data;
using System.Collections.Generic;
using UnityEngine;

namespace FastPathFinder
{
    //좌하단 -> 우상단으로 커지는 맵
    public class TileMapData
    {
        public int Index { get; private set; } = 0;

        //Tile
        private int _row;
        private int _column;
        private int _totalTileCount;
        private List<TileData> _tileDatas = new List<TileData>();
        private List<int> _tileDataMapIds = new List<int>();    //MapId -> TileIndex 변환용

        //Position 3D
        private Vector3 _center3D;
        private Vector3 _leftBottom3D;
        private Vector3 _rightTop3D;
        private Vector3 _leftTop3D;
        private Vector3 _rightBottom3D;
        private Vector3 _firstTileWorldPosition3D;

        //Position 2D
        private Vector2 _center2D;
        private Vector2 _leftBottom2D;
        private Vector2 _rightTop2D;
        private Vector2 _leftTop2D;
        private Vector2 _rightBottom2D;
        private Vector2 _firstTileWorldPosition2D;

        //Size
        private Vector2 _mapSize2D;
        private Vector3 _mapSize3D;
        private Vector2 _tileSize2D;
        private Vector3 _tileSize3D;
        private float _height;

        //PathFinding
        private BitFlagMap _bitTileMap;

        #region Public
        //Tile
        public int Row => _row;
        public int Column => _column;
        public int TotalTileCount => _totalTileCount;
        public IReadOnlyList<TileData> TileDatas;

        //Position 3D
        public Vector3 Center3D => _center3D;
        public Vector3 LeftBottom3D => _leftBottom3D;
        public Vector3 RightTop3D => _rightTop3D;
        public Vector3 LeftTop3D => _leftTop3D;
        public Vector3 RightBottom3D => _rightBottom3D;

        //Position 2D
        public Vector2 Center2D => _center2D;
        public Vector2 LeftBottom2D => _leftBottom2D;
        public Vector2 RightTop2D => _rightTop2D;
        public Vector2 LeftTop2D => _leftTop2D;
        public Vector2 RightBottom2D => _rightBottom2D;

        //Size
        public Vector2 MapSize2D => _mapSize2D;
        public Vector3 MapSize3D => _mapSize3D;

        public Vector2 TileSize2D => _tileSize2D;
        public Vector3 TileSize3D => _tileSize3D;
        public float Height => _height;

        //PathFinding
        public BitFlagMap BitTileMap => _bitTileMap;
        #endregion

        #region Init & Dispose
        //Row/Colum 으로 초기화
        public TileMapData(int row, int column, float height, Vector3 center, Vector3 leftBottom, Vector3 rightTop)
        {
            //Row / Column / Total
            _row = row;
            _column = column;
            _totalTileCount = row * column;
            _height = height;

            //위치값 & 맵 크기 갱신            
            UpdatePositionAndMapSize(center, leftBottom, rightTop);

            //Row, Column -> 설정 시 TileSize도 계산
            SetRowColumn(row, column, height);

            TileDatas = _tileDatas;
        }

        //TileSize로 초기화
        public TileMapData(Vector2 tileSize, float height, Vector3 center, Vector3 leftBottom, Vector3 rightTop)
        {
            _height = height;

            //위치값 갱신
            UpdatePositionAndMapSize(center, leftBottom, rightTop);

            //타일 크기 -> 설정 시 Row, Column도 계산
            SetTileSize(tileSize, height);

            TileDatas = _tileDatas;
        }

        public void Dispose()
        {
            if (_tileDatas.Count > 0)
            {
                foreach (var tile in _tileDatas)
                    tile.Dispose();
                _tileDatas.Clear();
                _tileDataMapIds.Clear();
            }

            _bitTileMap?.Dispose();
            _bitTileMap = null;
        }
        #endregion

        #region Tile Data
        private void CreateTileDatas()
        {
            if (_tileDatas.Count > 0)
            {
                foreach (var tile in _tileDatas)
                    tile.Dispose();
                _tileDatas.Clear();
                _tileDataMapIds.Clear();
            }

            //각 타일 Index/MapId/월드 위치/타일 위치 지정
            int currentRow = 0;
            var tileSizeHalf = _tileSize2D * 0.5f;
            Vector3 worldPosition =
                new Vector3(_leftBottom2D.x + tileSizeHalf.x, _center3D.y, _leftBottom2D.y + tileSizeHalf.y);
            Vector2Int tilePosition = Vector2Int.zero;
            for (int index = 0; index < _totalTileCount; ++index)
            {
                //Tile좌표를 단일값으로 저장 -> x + (y * column)
                int mapId = tilePosition.x + tilePosition.y * _column;
                TileData newTile = new TileData(index, mapId, _tileSize3D, worldPosition, tilePosition);

                _tileDatas.Add(newTile);
                _tileDataMapIds.Add(mapId);

                //첫 타일의 위치값 저장
                if (index == 0)
                {
                    _firstTileWorldPosition3D = worldPosition;
                    _firstTileWorldPosition2D = new Vector2(worldPosition.x, worldPosition.z);
                }

                //위치 계산
                int newRow = (index + 1) / _column;
                if (currentRow != newRow)
                {
                    worldPosition.x = _leftBottom2D.x + tileSizeHalf.x;
                    worldPosition.z += _tileSize2D.y;

                    tilePosition.x = 0;
                    ++tilePosition.y;

                    currentRow = newRow;
                }
                else
                {
                    worldPosition.x += _tileSize2D.x;
                    ++tilePosition.x;
                }
            }

            //BitFlagMap
            if (_bitTileMap != null)
            {
                _bitTileMap.Dispose();
                _bitTileMap = null;
            }
            _bitTileMap = new BitFlagMap(_column, _row);
        }

        private TileData GetTileByWorldPosition(Vector3 worldPosition)
        {
            var tilePosition = WorldPositionToTilePosition(worldPosition);
            int mapId = tilePosition.x + tilePosition.y * _column;
            if (_tileDataMapIds.Contains(mapId) == false) return null;

            var findTile = _tileDatas[_tileDataMapIds.IndexOf(mapId)];
            return findTile;

        }

        public void SetTileOccupy(Vector3 worldPosition, Type_Occupy occupyType, string uid = "")
        {
            var tile = GetTileByWorldPosition(worldPosition);
            if (tile == null) return;
            tile.SetOccupy(occupyType, uid);
        }
        #endregion

        #region Position
        public void UpdatePositionAndMapSize(Vector3 center, Vector3 leftBottom, Vector3 rightTop)
        {
            //위치값 - 3D
            _center3D = center;
            _leftBottom3D = new Vector3(leftBottom.x, center.y, leftBottom.z);
            _rightTop3D = new Vector3(rightTop.x, center.y, rightTop.z);
            _leftTop3D = new Vector3(_leftBottom3D.x, center.y, _rightTop3D.z);
            _rightBottom3D = new Vector3(_rightTop3D.x, center.y, _leftBottom3D.z);

            //위치값 2D
            _center2D = new Vector2(_center3D.x, _center3D.z);
            _leftBottom2D = new Vector2(_leftBottom3D.x, _leftBottom3D.z);
            _rightTop2D = new Vector2(_rightTop3D.x, _rightTop3D.z);
            _leftTop2D = new Vector2(_leftTop3D.x, _leftTop3D.z);
            _rightBottom2D = new Vector2(_rightBottom3D.x, _rightBottom3D.z);

            //맵 크기
            _mapSize2D.x = _rightTop2D.x - _leftBottom2D.x;
            _mapSize2D.y = _rightTop2D.y - _leftBottom2D.y;
            _mapSize3D = new Vector3(_mapSize2D.x, _height, _mapSize2D.y);
        }

        public Vector2Int WorldPositionToTilePosition(Vector3 worldPosition)
        {
            float calcTileX = (worldPosition.x - _firstTileWorldPosition2D.x) % _tileSize2D.x;
            float calcTileY = (worldPosition.z - _firstTileWorldPosition2D.y) % _tileSize2D.y;

            return new Vector2Int(Mathf.FloorToInt(calcTileX), Mathf.FloorToInt(calcTileY));
        }

        public bool IsWorldPositionInTileMap(Vector3 worldPosition)
        {
            var tilePosition = WorldPositionToTilePosition(worldPosition);
            int mapId = tilePosition.x + tilePosition.y * _column;
            return _tileDataMapIds.Contains(mapId);
        }
        #endregion

        #region Size & Height
        public void SetRowColumn(int row, int column, float height)
        {
            _row = row;
            _column = column;
            _totalTileCount = row * column;

            //타일 크기
            _mapSize3D.y = height;
            _tileSize2D.x = _mapSize2D.x / (float)column;
            _tileSize2D.y = _mapSize2D.y / (float)row;
            _tileSize3D = new Vector3(_tileSize2D.x, height, _tileSize2D.y);

            CreateTileDatas();
        }

        public void SetTileSize(Vector2 tileSize, float height)
        {
            //타일 크기
            _tileSize2D = tileSize;
            _tileSize3D = new Vector3(_tileSize2D.x, height, _tileSize2D.y);

            //Row / Column / Total
            _mapSize3D.y = height;
            _row = Mathf.FloorToInt(_mapSize2D.x / tileSize.x);
            _column = Mathf.FloorToInt(_mapSize2D.y / tileSize.y);
            _totalTileCount = _row * _column;

            CreateTileDatas();
        }
        #endregion

        #region BitTileMap
        public void SetBitTileMapFlag(Vector2Int tilePosition, bool flag)
        {
            _bitTileMap.SetFlag(tilePosition.x, tilePosition.y, flag);
        }

        public bool GetBitTileMapFlag(Vector2Int tilePosition)
        {
            return _bitTileMap.GetFlag(tilePosition.x, tilePosition.y);
        }
        #endregion
    }
}
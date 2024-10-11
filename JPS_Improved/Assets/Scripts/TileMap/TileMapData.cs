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
        public readonly int Row;
        public readonly int Column;
        public readonly int TotalTileCount;
        private List<TileData> _tileDatas = new List<TileData>();
        public IReadOnlyList<TileData> TileDatas;
        private List<int> _tileDataMapIds = new List<int>();    //MapId -> TileIndex 변환용

        //Position
        public readonly Vector2 Center;
        public readonly Vector2 LeftBottom;
        public readonly Vector2 RightTop;
        public readonly Vector2 LeftTop;
        public readonly Vector2 RightBottom;
        private readonly Vector2 _firstTileWorldPosition;

        //Size
        public readonly Vector2 MapSize;
        public readonly Vector2 TileSize;

        //PathFinding
        private BitFlagMap _bitTileMap;
        public BitFlagMap BitTileMap => _bitTileMap;

        #region Init & Dispose
        public TileMapData(int row, int column, Vector3 center, Vector3 leftBottom, Vector3 rightTop)
        {
            Row = row;
            Column = column;
            TotalTileCount = row * column;

            //위치값 - 평면이라 z값이 y값이됨
            Center = new Vector2(center.x, center.z);
            LeftBottom = new Vector2(leftBottom.x, leftBottom.z);
            RightTop = new Vector2(rightTop.x, rightTop.z);
            LeftTop = new Vector2(LeftBottom.x, RightTop.y);
            RightBottom = new Vector2(RightTop.x, LeftBottom.y);

            //맵 크기
            MapSize = Vector2.zero;
            MapSize.x = rightTop.x - leftBottom.x;
            MapSize.y = rightTop.y - leftBottom.y;

            //타일 크기
            TileSize = Vector2.zero;
            TileSize.x = MapSize.x / (float)column;
            TileSize.y = MapSize.y / (float)row;

            //각 타일 Index/MapId/월드 위치/타일 위치 지정
            var tileSizeHalf = TileSize * 0.5f;
            Vector2 worldPosition = LeftBottom + tileSizeHalf;
            Vector2Int tilePosition = Vector2Int.zero;
            for (int index = 0; index < TotalTileCount; ++index)
            {
                //Tile좌표를 단일값으로 저장 -> x + (y * column)
                int mapId = tilePosition.x + tilePosition.y * column;
                TileData newTile = new TileData(index, mapId, TileSize, worldPosition, tilePosition);

                _tileDatas.Add(newTile);
                _tileDataMapIds.Add(mapId);

                if (index == 0)
                    _firstTileWorldPosition = worldPosition;

                //위치 계산
                if (index != 0 && (index % column == 0))
                {
                    worldPosition.x = leftBottom.x + tileSizeHalf.x;
                    worldPosition.y += tileSizeHalf.y;

                    tilePosition.x = 0;
                    ++tilePosition.y;
                }
                else
                {
                    worldPosition.x += tileSizeHalf.x;

                    ++tilePosition.x;
                }
            }

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

        #region Tile
        private TileData GetTileByWorldPosition(Vector3 worldPosition)
        {
            var tilePosition = WorldPositionToTilePosition(worldPosition);
            int mapId = tilePosition.x + tilePosition.y * Column;
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
        public Vector2Int WorldPositionToTilePosition(Vector3 worldPosition)
        {
            float calcTileX = (worldPosition.x - _firstTileWorldPosition.x) % TileSize.x;
            float calcTileY = (worldPosition.z - _firstTileWorldPosition.y) % TileSize.y;

            return new Vector2Int(Mathf.FloorToInt(calcTileX), Mathf.FloorToInt(calcTileY));
        }

        public bool IsWorldPositionInTileMap(Vector3 worldPosition)
        {
            var tilePosition = WorldPositionToTilePosition(worldPosition);
            int mapId = tilePosition.x + tilePosition.y * Column;
            return _tileDataMapIds.Contains(mapId);
        }
        #endregion
    }
}
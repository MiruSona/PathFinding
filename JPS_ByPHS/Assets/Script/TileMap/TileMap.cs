#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFindingTest
{
    public class TileMap : IDisposable
    {
        public readonly Vector3 MapCenterPos;       //맵 중앙 위치
        public readonly Vector2 MapSize;            //맵 전체 크기
        public readonly Vector2 MapHalfSize;        //맵 전체 크기

        public readonly float TileSize;             //타일 크기
        public readonly Vector2Int TileNum;         //타일 갯수
        public readonly Vector2 TileSizePercent;    //타일 비율
        public readonly int TotalTileNum;           //총 타일 갯수

        private Tile[,] _tiles = null;

        public TileMap(Vector2 mapSize, Vector3 mapCenterPos, float tileSize)
        {
            MapSize = mapSize;
            MapHalfSize = mapSize * 0.5f;
            MapCenterPos = mapCenterPos;
            MapCenterPos.y = 0f;

            TileSize = tileSize;
            float sizePercentX = mapSize.x / tileSize;
            float sizePercentY = mapSize.y / tileSize;
            TileNum.x = Mathf.RoundToInt(sizePercentX);
            TileNum.y = Mathf.RoundToInt(sizePercentY);
            TileSizePercent = new Vector2(1.0f / sizePercentX, 1.0f / sizePercentY);
            TotalTileNum = TileNum.x * TileNum.y;

            _tiles = new Tile[TileNum.x, TileNum.y];

            var halfMapSize = new Vector3(MapSize.x, 0, MapSize.y) * 0.5f;
            var mapBottomLeftPos = MapCenterPos - halfMapSize;

            var tileRadius = tileSize * 0.5f;
            for (int x = 0; x < TileNum.x; x++)
            {
                for (int y = 0; y < TileNum.y; y++)
                {
                    var tileIndex = new TileIndex(x, y);
                    var tileRelativePos = new Vector3(x * tileSize + tileRadius, 0, y * tileSize + tileRadius);
                    Vector3 tileWorldPos = mapBottomLeftPos + tileRelativePos;
                    _tiles[x, y] = new Tile(tileRadius, tileIndex, tileWorldPos);
                }
            }
        }

        public Tile GetTileByWorldPosition(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x + MapHalfSize.x) / MapSize.x;
            float percentY = (worldPosition.z + MapHalfSize.y) / MapSize.y;

            int x = Mathf.FloorToInt(percentX / TileSizePercent.x);
            int y = Mathf.FloorToInt(percentY / TileSizePercent.y);

            if (x < 0) x = 0;
            if (x >= TileNum.x) x = TileNum.x - 1;

            if (y < 0) y = 0;
            if (y >= TileNum.y) y = TileNum.y - 1;

            return _tiles[x, y];
        }

        public Tile GetTileByIndex(int indexX, int indexY)
        {
            if (IsValidIndex(indexX, indexY) == false) return null;

            return _tiles[indexX, indexY];
        }

        public Tile GetTileByIndex(TileIndex tileIndex)
        {
            if (IsValidIndex(tileIndex) == false) return null;

            return _tiles[tileIndex.x, tileIndex.y];
        }

        public ref readonly Tile[,] GetTiles()
        {
            return ref _tiles;
        }

        public bool IsValidIndex(int indexX, int indexY)
        {
            if (indexX < 0 || indexX > TileNum.x - 1) return false;
            if (indexY < 0 || indexY > TileNum.y - 1) return false;
            return true;
        }

        public bool IsValidIndex(TileIndex tileIndex)
        {
            return IsValidIndex(tileIndex.x, tileIndex.y);
        }

        #region Tile Occupy
        public List<Tile> GetOccipiedTiles(uint occupy)
        {
            if (_tiles == null) return null;

            var tiles = new List<Tile>();
            foreach(var tile in _tiles)
            {
                if (tile.IsOccupied(occupy) == false) continue;
                tiles.Add(tile);
            }

            return tiles;
        }
        #endregion

        #region Tile Clear
        public void ClearAllTileOccupy()
        {
            if (_tiles == null) return;

            foreach (var tile in _tiles)
            {
                tile.ClearOccupy();
            }
        }

        public void ClearAllTileVisited()
        {
            if (_tiles == null) return;

            foreach (var tile in _tiles)
            {
                tile.ClearVisited();
            }
        }
        #endregion

        public void Dispose()
        {
            _tiles = null;
        }
    }
}
#endif
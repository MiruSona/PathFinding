#if UNITY_EDITOR
using System;
using UnityEngine;

namespace PathFindingTest
{
    public static class OccupyFlag
    {
        public const uint None = 0;              //0 = 0000 0000
        public const uint Ally = 1;              //1 = 0000 0001
        public const uint Enemy = 1 << 1;        //2 = 0000 0010
        public const uint Obstacle = 1 << 2;     //4 = 0000 0100
    }

    public static class DirectionFlag
    {
        //정방향
        public const uint Left = 0;             //0 = 0000 0000
        public const uint Right = 1;            //1 = 0000 0001
        public const uint Up = 1 << 1;          //2 = 0000 0010
        public const uint Down = 1 << 2;        //4 = 0000 0100

        //대각선
        public const uint UpLeft = 1 << 3;      //8 = 0000 1000
        public const uint UpRight = 1 << 4;     //16 = 0001 0000
        public const uint DownLeft = 1 << 5;    //32 = 0010 0000
        public const uint DownRight = 1 << 6;   //64 = 0100 0000
    }

    public struct TileIndex : IEquatable<TileIndex>
    {
        public readonly int x, y;
        
        public TileIndex(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(TileIndex other)
        {
            return this.x == other.x && this.y == other.y;
        }

        public static int Distance(TileIndex a, TileIndex b)
        {
            /*대각선의 경우 거리가 아래와 같음
             * (0,0) ~ (1,1) = 1
             * (0,0) ~ (0,1) = 1
             * (0,0) ~ (1,2) = 2
             * (0,0) ~ (2,2) = 2
             */

            int distRow = Mathf.Abs(b.x - a.x);
            int distCol = Mathf.Abs(b.y - a.y);

            return Mathf.Max(distRow, distCol);
        }

        public int Distance(TileIndex tileIndex)
        {
            return Distance(this, tileIndex);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }

    public class Tile
    {
        public readonly float Size;             //타일 지름
        public readonly float Radius;           //타일 반지름
        public readonly TileIndex Index;        //타일 인덱스

        public readonly Vector3 WorldSize;       //월드 크기(지름)
        public readonly Vector3 WorldPosition;   //월드 위치

        private BitFlag _occupyFlags;            //점유 표시(Bit Flag)

        //길찾기용
        public int gCost, hCost;
        public int fCost => gCost + hCost;
        public Tile ParentTile;
        public Color TileColor { get; private set; } = Color.white;

        public bool IsVisited = false;
        public bool IsBlocked => !_occupyFlags.IsZero();

        public Tile(float tileRadius, TileIndex tileIndex, Vector3 worldPosition)
        {
            Size = tileRadius * 2.0f;
            Radius = tileRadius;
            Index = tileIndex;

            WorldSize = new Vector3(Size, 0f, Size);
            WorldPosition = worldPosition;

            _occupyFlags = new BitFlag();
        }

        #region Occupy
        public bool IsOccupied(uint occupyFlag)
        {
            return _occupyFlags.IsOn(occupyFlag);
        }

        public bool HasSameOccupied(Tile other)
        {
            return (_occupyFlags.Flags & other._occupyFlags.Flags) != 0;
        }

        public void SetOccupy(uint occupyFlag)
        {
            if (_occupyFlags.IsOn(occupyFlag)) return;
            _occupyFlags.Set(occupyFlag);

            //타일 색
            switch (occupyFlag)
            {
                case OccupyFlag.Obstacle: TileColor = Color.gray; break;
                case OccupyFlag.Ally: TileColor = Color.blue; break;
                case OccupyFlag.Enemy: TileColor = Color.red; break;
                case OccupyFlag.None: TileColor = Color.white; break;
            }
        }

        public void RemoveOccupy(uint occupyFlag)
        {
            if (_occupyFlags.IsOn(occupyFlag) == false) return;
            _occupyFlags.Remove(occupyFlag);
            TileColor = Color.white;
        }

        public void ClearOccupy()
        {
            _occupyFlags.Clear();
            TileColor = Color.white;
            IsVisited = false;
        }
        #endregion

        public void ClearVisited()
        {
            //타일 색
            switch (_occupyFlags.Flags)
            {
                case OccupyFlag.Obstacle: TileColor = Color.gray; break;
                case OccupyFlag.Ally: TileColor = Color.blue; break;
                case OccupyFlag.Enemy: TileColor = Color.red; break;
                case OccupyFlag.None: TileColor = Color.white; break;
            }
            IsVisited = false;
        }

        #region ETC
        public void SetColor(Color color)
        {
            TileColor = color;
        }
        #endregion
    }
}
#endif
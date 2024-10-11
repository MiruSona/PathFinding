using UnityEngine;

namespace FastPathFinder
{
    public enum Type_Occupy
    {
        Empty = 0,
        Obstacle,
        Ally,
        Enemy,
    }

    public class TileData
    {
        //Index
        public readonly int Index;
        public readonly int MapId;  //Tile좌표를 단일값으로 저장 -> x + (y * column)

        //Data
        public readonly Vector2 Size;
        public readonly Vector2 WorldPosition;
        public readonly Vector2Int TilePostion;

        //Occupy
        public Type_Occupy Occupy { get; private set; } = Type_Occupy.Empty;
        public string OccupyUID { get; private set; }   //점령 중인 대상 UID
        public string OnPathUID { get; private set; }   //이 타일을 길로 사용하는 대상 UID

        #region Init & Dispose
        public TileData(int index, int mapId, Vector2 size, Vector2 worldPosition, Vector2Int tilePosition)
        {
            Index = index;
            MapId = mapId;
            Size = size;
            WorldPosition = worldPosition;
            TilePostion = tilePosition;
        }

        public void Dispose()
        {

        }
        #endregion

        #region Occupy
        public void SetOccupy(Type_Occupy occupy, string uid = "")
        {
            if (string.IsNullOrEmpty(uid))
            {
                if (occupy == Type_Occupy.Ally || occupy == Type_Occupy.Enemy)
                    occupy = Type_Occupy.Enemy;
                uid = string.Empty;
            }

            Occupy = occupy;
            OccupyUID = uid;
        }

        public void SetOnPathUID(string uid)
        {
            OnPathUID = uid;
        }
        #endregion
    }
}
using FastPathFinder.Data;
using UnityEngine;

namespace FastPathFinder
{
    public class TileViewer : MonoBehaviour
    {
        private TileData _data = null;
        public TileData Data => _data;
        private BitFlagMap _bitTileMap;
        private Color _gizmoColor = Color.white;

        public void Init(TileData tileData, BitFlagMap bitTileMap)
        {
            _data = tileData;
            _bitTileMap = bitTileMap;
            transform.position = _data.WorldPosition3D;
        }

        public void Dispose()
        {
            _data = null;
            _bitTileMap = null;
        }

        public void UpdateViewer(bool isShowBitTileMap)
        {
            if (isShowBitTileMap == false)
            {
                switch (_data.Occupy)
                {
                    case Type_Occupy.Empty: _gizmoColor = Color.white; break;
                    case Type_Occupy.Ally: _gizmoColor = Color.blue; break;
                    case Type_Occupy.Enemy: _gizmoColor = Color.red; break;
                    case Type_Occupy.Obstacle: _gizmoColor = Color.gray; break;
                    case Type_Occupy.Terrain: _gizmoColor = Color.black; break;
                }
            }
            else
            {
                bool flag = _bitTileMap.GetFlag(_data.TilePostion.x, _data.TilePostion.y);
                if (flag)
                    _gizmoColor = Color.red;
                else
                    _gizmoColor = Color.white;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            var size = _data.Size3D;
            Gizmos.DrawWireCube(transform.position, size);
        }
    }
}
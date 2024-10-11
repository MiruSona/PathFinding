using UnityEngine;

namespace FastPathFinder
{
    public class TileViewer : MonoBehaviour
    {
        private TileData _data = null;
        public TileData Data => _data;
        private Color _gizmoColor = Color.white;

        public void Init(TileData tileData)
        {
            _data = tileData;
            transform.position = _data.WorldPosition;
        }

        public void Dispose()
        {
            _data = null;
        }

        public void UpdateViewer()
        {
            switch (_data.Occupy)
            {
                case Type_Occupy.Empty: _gizmoColor = Color.white; break;
                case Type_Occupy.Obstacle: _gizmoColor = Color.gray; break;
                case Type_Occupy.Ally: _gizmoColor = Color.blue; break;
                case Type_Occupy.Enemy: _gizmoColor = Color.red; break;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            var size = new Vector3(_data.Size.x, 0f, _data.Size.y);
            Gizmos.DrawWireCube(transform.position, size);
        }
    }
}
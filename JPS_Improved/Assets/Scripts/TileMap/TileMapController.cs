using UnityEngine;

namespace FastPathFinder
{
    public class TileMapController : MonoBehaviour
    {
        [Header("Option")]
        [SerializeField] private bool UseTileSize = true;
        [SerializeField] private int RowNum = 1;
        [SerializeField] private int ColumnNum = 1;
        [SerializeField] private Vector2 TileSze = Vector2.one;


        [Header("Anchor")]
        [SerializeField] private GizmoAnchor LeftBottomAnchor;
        [SerializeField] private GizmoAnchor RightTopAnchor;

        [Header("Viewer")]
        [SerializeField] private TileMapViewer TileMapViewer;

        [Header("Gizmo")]
        [SerializeField] private Color GizmoColor = Color.green;

        //Data
        private TileMapData _tileMapData;

        private void Awake()
        {
            var centerPos = transform.position;
            var leftBottomPos = LeftBottomAnchor.GetWorldPosition();
            var rightTopPos = RightTopAnchor.GetWorldPosition();

            if (UseTileSize)
                _tileMapData = new TileMapData(TileSze, centerPos, leftBottomPos, rightTopPos);
            else
                _tileMapData = new TileMapData(RowNum, ColumnNum, centerPos, leftBottomPos, rightTopPos);

            TileMapViewer.Init(_tileMapData);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = GizmoColor;

            var leftBottomPos = LeftBottomAnchor.GetWorldPosition();
            var rightTopPos = RightTopAnchor.GetWorldPosition();
            var tileMapSize = Vector3.one;
            tileMapSize.x = rightTopPos.x - leftBottomPos.x;
            tileMapSize.z = rightTopPos.z - leftBottomPos.z;

            var centerPos = leftBottomPos + tileMapSize * 0.5f;
            Gizmos.DrawWireCube(centerPos, tileMapSize);
        }
    }
}
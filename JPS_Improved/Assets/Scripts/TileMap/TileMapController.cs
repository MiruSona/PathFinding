using UnityEngine;

namespace FastPathFinder
{
    public class TileMapController : MonoBehaviour
    {
        [Header("Option")]
        [SerializeField] private bool UseTileSize = true;
        [SerializeField] private bool IsShowBitTileMap = false;
        [SerializeField] private int RowNum = 1;
        [SerializeField] private int ColumnNum = 1;
        [SerializeField] private Vector2 TileSze = Vector2.one;
        [SerializeField] private float Height = 1f;

        [Header("Anchor")]
        [SerializeField] private GizmoAnchor LeftBottomAnchor;
        [SerializeField] private GizmoAnchor RightTopAnchor;


        [Header("Gizmo")]
        [SerializeField] private Color GizmoColor = Color.green;

        //Data
        private TileMapData _tileMapData;

        //Viewer
        private TileMapViewer _tileMapViewer;

        #region Init
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            var centerPos = transform.position;
            var leftBottomPos = LeftBottomAnchor.GetWorldPosition();
            var rightTopPos = RightTopAnchor.GetWorldPosition();

            if (UseTileSize)
                _tileMapData = new TileMapData(TileSze, Height, centerPos, leftBottomPos, rightTopPos);
            else
                _tileMapData = new TileMapData(RowNum, ColumnNum, Height, centerPos, leftBottomPos, rightTopPos);

            _tileMapViewer = new GameObject("TileMapViewer").AddComponent<TileMapViewer>();
            _tileMapViewer.transform.SetParent(null);
            _tileMapViewer.Init(_tileMapData);

            UpdateOverlapTerrain();
        }
        #endregion

        #region Update
        public void UpdateSize()
        {
            if (UseTileSize)
                _tileMapData.SetTileSize(TileSze, Height);
            else
                _tileMapData.SetRowColumn(RowNum, ColumnNum, Height);

            _tileMapViewer.CreateTileViewers(IsShowBitTileMap);
        }

        public void UpdateOverlapTerrain()
        {
            foreach (var tileData in _tileMapData.TileDatas)
            {
                var position = tileData.WorldPosition3D;
                var size = tileData.Size3D;

                bool isOverlaped = Physics.CheckBox(position, size * 0.5f);
                if (isOverlaped)
                    tileData.SetOccupy(Type_Occupy.Terrain);
                else
                    tileData.SetOccupy(Type_Occupy.Empty);

                _tileMapData.SetBitTileMapFlag(tileData.TilePostion, isOverlaped);
            }

            _tileMapViewer.UpdateViewer(IsShowBitTileMap);
        }
        #endregion

#if UNITY_EDITOR
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
#endif
    }
}
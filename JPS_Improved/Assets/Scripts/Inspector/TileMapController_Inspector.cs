#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FastPathFinder
{
    [CustomEditor(typeof(TileMapController))]
    public class TileMapController_Inspector : Editor
    {
        private TileMapController _tileMapConroller = null;

        private void OnEnable()
        {
            _tileMapConroller = (TileMapController)target;
        }

        private void OnDisable()
        {
            _tileMapConroller = null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("맵 갱신"))
            {
                _tileMapConroller.UpdateSize();
                _tileMapConroller.UpdateOverlapTerrain();
            }
        }
    }
}
#endif
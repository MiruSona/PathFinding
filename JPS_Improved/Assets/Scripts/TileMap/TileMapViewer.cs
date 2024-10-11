using System.Collections.Generic;
using UnityEngine;

namespace FastPathFinder
{
    public class TileMapViewer : MonoBehaviour
    {
        //Data
        private TileMapData _data = null;
        public TileMapData Data => _data;

        //Viewer
        private TileViewer _tileViewerPrefab;
        private List<TileViewer> _tileViewers = new List<TileViewer>();

        public void Init(TileMapData data)
        {
            _data = data;

            _tileViewerPrefab = Resources.Load<TileViewer>("Prefab/TileViewer");
            foreach (var tileData in data.TileDatas)
            {
                var newTileViewer = Instantiate(_tileViewerPrefab);
                newTileViewer.Init(tileData);
                newTileViewer.transform.SetParent(transform);
                _tileViewers.Add(newTileViewer);
            }
        }

        public void Dispose()
        {
            foreach (var tileViewer in _tileViewers)
                tileViewer.Dispose();
            _tileViewers.Clear();

            _tileViewerPrefab = null;
            _data = null;
        }

        public void UpdateViewer()
        {
            foreach (var tileViewer in _tileViewers)
            {
                tileViewer.UpdateViewer();
            }
        }
    }
}
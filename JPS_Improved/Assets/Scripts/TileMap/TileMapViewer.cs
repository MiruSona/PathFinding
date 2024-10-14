using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
        private ObjectPool<TileViewer> _tileViewerPool;

        public void Init(TileMapData data)
        {
            _data = data;

            _tileViewerPool = new ObjectPool<TileViewer>(
                OnCreateTileViewer, OnGetTileViewer, OnReleaseTileViewer, OnDestroyTileViewer);

            _tileViewerPrefab = Resources.Load<TileViewer>("Prefab/TileViewer");
            foreach (var tileData in data.TileDatas)
            {
                var newTileViewer = _tileViewerPool.Get();
                newTileViewer.Init(tileData, data.BitTileMap);
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

        #region Pooling
        //Pool에서 생성 시
        private TileViewer OnCreateTileViewer()
        {
            var newTileViewer = Instantiate(_tileViewerPrefab);
            newTileViewer.transform.SetParent(transform);
            return newTileViewer;
        }

        //Pool에서 받을 때
        private void OnGetTileViewer(TileViewer tileViewer)
        {
            tileViewer.gameObject.SetActive(true);
        }

        //Pool에 넣을때
        private void OnReleaseTileViewer(TileViewer tileViewer)
        {
            tileViewer.Dispose();
            tileViewer.gameObject.SetActive(false);
        }

        //아이템이 파괴되는 경우
        private void OnDestroyTileViewer(TileViewer tileViewer)
        {
            tileViewer.Dispose();
            Destroy(tileViewer.gameObject);
        }
        #endregion

        #region Viewer
        //타일맵 데이터 바인드 + 타일 뷰어 생성
        public void CreateTileViewers(bool isShowBitTileMap = false)
        {
            if (_tileViewers.Count > 0)
            {
                foreach (var tileViewer in _tileViewers)
                    _tileViewerPool.Release(tileViewer);
                _tileViewers.Clear();
            }

            foreach (var tileData in _data.TileDatas)
            {
                var newTileViewer = _tileViewerPool.Get();
                newTileViewer.Init(tileData, _data.BitTileMap);
                _tileViewers.Add(newTileViewer);
            }

            UpdateViewer(isShowBitTileMap);
        }

        public void UpdateViewer(bool isShowBitTileMap = false)
        {
            foreach (var tileViewer in _tileViewers)
            {
                tileViewer.UpdateViewer(isShowBitTileMap);
            }
        }
        #endregion
    }
}
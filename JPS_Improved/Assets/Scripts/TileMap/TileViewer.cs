using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastPathFinder
{
    public class TileViewer : MonoBehaviour
    {
        private TileData _data = null;
        public TileData Data => _data;
        public bool isDrawCube = true;

        public void Init(TileData tileData)
        {
            _data = tileData;
        }

        public void SetColor()
        {
            
        }

        private void OnDrawGizmos()
        {
            if (isDrawCube == false) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}
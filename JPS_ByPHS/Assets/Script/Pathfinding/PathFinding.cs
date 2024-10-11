#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFindingTest
{
    public class PathFinding
    {
        private readonly TileMap _tileMap;
        private Tile[,] _tiles;

        private Tile _startTile, _endTile;
        private List<Tile> _openList = new List<Tile>();

        private const int COST_STRAIGHT = 10;
        private const int COST_DIAGONAL = 14;

        public float WaitTime = 0.1f;

        private Color _notSearchColor = new Color(1.0f, 1.0f, 0.5f);
        private Color _diagonalColor = new Color(0.6f, 0.6f, 0f);

        public PathFinding(TileMap tileMap)
        {
            _tileMap = tileMap;
        }

        private void BeginPathFinding()
        {
            _tiles = _tileMap.GetTiles();
            _openList.Clear();

            _startTile = _tileMap.GetOccipiedTiles(OccupyFlag.Ally)[0];
            _endTile = _tileMap.GetOccipiedTiles(OccupyFlag.Enemy)[0];
            
            _startTile.gCost = 0;
            _startTile.hCost = CalcHeuristic(_startTile.Index, _endTile.Index);
            _openList.Add(_startTile);
        }

        public IEnumerator ProcessPathFinding()
        {
            BeginPathFinding();

            Tile currentTile = null;
            while (_openList.Count > 0)
            {
                currentTile = GetLowestFCost(_openList);
                _openList.Remove(currentTile);

                if (currentTile == _endTile)
                {
                    Debug.Log($"Find Path");
                    ShowPath(_endTile);
                    break;
                }

                if (currentTile.IsVisited) continue;

                CheckStraightRight(currentTile);
                if(WaitTime > 0f) yield return new WaitForSeconds(WaitTime);
                CheckStraightUp(currentTile);
                if (WaitTime > 0f) yield return new WaitForSeconds(WaitTime);
                CheckStraightLeft(currentTile);
                if (WaitTime > 0f) yield return new WaitForSeconds(WaitTime);
                CheckStraightDown(currentTile);
                if (WaitTime > 0f) yield return new WaitForSeconds(WaitTime);
                CheckDiagonalUpRight(currentTile);
                if (WaitTime > 0f) yield return new WaitForSeconds(WaitTime);
                CheckDiagonalUpLeft(currentTile);
                if (WaitTime > 0f) yield return new WaitForSeconds(WaitTime);
                CheckDiagonalDownLeft(currentTile);
                if (WaitTime > 0f) yield return new WaitForSeconds(WaitTime);
                CheckDiagonalDownRight(currentTile);
                if (WaitTime > 0f) yield return new WaitForSeconds(WaitTime);

                currentTile.IsVisited = true;
            }

            if (currentTile != _endTile)
            {
                Debug.Log($"No Path");
            }
        }

        #region 직선 방향 체크
        private bool CheckStraightRight(Tile startTile, bool isSearch = true)
        {
            bool isFindJumpingPoint = false;
            int currentX = startTile.Index.x + 1;
            int currentY = startTile.Index.y;

            var currentTile = GetValidIndexTile(currentX, currentY);
            if (currentTile == null) return isFindJumpingPoint;

            while (IsMovableTile(currentTile))
            {
                //도착 체크
                if (currentTile == _endTile)
                {
                    if (isSearch)
                        AddToOpenList(currentTile, startTile);

                    isFindJumpingPoint = true;
                    break;
                }

                //위가 막혔나 체크
                if(IsBlockedTile(currentX, currentY + 1))
                {
                    //오른쪽 위가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX + 1, currentY + 1) == false)
                    {
                        if (isSearch)
                        {
                            AddToOpenList(currentTile, startTile);
                            currentTile.SetColor(Color.cyan);
                        }
                        
                        isFindJumpingPoint = true;
                        break;
                    }
                }

                //아래가 막혔나 체크
                if (IsBlockedTile(currentX, currentY - 1))
                {
                    //오른쪽 아래가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX + 1, currentY - 1) == false)
                    {
                        if (isSearch)
                        {
                            AddToOpenList(currentTile, startTile);
                            currentTile.SetColor(Color.cyan);
                        }

                        isFindJumpingPoint = true;
                        break;
                    }
                }

                //전부 해당 안되면 방문했다 표시하고 넘어가기
                if(isSearch) currentTile.IsVisited = true;

                //방문 타일 표시
                if (isSearch) currentTile.SetColor(Color.yellow);
                else currentTile.SetColor(_notSearchColor);

                //이동
                ++currentX;
                currentTile = GetValidIndexTile(currentX, currentY);

                if (currentTile == null) break;               
            }

            return isFindJumpingPoint;
        }

        private bool CheckStraightLeft(Tile startTile, bool isSearch = true)
        {
            bool isFindJumpingPoint = false;
            int currentX = startTile.Index.x - 1;
            int currentY = startTile.Index.y;

            var currentTile = GetValidIndexTile(currentX, currentY);
            if (currentTile == null) return isFindJumpingPoint;

            while (IsMovableTile(currentTile))
            {
                //도착 체크
                if (currentTile == _endTile)
                {
                    if (isSearch)
                        AddToOpenList(currentTile, startTile);

                    isFindJumpingPoint = true;
                    break;
                }
                //위가 막혔나 체크
                if (IsBlockedTile(currentX, currentY + 1))
                {
                    //왼쪽 위가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX - 1, currentY + 1) == false)
                    {
                        if (isSearch)
                        {
                            AddToOpenList(currentTile, startTile);
                            currentTile.SetColor(Color.cyan);
                        }

                        isFindJumpingPoint = true;
                        break;
                    }
                }

                //아래가 막혔나 체크
                if (IsBlockedTile(currentX, currentY - 1))
                {
                    //왼쪽 아래가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX - 1, currentY - 1) == false)
                    {
                        if (isSearch)
                        {
                            AddToOpenList(currentTile, startTile);
                            currentTile.SetColor(Color.cyan);
                        }
                        
                        isFindJumpingPoint = true;
                        break;
                    }
                }

                //전부 해당 안되면 방문했다 표시하고 넘어가기
                if (isSearch) currentTile.IsVisited = true;

                //방문 타일 표시
                if (isSearch) currentTile.SetColor(Color.yellow);
                else currentTile.SetColor(_notSearchColor);

                //이동
                --currentX;
                currentTile = GetValidIndexTile(currentX, currentY);

                if (currentTile == null) break;                
            }

            return isFindJumpingPoint;
        }

        private bool CheckStraightUp(Tile startTile, bool isSearch = true)
        {
            bool isFindJumpingPoint = false;
            int currentX = startTile.Index.x;
            int currentY = startTile.Index.y + 1;

            var currentTile = GetValidIndexTile(currentX, currentY);
            if (currentTile == null) return isFindJumpingPoint;

            while (IsMovableTile(currentTile))
            {
                //도착 체크
                if (currentTile == _endTile)
                {
                    if (isSearch)
                        AddToOpenList(currentTile, startTile);

                    isFindJumpingPoint = true;
                    break;
                }

                //오른쪽이 막혔나 체크
                if(IsBlockedTile(currentX + 1, currentY))
                {
                    //오른쪽 위가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX + 1, currentY + 1) == false)
                    {
                        if (isSearch)
                        {
                            AddToOpenList(currentTile, startTile);
                            currentTile.SetColor(Color.cyan);
                        }
                        
                        isFindJumpingPoint = true;
                        break;
                    }
                }

                //왼쪽이 막혔나 체크
                if (IsBlockedTile(currentX - 1, currentY))
                {
                    //왼쪽 위가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX - 1, currentY + 1) == false)
                    {
                        if (isSearch)
                        {
                            AddToOpenList(currentTile, startTile);
                            currentTile.SetColor(Color.cyan);
                        }
                        
                        isFindJumpingPoint = true;
                        break;
                    }
                }

                //전부 해당 안되면 방문했다 표시하고 넘어가기
                if (isSearch) currentTile.IsVisited = true;

                //방문 타일 표시
                if (isSearch) currentTile.SetColor(Color.yellow);
                else currentTile.SetColor(_notSearchColor);

                //이동
                ++currentY;
                currentTile = GetValidIndexTile(currentX, currentY);

                if (currentTile == null) break;
            }

            return isFindJumpingPoint;
        }

        private bool CheckStraightDown(Tile startTile, bool isSearch = true)
        {
            bool isFind = false;
            int currentX = startTile.Index.x;
            int currentY = startTile.Index.y - 1;

            var currentTile = GetValidIndexTile(currentX, currentY);
            if (currentTile == null) return isFind;

            while (IsMovableTile(currentTile))
            {
                //도착 체크
                if (currentTile == _endTile)
                {
                    if (isSearch)
                        AddToOpenList(currentTile, startTile);

                    isFind = true;
                    break;
                }

                //오른쪽이 막혔나 체크
                if (IsBlockedTile(currentX + 1, currentY))
                {
                    //오른쪽 아래가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX + 1, currentY - 1) == false)
                    {
                        if (isSearch)
                        {
                            AddToOpenList(currentTile, startTile);
                            currentTile.SetColor(Color.cyan);
                        }

                        isFind = true;
                        break;
                    }
                }

                //왼쪽이 막혔나 체크
                if (IsBlockedTile(currentX - 1, currentY))
                {
                    //왼쪽 아래가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX - 1, currentY - 1) == false)
                    {
                        if (isSearch)
                        {
                            AddToOpenList(currentTile, startTile);
                            currentTile.SetColor(Color.cyan);
                        }

                        isFind = true;
                        break;
                    }
                }

                //전부 해당 안되면 방문했다 표시하고 넘어가기
                if (isSearch) currentTile.IsVisited = true;

                //방문 타일 표시
                if (isSearch) currentTile.SetColor(Color.yellow);
                else currentTile.SetColor(_notSearchColor);

                //이동
                --currentY;
                currentTile = GetValidIndexTile(currentX, currentY);

                if (currentTile == null) break;                
            }

            return isFind;
        }
        #endregion

        #region 대각선 방향 체크
        private void CheckDiagonalUpRight(Tile startTile)
        {
            int currentX = startTile.Index.x + 1;
            int currentY = startTile.Index.y + 1;

            var currentTile = GetValidIndexTile(currentX, currentY);
            if (currentTile == null) return;

            while (IsMovableTile(currentTile))
            {
                //도착 처리
                if (currentTile == _endTile)
                {
                    AddToOpenList(currentTile, startTile);
                    break;
                }

                //왼쪽이 막혔나 체크
                if (IsBlockedTile(currentX - 1, currentY))
                {
                    //왼쪽 위가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX - 1, currentY + 1) == false)
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //아래가 막혔나 체크
                if (IsBlockedTile(currentX, currentY - 1))
                {
                    //오른쪽 아래가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX + 1, currentY - 1) == false)
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //오른쪽 타일이 비었는지 체크 - 탐색용
                if(IsMovableTile(currentX + 1, currentY))
                {
                    //오른쪽 탐색 - 점프포인트 찾았으면 OpenList에 추가
                    if (CheckStraightRight(currentTile, false))
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //위쪽 타일이 비었는지 체크 - 탐색용
                if (IsMovableTile(currentX, currentY + 1))
                {
                    //위쪽 탐색 - 점프포인트 찾았으면 OpenList에 추가
                    if (CheckStraightUp(currentTile, false))
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //전부 해당 안되면 방문했다 표시하고 넘어가기
                currentTile.IsVisited = true;

                //방문 타일 표시
                currentTile.SetColor(_diagonalColor);

                //이동
                ++currentX;
                ++currentY;
                currentTile = GetValidIndexTile(currentX, currentY);

                if (currentTile == null) break;
            }
        }

        private void CheckDiagonalUpLeft(Tile startTile)
        {
            int currentX = startTile.Index.x - 1;
            int currentY = startTile.Index.y + 1;

            var currentTile = GetValidIndexTile(currentX, currentY);
            if (currentTile == null) return;

            while (IsMovableTile(currentTile))
            {
                //도착 처리
                if (currentTile == _endTile)
                {
                    AddToOpenList(currentTile, startTile);
                    break;
                }

                //오른쪽이 막혔나 체크
                if (IsBlockedTile(currentX + 1, currentY))
                {
                    //오른쪽 위가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX + 1, currentY + 1) == false)
                    {                        
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //아래가 막혔나 체크
                if (IsBlockedTile(currentX, currentY - 1))
                {
                    //왼쪽 아래가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX - 1, currentY - 1) == false)
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //왼쪽 타일이 비었는지 체크 - 탐색용
                if (IsMovableTile(currentX - 1, currentY))
                {
                    //왼쪽 탐색 - 점프포인트 찾았으면 OpenList에 추가
                    if (CheckStraightLeft(currentTile, false))
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //위쪽 타일이 비었는지 체크 - 탐색용
                if (IsMovableTile(currentX, currentY + 1))
                {
                    //위쪽 탐색 - 점프포인트 찾았으면 OpenList에 추가
                    if (CheckStraightUp(currentTile, false))
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //전부 해당 안되면 방문했다 표시하고 넘어가기
                currentTile.IsVisited = true;

                //방문 타일 표시
                currentTile.SetColor(_diagonalColor);

                //이동
                --currentX;
                ++currentY;
                currentTile = GetValidIndexTile(currentX, currentY);

                if (currentTile == null) break;                
            }
        }

        private void CheckDiagonalDownRight(Tile startTile)
        {
            int currentX = startTile.Index.x + 1;
            int currentY = startTile.Index.y - 1;

            var currentTile = GetValidIndexTile(currentX, currentY);
            if (currentTile == null) return;

            while (IsMovableTile(currentTile))
            {
                //도착 처리
                if (currentTile == _endTile)
                {
                    AddToOpenList(currentTile, startTile);
                    break;
                }

                //왼쪽이 막혔나 체크
                if (IsBlockedTile(currentX - 1, currentY))
                {
                    //왼쪽 아래가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX - 1, currentY - 1) == false)
                    {                        
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //위가 막혔나 체크
                if (IsBlockedTile(currentX, currentY + 1))
                {
                    //오른쪽 위가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX + 1, currentY + 1) == false)
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //오른쪽 타일이 비었는지 체크 - 탐색용
                if (IsMovableTile(currentX + 1, currentY))
                {
                    //오른쪽 탐색 - 무언가 있으면 OpenList에 추가
                    if (CheckStraightRight(currentTile, false))
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //아래쪽 타일이 비었는지 체크 - 탐색용
                if (IsMovableTile(currentX, currentY - 1))
                {
                    //아래쪽 탐색 - 무언가 있으면 OpenList에 추가
                    if (CheckStraightDown(currentTile, false))
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //전부 해당 안되면 방문했다 표시하고 넘어가기
                currentTile.IsVisited = true;

                //방문 타일 표시
                currentTile.SetColor(_diagonalColor);

                //이동
                ++currentX;
                --currentY;
                currentTile = GetValidIndexTile(currentX, currentY);

                if (currentTile == null) break;                
            }
        }

        private void CheckDiagonalDownLeft(Tile startTile)
        {
            int currentX = startTile.Index.x - 1;
            int currentY = startTile.Index.y - 1;

            var currentTile = GetValidIndexTile(currentX, currentY);
            if (currentTile == null) return;

            while (IsMovableTile(currentTile))
            {
                //도착 처리
                if (currentTile == _endTile)
                {
                    AddToOpenList(currentTile, startTile);
                    break;
                }

                //오른쪽이 막혔나 체크
                if (IsBlockedTile(currentX + 1, currentY))
                {
                    //오른쪽 아래가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX + 1, currentY - 1) == false)
                    {                        
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //위가 막혔나 체크
                if (IsBlockedTile(currentX, currentY + 1))
                {
                    //왼쪽 위가 비었으면 ForcedNeighbor
                    if (IsBlockedTile(currentX - 1, currentY + 1) == false)
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //왼쪽 타일이 비었는지 체크 - 탐색용
                if (IsMovableTile(currentX - 1, currentY))
                {
                    //왼쪽 탐색 - 무언가 있으면 OpenList에 추가
                    if (CheckStraightLeft(currentTile, false))
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //아래쪽 타일이 비었는지 체크 - 탐색용
                if (IsMovableTile(currentX, currentY - 1))
                {
                    //아래쪽 탐색 - 무언가 있으면 OpenList에 추가
                    if (CheckStraightDown(currentTile, false))
                    {
                        AddToOpenList(currentTile, startTile);
                        currentTile.SetColor(Color.cyan);
                        break;
                    }
                }

                //전부 해당 안되면 방문했다 표시하고 넘어가기
                currentTile.IsVisited = true;

                //방문 타일 표시
                currentTile.SetColor(_diagonalColor);

                //이동
                --currentX;
                --currentY;
                currentTile = GetValidIndexTile(currentX, currentY);

                if (currentTile == null) break;                
            }
        }
        #endregion

        #region 길찾을 때 사용한 타일 관련 함수
        private bool IsBlockedTile(int indexX, int indexY)
        {
            if (_tileMap.IsValidIndex(indexX, indexY) == false) return false;

            var tile = _tiles[indexX, indexY];
            if (tile.IsBlocked)
            {
                if (tile == _endTile) return false;
                return true;
            }

            return false;
        }

        private bool IsMovableTile(int indexX, int indexY)
        {
            if (_tileMap.IsValidIndex(indexX, indexY) == false) return false;

            return IsMovableTile(_tiles[indexX, indexY]);
        }

        private bool IsMovableTile(Tile tile)
        {
            if (tile.IsBlocked)
            {
                if (tile == _endTile) return true;
                return false;
            }
            if (tile.IsVisited) return false;

            return true;
        }

        private Tile GetValidIndexTile(int indexX, int indexY)
        {
            if (_tileMap.IsValidIndex(indexX, indexY) == false) return null;
            return _tiles[indexX, indexY];
        }

        private int CalcHeuristic(TileIndex _currentIndex, TileIndex _endIndex)
        {
            int x = Mathf.Abs(_currentIndex.x - _endIndex.x);
            int y = Mathf.Abs(_currentIndex.y - _endIndex.y);
            int reming = Mathf.Abs(x - y);

            return COST_DIAGONAL * Mathf.Min(x, y) + COST_STRAIGHT * reming;
        }

        private Tile GetLowestFCost(List<Tile> openTiles)
        {
            if(openTiles == null) return null;
            if(openTiles.Count == 0) return null;

            var lowestTile = openTiles[0];
            for(int i = 1; i < openTiles.Count; ++i)
            {
                if (openTiles[i].fCost < lowestTile.fCost)
                    lowestTile = openTiles[i];
            }

            return lowestTile;
        }

        private void AddToOpenList(Tile currentTile, Tile parentTile)
        {
            currentTile.ParentTile = parentTile;
            currentTile.gCost = parentTile.gCost + CalcHeuristic(parentTile.Index, currentTile.Index);
            currentTile.hCost = CalcHeuristic(currentTile.Index, _endTile.Index);
            _openList.Add(currentTile);
        }
        #endregion

        #region 그리기
        private void ShowPath(Tile tile)
        {
            if(tile == null) return;

            if (tile != _startTile && tile != _endTile)
                tile.SetColor(Color.green);

            if(tile.ParentTile != null)
            {
                var start = tile.WorldPosition;
                var end = tile.ParentTile.WorldPosition;
                Debug.DrawLine(start, end, Color.green, 5);
            }

            ShowPath(tile.ParentTile);
        }
        #endregion
    }
}
#endif
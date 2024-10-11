using System;

namespace FastPathFinder.BitFlagMap
{
    /// <summary>
    /// UInt64 기준으로 BitFlag를 TileMap처럼 처리하는 클래스
    /// 0 = Off, 1 = On
    /// 한 줄씩 처리 : 
    /// ex) 가로가 80, 세로가 3일 경우
    /// 한줄 = 80 / 64(UInt64의 비트 수) + 1 = 2
    /// 즉, UInt64 배열 2개가 한줄이고 세로가 3이니 총 UInt64 배열 6개 사용
    /// </summary>
    public class BitFlagMap : IDisposable
    {
        //Data
        private UInt64[] _bitFlags = null;
        private readonly int _width = 0;
        private readonly int _depth = 0;

        //Public Data
        /// <summary> 입력한 가로 크기 </summary>
        public int Width => _width;
        /// <summary> 입력한 세로 크기 == Depth </summary>
        public int Height => _depth;
        /// <summary> 전체 크기 = width  * depth </summary>
        public int TotalSize => _width * _depth;

        /// <summary> Array 가로 크기 </summary>
        public readonly int ArrayWidthSize;
        /// <summary> Array 전체 크기(ArrayWidthSize * _depth) </summary>
        public readonly int ArrayTotalSize;
        /// <summary> 가로 한줄 bit 개수(최대 표현 가능 개수) </summary>
        public readonly int BitWidth;

        //Const
        public const int BIT_SIZE = 8 * sizeof(UInt64);
        public const int BIT_MAX_SHIFT = BIT_SIZE - 1;
        public const UInt64 ONE = (UInt64)1;

        #region Init & Dispose
        public BitFlagMap(int width, int depth)
        {
            _width = width;
            _depth = depth;

            int quot = width / BIT_SIZE;
            int rem = width % BIT_SIZE;
            ArrayWidthSize = quot;
            if (rem > 0) ArrayWidthSize++;

            ArrayTotalSize = ArrayWidthSize * depth;
            BitWidth = ArrayWidthSize * BIT_SIZE;

            _bitFlags = new UInt64[ArrayTotalSize];
        }

        public void Dispose()
        {
            _bitFlags = null;
        }
        #endregion

        #region Bit 연산
        /// <summary>
        /// x, y (열, 행) 위치를 Bit위치로 변환
        /// </summary>
        private int PositionToBitPosition(int x, int y)
        {
            return x + y * BitWidth;
        }

        /// <summary>
        /// Bit위치로 타겟Bits 구하기
        /// </summary>
        private UInt64 BitPositionToTargetBits(int bitPos)
        {
            return ONE << (bitPos % BIT_SIZE);
        }
        #endregion

        #region Get / Set
        /// <summary>
        /// x, y 위치의 Flag 반환
        /// </summary>
        public bool GetFlag(int x, int y)
        {
            if (x < 0 || x >= Width) return false;
            if (y < 0 || y >= Height) return false;

            var bitPos = PositionToBitPosition(x, y);
            var arrayIndex = bitPos / BIT_SIZE;
            UInt64 confirmBits = BitPositionToTargetBits(bitPos);

            return (_bitFlags[arrayIndex] & confirmBits) > 0;
        }

        /// <summary>
        /// x, y 위치의 Flag 설정
        /// </summary>
        public void SetFlag(int x, int y, bool value)
        {
            if (x < 0 || x >= Width) return;
            if (y < 0 || y >= Height) return;

            var bitPos = PositionToBitPosition(x, y);
            var arrayIndex = bitPos / BIT_SIZE;
            UInt64 targetBits = BitPositionToTargetBits(bitPos);

            if (value)
                _bitFlags[arrayIndex] |= targetBits;
            else
                _bitFlags[arrayIndex] &= ~targetBits;
        }
        #endregion
    }
}

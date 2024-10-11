using FastPathFinder.Data;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Test1))]
public class Test1_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("테스트"))
        {
            var tileMap = (Test1)target;
            tileMap.Test();
        }
    }
}

public class Test1 : MonoBehaviour
{
    //Map
    private int[] _binaryMap = null;
    private int _intSize = 31;

    //Size
    public int Row = 10000;
    public int Column = 10000;
    public int TryNum = 100000;

    public void Test()
    {
        /* 테스트1
        UInt16 temp1 = (UInt16)1 << 15;
        Debug.Log($"temp1 10진수: {temp1}");
        Debug.Log($"temp1 2진수: {Convert.ToString(temp1, 2)}");

        UInt16 temp2 = (UInt16)1 << 16;
        Debug.Log($"temp2 10진수: {temp2}");
        Debug.Log($"temp2 2진수: {Convert.ToString(temp2, 2)}");
        */

        /* 테스트2
        //Int 크기 = 원래 Int 크기 -부호 비트;
        _intSize = (1 << sizeof(int)) - 1;

        _binaryMap = MakeBinaryMap(Row, Column);

        //0 ~ 7
        SetValue(0, 0, true);
        SetValue(1, 0, true);
        SetValue(2, 0, true);
        SetValue(7, 0, true);

        for (int i = 0; i < _binaryMap.Length; i++)
        {
            var binary = _binaryMap[i];

            Debug.Log($"{i} map : {Convert.ToString(binary, 2).PadLeft(_intSize, '0')}");
        }
        */

        /* 테스트 3
        _binaryMap = MakeBinaryMap(Row, Column);

        //테스트 좌표
        int[] testX = new int[TryNum];
        int[] testY = new int[TryNum];
        for (int i = 0; i < TryNum; ++i)
        {
            testX[i] = UnityEngine.Random.Range(0, Column);
            testY[i] = UnityEngine.Random.Range(0, Row);
        }

        TimeChecker.StartTimer();

        for (int i = 0; i < TryNum; ++i)
            GetValue2(testX[i], testY[i]);

        string time = TimeChecker.StopTimer();

        Debug.Log($"time : {time}");
        */

        string time = string.Empty;
        TimeChecker.StartTimer();
        var testMap = new BitFlagMap(Column, Row);
        time = TimeChecker.StopTimer();
        Debug.Log($"time0 : {time}");

        int memorySize = testMap.GetMemorySize();
        Debug.Log($"total byte size : {memorySize}");
        Debug.Log($"total size : {ByteToText(memorySize)}");

        //맵 정상적으로 생성했나 체크
        int maxX = Column - 1;
        int maxY = Row - 1;
        bool isOk = testMap.SetFlag(maxX, maxY, true);
        Debug.Log($"Is Map valid? : {isOk}");

        //테스트 좌표
        int[] testX = new int[TryNum];
        int[] testY = new int[TryNum];
        for (int i = 0; i < TryNum; ++i)
        {
            testX[i] = UnityEngine.Random.Range(0, Column);
            testY[i] = UnityEngine.Random.Range(0, Row);
        }

        TimeChecker.StartTimer();
        for (int i = 0; i < TryNum; ++i)
        {
            testMap.SetFlag(testX[i], testY[i], true);
        }
        time = TimeChecker.StopTimer();
        Debug.Log($"time1 : {time}");

        TimeChecker.StartTimer();
        for (int i = 0; i < TryNum; ++i)
        {
            bool value = testMap.GetFlag(testX[i], testY[i]);
        }
        time = TimeChecker.StopTimer();
        Debug.Log($"time2 : {time}");        
    }

    private int[] MakeBinaryMap(int row, int col)
    {
        int size = row * col;
        int arrayNum = size / _intSize + 1;
        int remainder = size % _intSize;
        int notUseBinary = 0;

        //계산식 = 2 ^ (Int크기 - 나머지) - 1;
        if (remainder > 0)
        {
            notUseBinary = (1 << (_intSize - remainder)) - 1;
        }

        //사용 안하는 칸 수 만큼 1로 채움
        var result = new int[arrayNum];
        result[arrayNum - 1] |= notUseBinary;

        return result;
    }

    private bool GetValue(int x, int y)
    {
        int index = x + y * Column;
        int arrayIndex = index / _intSize;
        int remainder = index % _intSize;
        int shiftCount = _intSize - remainder - 1;
        int confirmBinary = 1 << shiftCount;

        return (_binaryMap[arrayIndex] & confirmBinary) > 0;
    }

    private int GetValue2(int x, int y)
    {
        int index = x + y * Column;
        int arrayIndex = index / _intSize;
        int confirmBinary = 1 << (_intSize - (index % _intSize) - 1);

        return _binaryMap[arrayIndex] & confirmBinary;
    }

    private void SetValue(int x, int y, bool value)
    {
        if (x >= Column) return;
        if (y >= Row) return;

        int index = x + y * Column;
        int arrayIndex = index / _intSize;
        int remainder = index % _intSize;
        int shiftCount = _intSize - remainder - 1;
        int targetBinary = 1 << shiftCount;

        if (value)
        {
            _binaryMap[arrayIndex] |= targetBinary;
        }
        else
        {
            _binaryMap[arrayIndex] &= ~targetBinary;
        }
    }

    private string ByteToText(long byteSize)
    {
        var sizeText = new StringBuilder();
        if (byteSize < 0)
        {
            sizeText.Append("-");
            byteSize *= -1;
        }

        //0 ~ 1023 = Byte
        if (byteSize < 1024L)
        {
            sizeText.AppendFormat("{0:0.00}", byteSize);
            sizeText.Append("Bytes");
        }
        //1024 ~ 1048575 = KB
        else if (byteSize < 1048576L)
        {
            sizeText.AppendFormat("{0:0.00}", byteSize / 1024.0f);
            sizeText.Append("KB");
        }
        //1048576 ~ 1073741823 = MB
        else if (byteSize < 1073741824L)
        {
            sizeText.AppendFormat("{0:0.00}", byteSize / 1048576.0f);
            sizeText.Append("MB");
        }
        //그보다 크면 다 GB로
        else
        {
            sizeText.AppendFormat("{0:0.00}", byteSize / 1073741824.0f);
            sizeText.Append("GB");
        }

        return sizeText.ToString();
    }
}

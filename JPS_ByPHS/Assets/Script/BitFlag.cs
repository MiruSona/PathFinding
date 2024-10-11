#if UNITY_EDITOR
using System;
using UnityEngine;

namespace PathFindingTest
{
    public struct BitFlag
    {
        public uint Flags { get; private set; }

        public BitFlag(uint flag = 0)
        {
            Flags = flag;
        }

        public bool IsZero()
        {
            return Flags == 0;
        }

        public bool IsOn(uint flag)
        {
            if (flag == 0) return flag == 0;
            return (Flags & flag) != 0;
        }

        public void Set(uint flag)
        {
            if (IsOn(flag)) return;
            Flags = Flags | flag;
        }

        public void Remove(uint flag)
        {
            if (IsOn(flag) == false) return;
            Flags = Flags & ~flag;
        }

        public void Clear()
        {
            Flags = 0;
        }

        public override string ToString()
        {
            return Convert.ToString(Flags, 2);
        }
    }
}
#endif
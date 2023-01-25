using System;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    [Flags, Serializable]
    public enum Direction
    {
        None,
        South = 0b0001,
        North = 0b0010,
        West = 0b0100,
        East = 0b1000,
    }

    public static class DirectionExt
    {
        private static readonly int[] SignMap = { 0, -1, 1, 0 };

        private static readonly int[] OrdinalMap4 = {
            -1,
            0, // South
            1, // North
            -1,
            2, // West
            -1, -1, -1,
            3, // East
            -1, -1, -1, -1, -1, -1, -1,
        };

        private static readonly int[] OrdinalMap8 = {
            -1,
            0, // South
            1, // North
            -1,
            2, // West
            3, // Southwest
            4, // Northwest
            -1,
            5, // East
            6, // Southeast
            7, // Northeast
            -1, -1, -1, -1, -1,
        };

        public static int ToOrdinalIndex4(this Direction direction)
        {
            return OrdinalMap4[(int)direction];
        }

        public static int ToOrdinalIndex8(this Direction direction)
        {
            return OrdinalMap8[(int)direction];
        }

        public static bool IsValid(this Direction direction)
        {
            return OrdinalMap8[(int)direction] >= 0;
        }


        public static Index ToIndex(this Direction direction)
        {
            var xFlag = unchecked(((uint)direction & 0b1100) >> 2);
            var x = SignMap[xFlag];
            var yFlag = unchecked((uint)direction & 0b11);
            var y = SignMap[yFlag];

            return new Index(x, y);
        }

        public static Vector3 ToVector3(this Direction direction)
        {
            var index = direction.ToIndex();
            return new Vector3(index.x, 0, index.y).normalized;
        }

        public static bool Has(this Direction direction, Direction flag)
        {
            return (direction & flag) != Direction.None;
        }
    }
}
using System;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    [Serializable]
    public enum DirectionId
    {
        Invalid, // Must be the first item!

        East,
        West,
        North,
        South,
    }

    [Serializable]
    public struct Direction
    {
        private static readonly Index[] Indices;
        private static readonly Direction[] Directions;
        private static readonly Direction[] Opposites;
        private static readonly string[] StringValues;

        // We're using an enum for its underlying type, even though an int would be better.
        // This is so the inspector will treat its value as an enum.
        [SerializeField] private DirectionId value;

        // We expose the enum value for use with switch statements 
        public DirectionId Id => value;

        public const int Length = 4;

        public int Ordinal => (int)value - 1;

        private Direction(DirectionId value)
        {
            this.value = value;
        }

        public Index AsIndex => Indices[Ordinal];

        public Direction Opposite => Opposites[Ordinal];

        public bool IsValid => (int)value is >= 0 and < Length;

        public static ReadOnlySpan<Direction> Values => Directions;
        public static ReadOnlySpan<string> Names => StringValues;

        public override string ToString() => StringValues[Ordinal];

        static Direction()
        {
            East = new Direction(DirectionId.East);
            West = new Direction(DirectionId.West);
            North = new Direction(DirectionId.North);
            South = new Direction(DirectionId.South);

            Indices = new Index[Length];
            Indices[East.Ordinal] = new Index(1, 0);
            Indices[West.Ordinal] = new Index(-1, 0);
            Indices[North.Ordinal] = new Index(0, 1);
            Indices[South.Ordinal] = new Index(0, -1);

            Opposites = new Direction[Length];
            Opposites[East.Ordinal] = West;
            Opposites[West.Ordinal] = East;
            Opposites[North.Ordinal] = South;
            Opposites[South.Ordinal] = North;

            Directions = new Direction[Length];
            Directions[East.Ordinal] = East;
            Directions[West.Ordinal] = West;
            Directions[North.Ordinal] = North;
            Directions[South.Ordinal] = South;

            StringValues = new string[Length];
            StringValues[East.Ordinal] = "East";
            StringValues[West.Ordinal] = "West";
            StringValues[North.Ordinal] = "North";
            StringValues[South.Ordinal] = "South";
        }
        
        public static Direction FromIndices(Index from, Index to)
        {
            return FromIndexNormalized(to - from);
        }

        public static Direction FromIndexNormalized(in Index index)
        {
            var signX = Math.Sign(index.x);
            var signY = Math.Sign(index.y);
            if (Math.Abs(signX) == Math.Abs(signY)) {
                return default;
            }

            return signX switch {
                -1 => West,
                1 => East,
                _ => signY switch {
                    -1 => South,
                    1 => North,
                    _ => default,
                },
            };
        }

        public static readonly Direction East;
        public static readonly Direction West;
        public static readonly Direction North;
        public static readonly Direction South;

        public bool Equals(Direction other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is Direction other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Ordinal.GetHashCode();
        }

        public static bool operator ==(Direction left, Direction right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !left.Equals(right);
        }

        public Vector3 ToVector3()
        {
            var index = AsIndex;
            return new Vector3(index.x, 0, index.y).normalized;
        }

        public Vector2 ToVector2()
        {
            var index = AsIndex;
            return new Vector2(index.x, index.y).normalized;
        }
    }

    [Serializable]
    public class DirectionList<T>
    {
        [SerializeField, HideInInspector] private T[] data = new T[Direction.Length];

        public T this[Direction direction]
        {
            get => data[direction.Ordinal];
            set => data[direction.Ordinal] = value;
        }

        public void Clear()
        {
            for (var i = 0; i < data.Length; ++i) {
                data[i] = default;
            }
        }
    }
}
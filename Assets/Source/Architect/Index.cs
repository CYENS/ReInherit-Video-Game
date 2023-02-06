using System;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    [Serializable]
    public struct Index
    {
        public int x;
        public int y;

        public const float UnitsPerIndex = 9;

        public Index(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Index West => new(x - 1, y);
        public Index East => new(x + 1, y);
        public Index South => new(x, y - 1);
        public Index North => new(x, y + 1);
        public Index NorthWest => new(x - 1, y + 1);
        public Index NorthEast => new(x + 1, y + 1);
        public Index SouthWest => new(x - 1, y - 1);
        public Index SouthEast => new(x + 1, y - 1);

        public Index Towards(Direction direction) => this + direction.ToIndex();

        public Vector3 WorldCenter => new(
            x * UnitsPerIndex + UnitsPerIndex * 0.5f,
            0,
            y * UnitsPerIndex + UnitsPerIndex * 0.5f
        );

        public static Index FromWorld(Vector3 worldCoordinates) => new() {
            x = Mathf.RoundToInt((worldCoordinates.x - UnitsPerIndex * 0.5f) / UnitsPerIndex),
            y = Mathf.RoundToInt((worldCoordinates.z - UnitsPerIndex * 0.5f) / UnitsPerIndex),
        };

        public Vector3 WorldSECorner => new(
            x * UnitsPerIndex + UnitsPerIndex,
            0,
            y * UnitsPerIndex
        );

        public Vector3 WorldNECorner => new(
            x * UnitsPerIndex + UnitsPerIndex,
            0,
            y * UnitsPerIndex + UnitsPerIndex
        );

        public Vector3 WorldSWCorner => new(
            x * UnitsPerIndex,
            0,
            y * UnitsPerIndex
        );

        public Vector3 WorldNWCorner => new(
            x * UnitsPerIndex,
            0,
            y * UnitsPerIndex + UnitsPerIndex
        );

        public static Index Zero => new();
        public static Index One => new(1, 1);

        public override bool Equals(object obj)
        {
            if (obj is Index index) {
                return Equals(index);
            }

            return false;
        }

        public bool Equals(Index rhs) => x == rhs.x && y == rhs.y;

        public bool Equals(int iX, int iY) => x == iX && y == iY;

        public override int GetHashCode() => HashCode.Combine(x, y);

        public static bool operator ==(Index lhs, Index rhs) => lhs.x == rhs.x && lhs.y == rhs.y;

        public static bool operator !=(Index lhs, Index rhs) => lhs.x != rhs.x || lhs.y != rhs.y;

        public static Index operator +(Index lhs, Index rhs) => new(lhs.x + rhs.x, lhs.y + rhs.y);

        public static Index operator -(Index lhs, Index rhs) => new(lhs.x - rhs.x, lhs.y - rhs.y);

        public static Index operator -(Index value) => new(-value.x, -value.y);

        public static Index operator +(Index value) => value;

        public static Index operator *(Index lhs, Index rhs) => new(lhs.x * rhs.x, lhs.y * rhs.y);

        public static Index operator /(Index lhs, Index rhs) => new(lhs.x / rhs.x, lhs.y / rhs.y);

        public static Index operator *(Index lhs, int rhs) => new(lhs.x * rhs, lhs.y * rhs);

        public static Index operator /(Index lhs, int rhs) => new(lhs.x / rhs, lhs.y / rhs);

        public override string ToString()
        {
            return $"[{x}, {y}]";
        }
    }
}
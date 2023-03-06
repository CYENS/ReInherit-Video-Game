using System;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    [Serializable]
    public struct Index
    {
        public const int MaxSizeX = 48;
        public const int MaxSizeY = 32;

        private const int OffsetX = MaxSizeX / 2;
        private const int OffsetY = 0;

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

        public static readonly Index MinValue = new(0, 0);
        public static readonly Index MaxValue = new(MaxSizeX, MaxSizeY);

        private int WorldX => x - OffsetX;
        private int WorldY => y - OffsetY;

        public Index Towards(Direction direction) => this + direction.AsIndex;

        public Vector3 WorldCenter => new(
            WorldX * UnitsPerIndex + UnitsPerIndex * 0.5f,
            0,
            WorldY * UnitsPerIndex + UnitsPerIndex * 0.5f
        );

        public static float TransformX(int pointX) => (pointX - OffsetX) * UnitsPerIndex + UnitsPerIndex * 0.5f;
        public static float TransformY(int pointY) => (pointY - OffsetY) * UnitsPerIndex + UnitsPerIndex * 0.5f;

        public static bool AreAdjacent(Index lhs, Index rhs)
        {
            var diff = lhs - rhs;
            diff.x = Mathf.Abs(diff.x);
            diff.y = Mathf.Abs(diff.y);

            return diff is { x: <= 1, y: <= 1 } && diff.x != diff.y;
        }

        public bool IsValid => x is >= 0 and < MaxSizeX && y is >= 0 and < MaxSizeY;

        public static Index FromWorld(Vector3 worldCoordinates) => new() {
            x = Mathf.RoundToInt((worldCoordinates.x - UnitsPerIndex * 0.5f) / UnitsPerIndex) + OffsetX,
            y = Mathf.RoundToInt((worldCoordinates.z - UnitsPerIndex * 0.5f) / UnitsPerIndex) + OffsetY,
        };

        public static Vector2 ToFractionalIndex(Vector3 worldCoordinates) => new() {
            x = (worldCoordinates.x - UnitsPerIndex * 0.5f) / UnitsPerIndex + OffsetX,
            y = (worldCoordinates.z - UnitsPerIndex * 0.5f) / UnitsPerIndex + OffsetY,
        };

        public Vector3 WorldSECorner => new(
            WorldX * UnitsPerIndex + UnitsPerIndex,
            0,
            WorldY * UnitsPerIndex
        );

        public Vector3 WorldNECorner => new(
            WorldX * UnitsPerIndex + UnitsPerIndex,
            0,
            WorldY * UnitsPerIndex + UnitsPerIndex
        );

        public Vector3 WorldSWCorner => new(
            WorldX * UnitsPerIndex,
            0,
            WorldY * UnitsPerIndex
        );

        public Vector3 WorldNWCorner => new(
            WorldX * UnitsPerIndex,
            0,
            WorldY * UnitsPerIndex + UnitsPerIndex
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
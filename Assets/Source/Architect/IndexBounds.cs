using System;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    [Serializable]
    public struct IndexBounds
    {
        public Index min;
        public Index max;

        public IndexBounds(int minX, int minY, int maxX, int maxY)
        {
            min = new Index(minX, minY);
            max = new Index(maxX, maxY);
        }

        public IndexBounds(Index min, Index max)
        {
            this.min = min;
            this.max = max;
        }

        public readonly int Width => Math.Max(0, max.x - min.x);
        public readonly int Height => Math.Max(0, max.y - min.y);

        public static IndexBounds Unused => new() {
            min = new Index(int.MaxValue, int.MaxValue),
            max = new Index(int.MinValue, int.MinValue),
        };

        public Index BottomLeft
        {
            get => min;
            set => min = value;
        }

        public Index TopRight
        {
            get => max;
            set => max = value;
        }

        public Index BottomRight
        {
            get => new(max.x, min.y);
            set {
                max.x = value.x;
                min.y = value.y;
            }
        }

        public Index TopLeft
        {
            get => new(min.x, max.y);
            set {
                min.x = value.x;
                max.y = value.y;
            }
        }

        public static IndexBounds Zero => new();

        public readonly IndexBounds Readjusted => new(
            Mathf.Min(min.x, max.x), Mathf.Min(min.y, max.y),
            Mathf.Max(min.x, max.x), Mathf.Max(min.y, max.y)
        );

        public int Area => Width * Height;

        public readonly bool Contains(Index index) =>
            index.x >= min.x && index.y >= min.y && index.x < max.x && index.y < max.y;

        public readonly bool Contains(IndexBounds bounds) =>
            bounds.min.x >= min.x && bounds.min.y >= min.y && bounds.max.x < max.x && bounds.max.y < max.y;

        public readonly bool ContainsInclusive(IndexBounds bounds) =>
            bounds.min.x >= min.x && bounds.min.y >= min.y && bounds.max.x <= max.x && bounds.max.y <= max.y;

        /// Readjusts min and max x and y coordinates to be in the correct format,
        /// where max.x >= min.x and max.y >= min.y. Note that NO values are clamped,
        /// instead they're rearranged when applicable.
        public void ReadjustCorners()
        {
            var newMin = new Index(Mathf.Min(min.x, max.x), Mathf.Min(min.y, max.y));
            var newMax = new Index(Mathf.Max(min.x, max.x), Mathf.Max(min.y, max.y));
            min = newMin;
            max = newMax;
        }


        public void Reset()
        {
            min = new Index(int.MaxValue, int.MaxValue);
            max = new Index(int.MinValue, int.MinValue);
        }

        public void AddInclude(Index index)
        {
            min.x = Math.Min(index.x, min.x);
            min.y = Math.Min(index.y, min.y);
            max.x = Math.Max(index.x, max.x);
            max.y = Math.Max(index.y, max.y);
        }

        public void AddInclude(IndexBounds bounds)
        {
            AddInclude(bounds.min);
            AddInclude(bounds.max);
        }

        public readonly IndexBounds WithPadding(int padding)
        {
            var bounds = this;
            bounds.min.x -= padding;
            bounds.min.y -= padding;
            bounds.max.x += padding;
            bounds.max.y += padding;
            bounds.ReadjustCorners();
            return bounds;
        }
        
        public readonly IndexBounds Clipped
        {
            get {
                var newMin = new Index(
                    Mathf.Max(FullBounds.min.x, Mathf.Min(min.x, max.x)),
                    Mathf.Max(FullBounds.min.y, Mathf.Min(min.y, max.y))
                );

                var newMax = new Index(
                    Mathf.Min(FullBounds.max.x, Mathf.Max(min.x, max.x)),
                    Mathf.Min(FullBounds.max.y, Mathf.Max(min.y, max.y))
                );

                return new IndexBounds(newMin, newMax);
            }
        }

        public static readonly IndexBounds FullBounds = new(Index.MinValue, Index.MaxValue);

        public static bool operator ==(IndexBounds lhs, IndexBounds rhs) => lhs.min == rhs.min && lhs.max == rhs.max;

        public static bool operator !=(IndexBounds lhs, IndexBounds rhs) => lhs.min != rhs.min || lhs.max != rhs.max;

        public static IndexBounds operator +(IndexBounds lhs, Index rhs) => new(lhs.min + rhs, lhs.max + rhs);

        public static IndexBounds operator -(IndexBounds lhs, Index rhs) => new(lhs.min - rhs, lhs.max - rhs);

        public bool Equals(IndexBounds other) => min.Equals(other.min) && max.Equals(other.max);

        public override int GetHashCode() => HashCode.Combine(min, max);

        public override bool Equals(object obj) => obj is IndexBounds other && Equals(other);

        public override string ToString() => $"[Min: {min}, Max: {max}]";
    }
}
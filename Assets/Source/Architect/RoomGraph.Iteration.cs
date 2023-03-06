namespace Cyens.ReInherit.Architect
{
    public partial class RoomGraph
    {
        /// Iterate through all the indices covered by the given bounds
        private IndexIterator Iterate(in IndexBounds bounds) => new(bounds);

        /// Iterate through the indices of one side (towards the given direction) of the given bounds
        public IndexIterator IterateSide(Direction direction, in IndexBounds bounds)
        {
            return direction.Id switch {
                DirectionId.East => IterateEast(bounds),
                DirectionId.West => IterateWest(bounds),
                DirectionId.North => IterateNorth(bounds),
                DirectionId.South => IterateSouth(bounds),
                _ => new IndexIterator(IndexBounds.Zero),
            };
        }

        /// Iterates through the indices on the north side
        private IndexIterator IterateNorth(in IndexBounds bounds)
        {
            if (bounds.Height == 0) {
                return new IndexIterator(IndexBounds.Zero);
            }

            var min = new Index(bounds.min.x, bounds.max.y - 1);
            var max = new Index(bounds.max.x, bounds.max.y);
            return new IndexIterator(new IndexBounds(min, max));
        }

        /// Iterates through the indices on the south side
        private IndexIterator IterateSouth(in IndexBounds bounds)
        {
            if (bounds.Height == 0) {
                return new IndexIterator(IndexBounds.Zero);
            }

            var min = new Index(bounds.min.x, bounds.min.y);
            var max = new Index(bounds.max.x, bounds.min.y + 1);
            return new IndexIterator(new IndexBounds(min, max));
        }

        /// Iterates through the indices on the east side
        private IndexIterator IterateEast(in IndexBounds bounds)
        {
            if (bounds.Width == 0) {
                return new IndexIterator(IndexBounds.Zero);
            }

            var min = new Index(bounds.max.x - 1, bounds.min.y);
            var max = new Index(bounds.max.x, bounds.max.y);
            return new IndexIterator(new IndexBounds(min, max));
        }

        /// Iterates through the indices on the west side
        private IndexIterator IterateWest(in IndexBounds bounds)
        {
            if (bounds.Width == 0) {
                return new IndexIterator(IndexBounds.Zero);
            }

            var min = new Index(bounds.min.x, bounds.min.y);
            var max = new Index(bounds.min.x + 1, bounds.max.y);
            return new IndexIterator(new IndexBounds(min, max));
        }


        public struct IndexIterator
        {
            public IndexIterator(in IndexBounds bounds)
            {
                m_bounds = bounds.Clipped;
                m_current = new Index(m_bounds.min.x - 1, m_bounds.min.y);
            }

            private Index m_current;
            private readonly IndexBounds m_bounds;

            public Index Current => m_current;

            public IndexIterator GetEnumerator() => this;

            public bool MoveNext()
            {
                ++m_current.x;
                if (m_current.x < m_bounds.max.x) {
                    return true;
                }

                m_current.x = m_bounds.min.x;
                ++m_current.y;

                return m_current.y < m_bounds.max.y;
            }
        }
    }
}
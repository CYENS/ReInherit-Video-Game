using System;
using System.Collections.Generic;
using Cyens.ReInherit.Extensions;

namespace Cyens.ReInherit.Pooling
{
    [Serializable]
    public class TempList<T> : List<T>, IDisposable
    {
        private const int DefaultCapacity = 32;

        private static readonly List<TempList<T>> Pool = new();

        private TempList() { }

        private TempList(int capacity) : base(capacity) { }

        public static TempList<T> Get(int capacity = DefaultCapacity)
        {
            if (Pool.TryPop(out var list)) {
                list.Clear();
                list.ReserveCapacity(capacity);
                list.m_isPooled = false;
                return list;
            }

            return new TempList<T>(capacity);
        }

        public static void Gather(TempList<T> list)
        {
            if (list.m_isPooled) {
                return;
            }

            list.m_isPooled = true;
            list.Clear();
            Pool.Add(list);
        }

        private bool m_isPooled;

        public void Dispose()
        {
            Gather(this);
        }
    }
}
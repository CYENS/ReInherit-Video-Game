using System;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    [Serializable]
    public class GridData<T>
    {
        [SerializeField] private T[,] m_data;

        public GridData()
        {
            m_data = new T[Index.MaxSizeX, Index.MaxSizeY];
        }

        public T this[int x, int y]
        {
            get => m_data[x, y];
            set => m_data[x, y] = value;
        }

        public T this[in Index index]
        {
            get => m_data[index.x, index.y];
            set => m_data[index.x, index.y] = value;
        }

        public bool TryGet(in Index index, out T result)
        {
            if (!index.IsValid) {
                result = default;
                return false;
            }

            result = this[index];
            return true;
        }

        public T GetRaw(in Index index)
        {
            return m_data[index.x, index.y];
        }
    }

}
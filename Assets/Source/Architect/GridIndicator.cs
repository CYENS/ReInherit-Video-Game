using Cyens.ReInherit.Extensions;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    public class GridIndicator : MonoBehaviour
    {
        [SerializeField] private Material material;

        [SerializeField] private Color drawingColor = Color.white.WithAlpha(0.25f);

        [SerializeField] private Index start;
        [SerializeField] private Index end;

        private IndexBounds m_bounds;

        [SerializeField] private bool isDrawing;

        private const float YOffset = 0.01f;

        public bool IsDrawing
        {
            get => isDrawing;
            set => isDrawing = value;
        }

        private void OnValidate()
        {
            SetArea(m_bounds, drawingColor);
        }

        public void SetArea(IndexBounds bounds)
        {
            SetArea(bounds, drawingColor);
        }

        public void SetArea(IndexBounds bounds, Color color)
        {
            start = bounds.min;
            end = bounds.max;
            drawingColor = color;
            Refresh();
        }

        public void SetArea(Index index)
        {
            end = index;
            Refresh();
        }

        public void SetArea(Index index, Color color)
        {
            end = index;
            drawingColor = color;
            Refresh();
        }

        private Vector3 m_min;
        private Vector3 m_max;

        private void Refresh()
        {
            m_bounds = new IndexBounds(start, end).Readjusted;
            m_min = m_bounds.min.WorldSWCorner;
            m_max = m_bounds.max.WorldNECorner;
        }

        public Color Color
        {
            get => drawingColor;
            set => drawingColor = value;
        }

        public void Clear()
        {
            Clear(Index.Zero);
        }

        public void Clear(Index first)
        {
            start = first;
            end = start;
            Refresh();
        }

        public IndexBounds Bounds => m_bounds;
        
        public IndexBounds InclusiveBounds => new(m_bounds.min, m_bounds.max + Index.One);

        public Index Min => m_bounds.min;

        public Index Max => m_bounds.max;

        private void OnRenderObject()
        {
            if (!isDrawing) {
                return;
            }

            material.color = drawingColor;

            material.SetPass(0);

            GL.Begin(GL.QUADS);
            {
                GL.Vertex3(m_max.x, YOffset, m_min.z);
                GL.Vertex3(m_min.x, YOffset, m_min.z);
                GL.Vertex3(m_min.x, YOffset, m_max.z);
                GL.Vertex3(m_max.x, YOffset, m_max.z);
            }
            GL.End();
        }
    }
}
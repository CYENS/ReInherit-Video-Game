using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    public class GridRenderer : MonoBehaviour
    {
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int Cull = Shader.PropertyToID("_Cull");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        [SerializeField] Material lineMaterial;

        private int m_bottom;
        private int m_left;
        private int m_right;
        private int m_top;

        public RoomGraph roomGraph;

        private void Start()
        {
            var minIndex = RoomGraph.Bounds.min;
            var maxIndex = RoomGraph.Bounds.max;
            m_left = minIndex.x;
            m_right = maxIndex.x;
            m_top = maxIndex.y;
            m_bottom = minIndex.y;
        }

        // Update is called once per frame
        private void OnRenderObject()
        {
            lineMaterial.SetPass(0);

            GL.Begin(GL.LINES);
            {
                for (var x = m_left; x <= m_right; ++x) {
                    Line(x, m_top, x, m_bottom);
                }

                //
                for (var y = m_bottom; y <= m_top; ++y) {
                    Line(m_left, y, m_right, y);
                }
            }
            GL.End();
        }

        private void DrawLine(Vector2 from, Vector2 to)
        {
            from = new Vector2(TransformPoint(from.x), TransformPoint(from.y));
            to = new Vector2(TransformPoint(to.x), TransformPoint(to.y));

            var width = 0.1f;

            var dir = (to - from).normalized;
            var cross = new Vector2(-dir.y, dir.x);

            var p1 = from + cross * width;
            var p2 = from - cross * width;
            var p3 = to + cross * width;
            var p4 = to - cross * width;

            GL.Vertex3(p1.x, 0, p1.y);
            GL.Vertex3(p2.x, 0, p2.y);
            GL.Vertex3(p3.x, 0, p3.y);
            GL.Vertex3(p4.x, 0, p4.y);
        }

        private float TransformPoint(float value)
        {
            return value * Index.UnitsPerIndex;
        }

        private void Line(float x1, float y1, float x2, float y2)
        {
            // DrawLine(new Vector2(x1, y1), new Vector2(x2, y2));

            GL.Vertex3(TransformPoint(x1), 0, TransformPoint(y1));
            GL.Vertex3(TransformPoint(x2), 0, TransformPoint(y2));
        }
    }
}
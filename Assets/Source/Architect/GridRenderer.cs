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
        //
        // private int m_bottom;
        // private int m_left;
        // private int m_right;
        //
        // private int m_top;
        //     =

        private Index m_minIndex;
        private Index m_maxIndex;

        private IndexBounds Bounds => IndexBounds.FullBounds;

        private float m_west;
        private float m_east;
        private float m_north;
        private float m_south;
        
        private float TransformX(int x)
        {
            return Index.TransformX(x) - Index.UnitsPerIndex * 0.5f;
        }
        
        private float TransformY(int y)
        {
            return Index.TransformY(y) - Index.UnitsPerIndex * 0.5f;
        }

        private void Start()
        {
            m_west =  TransformX(Bounds.min.x);
            m_east =  TransformX(Bounds.max.x - 1);
            m_south = TransformY(Bounds.min.y);
            m_north = TransformY(Bounds.max.y - 1);
        }

        // Update is called once per frame
        private void OnRenderObject()
        {
            lineMaterial.SetPass(0);

            GL.Begin(GL.LINES);
            {
                for (var x = Bounds.min.x; x < Bounds.max.x; ++x) {
                    var point = TransformX(x);
                    Line(point, m_north, point, m_south);
                    // DrawLine(new Vector2(point, m_north), new Vector2(point, m_south));
                }

                //
                for (var y = Bounds.min.y; y < Bounds.max.y; ++y) {
                    var point = TransformY(y);
                    Line(m_west, point, m_east, point);
                    // DrawLine(new Vector2(m_west, point), new Vector2(m_east, point));
                }
            }
            GL.End();
        }

        private void DrawLine(Vector2 from, Vector2 to)
        {
            var width = 0.25f;
        
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
        
        private void Line(float x1, float y1, float x2, float y2)
        {
            GL.Vertex3(x1, 0, y1);
            GL.Vertex3(x2, 0, y2);
        }
    }
}
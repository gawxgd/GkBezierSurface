using GKbezierPlain.Algorithm;
using System.Drawing;
using System.Numerics;

namespace GKbezierPlain.Geometry
{
    public class Triangle
    {
        public Vertex Vertex1 { get; set; }
        public Vertex Vertex2 { get; set; }
        public Vertex Vertex3 { get; set; }
        
        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            Vertex1 = v1;
            Vertex2 = v2;
            Vertex3 = v3;
            //IsValidTraingle(v1.Position, v2.Position, v3.Position);
        }
        public bool IsValidTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            float sideA = Vector3.Distance(v1, v2);
            float sideB = Vector3.Distance(v2, v3);
            float sideC = Vector3.Distance(v3, v1);

            return (sideA + sideB > sideC) &&
                   (sideA + sideC > sideB) &&
                   (sideB + sideC > sideA);
        }


        public void DrawTriangle(Graphics g, int drawType, Brush color)
        {
            PointF[] points = new PointF[]
            {
                new PointF(Vertex1.Position.X, Vertex1.Position.Y),
                new PointF(Vertex2.Position.X, Vertex2.Position.Y),
                new PointF(Vertex3.Position.X, Vertex3.Position.Y)
            };

            switch (drawType)
            {
                case 0:
                    FillPolygonAlgorithm.FillPolygon(g, points, color);

                    using (Pen pen = new Pen(Color.Black, 1))
                    {
                        g.DrawPolygon(pen, points);
                    }

                    break;

                case 1: 
                    using (Pen pen = new Pen(Color.Black, 1))
                    {
                        g.DrawPolygon(pen, points);
                    }
                    break;

                case 2: 
                    FillPolygonAlgorithm.FillPolygon(g, points, color);
                    break;
            }
        }
        public void RotateTriangle(float alpha, float beta)
        {
            RotationAlgorithm.RotateVertex(Vertex1, alpha, beta);
            RotationAlgorithm.RotateVertex(Vertex2, alpha, beta);
            RotationAlgorithm.RotateVertex(Vertex3, alpha, beta);
        }
    }
}

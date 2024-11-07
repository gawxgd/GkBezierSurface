using GKbezierPlain.Algorithm;
using System;
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
                    FillPolygonWithBucketSort.FillPolygon(g, Brushes.Green, new Vertex[] { Vertex1, Vertex2, Vertex3 });
                    //FillPolygonAlgorithm.FillPolygon(g, points, color);

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

        public static Vector3 BarycentricCoordinates(Vector3 p, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            float areaABC = Area(v0, v1, v2);
            float areaPBC = Area(p, v1, v2);
            float areaPCA = Area(p, v0, v2);
            float areaPAB = Area(p, v0, v1);

            float lambda0 = areaPBC / areaABC;
            float lambda1 = areaPCA / areaABC;
            float lambda2 = areaPAB / areaABC;

            return new Vector3(lambda0, lambda1, lambda2);
        }

        private static float Area(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;
            return 0.5f * Vector3.Cross(edge1, edge2).Length();
        }

        public Vector3 InterpolateColor(Vector3 p)
        {
            Vector3 barycentric = BarycentricCoordinates(p, Vertex1.Position, Vertex2.Position, Vertex3.Position);

            // Interpolacja normalnych
            Vector3 interpolatedNormal = barycentric.X * Vertex1.Normal + barycentric.Y * Vertex2.Normal + barycentric.Z * Vertex3.Normal;

            // Interpolacja współrzędnej z (zakładając, że z jest wartością wzdłuż osi Z)
            float interpolatedZ = barycentric.X * Vertex1.Position.Z + barycentric.Y * Vertex2.Position.Z + barycentric.Z * Vertex3.Position.Z;

            // Kolor może być obliczony np. na podstawie interpolacji normalnej (do oświetlenia)
            // W tym przykładzie po prostu zmieniamy kolor na podstawie interpolowanego normalnego wektora.
            int r = (int)MathHelper.Clamp(255 * Math.Abs(interpolatedNormal.X), 0, 255);
            int g = (int)MathHelper.Clamp(255 * Math.Abs(interpolatedNormal.Y), 0, 255);
            int b = (int)MathHelper.Clamp(255 * Math.Abs(interpolatedNormal.Z), 0, 255);

            return new Vector3(r, g, b);
        }
    }
}

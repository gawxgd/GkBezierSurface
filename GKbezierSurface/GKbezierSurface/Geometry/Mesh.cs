using GKbezierPlain.Algorithm;
using GKbezierSurface.AlgorithmConfigurations;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace GKbezierPlain.Geometry
{
    public class Mesh
    {
        private Vector3[,] controlPoints;
        public List<Triangle> Triangles { get; private set; }
        public Brush Bcolor { get; set; }
        public Mesh(Vector3[,] controlPoints)
        {
            Triangles = new List<Triangle>();
            Bcolor = Brushes.Green;
            this.controlPoints = controlPoints;
        }

        public void AddTriangle(Triangle triangle)
        {

            Triangles.Add(triangle);
        }

        public void DrawMesh(Graphics g, int drawType, CalculateColorConfiguration colorConfiguration)
        {
            foreach (var triangle in Triangles)
            {
                triangle.DrawTriangle(g, drawType, colorConfiguration);
            }

            if(drawType == 1 || drawType == 0)
                DrawControlPoints(g);
        }

        private void DrawControlPoints(Graphics g)
        {
            int pointSize = 5;

            for (int i = 0; i < controlPoints.GetLength(0); i++)
            {
                for (int j = 0; j < controlPoints.GetLength(1); j++)
                {
                    Vector3 point = controlPoints[i, j];

                    g.FillEllipse(Brushes.Red, point.X - pointSize / 2, point.Y - pointSize / 2, pointSize, pointSize);

                    if (j < controlPoints.GetLength(1) - 1)
                    {
                        Vector3 nextPoint = controlPoints[i, j + 1];
                        g.DrawLine(Pens.Blue, point.X, point.Y, nextPoint.X, nextPoint.Y);
                    }

                    if (i < controlPoints.GetLength(0) - 1)
                    {
                        Vector3 nextPoint = controlPoints[i + 1, j];
                        g.DrawLine(Pens.Blue, point.X, point.Y, nextPoint.X, nextPoint.Y);
                    }
                }
            }
        }

        public void RotateControlPoints(float alpha, float beta)
        {
            var rotationX = Matrix4x4.CreateRotationX(alpha);
            var rotationY = Matrix4x4.CreateRotationY(beta);

            for (int i = 0; i < controlPoints.GetLength(0); i++)
            {
                for (int j = 0; j < controlPoints.GetLength(1); j++)
                {
                    Vector3 point = controlPoints[i, j];
                    controlPoints[i, j] = RotationAlgorithm.RotateControlPoint(point, alpha,beta);
                }
            }
        }
    }
}

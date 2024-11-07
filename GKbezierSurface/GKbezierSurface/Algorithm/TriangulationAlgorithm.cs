using GKbezierPlain.Geometry;
using System.Diagnostics;
using System.Numerics;

namespace GKbezierPlain.Algorithm
{

    public static class TriangulationAlgorithm
    {
        public static Vector3 CalculateBezierSurfacePoint(double u, double v, Vector3[,] controlPoints)
        {
            int n = controlPoints.GetLength(0) - 1;
            int m = controlPoints.GetLength(1) - 1;
            Vector3 point = new Vector3(0, 0, 0);

            for (int i = 0; i <= n; i++)
            {
                for (int j = 0; j <= m; j++)
                {
                    double B_u = MathHelper.Bernstein(i, n, u);
                    double B_v = MathHelper.Bernstein(j, m, v);
                    point += controlPoints[i, j] * (float)(B_u * B_v);
                }
            }
            return point;
        }
        public static void TriangulateSurface(Vector3[,] controlPoints, Mesh mesh, int steps, float alpha, float beta)
        {
            mesh.Triangles.Clear();
            for (int i = 0; i < steps; i++)
            {
                double u = (double)i / steps;
                double uNext = (double)(i + 1) / steps;

                for (int j = 0; j < steps; j++)
                {
                    double v = (double)j / steps;
                    double vNext = (double)(j + 1) / steps;

                    Vector3 p1 = CalculateBezierSurfacePoint(u, v, controlPoints);
                    Vector3 p2 = CalculateBezierSurfacePoint(uNext, v, controlPoints);
                    Vector3 p3 = CalculateBezierSurfacePoint(u, vNext, controlPoints);
                    Vector3 p4 = CalculateBezierSurfacePoint(uNext, vNext, controlPoints);

                    Vertex v1 = Vertex.SetVertexVectors(p1, u, v, controlPoints);
                    Vertex v2 = Vertex.SetVertexVectors(p2, uNext, v, controlPoints);
                    Vertex v3 = Vertex.SetVertexVectors(p3, u, vNext, controlPoints);
                    Vertex v4 = Vertex.SetVertexVectors(p4, uNext, vNext, controlPoints);

                    RotationAlgorithm.RotateVertex(v1, alpha, beta);
                    RotationAlgorithm.RotateVertex(v2, alpha, beta);
                    RotationAlgorithm.RotateVertex(v3, alpha, beta);
                    RotationAlgorithm.RotateVertex(v4, alpha, beta);

                    var triangle1 = new Triangle(v1, v2, v3);
                    var triangle2 = new Triangle(v2, v4, v3);

                    mesh.AddTriangle(triangle1);
                    mesh.AddTriangle(triangle2);
                }
            }
        }
    }
}
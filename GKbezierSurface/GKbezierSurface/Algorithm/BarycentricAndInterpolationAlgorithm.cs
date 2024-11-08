using GKbezierPlain.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GKbezierSurface.Algorithm
{
    public static class BarycentricAndInterpolationAlgorithm
    {
        public static Vector3 CalculateBarycentricCoordinates(PointF point, Triangle triangle)
        {
            Vector3 p = new Vector3(point.X, point.Y, 0);

            Vector3 a = new Vector3(triangle.Vertex1.PositionRotated.X, triangle.Vertex1.PositionRotated.Y, 0);
            Vector3 b = new Vector3(triangle.Vertex2.PositionRotated.X, triangle.Vertex2.PositionRotated.Y, 0);
            Vector3 c = new Vector3(triangle.Vertex3.PositionRotated.X, triangle.Vertex3.PositionRotated.Y, 0);

            // Calculate the area of the whole triangle using the cross product
            float areaABC = Vector3.Cross(b - a, c - a).Length() / 2;

            // Calculate areas of sub-triangles to get barycentric coordinates
            float areaPBC = Vector3.Cross(b - p, c - p).Length() / 2;
            float areaPCA = Vector3.Cross(c - p, a - p).Length() / 2;

            // Barycentric coordinates as ratios of sub-triangle areas to the main triangle area
            float alpha = areaPBC / areaABC;  // Weight for Vertex1
            float beta = areaPCA / areaABC;   // Weight for Vertex2
            float gamma = 1 - alpha - beta;   // Weight for Vertex3

            return new Vector3(alpha, beta, gamma);
        }

        public static Vector2 InterpolateUV(Triangle triangle, Vector3 barycentricCoords)
        {
            // Interpolate U and V using barycentric coordinates
            float u = (float)(barycentricCoords.X * triangle.Vertex1.OriginU
                            + barycentricCoords.Y * triangle.Vertex2.OriginU
                            + barycentricCoords.Z * triangle.Vertex3.OriginU);

            float v = (float)(barycentricCoords.X * triangle.Vertex1.OriginV
                            + barycentricCoords.Y * triangle.Vertex2.OriginV
                            + barycentricCoords.Z * triangle.Vertex3.OriginV);

            return new Vector2(u, v);
        }

        public static Vector3 InterpolateNormal(Triangle triangle, Vector3 barycentricCoords)
        {
            // Interpolating the normal based on triangle vertex normals
            Vector3 N = barycentricCoords.X * triangle.Vertex1.NormalRotated
                      + barycentricCoords.Y * triangle.Vertex2.NormalRotated
                      + barycentricCoords.Z * triangle.Vertex3.NormalRotated;
            return Vector3.Normalize(N);
        }

        public static float InterpolateZ(Triangle triangle, Vector3 barycentricCoords)
        {
            // Interpolating the Z-coordinate for the point inside the triangle
            float interpolatedZ = barycentricCoords.X * triangle.Vertex1.PositionRotated.Z
                                + barycentricCoords.Y * triangle.Vertex2.PositionRotated.Z
                                + barycentricCoords.Z * triangle.Vertex3.PositionRotated.Z;
            return interpolatedZ;
        }

        public static Vector3 InterpolateTangentU(Triangle triangle, Vector3 barycentricCoords)
        {
            // Interpolating the TangentURotated based on triangle vertex tangents
            Vector3 tangentU = barycentricCoords.X * triangle.Vertex1.TangentURotated
                             + barycentricCoords.Y * triangle.Vertex2.TangentURotated
                             + barycentricCoords.Z * triangle.Vertex3.TangentURotated;
            return Vector3.Normalize(tangentU);
        }

        public static Vector3 InterpolateTangentV(Triangle triangle, Vector3 barycentricCoords)
        {
            // Interpolating the TangentVRotated based on triangle vertex tangents
            Vector3 tangentV = barycentricCoords.X * triangle.Vertex1.TangentVRotated
                             + barycentricCoords.Y * triangle.Vertex2.TangentVRotated
                             + barycentricCoords.Z * triangle.Vertex3.TangentVRotated;
            return Vector3.Normalize(tangentV);
        }
    }
}

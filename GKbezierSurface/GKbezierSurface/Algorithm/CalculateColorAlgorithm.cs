using GKbezierPlain.Algorithm;
using GKbezierPlain.Geometry;
using GKbezierSurface.AlgorithmConfigurations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GKbezierSurface.Algorithm
{
    public static class CalculateColorAlgorithm
    {
        
        public static Color GetLambertColor(PointF point, CalculateColorConfiguration colorConfiguration, Triangle triangle)
        {
            var a = triangle.Vertex1.NormalRotated;
            // calculate N and interpolated Z
            Vector3 barycentricCoords = CalculateBarycentricCoordinates(point, triangle);

            Vector3 N = InterpolateNormal(triangle, barycentricCoords);
            float interpolatedZ = InterpolateZ(triangle, barycentricCoords);
            
            Vector3 point3D = new Vector3(point.X, point.Y, interpolatedZ);

            Vector3 L = point3D - new Vector3(0, 0, colorConfiguration.Z);
            if (L.LengthSquared() < 1e-6f)
            {
                L = new Vector3(0, 0, colorConfiguration.Z);
            }
            else
            {
                L = Vector3.Normalize(L);
            }

            Vector3 IO = colorConfiguration.ObjectColor;
            Vector3 IL = colorConfiguration.LightColor;
            float kd = colorConfiguration.Kd;
            float ks = colorConfiguration.Ks;
            int m = colorConfiguration.M;

            return CalculateColor(N, L, IO, IL, kd, ks, m);
        }

        private static Vector3 CalculateBarycentricCoordinates(PointF point, Triangle triangle)
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

        private static Vector3 InterpolateNormal(Triangle triangle, Vector3 barycentricCoords)
        {
            // Interpolating the normal based on triangle vertex normals
            Vector3 N = barycentricCoords.X * triangle.Vertex1.NormalRotated
                      + barycentricCoords.Y * triangle.Vertex2.NormalRotated
                      + barycentricCoords.Z * triangle.Vertex3.NormalRotated;
            return Vector3.Normalize(N);
        }

        private static float InterpolateZ(Triangle triangle, Vector3 barycentricCoords)
        {
            // Interpolating the Z-coordinate for the point inside the triangle
            float interpolatedZ = barycentricCoords.X * triangle.Vertex1.PositionRotated.Z
                                + barycentricCoords.Y * triangle.Vertex2.PositionRotated.Z
                                + barycentricCoords.Z * triangle.Vertex3.PositionRotated.Z;
            return interpolatedZ;
        }
        private static Color CalculateColor(Vector3 N, Vector3 L, Vector3 IO, Vector3 IL, float kd, float ks, int m)
        {
            // Normalize vectors
            N = Vector3.Normalize(N);
            L = Vector3.Normalize(L);
            Vector3 V = new Vector3(0, 0, 1); // Camera vector
            V = Vector3.Normalize(V);

            // Calculate dot products (cosines)
            float cosNL = Math.Max(Vector3.Dot(N, L), 0);
            if(cosNL < 0)
                cosNL = 0;
            Vector3 R = 2 * cosNL * N - L; // Reflect L about N
            R = Vector3.Normalize(R);
            float cosVR = Math.Max(Vector3.Dot(V, R), 0);
            if(cosVR < 0)
                cosVR = 0;
            // Calculate each RGB component separately
            float r = kd * IL.X * IO.X * cosNL + ks * IL.X * IO.X * (float)Math.Pow(cosVR, m);
            float g = kd * IL.Y * IO.Y * cosNL + ks * IL.Y * IO.Y * (float)Math.Pow(cosVR, m);
            float b = kd * IL.Z * IO.Z * cosNL + ks * IL.Z * IO.Z * (float)Math.Pow(cosVR, m);

            // Clamp and convert to 0-255 range
            r = MathHelper.Clamp(r * 255, 0, 255);
            g = MathHelper.Clamp(g * 255, 0, 255);
            b = MathHelper.Clamp(b * 255, 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
        
    }
}

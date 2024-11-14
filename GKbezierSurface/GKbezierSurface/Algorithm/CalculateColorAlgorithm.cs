using GKbezierPlain.Algorithm;
using GKbezierPlain.Geometry;
using GKbezierSurface.AlgorithmConfigurations;
using System;
using System.Drawing;
using System.Numerics;

namespace GKbezierSurface.Algorithm
{
   using static BarycentricAndInterpolationAlgorithm;

    public static class CalculateColorAlgorithm
    {
        private static Matrix4x4 GetTransformationMatrix(Vector3 tangentU, Vector3 tangentV, Vector3 normal)
        {
            Matrix4x4 rotationMatrix = new Matrix4x4(
                tangentU.X, tangentV.X, normal.X, 0,
                tangentU.Y, tangentV.Y, normal.Y, 0,
                tangentU.Z, tangentV.Z, normal.Z, 0,
                0, 0, 0, 0
            );

            return rotationMatrix;
        }
        private static Vector3 lightAnimation(int z)
        {
            float radius = 100.0f;
            float speed = 0.1f;

            float x = radius * (float)Math.Cos(z * speed);
            float y = radius * (float)Math.Sin(z * speed);
            float lightZ = 50.0f;

            return new Vector3(x, y, lightZ);
        }

        public static Color GetLambertColor(PointF point, CalculateColorConfiguration colorConfiguration, Triangle triangle)
        {
            Vector3 barycentricCoords = CalculateBarycentricCoordinates(point, triangle);
            Vector3 Nsurface = InterpolateNormal(triangle, barycentricCoords); //experimental

            Vector3 N;
            if (colorConfiguration.IsNormalMapMode)
            {
                Vector2 uv = InterpolateUV(triangle, barycentricCoords);
                Vector3 Ntexture = colorConfiguration.NormalMapHelper.GetNormalAtUV(uv.X, uv.Y);

                //Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(Nsurface, (float)Math.PI / 2);
                Matrix4x4 customRotationMatrix = GetTransformationMatrix(InterpolateTangentU(triangle,barycentricCoords), 
                   InterpolateTangentV(triangle,barycentricCoords), Nsurface);
                //N = Vector3.Transform(Ntexture, rotationMatrix); // Adjust N with rotation based on surface normal
                N = Vector3.Transform(Ntexture, customRotationMatrix);
            }
            else
            {
                N = Nsurface;
            }

            float interpolatedZ = InterpolateZ(triangle, barycentricCoords);

            Vector3 point3D = new Vector3(point.X, point.Y, interpolatedZ);

            Vector3 lightSource = lightAnimation(colorConfiguration.Z);

            Vector3 L = point3D - lightSource;
            if (L.LengthSquared() < 1e-6f)
            {
                L = new Vector3(0, 0, colorConfiguration.Z);
            }
            else
            {
                L = Vector3.Normalize(L);
            }

            Vector3 IO;

            if (colorConfiguration.IsTextureMode == false)
            {
                IO = colorConfiguration.ObjectColor;
            }
            else
            {
                Vector2 uv = InterpolateUV(triangle, barycentricCoords);
                Color textureColor = colorConfiguration.TextureHelper.GetColorAtUV(uv.X, uv.Y);
                IO = new Vector3(textureColor.R, textureColor.G, textureColor.B) / 255;
            }

            Vector3 IL = colorConfiguration.LightColor;
            float kd = colorConfiguration.Kd;
            float ks = colorConfiguration.Ks;
            int m = colorConfiguration.M;

            return CalculateColor(N, L, IO, IL, kd, ks, m);
        }

        private static Color CalculateColor(Vector3 N, Vector3 L, Vector3 IO, Vector3 IL, float kd, float ks, int m)
        {
            N = Vector3.Normalize(N);
            L = Vector3.Normalize(L);
            Vector3 V = new Vector3(0, 0, 1); 
            V = Vector3.Normalize(V);

            float cosNL = Math.Max(Vector3.Dot(N, L), 0);
            if (cosNL < 0)
                cosNL = 0;
            Vector3 R = 2 * cosNL * N - L; 
            R = Vector3.Normalize(R);
            float cosVR = Math.Max(Vector3.Dot(V, R), 0);
            if (cosVR < 0)
                cosVR = 0;

            float r = kd * IL.X * IO.X * cosNL + ks * IL.X * IO.X * (float)Math.Pow(cosVR, m);
            float g = kd * IL.Y * IO.Y * cosNL + ks * IL.Y * IO.Y * (float)Math.Pow(cosVR, m);
            float b = kd * IL.Z * IO.Z * cosNL + ks * IL.Z * IO.Z * (float)Math.Pow(cosVR, m);

            r = MathHelper.Clamp(r * 255, 0, 255);
            g = MathHelper.Clamp(g * 255, 0, 255);
            b = MathHelper.Clamp(b * 255, 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }

    }
}

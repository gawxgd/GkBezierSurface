using GKbezierPlain.Algorithm;
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
        public static Color CalculateColor(Vector3 N, Vector3 L, Vector3 IO, Vector3 IL, float kd, float ks, int m)
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

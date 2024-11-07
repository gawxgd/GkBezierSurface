using GKbezierPlain.Geometry;
using GKbezierSurface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace GKbezierPlain.Algorithm
{

    public static class MathHelper
    {
        public static int[,] binomialCoefficients = new int[2, MainForm.GRID_COUNT];

        public static void PrecomputeBinomialCoefficients()
        {
            int n = MainForm.GRID_COUNT - 1;
            for (int i = 0; i <= n; i++)
            {
                binomialCoefficients[0, i] = ComputeBinomialCoefficient(n, i);
            }

            n = MainForm.GRID_COUNT - 2;
            for (int i = 0; i <= n; i++)
            {
                binomialCoefficients[1, i] = ComputeBinomialCoefficient(n, i);
            }
        }

        public static int ComputeBinomialCoefficient(int n, int k)
        {
            if (k > n) return 0;
            if (k == 0 || k == n) return 1;

            int result = 1;
            for (int i = 1; i <= k; i++)
            {
                result *= (n - (k - i));
                result /= i;
            }
            return result;
        }

        public static double Bernstein(int i, int n, double param)
        {
            var precomputedCoefficents = binomialCoefficients;
            int coefficent = int.MaxValue;

            if (n == MainForm.GRID_COUNT - 1)
            {
                coefficent = precomputedCoefficents[0, i];
            }
            else if (n == MainForm.GRID_COUNT - 2)
            {
                coefficent = precomputedCoefficents[1, i];
            }

            if (coefficent == int.MaxValue)
            {
                MessageBox.Show("error coefficent");
                return 0;
            }
            return coefficent * Math.Pow(param, i) * Math.Pow(1 - param, n - i);
        }

        public static Vector3 CalculateTangentU(double u, double v, Vector3[,] controlPoints)
        {
            int nMinus1 = controlPoints.GetLength(0) - 2;
            int m = controlPoints.GetLength(1) - 1;
            Vector3 tangentU = new Vector3(0, 0, 0);

            for (int i = 0; i <= nMinus1; i++)
            {
                for (int j = 0; j <= m; j++)
                {
                    double B_u = Bernstein(i, nMinus1, u);
                    double B_v = Bernstein(j, m, v);

                    tangentU += (controlPoints[i + 1, j] - controlPoints[i, j]) * (float)(B_u * B_v);
                }
            }

            return tangentU * nMinus1;
        }
        public static Vector3 CalculateTangentV(double u, double v, Vector3[,] controlPoints)
        {
            int n = controlPoints.GetLength(0) - 1;
            int mMinus1 = controlPoints.GetLength(1) - 2;
            Vector3 tangentV = new Vector3(0, 0, 0);

            for (int i = 0; i <= n; i++)
            {
                for (int j = 0; j <= mMinus1; j++)
                {
                    double B_u = Bernstein(i, n, u);
                    double B_v = Bernstein(j, mMinus1, v);

                    tangentV += (controlPoints[i, j + 1] - controlPoints[i, j]) * (float)(B_u * B_v);
                }
            }

            return mMinus1 * tangentV;
        }
        public static Vector3 CalculateNormalVector(Vector3 tangentU, Vector3 tangentV)
        {
            return Vector3.Cross(tangentU, tangentV);
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
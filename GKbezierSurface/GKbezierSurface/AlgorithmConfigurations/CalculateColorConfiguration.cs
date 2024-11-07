using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKbezierSurface.AlgorithmConfigurations
{
    public class CalculateColorConfiguration
    {
        private float _kd;
        public float Kd { get => _kd; set => _kd =  value / 100.0f; }

        private float _ks;
        public float Ks { get => _ks; set => _ks = value / 100.0f; }

        public int M { get; set; }

        public Color LightColor { get; set; }
        public CalculateColorConfiguration(float kd, float ks, int m, Color lightColor)
        {
            Kd = kd;
            Ks = ks;
            M = m;
            LightColor = lightColor;
        }
    }
}

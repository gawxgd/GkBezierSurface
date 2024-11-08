using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
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

        public Vector3 LightColor { get; set; }

        public Vector3 ObjectColor { get; set; }

        public int Z { get; set; }

        public TextureHelper TextureHelper { get; set; }

        public bool IsTextureMode { get; set; }
        public bool IsNormalMapMode { get; set; }
        public NormalMapHelper NormalMapHelper { get; set; }

        public CalculateColorConfiguration(float kd, float ks, int m, 
            Color lightColor, Color objectColor, 
            int z, TextureHelper textureHelper, 
            bool isTextureMode, NormalMapHelper normalMapHelper, 
            bool isNormalMapMode)
        {
            Kd = kd;
            Ks = ks;
            M = m;
            LightColor = new Vector3(lightColor.R, lightColor.G, lightColor.B) / 255;
            ObjectColor = new Vector3(objectColor.R, objectColor.G, objectColor.B) / 255;
            Z = z;

            TextureHelper = textureHelper;
            IsTextureMode = isTextureMode;
            
            IsNormalMapMode = isNormalMapMode;
            NormalMapHelper = normalMapHelper;
        }
    }
}

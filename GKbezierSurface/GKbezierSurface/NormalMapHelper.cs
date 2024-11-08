using GKbezierPlain.Algorithm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GKbezierSurface
{
    public class NormalMapHelper
    {
        private Bitmap normalMap;

        public void LoadNormalMap()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif|All Files|*.*",
                Title = "Select a Texture File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                normalMap = new Bitmap(openFileDialog.FileName);
            }
        }

        public Vector3 GetNormalAtUV(float u, float v)
        {
            if (normalMap == null) return new Vector3(0, 0, 1); // Default normal

            int x = (int)(MathHelper.Clamp(u, 0, 1) * (normalMap.Width - 1));
            int y = (int)(MathHelper.Clamp(v, 0, 1) * (normalMap.Height - 1));

            Color color = normalMap.GetPixel(x, y);

            float nx = (color.R / 255.0f) * 2 - 1; // Map R to [-1, 1]
            float ny = (color.G / 255.0f) * 2 - 1; // Map G to [-1, 1]
            float nz = color.B / 255.0f;           // Map B to [0, 1]

            return Vector3.Normalize(new Vector3(nx, ny, nz));
        }


    }
}

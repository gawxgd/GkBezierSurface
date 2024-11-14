using GKbezierPlain.Algorithm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        private string GetSolutionDirectory()
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;

            while (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
            {
                if (Directory.GetFiles(directory, "*.sln").Length > 0)
                {
                    return Path.Combine(directory, "MapsAndTextures");
                }
                directory = Directory.GetParent(directory)?.FullName;
            }

            return null;
        }

        public void LoadDeafault()
        {
            var path = Path.Combine(Application.StartupPath, "Resources", "normal.jpg");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The texture file was not found.", path);
            }
            normalMap = new Bitmap(path);
        }

        public void LoadNormalMap()
        {
            string solutionDirectory = GetSolutionDirectory();
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = solutionDirectory,
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
            if (normalMap == null) return new Vector3(0, 0, 1); 

            int x = (int)(MathHelper.Clamp(u, 0, 1) * (normalMap.Width - 1));
            int y = (int)(MathHelper.Clamp(v, 0, 1) * (normalMap.Height - 1));

            Color color = normalMap.GetPixel(x, y);

            float nx = (color.R / 255.0f) * 2 - 1; 
            float ny = (color.G / 255.0f) * 2 - 1; 
            float nz = color.B / 255.0f;           

            return Vector3.Normalize(new Vector3(nx, ny, nz));
        }


    }
}

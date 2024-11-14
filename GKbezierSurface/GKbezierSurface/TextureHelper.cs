

using GKbezierPlain.Algorithm;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GKbezierSurface
{
    public class TextureHelper
    {
        private Bitmap texture;

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
            var path = Path.Combine(Application.StartupPath, "Resources", "texture.jpg");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The texture file was not found.", path);
            }
            texture = new Bitmap(path);
        }

        public void LoadTexture()
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
                texture = new Bitmap(openFileDialog.FileName);
            }
        }

        public Color GetColorAtUV(float u, float v)
        {
            if(texture == null)
            {
                return Color.White;
            }
            
            int x = (int)(MathHelper.Clamp(u, 0, 1) * (texture.Width - 1));
            int y = (int)(MathHelper.Clamp(v, 0, 1) * (texture.Height - 1));
            var pixel = texture.GetPixel(x, y);

            return texture.GetPixel(x, y);
        }
    }
}

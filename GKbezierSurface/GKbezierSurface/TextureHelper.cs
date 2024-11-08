﻿

using GKbezierPlain.Algorithm;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GKbezierSurface
{
    public class TextureHelper
    {
        private Bitmap texture;

        public void LoadTexture()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
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

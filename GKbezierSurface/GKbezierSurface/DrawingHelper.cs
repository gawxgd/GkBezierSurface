using GKbezierPlain.Geometry;
using GKbezierSurface.AlgorithmConfigurations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GKbezierSurface
{
    public class DrawingHelper
    {
        private PictureBox pBox;
        private Bitmap bufferBitmap;

        public DrawingHelper(PictureBox pictureBox)
        {
            this.pBox = pictureBox;

            var width = pBox.ClientRectangle.Width;
            var height = pBox.ClientRectangle.Height;

            bufferBitmap = new Bitmap(width, height);
            ClearBuffer();

            var tempBitmap = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(tempBitmap))
            {
                graphics.Clear(Color.Black);
            }

            pBox.Image = tempBitmap;
            pBox.Refresh();
        }

        private void DrawBuffer()
        {
            //(bufferBitmap, pictureBox.Image) = (pictureBox.Image as Bitmap, bufferBitmap);
            var temp = bufferBitmap;
            bufferBitmap = pBox.Image as Bitmap;
            pBox.Image = temp;
        }

        public void Dispose()
        {
            bufferBitmap.Dispose();
            pBox.Image?.Dispose();
        }

        private void ClearBuffer()
        {
            using (var gBuffer = Graphics.FromImage(bufferBitmap))
            {
                gBuffer.Clear(Color.White);
            }
        }
        

        public void Draw(Mesh mesh, int drawType, CalculateColorConfiguration colorConfiguration)
        {
            using (var graphics = Graphics.FromImage(bufferBitmap))
            {
                graphics.ScaleTransform(1, -1);
                graphics.TranslateTransform(bufferBitmap.Width / 2.0f, -bufferBitmap.Height / 2.0f);
                
                graphics.Clear(Color.White);

                mesh.DrawMesh(graphics, drawType, colorConfiguration);
            }

            DrawBuffer();
            ClearBuffer();
            pBox.Refresh();
        }
    }
}
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GKbezierPlain.Geometry
{
    public class Mesh
    {
        public List<Triangle> Triangles { get; private set; }
        public Brush Bcolor { get; set; }
        public Mesh()
        {
            Triangles = new List<Triangle>();
            Bcolor = Brushes.Green;
        }

        public void AddTriangle(Triangle triangle)
        {

            Triangles.Add(triangle);
        }

        public void DrawMesh(Graphics g, int drawType)
        {
            //g.Clear(Color.White);

            foreach (var triangle in Triangles)
            {
                triangle.DrawTriangle(g, drawType, Bcolor);
            }
        }

        public void RotateMesh(float alpha, float beta, int drawType)
        {
            foreach (var triangle in Triangles)
            {
                triangle.RotateTriangle(alpha, beta);
            }
        }
    }
}

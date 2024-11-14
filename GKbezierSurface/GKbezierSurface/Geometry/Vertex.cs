
using System.Drawing;
using System.Numerics;

using GKbezierPlain.Algorithm;

namespace GKbezierPlain.Geometry
{
    public class Vertex
    {
        public Vector3 Position { get; set; } 
        public Vector3 TangentU { get; set; } 
        public Vector3 TangentV { get; set; } 
        public Vector3 Normal { get; set; }   

        public Vector3 PositionRotated { get; set; } 
        public Vector3 TangentURotated { get; set; } 
        public Vector3 TangentVRotated { get; set; } 
        public Vector3 NormalRotated { get; set; }   

        public double OriginU { get; set; }
        public double OriginV { get; set; }

        public Vertex(Vector3 position)
        {
            Position = position;
        }

        public Vertex(Vector3 position, double originU, double originV)
        {
            Position = position;
            OriginU = originU;
            OriginV = originV;
        }
        public Vertex(Vector3 position, double u, double v, Vector3 tangentU, Vector3 tangentV, Vector3 normal)
        {
            Position = position;
            TangentU = tangentU;
            TangentV = tangentV;
            Normal = normal;

            OriginU = u;
            OriginV = v;

            PositionRotated = Position;
            TangentURotated = TangentU;
            TangentVRotated = TangentV;
            NormalRotated = Normal;
        }
        public static Vertex SetVertexVectors(Vector3 position, double u, double v, Vector3[,] controlPoints)
        {
            var tangentU = MathHelper.CalculateTangentU(u, v, controlPoints);
            var tangentV = MathHelper.CalculateTangentV(u, v, controlPoints);
            var normal = MathHelper.CalculateNormalVector(tangentU, tangentV);

            return new Vertex(position, u, v, tangentU, tangentV, normal);
        }
        public void DrawVertex2D(Graphics g, Brush color, int size = 14)
        {
            int x = (int)(Position.X - size / 2);
            int y = (int)(Position.Y - size / 2);

            g.FillEllipse(color, x, y, size, size);
        }
    }
}

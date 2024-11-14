using GKbezierPlain.Geometry;
using System.Numerics;

namespace GKbezierPlain.Algorithm
{
    public static class RotationAlgorithm
    {
        public static void RotateVertex(Vertex vertex, float alpha, float beta)
        {

            Quaternion rotationZ = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, alpha);
            Quaternion rotationX = Quaternion.CreateFromAxisAngle(Vector3.UnitX, beta);

            Quaternion rotation = Quaternion.Concatenate(rotationZ, rotationX);

            vertex.PositionRotated = Vector3.Transform(vertex.Position, rotation);
            vertex.TangentURotated = Vector3.Transform(vertex.TangentU, rotationZ);
            vertex.TangentVRotated = Vector3.Transform(vertex.TangentV, rotation);
            vertex.NormalRotated = Vector3.Transform(vertex.Normal, rotation);
        }
    }
}
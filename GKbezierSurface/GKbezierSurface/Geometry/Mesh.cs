using GKbezierPlain.Algorithm;
using GKbezierSurface.AlgorithmConfigurations;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace GKbezierPlain.Geometry
{
	public class Mesh
	{
		private Vector3[,] controlPoints;
		public List<Triangle> Triangles { get; private set; }

		public List<Triangle> Tetrahedron { get; private set; }

		public List<Vertex> TetrahedronVertices { get; private set; }

		public (Vertex v0, Vertex v1, Vertex v2) CalculateTangentSpace(Vertex v0, Vertex v1, Vertex v2)
		{
			Vector3 edge1 = v1.Position - v0.Position;
			Vector3 edge2 = v2.Position - v0.Position;

			float deltaU1 = (float)(v1.OriginU - v0.OriginU);
			float deltaV1 = (float)(v1.OriginV - v0.OriginV);
			float deltaU2 = (float)(v2.OriginU - v0.OriginU);
			float deltaV2 = (float)(v2.OriginV - v0.OriginV);

			float f = 1.0f / (deltaU1 * deltaV2 - deltaU2 * deltaV1);

			Vector3 tangentU = f * (deltaV2 * edge1 - deltaV1 * edge2);
			Vector3 tangentV = f * (-deltaU2 * edge1 + deltaU1 * edge2);


			Vector3 normal = Vector3.Normalize(Vector3.Cross(edge1, edge2));
			tangentU = Vector3.Normalize(tangentU);
			tangentV = Vector3.Normalize(tangentV);

			v0.TangentU = tangentU;
			v0.TangentV = tangentV;
			v0.Normal = normal;
			v0.NormalRotated = normal;
			v0.TangentURotated = tangentU;
			v0.TangentVRotated = tangentV;

			v1.TangentU = tangentU;
			v1.TangentV = tangentV;
			v1.Normal = normal;
			v1.NormalRotated = normal;
			v1.TangentURotated = tangentU;
			v1.TangentVRotated = tangentV;

			v2.TangentU = tangentU;
			v2.TangentV = tangentV;
			v2.Normal = normal;
			v2.NormalRotated = normal;
			v2.TangentURotated = tangentU;
			v2.TangentVRotated = tangentV;

			return (v0, v1, v2);
		}
		public Brush Bcolor { get; set; }
		public Mesh(Vector3[,] controlPoints)
		{
			Triangles = new List<Triangle>();
			Bcolor = Brushes.Green;
			this.controlPoints = controlPoints;
			Tetrahedron = new List<Triangle>();
			TetrahedronVertices = new List<Vertex>();

			Vector3 v0 = new Vector3(0, 0, 0);
			Vector3 v1 = new Vector3(200, 0, 0);
			Vector3 v2 = new Vector3(100, 172, 0);
			Vector3 v3 = new Vector3(100, 56, 160);
			Vertex V0 = new Vertex(v0, 0.0, 0.0);
			V0.PositionRotated = v0;
			Vertex V1 = new Vertex(v1, 1.0, 0.0);
			V1.PositionRotated = v1;
			Vertex V2 = new Vertex(v2, 0.5, 1.0);
			V2.PositionRotated = v2;
			Vertex V3 = new Vertex(v3, 0.5, 0.5);
			V3.PositionRotated = v3;

			Vertex V02 = new Vertex(v0, 0.0, 0.0);
			V02.PositionRotated = v0;
			Vertex V03 = new Vertex(v0, 0.0, 0.0);
			V03.PositionRotated = v0;

			Vertex V12 = new Vertex(v1, 1.0, 0.0);
			V12.PositionRotated = v1;
			Vertex V13 = new Vertex(v1, 1.0, 0.0);
			V13.PositionRotated = v1;

			Vertex V22 = new Vertex(v2, 0.5, 1.0);
			V22.PositionRotated = v2;
			Vertex V23 = new Vertex(v2, 0.5, 1.0);
			V23.PositionRotated = v2;

			Vertex V32 = new Vertex(v3, 0.5, 0.5);
			V32.PositionRotated = v3;
			Vertex V33 = new Vertex(v3, 0.5, 0.5);
			V33.PositionRotated = v3;

			(V0, V1, V2) = CalculateTangentSpace(V0, V1, V2);
			(V02, V12, V3) = CalculateTangentSpace(V02, V12, V3);
			(V03, V22, V32) = CalculateTangentSpace(V03, V22, V32);
			(V13, V23, V33) = CalculateTangentSpace(V13, V23, V33);

			TetrahedronVertices.Add(V0);
			TetrahedronVertices.Add(V02);
			TetrahedronVertices.Add(V03);

			TetrahedronVertices.Add(V1);
			TetrahedronVertices.Add(V12);
			TetrahedronVertices.Add(V13);

			TetrahedronVertices.Add(V2);
			TetrahedronVertices.Add(V22);
			TetrahedronVertices.Add(V23);

			TetrahedronVertices.Add(V3);
			TetrahedronVertices.Add(V32);
			TetrahedronVertices.Add(V33);

			AddTetrahedronWall(new Triangle(V0, V1, V2));
			AddTetrahedronWall(new Triangle(V02, V12, V3));
			AddTetrahedronWall(new Triangle(V03, V22, V32));
			AddTetrahedronWall(new Triangle(V13, V23, V33));

		}

		public void AddTriangle(Triangle triangle)
		{

			Triangles.Add(triangle);
		}

		public void AddTetrahedronWall(Triangle triangle)
		{
			Tetrahedron.Add(triangle);
		}

		public void DrawMesh(Graphics g, int drawType, CalculateColorConfiguration colorConfiguration)
		{
			foreach (var triangle in Triangles)
			{
				triangle.DrawTriangle(g, drawType, colorConfiguration);
			}

			if (drawType == 1 || drawType == 0)
				DrawControlPoints(g);
		}

		private void DrawControlPoints(Graphics g)
		{
			int pointSize = 5;

			for (int i = 0; i < controlPoints.GetLength(0); i++)
			{
				for (int j = 0; j < controlPoints.GetLength(1); j++)
				{
					Vector3 point = controlPoints[i, j];

					g.FillEllipse(Brushes.Red, point.X - pointSize / 2, point.Y - pointSize / 2, pointSize, pointSize);

					if (j < controlPoints.GetLength(1) - 1)
					{
						Vector3 nextPoint = controlPoints[i, j + 1];
						g.DrawLine(Pens.Blue, point.X, point.Y, nextPoint.X, nextPoint.Y);
					}

					if (i < controlPoints.GetLength(0) - 1)
					{
						Vector3 nextPoint = controlPoints[i + 1, j];
						g.DrawLine(Pens.Blue, point.X, point.Y, nextPoint.X, nextPoint.Y);
					}
				}
			}
		}

		public void RotateControlPoints(float alpha, float beta)
		{
			var rotationX = Matrix4x4.CreateRotationX(alpha);
			var rotationY = Matrix4x4.CreateRotationY(beta);

			for (int i = 0; i < controlPoints.GetLength(0); i++)
			{
				for (int j = 0; j < controlPoints.GetLength(1); j++)
				{
					Vector3 point = controlPoints[i, j];
					controlPoints[i, j] = RotationAlgorithm.RotateControlPoint(point, alpha, beta);
				}
			}
		}

		public void RotateTetrahedron(float alpha, float beta)
		{
			var rotationX = Matrix4x4.CreateRotationX(alpha);
			var rotationY = Matrix4x4.CreateRotationY(beta);

			for (int i = 0; i < TetrahedronVertices.Count; i++)
			{
				var position = TetrahedronVertices[i].Position;
				var normal = TetrahedronVertices[i].Normal;
				var tangentU = TetrahedronVertices[i].TangentU;
				var tangentV = TetrahedronVertices[i].TangentV;


				var point = new Vector3(position.X, position.Y, position.Z);
				TetrahedronVertices[i].PositionRotated = RotationAlgorithm.RotateControlPoint(point, alpha, beta);
				TetrahedronVertices[i].NormalRotated = RotationAlgorithm.RotateControlPoint(normal, alpha, beta);
				TetrahedronVertices[i].TangentURotated = RotationAlgorithm.RotateControlPoint(tangentU, alpha, beta);
				TetrahedronVertices[i].TangentVRotated = RotationAlgorithm.RotateControlPoint(tangentV, alpha, beta);

			}
		}
		public void DrawTetrahedronAtOrigin(Graphics g, int drawType, CalculateColorConfiguration colorConfiguration)
		{
			Tetrahedron.Clear();

			var V0 = TetrahedronVertices[0];
			var V02 = TetrahedronVertices[1];
			var V03 = TetrahedronVertices[2];

			var V1 = TetrahedronVertices[3];
			var V12 = TetrahedronVertices[4];
			var V13 = TetrahedronVertices[5];


			var V2 = TetrahedronVertices[6];
			var V22 = TetrahedronVertices[7];
			var V23 = TetrahedronVertices[8];

			var V3 = TetrahedronVertices[9];
			var V32 = TetrahedronVertices[10];
			var V33 = TetrahedronVertices[11];


			AddTetrahedronWall(new Triangle(V0, V1, V2));
			AddTetrahedronWall(new Triangle(V02, V12, V3));
			AddTetrahedronWall(new Triangle(V03, V22, V32));
			AddTetrahedronWall(new Triangle(V13, V23, V33));

			foreach (var triangle in Tetrahedron)
			{
				triangle.DrawTriangle(g, drawType, colorConfiguration);
			}
		}
	}
}

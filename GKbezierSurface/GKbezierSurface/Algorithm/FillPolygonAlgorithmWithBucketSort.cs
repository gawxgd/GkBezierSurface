using GKbezierPlain.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System;
using GKbezierSurface.AlgorithmConfigurations;
using GKbezierSurface.Algorithm;

namespace GKbezierPlain.Algorithm
{
    public class FillPolygonWithBucketSort
    {
        private static (int minY, int maxY) GetYBounds(Vertex[] vertices)
        {
            var minY = vertices.Min(v => (int)v.PositionRotated.Y);
            var maxY = vertices.Max(v => (int)v.PositionRotated.Y);
            return (minY, maxY);
        }
        private static float CalculateDxDy(Vertex start, Vertex end)
        {
            return (float)(end.PositionRotated.X - start.PositionRotated.X) /
                        (end.PositionRotated.Y - start.PositionRotated.Y);
        }
        private static List<EdgeBucket>[] InitializeEdgeTable(int minY, int maxY, Vertex[] vertices)
        {
            var edgeTable = new List<EdgeBucket>[maxY - minY + 1];

            for (var i = 0; i < edgeTable.Length; i++)
            {
                edgeTable[i] = new List<EdgeBucket>();
            }

            for (var i = 0; i < vertices.Length; i++)
            {
                var start = vertices[i];
                var end = vertices[(i + 1) % vertices.Length];

                if (start.PositionRotated.Y > end.PositionRotated.Y)
                {
                    (end, start) = (start, end);
                }

                if (start.PositionRotated.Y != end.PositionRotated.Y)
                {
                   var dxdy = CalculateDxDy(start, end);

                    edgeTable[(int)start.PositionRotated.Y - minY]
                        .Add(new EdgeBucket((int)end.PositionRotated.Y,start.PositionRotated.X, dxdy));
                }
            }

            return edgeTable;
        }

        public static void FillPolygon(Graphics graphics, CalculateColorConfiguration colorConfiguration, Vertex[] vertices, Triangle triangle)
        {
            (var minY, var maxY) = GetYBounds(vertices);

            var edgeTable = InitializeEdgeTable(minY, maxY, vertices);

            var activeEdges = new List<EdgeBucket>();

            for (var y = minY; y <= maxY; y++)
            {
                activeEdges.AddRange(edgeTable[y - minY]);

                activeEdges.RemoveAll(edge => edge.MaxY == y);

                activeEdges.Sort((e1, e2) => e1.currentX.CompareTo(e2.currentX));

                for (var i = 0; i < activeEdges.Count; i += 2)
                {
                    var xStart = (int)Math.Ceiling(activeEdges[i].currentX);
                    var xEnd = (int)Math.Floor(activeEdges[i + 1].currentX);

                    DrawScanLine(graphics, y, xStart, xEnd, colorConfiguration,triangle);
                }

                foreach (var edge in activeEdges)
                {
                    edge.currentX += edge.dxdy;
                }
            }
        }
        private static void DrawScanLine(Graphics g, int y, int startX, int endX, CalculateColorConfiguration colorConfiguration, Triangle triangle)
        {
            for (int x = startX; x <= endX; x++)
            {
                var color = CalculateColorAlgorithm.GetLambertColor(new PointF(x, y), colorConfiguration, triangle);
                Brush cBrush = new SolidBrush(color);
                g.FillRectangle(cBrush, x, y, 1, 1);
            }
        }
        private class EdgeBucket
        {
            public int MaxY { get; set; }
            public float currentX { get; set; }
            public float dxdy { get; set; }

            public EdgeBucket(int maxY, float x, float slope)
            {
                MaxY = maxY;
                currentX = x;
                dxdy = slope;
            }
        }
    }
}
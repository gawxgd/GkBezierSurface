using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GKbezierPlain.Algorithm
{
    public static class FillPolygonAlgorithm
    {
        private static List<List<Edge>> InitializeEdgeTable(PointF[] polygonPoints, out int minY, out int maxY)
        {
            maxY = (int)polygonPoints.Max(point => point.Y);
            minY = (int)polygonPoints.Min(point => point.Y);

            var edgeTable = new List<List<Edge>>(new List<Edge>[maxY - minY + 1]);

            for (int i = 0; i < edgeTable.Count; i++)
                edgeTable[i] = new List<Edge>();

            for (int i = 0; i < polygonPoints.Length; i++)
            {
                PointF start = polygonPoints[i];
                PointF end = polygonPoints[(i + 1) % polygonPoints.Length]; // Wrap around to first point

                if (start.Y == end.Y) continue; // Skip horizontal edges

                if (start.Y > end.Y)
                {
                    var temp = start;
                    start = end;
                    end = temp;
                }

                Edge edge = new Edge(start, end);
                edgeTable[(int)start.Y - minY].Add(edge);
            }

            return edgeTable;
        }

        public static void FillPolygon(Graphics g, PointF[] polygonPoints, Brush color)
        {
            int minY, maxY;
            var edgeTable = InitializeEdgeTable(polygonPoints, out minY, out maxY);
            var activeEdgeTable = new List<Edge>();

            for (int y = minY; y <= maxY; y++)
            {
                activeEdgeTable.AddRange(edgeTable[y - minY]);

                activeEdgeTable.RemoveAll(edge => edge.maxY == y);

                activeEdgeTable = activeEdgeTable.OrderBy(edge => edge.currentX).ToList();

                for (int i = 0; i < activeEdgeTable.Count - 1; i += 2)
                {
                    int startX = (int)Math.Ceiling(activeEdgeTable[i].currentX);
                    int endX = (int)Math.Floor(activeEdgeTable[i + 1].currentX);

                    DrawScanLine(g, y, startX, endX, color);
                }


                foreach (var edge in activeEdgeTable)
                {
                    edge.currentX += edge.dxdy;
                }
            }
        }

        private static void DrawScanLine(Graphics g, int y, int startX, int endX, Brush color)
        {
            for (int x = startX; x <= endX; x++)
            {
                g.FillRectangle(color, x, y, 1, 1);
            }
        }

        private class Edge
        {
            public int maxY;
            public double currentX;
            public double dxdy;

            public Edge(PointF start, PointF end)
            {
                maxY = (int)Math.Round(end.Y);
                currentX = start.X;

                dxdy = (end.X - start.X) / (end.Y - start.Y);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace GKbezierPlain.Algorithm
{
    public static class FillPolygonAlgorithm
    {
        private static List<List<Edge>> InitializeEdgeTable(PointF[] polygonPoints, out int minY, out int maxY)
        {
            maxY = polygonPoints.Max(point => (int)point.Y);
            minY = polygonPoints.Min(point => (int)point.Y);

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
                // Add new edges for this scan line
                activeEdgeTable.AddRange(edgeTable[y - minY]);

                // Remove edges where y reaches maxY
                activeEdgeTable.RemoveAll(edge => edge.maxY == y);

                // Sort by currentX for correct ordering of intersections
                activeEdgeTable.Sort((e1, e2) => e1.currentX.CompareTo(e2.currentX));

                for (int i = 0; i < activeEdgeTable.Count - 1; i += 2)
                {
                    int startX = (int)Math.Ceiling(activeEdgeTable[i].currentX);
                    int endX = (int)Math.Floor(activeEdgeTable[i + 1].currentX);

                    DrawScanLine(g, y, startX, endX, color);
                }

                // Update currentX for each edge in the active edge table
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
            public float currentX;
            public float dxdy;

            public Edge(PointF start, PointF end)
            {
                maxY = (int)Math.Round(end.Y);
                currentX = start.X;
                
                //if(end.X - start.X == 0 || end.Y - start.Y ==0)
                //    dxdy = 0;
                //else
                    dxdy = (float)(end.X - start.X) / (end.Y - start.Y);
            }
        }
    }
}

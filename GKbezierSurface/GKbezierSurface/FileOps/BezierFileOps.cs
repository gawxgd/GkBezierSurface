using GKbezierSurface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Windows.Forms;

namespace GKbezierPlain.FileOps
{
    public class BezierFileOps
    {
        public Vector3[,] ControlPoints { get; private set; }

        // Constructor initializing ControlPoints for a 4x4 grid
        public BezierFileOps()
        {
            ControlPoints = new Vector3[MainForm.GRID_COUNT, MainForm.GRID_COUNT];
        }

        // Method to load control points from a file
        public bool LoadControlPoints(string filePath)
        {
            try
            {
                var controlPointsList = new List<Vector3>();


                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(' ');

                        if (parts.Length == 3)
                        {
                            if (float.TryParse(parts[0], out float x) &&
                                float.TryParse(parts[1], out float y) &&
                                float.TryParse(parts[2], out float z))
                            {
                                controlPointsList.Add(new Vector3(x, y, z));
                            }
                            else
                            {
                                Debug.WriteLine($"Error parsing control point: {line}");
                                return false;
                            }
                        }
                    }
                }


                if (controlPointsList.Count != MainForm.CONTROL_POINTS_COUNT)
                {
                    Debug.WriteLine($"Error: Expected {MainForm.CONTROL_POINTS_COUNT} control points for a 4x4 grid.");
                    return false;
                }

                for (int i = 0; i < MainForm.GRID_COUNT; i++)
                {
                    for (int j = 0; j < MainForm.GRID_COUNT; j++)
                    {
                        ControlPoints[i, j] = controlPointsList[i * MainForm.GRID_COUNT + j];
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading control points: {ex.Message}");
                return false;
            }
        }
    }
}

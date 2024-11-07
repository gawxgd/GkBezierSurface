using GKbezierPlain.Algorithm;
using GKbezierPlain.FileOps;
using GKbezierPlain.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GKbezierSurface
{
    public partial class MainForm : Form
    {
        public const int CONTROL_POINTS_COUNT = 16;
        public const int GRID_COUNT = 4;

        public Mesh mesh = new Mesh();
        private Vector3[,] controlPoints;

        private TrackBar alphaSlider;
        private TrackBar betaSlider;
        private TrackBar triangulationSlider;
        private ComboBox drawTypeComboBox;
        private PictureBox pictureBox;

        private DrawingHelper drawingHelper;

        public MainForm()
        {
            InitializeComponent();
            InitializeComponentCustom();
            SetupFixedSize();
            SetupEventHandlers();
            MathHelper.PrecomputeBinomialCoefficients();
            drawingHelper = new DrawingHelper(pictureBox);

            BezierFileOps bezierOps = new BezierFileOps();
            string filePath = Path.Combine(Application.StartupPath, "Resources", "points.txt");
            bool success = bezierOps.LoadControlPoints(filePath);
            if (success)
            {
                var alpha = alphaSlider.Value;
                var beta = betaSlider.Value;

                controlPoints = bezierOps.ControlPoints;
                foreach(var point in controlPoints)
                {
                    Debug.WriteLine(point);
                }
                TriangulationAlgorithm.TriangulateSurface(controlPoints, mesh, triangulationSlider.Value, alpha, beta);
                drawingHelper.Draw(mesh, drawTypeComboBox.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Failed to load control points.");
            }

        }

        private void OnTriangulationSliderValueChanged(object sender, EventArgs e)
        {
            var alpha = alphaSlider.Value;
            var beta = betaSlider.Value;

            TriangulationAlgorithm.TriangulateSurface(controlPoints, mesh, triangulationSlider.Value, alpha, beta);
            drawingHelper.Draw(mesh, drawTypeComboBox.SelectedIndex);
        }
        
        private void drawComboValueChanged(object sender, EventArgs e)
        {
            drawingHelper.Draw(mesh, drawTypeComboBox.SelectedIndex);
        }

        private void SetupEventHandlers()
        {
            triangulationSlider.ValueChanged += OnTriangulationSliderValueChanged;
            alphaSlider.ValueChanged += OnTriangulationSliderValueChanged;
            betaSlider.ValueChanged += OnTriangulationSliderValueChanged;
            drawTypeComboBox.SelectedValueChanged += drawComboValueChanged;
        }

        private void SetupFixedSize()
        {
            this.ClientSize = new System.Drawing.Size(1000, 800);

            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            this.MaximizeBox = false;

            this.MinimizeBox = true;

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponentCustom()
        {
            this.SuspendLayout();

            // Main Panel to hold canvas and control panel
            Panel mainPanel = new Panel() { Dock = DockStyle.Fill, Padding = new Padding(10) };
            this.Controls.Add(mainPanel);

            pictureBox = new PictureBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Height = 800,
                Width = 700,
                BackColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle,
            };
            mainPanel.Controls.Add(pictureBox);


            // Control Panel (right)
            FlowLayoutPanel controlPanel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Right,
                Width = 250,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                Padding = new Padding(10)
            };
            mainPanel.Controls.Add(controlPanel);

            // Rotation Angles GroupBox
            GroupBox rotationGroup = new GroupBox() { Text = "Rotation Angles", Width = 230 };
            controlPanel.Controls.Add(rotationGroup);

            FlowLayoutPanel rotationPanel = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown };
            rotationGroup.Controls.Add(rotationPanel);

            rotationPanel.Controls.Add(new Label() { Text = "Alpha (°)" });
            alphaSlider = new TrackBar() { Minimum = -45, Maximum = 45, TickFrequency = 1 };
            rotationPanel.Controls.Add(alphaSlider);

            rotationPanel.Controls.Add(new Label() { Text = "Beta (°)" });
            betaSlider = new TrackBar() { Minimum = 0, Maximum = 10, TickFrequency = 1 };
            rotationPanel.Controls.Add(betaSlider);

            // Triangulation GroupBox
            GroupBox triangulationGroup = new GroupBox() { Text = "Triangulation", Width = 230 };
            controlPanel.Controls.Add(triangulationGroup);

            FlowLayoutPanel triangulationPanel = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown };
            triangulationGroup.Controls.Add(triangulationPanel);

            triangulationPanel.Controls.Add(new Label() { Text = "Detail Level" });
            triangulationSlider = new TrackBar() { Minimum = 1, Maximum = 20, TickFrequency = 1 };
            triangulationPanel.Controls.Add(triangulationSlider);

            // Drawing GroupBox
            GroupBox drawingGroup = new GroupBox() { Text = "Drawing", Width = 230 };
            controlPanel.Controls.Add(drawingGroup);

            FlowLayoutPanel drawingPanel = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown };
            drawingGroup.Controls.Add(drawingPanel);

            drawingPanel.Controls.Add(new Label() { Text = "How to draw" });
            drawTypeComboBox = new ComboBox();
            drawTypeComboBox.Items.AddRange(new string[] { "Mesh and Fill", "Only Mesh", "Only Fill" });
            drawTypeComboBox.SelectedIndex = 0;
            drawingPanel.Controls.Add(drawTypeComboBox);

            // Lighting GroupBox
            GroupBox lightingGroup = new GroupBox() { Text = "Lighting", Width = 230 };
            controlPanel.Controls.Add(lightingGroup);

            FlowLayoutPanel lightingPanel = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown };
            lightingGroup.Controls.Add(lightingPanel);

            lightingPanel.Controls.Add(new Label() { Text = "Diffuse Coefficient (kd)" });
            TrackBar kdSlider = new TrackBar() { Minimum = 0, Maximum = 100, TickFrequency = 10 };
            lightingPanel.Controls.Add(kdSlider);

            lightingPanel.Controls.Add(new Label() { Text = "Specular Coefficient (ks)" });
            TrackBar ksSlider = new TrackBar() { Minimum = 0, Maximum = 100, TickFrequency = 10 };
            lightingPanel.Controls.Add(ksSlider);

            lightingPanel.Controls.Add(new Label() { Text = "Reflection Exponent (m)" });
            TrackBar mSlider = new TrackBar() { Minimum = 1, Maximum = 100, TickFrequency = 5 };
            lightingPanel.Controls.Add(mSlider);

            // Colors GroupBox
            GroupBox colorsGroup = new GroupBox() { Text = "Colors", Width = 230 };
            controlPanel.Controls.Add(colorsGroup);

            FlowLayoutPanel colorsPanel = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown };
            colorsGroup.Controls.Add(colorsPanel);

            colorsPanel.Controls.Add(new Label() { Text = "Light Color (IL)" });
            ComboBox lightColorComboBox = new ComboBox();
            lightColorComboBox.Items.AddRange(new string[] { "White", "Red", "Green", "Blue", "Yellow" });
            colorsPanel.Controls.Add(lightColorComboBox);

            colorsPanel.Controls.Add(new Label() { Text = "Object Color (IO)" });
            RadioButton solidColorRadio = new RadioButton() { Text = "Solid Color" };
            RadioButton textureRadio = new RadioButton() { Text = "Texture" };
            colorsPanel.Controls.Add(solidColorRadio);
            colorsPanel.Controls.Add(textureRadio);

            Button loadTextureButton = new Button() { Text = "Load Texture" };
            colorsPanel.Controls.Add(loadTextureButton);

            // Rendering Options GroupBox
            GroupBox renderOptionsGroup = new GroupBox() { Text = "Rendering Options", Width = 230 };
            controlPanel.Controls.Add(renderOptionsGroup);

            FlowLayoutPanel renderOptionsPanel = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown };
            renderOptionsGroup.Controls.Add(renderOptionsPanel);

            renderOptionsPanel.Controls.Add(new CheckBox() { Text = "Draw Wireframe" });
            renderOptionsPanel.Controls.Add(new CheckBox() { Text = "Fill Triangles" });
            renderOptionsPanel.Controls.Add(new CheckBox() { Text = "Enable Normal Map" });
            renderOptionsPanel.Controls.Add(new Button() { Text = "Load Normal Map" });

            // Light Animation GroupBox
            GroupBox lightAnimationGroup = new GroupBox() { Text = "Light Animation", Width = 230 };
            controlPanel.Controls.Add(lightAnimationGroup);

            FlowLayoutPanel lightAnimationPanel = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown };
            lightAnimationGroup.Controls.Add(lightAnimationPanel);

            lightAnimationPanel.Controls.Add(new Label() { Text = "Light Position (Z)" });
            TrackBar lightPositionSlider = new TrackBar() { Minimum = -10, Maximum = 10, TickFrequency = 1 };
            lightAnimationPanel.Controls.Add(lightPositionSlider);

            Button startStopAnimationButton = new Button() { Text = "Start/Stop Animation" };
            lightAnimationPanel.Controls.Add(startStopAnimationButton);

            // Main Form setup
            this.Text = "Bezier Surface Visualization";
            this.ClientSize = new System.Drawing.Size(1000, 800);
            this.ResumeLayout(false);
        }
    }
}

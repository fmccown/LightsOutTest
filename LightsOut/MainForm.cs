using System;
using System.Drawing;
using System.Windows.Forms;

namespace LightsOut
{
    public partial class MainForm : Form
    {
        private const int GridOffset = 25;     // Distance from upper-left side of window
        private int gridLength = 200;           // Size in pixels of grid
        private int numCells = 3;               // Number of cells in grid

        private bool[,] grid;                   // Stores on/off state of cells in grid
        private Random rand;		            // Used to generate random numbers

        private AboutForm aboutBox;             // About dialog box

        public MainForm()
        {
            InitializeComponent();

            rand = new Random();    // Initializes random number generator

            grid = new bool[numCells, numCells];

            // Turn entire grid on
            for (int r = 0; r < numCells; r++)
                for (int c = 0; c < numCells; c++)
                    grid[r, c] = true;

            // Default to 3x3 grid
            x3ToolStripMenuItem.Checked = true;
        }

        private void StartNewGame()
        {
            grid = new bool[numCells, numCells];

            // Fill grid with either white or black
            for (int r = 0; r < numCells; r++)
                for (int c = 0; c < numCells; c++)
                    grid[r, c] = rand.Next(2) == 1;

            // Redraw the grid
            Invalidate();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int cellLength = gridLength / numCells;

            for (int r = 0; r < numCells; r++)
                for (int c = 0; c < numCells; c++)
                {
                    // Get proper pen and brush for on/off grid section
                    Brush brush;
                    Pen pen;

                    if (grid[r, c])
                    {
                        pen = Pens.Black;
                        brush = Brushes.White;	// On
                    }
                    else
                    {
                        pen = Pens.White;
                        brush = Brushes.Black;	// Off
                    }

                    // Determine (x,y) coord of row and col to draw rectangle                    
                    int x = c * cellLength + GridOffset;
                    int y = r * cellLength + GridOffset;

                    // Draw outline and inner rectangle
                    g.DrawRectangle(pen, x, y, cellLength, cellLength);
                    g.FillRectangle(brush, x + 1, y + 1, cellLength - 1, cellLength - 1);
                }

        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            int cellLength = gridLength / numCells;

            // Make sure click was inside the grid
            if (e.X < GridOffset || e.X > cellLength * numCells + GridOffset ||
                e.Y < GridOffset || e.Y > cellLength * numCells + GridOffset)
                return;

            // Find row, col of mouse press
            int r = (e.Y - GridOffset) / cellLength;
            int c = (e.X - GridOffset) / cellLength;

            // Invert selected box and all surrounding boxes
            for (int i = r - 1; i <= r + 1; i++)
                for (int j = c - 1; j <= c + 1; j++)
                    if (i >= 0 && i < numCells && j >= 0 && j < numCells)
                        grid[i, j] = !grid[i, j];

            // Redraw grid
            this.Invalidate();
                        
            // Check to see if puzzle has been solved
            if (PlayerWon())
            {
                // Display winner dialog box just inside window
                MessageBox.Show(this, "Congratulations!  You've won!", "Lights Out!",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);                
            }
        }

        private bool PlayerWon()
        {
            for (int r = 0; r < numCells; r++)
                for (int c = 0; c < numCells; c++)
                    if (grid[r, c])
                        return false;

            return true;
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGameButton_Click(sender, e);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exitButton_Click(sender, e);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (aboutBox == null)
                aboutBox = new AboutForm();
            aboutBox.ShowDialog(this);

        }

        private void x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            numCells = 3;
            x3ToolStripMenuItem.Checked = true;
            x4ToolStripMenuItem.Checked = false;
            x5ToolStripMenuItem.Checked = false;
            StartNewGame();
        }

        private void x4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            numCells = 4;
            x3ToolStripMenuItem.Checked = false;
            x4ToolStripMenuItem.Checked = true;
            x5ToolStripMenuItem.Checked = false;
            StartNewGame();
        }

        private void x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            numCells = 5;
            x3ToolStripMenuItem.Checked = false;
            x4ToolStripMenuItem.Checked = false;
            x5ToolStripMenuItem.Checked = true;
            StartNewGame();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            gridLength = Math.Min(this.Width - (GridOffset * 2) - 10, 
                this.Height - (GridOffset * 2) - 65);
            this.Invalidate();
        }
    }
}

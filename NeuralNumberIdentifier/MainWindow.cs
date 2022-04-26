using NeuralNumberIdentifier.NeuralNetowork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNumberIdentifier
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        const int SIZE = 28;
        Network network;
        double[] input = new double[SIZE * SIZE];
        double[,] colors = new double[SIZE, SIZE];

        bool mouse = false;
        Graphics graphics;

        private void MainWindow_Load(object sender, EventArgs e)
        {
            network = new Network(new int[] { 784, 250, 100, 10 });
            network.LoadWeightsFromFile("weights.txt");
            graphics = pictureBoxPaint.CreateGraphics();
        }

        private void Clear()
        {
            graphics.Clear(Color.Black);
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    colors[i, j] = 0.0;
                    input[i + j * SIZE] = 0.0;
                }
            }
            label1.Text = "";
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void pictureBoxPaint_MouseDown(object sender, MouseEventArgs e)
        {
            mouse = true;
            label1.Text = "";
        }

        private void pictureBoxPaint_MouseUp(object sender, MouseEventArgs e)
        {
            mouse = false;
            int k = 0;
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    input[k] = colors[j, i];
                    k++;
                }
            }
            network.SetInput(input);
            network.ForwardFeed();
            int result = network.GetMaxNeuronIndex(network.Layers - 1);
            label1.Text = result + "";
        }

        private void pictureBoxPaint_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouse)
            {
                int mx = ((int)(e.X / 10)), my = ((int)(e.Y / 10));
                for (int i = 0; i < SIZE; i++)
                {
                    for (int j = 0; j < SIZE; j++)
                    {
                        double dist = (i - mx) * (i - mx) + (j - my) * (j - my);
                        if (dist < 1) dist = 1;
                        dist *= dist;
                        colors[i, j] += (0.2 / dist) * 3;
                        if (colors[i, j] > 1) colors[i, j] = 1.0;
                        if (colors[i, j] < 0.035) colors[i, j] = 0.0;

                        int color = (int)(colors[i, j] * 255);
                        graphics.FillRectangle(new SolidBrush(Color.FromArgb(color, color, color)), i * 10, j * 10, 10, 10);
                    }
                }
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                Clear();
            }
        }
    }
}

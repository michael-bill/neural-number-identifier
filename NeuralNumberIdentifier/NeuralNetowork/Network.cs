using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNumberIdentifier.NeuralNetowork
{
    class Network
    {
        public int Layers { get; set; }
        public Neuron[][] Neurons { get; set; }
        public double[][][] Weights { get; set; }

        public Network(int[] numbersOfNeurons)
        {
            Layers = numbersOfNeurons.Length;
            Neurons = new Neuron[Layers][];
            Weights = new double[Layers - 1][][];
            for (int i  = 0; i < Layers; i++)
            {
                Neurons[i] = new Neuron[numbersOfNeurons[i]];
                for (int j = 0; j < numbersOfNeurons[i]; j++)
                    Neurons[i][j] = new Neuron();
                if (i < Layers - 1)
                {
                    Weights[i] = new double[numbersOfNeurons[i]][];
                    for (int j = 0; j < numbersOfNeurons[i]; j++)
                    {
                        Weights[i][j] = new double[numbersOfNeurons[i + 1]];
                    }
                }
            }
        }

        public void SetRandomWeights()
        {
            Random rnd = new Random();
            for (int i = 0; i < Layers - 1; i++)
            {
                for (int j = 0; j < Neurons[i].Length; j++)
                {
                    for (int k = 0; k < Neurons[i + 1].Length; k++)
                    {
                        Weights[i][j][k] = rnd.Next(-100000, 100000) * 0.00002;
                    }
                }
            }
        }

        public void SetInput(double[] values)
        {
            for (int i = 0; i < Neurons[0].Length; i++)
                Neurons[0][i].Value = values[i];
        }

        public void ForwardFeed()
        {
            for (int k = 1; k < Layers; k++)
            {
                for (int i = 0; i < Neurons[k].Length; i++)
                {
                    Neurons[k][i].Value = 0;
                    for (int j = 0; j < Neurons[k - 1].Length; j++)
                        Neurons[k][i].Value += Neurons[k - 1][j].Value * Weights[k - 1][j][i];
                    Neurons[k][i].Activation();
                }
            }
        }

        public void ForwardFeed(int layerNumber)
        {
            for (int i = 0; i < Neurons[layerNumber].Length; i++)
            {
                Neurons[layerNumber][i].Value = 0;
                for (int j = 0; j < Neurons[layerNumber - 1].Length; j++)
                    Neurons[layerNumber][i].Value += Neurons[layerNumber - 1][j].Value * Weights[layerNumber - 1][j][i];
                Neurons[layerNumber][i].Activation();
            }
        }

        public void BackPropogation(int rightResult, double learningRate)
        {
            // Error counter
            for (int i = 0; i < Neurons[Layers - 1].Length; i++)
            {
                if (rightResult != i)
                    Neurons[Layers - 1][i].Error = -Math.Pow(Neurons[Layers - 1][i].Value, 2);
            }
            Neurons[Layers - 1][rightResult].Error = Math.Pow(Neurons[Layers - 1][rightResult].Value - 1.0, 2);

            for (int i = Layers - 2; i >= 0; i--)
            {
                for (int j = 0; j < Neurons[i].Length; j++)
                {
                    double error = 0.0;
                    for (int k = 0; k < Neurons[i + 1].Length; k++)
                    {
                        error += Neurons[i + 1][k].Error * Weights[i][j][k];
                    }
                    Neurons[i][j].Error = error;
                }
            }

            // Weights update
            for (int i = 0; i < Layers - 1; i++)
            {
                for (int j = 0; j < Neurons[i].Length; j++)
                {
                    for (int k = 0; k < Neurons[i + 1].Length; k++)
                    {
                        Weights[i][j][k] +=
                            learningRate *
                            Neurons[i + 1][k].Error *
                            SigmoidDerivative(Neurons[i + 1][k].Value * Neurons[i][j].Value);
                    }
                }
            }
        }

        public int GetMaxNeuronIndex(int layerNumber)
        {
            double[] values = new double[Neurons[layerNumber].Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = Neurons[layerNumber][i].Value;
            }
            double maxValue = values.Max<double>();
            return Array.IndexOf(values, maxValue);
        }

        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Pow(Math.E, -x));
        }

        public static double SigmoidDerivative(double x)
        {
            if (Math.Abs(x - 1) < 1e-9 || Math.Abs(x) < 1e-9) return 0.0;
            double result = x * (1.0 - x);
            return result;
        }

        public void SaveWeightsToFile(string path)
        {
            string text = "";
            for (int i = 0; i < Layers - 1; i++)
            {
                for (int j = 0; j < Neurons[i].Length; j++)
                {
                    for (int k = 0; k < Neurons[i + 1].Length; k++)
                    {
                        text += Weights[i][j][k] + " ";
                    }
                }
            }
            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter stream = new StreamWriter(file))
                {
                    stream.Write(text);
                }
            }
        }

        public void LoadWeightsFromFile(string path)
        {
            string text = File.ReadAllText(path);
            string[] textWeights = text.Split(' ');
            int c = 0;
            for (int i = 0; i < Layers - 1; i++)
            {
                for (int j = 0; j < Neurons[i].Length; j++)
                {
                    for (int k = 0; k < Neurons[i + 1].Length; k++)
                    {
                        Weights[i][j][k] = Convert.ToDouble(textWeights[c]);
                        c++;
                    }
                }
            }
        }
    }
}

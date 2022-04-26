using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNumberIdentifier.NeuralNetowork
{
    class Neuron
    {
        public double Value { get; set; }
        public double Error { get; set; }

        public void Activation()
        {
            Value = Network.Sigmoid(Value);
        }
    }
}

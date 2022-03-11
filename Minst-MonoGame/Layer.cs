using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Minst_MonoGame
{
    class Layer
    {
        int numberOfInputs;
        int numberOfOutputs;

        public float[] outputs;
        public float[] inputs;
        public float[][] weights;
        public float[][] weightsDelta;
        public float[] gamma;
        public float[] error;
        public float learningRate;

        public Layer(int _numberOfInputs, int _numberOfOutputs, float _learningRate)
        {
            this.numberOfInputs = _numberOfInputs;
            this.numberOfOutputs = _numberOfOutputs;
            learningRate = _learningRate;
            outputs = new float[numberOfOutputs];
            inputs = new float[numberOfInputs];
            //weights = new float[numberOfOutputs, numberOfInputs];
            weights = new float[numberOfOutputs][];
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = new float[numberOfInputs];
            }
            // weightsDelta = new float[numberOfOutputs, numberOfInputs];
            weightsDelta = new float[numberOfOutputs][];
            for (int i = 0; i < weightsDelta.Length; i++)
            {
                weightsDelta[i] = new float[numberOfInputs];
            }
            gamma = new float[numberOfOutputs];
            error = new float[numberOfOutputs];

            InitWeights();
        }

        public void UpdateWeights()
        {
            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weights[i][j] -= weightsDelta[i][j] * learningRate;
                }
            }
        }

        public float TanHDer(float value)
        {
            return 1 - (value * value);
        }

        public float TanH(float value)
        {
            return (float)Math.Tanh(value);
        }

        public float Sig(float value)
        {
            return 1.0f / (1.0f + (float)Math.Exp(-value));
        }

        public float SigDer(float sigmoidValue)
        {
            return (sigmoidValue * (1 - sigmoidValue));
            //return (float)(1 - (Math.Pow(sigmoidValue, 2)));
        }
        
        public void BackPropOutput(float[] expected)
        {
            for (int i = 0; i < numberOfOutputs; i++)
            {
                error[i] = outputs[i] - expected[i];
                //error[i] = error[i] * error[i];
                gamma[i] = error[i] * SigDer(outputs[i]);
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weightsDelta[i][j] = gamma[i] * inputs[j];
                }
            }

           
        }

        public void BackPropHidden(float[] gammaForward, float[][] weightsforward)
        {
            for (int i = 0; i < numberOfOutputs; i++)
            {
                gamma[i] = 0;

                for (int j = 0; j < gammaForward.Length; j++)
                {
                    gamma[i] += gammaForward[j] * weightsforward[j][i];
                }

                gamma[i] *= SigDer(outputs[i]);
            }

            for (int i = 0; i < numberOfOutputs; i++)
            {

                for (int j = 0; j < numberOfInputs; j++)
                //Parallel.ForEach(inputs, (input, state, index) =>
                {
                     weightsDelta[i][j] = gamma[i] * inputs[j];
                }//);
            }
        }

        public void InitWeights()
        {
            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weights[i][j] = (float)ThreadSafeRandom.NextDouble(-0.5, 0.5);
                }
            }
        }

        public float[] FeedForwardLayer(float[] inputs)
        {
            this.inputs = inputs;
            for (int i = 0; i < numberOfOutputs; i++)
            {               
                outputs[i] = 0;
                for (int j = 0; j < numberOfInputs; j++) 
                {
                    outputs[i] += inputs[j] * weights[i][ j];               
                }             
                outputs[i] = Sig(outputs[i]);
            }

            return outputs;
        }



    }
}

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
        public int numberOfInputs;
        public int numberOfOutputs;
        public static OpenCL opclRef = NeuralNet.openCL;
        public float[] outputs;
        public float[] inputs;
        public float[][] weights;
        public float[] weights_flat;
        public float[] weightsDelta_flat;
        public float[][] weightsDelta;
        public float[] gamma;
        public float[] error;
       // public int weights_rows;
       // public int weights_colums;
        public float learningRate;
        public bool outputHasBias;

        public Layer(int _numberOfInputs, int _numberOfOutputs, float _learningRate, bool _hasBias, OpenCL _opclRef)
        {
           // opclRef = _opclRef;
            this.numberOfInputs = _numberOfInputs;
            this.numberOfOutputs = _numberOfOutputs;
            outputHasBias = _hasBias;
            learningRate = _learningRate;
            outputs = new float[numberOfOutputs];
            inputs = new float[numberOfInputs];

         //   weights_rows = numberOfOutputs;
         //   weights_colums = numberOfInputs;
            weights = new float[numberOfOutputs][];
            weights_flat = new float[numberOfInputs * numberOfOutputs];
            weightsDelta_flat = new float[numberOfInputs * numberOfOutputs];
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = new float[numberOfInputs];
            }

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
            //for (int i = 0; i < numberOfOutputs; i++)
            Parallel.ForEach(outputs, (output, state, indexI) =>
            {

                for (int j = 0; j < numberOfInputs; j++)
                {
                        // weights[indexI][j] -= (weightsDelta[indexI][j])* learningRate ;

                    var v = GetElement((int)indexI, j, weights_flat, numberOfInputs);
                    var wd = GetElement((int)indexI, j, weightsDelta_flat, numberOfInputs);
                   // v = v - (weightsDelta[indexI][j] * learningRate);
                    v = v - (wd * learningRate);
                    SetElement((int)indexI, j, v, weights_flat, numberOfInputs);
                }
            });
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
                var err2 = (error[i] * error[i]);
                if (error[i] < 0)
                {
                    err2 *= -1;
                }
                //  error[i] = err2;
                gamma[i] = error[i] * SigDer(outputs[i]);
                for (int j = 0; j < numberOfInputs; j++)
                {
                    //weightsDelta[i][j] = gamma[i] * inputs[j];
                    var v = gamma[i] * inputs[j];
                    SetElement(i, j, v, weightsDelta_flat, numberOfInputs);
                }
            }


        }

        public void BackPropHidden(float[] gammaForward, float[][] weightsforward, float[] weightsforward_flat, int arraywidth)
        {
            // weightsforward_flat
            // weightsDelta_flat
            // gammaForward
            // gamma
            // array width
            // inputs
            // outputs

          //  opclRef.LoadData(weightsforward_flat, weightsDelta_flat, gammaForward,new int[] { gammaForward.Length }, gamma, new int[] {arraywidth}, inputs, new int[] { inputs.Length }, outputs, new int[] {outputs.Length});
         //   opclRef.ExecKernal("BackpropHidden", new int[] { outputs.Length }, weightsDelta_flat.Length);
            Parallel.ForEach(outputs, (input1, state, i) =>
            {
                gamma[i] = 0;

                for (int j = 0; j < gammaForward.Length; j++)
                {
                    
                    gamma[i] += gammaForward[j] * GetElement(j, (int)i, weightsforward_flat, arraywidth);

                }

                gamma[i] *= SigDer(outputs[i]);
            });

         //   var res = opclRef.ExecKernal("BackpropHidden2", new int[] { outputs.Length }, weightsDelta_flat.Length);
       //     weightsDelta_flat = res;
           
            Parallel.ForEach(outputs, (input1, state, indexI) =>
            {

                for (int j = 0; j < numberOfInputs; j++)
                {
                    
                    var v = gamma[indexI] * inputs[j];
                    SetElement((int)indexI, j, v, weightsDelta_flat, numberOfInputs);
                }


            });
        }

        void SetElement(int row, int col, float value, float[] weights_f, int arraywidth)
        {
            weights_f[arraywidth * row + col] = value;
        }

        float GetElement(int row, int col, float[] weights_f, int arraywidth)
        {
            return weights_f[arraywidth * row + col];
        }

        public void InitWeights()
        {


            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    var n = (float)ThreadSafeRandom.NextDouble(-0.4, 0.4);
                   // weights[i][j] = n;
                    SetElement(i, j, n, weights_flat, numberOfInputs);
                }
            }


        }

        public float[] FeedForwardLayer(float[] inputs)
        {
            this.inputs = inputs;

            //for (int i = 0; i < numberOfOutputs; i++)
            Parallel.ForEach(outputs, (output, state, indexI) =>
            {

                outputs[indexI] = 0;
                for (int j = 0; j < numberOfInputs; j++)
                {
                    //  var a = inputs[j] * weights[i, j];
                    var b = inputs[j] * GetElement((int)indexI, j, this.weights_flat, numberOfInputs);
                    //  outputs[i] += a;               
                    outputs[indexI] += b;
                }
                outputs[indexI] = Sig(outputs[indexI]);

            });

            return outputs;
        }



    }
}

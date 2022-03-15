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
        public bool outputHasBias;

        public Layer(int _numberOfInputs, int _numberOfOutputs, float _learningRate, bool _hasBias)
        {
            this.numberOfInputs = _numberOfInputs;
            this.numberOfOutputs = _numberOfOutputs;
            outputHasBias = _hasBias;
            learningRate = _learningRate;
            outputs = new float[numberOfOutputs];
            inputs = new float[numberOfInputs];
            

            weights = new float[numberOfOutputs][];
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
                        weights[indexI][j] -= (weightsDelta[indexI][j])* learningRate ;
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
                    weightsDelta[i][j] = gamma[i] * inputs[j];
                }
            }

           
        }

        public void BackPropHidden(float[] gammaForward, float[][] weightsforward)
        {
            //for (int i = 0; i < numberOfOutputs; i++)
            Parallel.ForEach(outputs, (input1, state, indexI) =>
            {
                gamma[indexI] = 0;

                for (int j = 0; j < gammaForward.Length; j++)
                {
                    gamma[indexI] = gamma[indexI] + (gammaForward[j] * weightsforward[j][indexI]);
                }

                gamma[indexI] *= SigDer(outputs[indexI]);
            });

            //for (int i = 0; i < numberOfOutputs; i++)
            Parallel.ForEach(outputs, (input1, state, indexI) =>
            {

                for (int j = 0; j < numberOfInputs; j++)
                {
                    weightsDelta[indexI][j] = gamma[indexI] * inputs[j];
                }

                /*  Parallel.ForEach(inputs, (input2, state, indexj) =>
                  {
                       weightsDelta[indexI][indexj] = gamma[indexI] * input2;
                  });*/
            });
        }

        public void InitWeights()
        {
            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weights[i][j] = (float)ThreadSafeRandom.NextDouble(-0.4, 0.4);
                }
            }
        }

        public float[] FeedForwardLayer(float[] inputs)
        {
            this.inputs = inputs;

            //for (int i = 0; i < numberOfOutputs; i++)
            Parallel.ForEach(outputs, (output, state, indexI) =>
            {
                if (outputHasBias)
                {
                    if (indexI < outputs.Length-1)
                    {

                        outputs[indexI] = 0;
                        for (int j = 0; j < numberOfInputs; j++)
                        {
                            outputs[indexI] += inputs[j] * weights[indexI][j];
                        }
                        outputs[indexI] = Sig(outputs[indexI]);
                    }
                    else
                    {
                        outputs[indexI] = 1;
                    }

                }
                else
                {
                    outputs[indexI] = 0;
                    for (int j = 0; j < numberOfInputs; j++)
                    {
                        outputs[indexI] += inputs[j] * weights[indexI][j];
                    }
                    outputs[indexI] = Sig(outputs[indexI]);
                }

            });

            return outputs;
        }



    }
}

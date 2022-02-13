using System;
using System.Collections.Generic;
using System.Text;

namespace Minst_MonoGame
{

    class NeuralNet
    {

        int[] layer;
        public Layer[] layers;
        public TestData testData;
        public float[] lastTargets;
        public string lastLabel;
        public long currentGen;
        public List<Extensions.Image> mnistTrainingImages;
        public bool isAutoEncoder = true;
        public float learningRate = 0.03147f;

        public NeuralNet(int[] layer, TestData _testData = null)
        {
            //BUILD NETWORKS INPUTS AND TARGETS FROM MNIST DATA
            // testData = _testData;
            mnistTrainingImages = new List<Extensions.Image>();
            foreach (var image in MnistReader.ReadTrainingData())
            {

                mnistTrainingImages.Add(image);

            }
            //Flatten
            foreach (var item in mnistTrainingImages)
            {
                byte[] flat = new byte[28 * 28 * 4];
                float[] flat_netInputs = new float[28 * 28];
                var count = 0;
                var count_netInputs = 0;

                for (int i = 0; i < 28; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        flat_netInputs[count_netInputs] = ((float)item.Data[i, j] / 255);
                        flat[count] = item.Data[i, j];
                        flat[count + 1] = item.Data[i, j];
                        flat[count + 2] = item.Data[i, j];
                        flat[count + 3] = 255;
                        count += 4;
                        count_netInputs++;
                    }
                }
                item.DataFlat = flat;
                item.DataFlat_NetInputs = flat_netInputs;
            }

            //Build Network
            this.layer = new int[layer.Length];
            for (int i = 0; i < layer.Length; i++)
            {
                this.layer[i] = layer[i];
            }

            layers = new Layer[layer.Length - 1];

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new Layer(layer[i], layer[i + 1], learningRate);
            }

        }

        public Extensions.Image GetRandomTrainingImage()
        {

            var randIndex = ThreadSafeRandom.Next(0, mnistTrainingImages.Count);



            return mnistTrainingImages[randIndex];
        }

        public float[] FeedForwardNetwork(float[] inputs, out float[] errors)
        {
            layers[0].FeedForwardLayer(inputs);
            for (int i = 1; i < layers.Length; i++)
            {
                layers[i].FeedForwardLayer(layers[i - 1].outputs);
            }

            for (int i = 0; i < layers[^1].outputs.Length; i++)
            {
                layers[^1].error[i] = layers[^1].outputs[i] - lastTargets[i];
                //gamma[i] = error[i] * SigDer(outputs[i]);
                //for (int j = 0; j < numberOfInputs; j++)
                // {
                //    weightsDelta[i, j] = gamma[i] * inputs[j];
                // }
            }

            errors = layers[^1].error;
            return layers[layers.Length - 1].outputs;
        }

        public void BackPropNetwork(float[] expected)
        {
            for (int i = layers.Length - 1; i >= 0; i--)
            {
                if (i == layers.Length - 1)
                {
                    layers[i].BackPropOutput(expected);
                }
                else
                {
                    layers[i].BackPropHidden(layers[i + 1].gamma, layers[i + 1].weights);
                }

                layers[i].UpdateWeights();
            }

            //for (int i = 0; i < layers.Length; i++)
            //{
            // }
        }


        public string[] GetNetworkStatus()
        {
            string[] data = new string[10];
            data[0] = "Inputs: ";
            foreach (var input in layers[0].inputs)
            {
                data[0] += input + " ";
            }

            data[1] = "Outputs: ";
            data[6] = "Guess:   ";
            data[6] = "Error:   ";
            float outTotal = 0;
            var count = 0;
            foreach (var output in layers[^1].outputs)
            {
                if (count > 300 && count < 310)
                {
                    data[1] += output.ToString("0.000") + " ";
                }
                outTotal += output;
                count++;

            }
            count = 0;
            foreach (var output in layers[^1].outputs)
            {
                // data[1] += output.ToString("0.000") + " ";
                // outTotal += output;
                if (count > 300 && count < 310)
                {
                    data[6] += ((int)((output / outTotal) * 100)).ToString("00") + "%   ";
                    data[7] += layers[^1].error[count].ToString("0.00  ");
                }
                count++;

            }

            data[2] = "Error: ";
            float counter = 0;
            foreach (var err in layers[^1].error)
            {
                counter += Math.Abs(err);
                //data[1] += err.ToString("0.000") + " ";
            }
            data[2] += counter.ToString("0.000");

            data[3] = "Guess: ";
            for (int j = 0; j < 10; j++)
            {
                if (layers[^1].outputs[j] > 0.9)
                {
                    data[3] += j;
                }
            }
            data[4] = "Output Length: " + layers[^1].outputs.Length;
            data[5] = "Gen: " + currentGen;



            return data;
        }

        public void Train(int count, out Extensions.Image image, out float[] res)
        {
            currentGen++;
            //Get next MnistImage
            image = GetRandomTrainingImage();
            //Set InputsArray
            float[] ins = new float[image.DataFlat_NetInputs.Length];
            image.DataFlat_NetInputs.CopyTo(ins, 0);
            //Set TargetsArray
            //If AutoEncoder

            if (isAutoEncoder)
            {
                lastTargets = new float[image.DataFlat_NetInputs.Length];
                image.DataFlat_NetInputs.CopyTo(lastTargets, 0);
            }
            else //If Classify
            {
                //lastLabel = image.Label.ToString();
                int lastLabel = image.Label;
                lastTargets = new float[10];
                for (int i = 0; i < lastTargets.Length; i++)
                {
                    if (i == lastLabel)
                    {
                        lastTargets[i] = 1;
                    }
                    else
                    {
                        lastTargets[i] = 0;
                    }
                }
            }



            //lastTargets[(int)image.Label] = 1;

            res = FeedForwardNetwork(ins, out float[] errors);



            BackPropNetwork(lastTargets);
            //BackPropNetwork(lastTargets);

            /* for (int i = 0; i < count; i++)
             {
                 currentGen++;
                 float[] ins = testData.NextRandomDataSet(out lastTargets, out lastLabel);
                 //watch.Restart();
                 var res = FeedForwardNetwork(ins, out float[] errors);
                 BackPropNetwork(lastTargets);
                 //watch.Stop();
             }*/
        }
    }
}

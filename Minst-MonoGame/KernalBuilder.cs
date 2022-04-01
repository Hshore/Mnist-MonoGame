using System;
using System.Collections.Generic;
using System.Text;

namespace Minst_MonoGame
{
    class KernalBuilder
    {
        // public static OpenCLTemplate.CLCalc.Program.Kernel new_kernal_CL;
        public static Dictionary<string, OpenCLTemplate.CLCalc.Program.Variable> Args_CL = new Dictionary<string, OpenCLTemplate.CLCalc.Program.Variable>();
        public static int[] netShape;

        public static string BuidlKernal(int[] _netShape, out Dictionary<string, OpenCLTemplate.CLCalc.Program.Variable> _args_CL)
        {
            netShape = _netShape;
            string GetWeightIndexFunction = @" 
                 int GetWeightIndex(int row, int col, int arraywidth)
                 {
                        return arraywidth * row + col;
                 }";
            string feedforwardlayerFunction = @"
                void feedforwardlayer( float * _in, float * _weights, float * _out, int inputLength, int globalid )
                {
                    _out[globalid] = 0;
                    for (int j = 0; j < inputLength; j++)
                         {
                              int index = GetWeightIndex(globalid, j, inputLength);
                              _out[globalid] += (_in[j] * _weights[index]);
                         }
                     float sig = 1.0f / (1.0f + exp(-_out[globalid]));
                     _out[globalid] = sig;
                    
                }";
            string backpropoutputFucntion = @"
                void backpropoutput(float * _error, float * _gamma, float * _in, float * _targets, float * _weightsDelta, float * _out, int inputLength, int globalid )
                {
                    _error[globalid] = (_out[globalid] - _targets[globalid]);
                    _gamma[globalid] = _error[globalid] * (_out[globalid] * (1 - _out[globalid]));
                    for (int j = 0; j < inputLength; j++)
                        {                           
                           int Windex = GetWeightIndex(globalid, j, inputLength);
                           float v = _gamma[globalid] * _in[j];                           
                           _weightsDelta[Windex] = (v *0.02f) + (_weightsDelta[Windex] * 0.02f);
                        }
                }";
            string backprophiddenFunction = @"
               void backprophidden(float * _out, float * _gamma, float * _gammaForward, float * weightsForward, int weightsWidth, int gammaForwardLength ,int globalid)
               {
                    _gamma[globalid] = 0;
                    for (int j = 0; j < gammaForwardLength; j++)
                    {
                        int windex = GetWeightIndex( j,globalid, weightsWidth);
                        _gamma[globalid] += _gammaForward[j] * weightsForward[windex];
                    }
                    _gamma[globalid] *= (_out[globalid] * (1 - _out[globalid]));
               }";
            string backprophidden2Function = @"
               void backprophidden2(float * _in, float * _gamma, float * _weightsDelta, float * _weightsDeltaOLD, int numberOfInputs ,int globalid)
               {
                    for (int j = 0; j < numberOfInputs; j++)
                    {   
                        float v = (_gamma[globalid] * _in[j]);
                        int windex = GetWeightIndex(  globalid,j,  numberOfInputs);
                      // _weightsDeltaOLD[windex] = _weightsDelta[windex];
                       _weightsDelta[windex] = (v *0.02f) + (_weightsDelta[windex] * 0.02f);                      
                    }
                }";
            string updateWeightsFunction = @"
                void updateWeights(float * _weights, float * _weightsDelta, int numberOfInputs ,int globalid)
                {
                    for (int j = 0; j < numberOfInputs; j++)
                    {                   
                        int currentWINDEX = GetWeightIndex(globalid, j, numberOfInputs);
                        int deltaINDEX = GetWeightIndex(globalid, j, numberOfInputs);
                        float newWeight = _weights[currentWINDEX] - (_weightsDelta[deltaINDEX] );
                        _weights[currentWINDEX] = newWeight ;
                                              
                    }

                }";
            Dictionary<string, float[]> floatArrays = new Dictionary<string, float[]>();
            Dictionary<string, int[]> intArrays = new Dictionary<string, int[]>();

            var count = 0;
            List<int> arrayLengths = new List<int>();
            foreach (var nodelayer in _netShape)
            {
                if (count == 0)
                {
                    var inNodes = new float[nodelayer];
                    floatArrays.Add("inNodes", inNodes);
                    arrayLengths.Add(inNodes.Length);
                }
                if (count > 0 && count < _netShape.Length - 1)
                {
                    var weights = new float[_netShape[count - 1] * nodelayer];
                    for (int i = 0; i < weights.Length; i++)
                    {
                        weights[i] = (float)ThreadSafeRandom.NextDouble(-0.5f, 0.5f);
                    }
                    arrayLengths.Add(_netShape[count - 1] * nodelayer);
                    var weightsDelta = new float[_netShape[count - 1] * nodelayer];
                    arrayLengths.Add(_netShape[count - 1] * nodelayer);
                    var hiddenNodes = new float[nodelayer];
                    arrayLengths.Add(nodelayer);
                    var errors = new float[nodelayer];
                    arrayLengths.Add(nodelayer);
                    var gamma = new float[nodelayer];
                    arrayLengths.Add(nodelayer);
                    var weightsDeltaOLD = new float[_netShape[count - 1] * nodelayer];

                    floatArrays.Add("weights" + count, weights);
                    floatArrays.Add("weightsDelta" + count, weightsDelta);
                    floatArrays.Add("hiddenNodes" + count, hiddenNodes);
                    floatArrays.Add("errors" + count, errors);
                    floatArrays.Add("gamma" + count, gamma);
                    floatArrays.Add("weightsDeltaOLD" + count, weightsDeltaOLD);
                }
                if (count == _netShape.Length - 1)
                {
                    var weights = new float[_netShape[count - 1] * nodelayer];
                    for (int i = 0; i < weights.Length; i++)
                    {
                        weights[i] = (float)ThreadSafeRandom.NextDouble(-0.5f, 0.5f);
                    }
                    arrayLengths.Add(_netShape[count - 1] * nodelayer);
                    var weightsDelta = new float[_netShape[count - 1] * nodelayer];
                    arrayLengths.Add(_netShape[count - 1] * nodelayer);
                    var outNodes = new float[nodelayer];
                    arrayLengths.Add(nodelayer);
                    var errorsout = new float[nodelayer];
                    arrayLengths.Add(nodelayer);
                    var gammaout = new float[nodelayer];
                    arrayLengths.Add(nodelayer);
                    var currentTargets = new float[nodelayer];
                    arrayLengths.Add(nodelayer);
                    var weightsDeltaOLD = new float[_netShape[count - 1] * nodelayer];


                    floatArrays.Add("weights" + count, weights);
                    floatArrays.Add("weightsDelta" + count, weightsDelta);
                    floatArrays.Add("outNodes", outNodes);
                    floatArrays.Add("errorsout", errorsout);
                    floatArrays.Add("gammaout", gammaout);
                    floatArrays.Add("currentTargets", currentTargets);
                    floatArrays.Add("weightsDeltaOLD" + count, weightsDeltaOLD);
                }
                count++;
            }
            intArrays.Add("arrayLengths", arrayLengths.ToArray());
            intArrays.Add("currentlayer", new int[1] { 1 });

            LoadData(floatArrays, intArrays);
            _args_CL = Args_CL;
            var k = "";
            k += GetWeightIndexFunction;
            k += feedforwardlayerFunction;
            k += backpropoutputFucntion;
            k += backprophiddenFunction;
            k += backprophidden2Function;
            k += updateWeightsFunction;
            k += BuildFeedForwardLayerKernel(floatArrays, intArrays, _netShape.Length);
            k += BuildBackPropOutputKernel(floatArrays, intArrays, _netShape.Length);
            k += BuildBackPropHiddenKernal(floatArrays, intArrays, _netShape.Length);
            k += BuildBackPropHidden2Kernal(floatArrays, intArrays, _netShape.Length);
            k += BuildUpdateWeightsKernal(floatArrays, intArrays, _netShape.Length);

            Console.WriteLine(k);
            return k;
        }

        static string BuildKernelHead(Dictionary<string, float[]> floatArrays, Dictionary<string, int[]> intArrays, string kernalName)
        {
            var kernelhead = $@"
                __kernel void
                {kernalName}(";


            foreach (var item in floatArrays)
            {

                kernelhead += $@" __global float * {item.Key},
                                ";


            }
            var count = 0;
            foreach (var item in intArrays)
            {
                if (count < intArrays.Count - 1)
                {
                    kernelhead += $@" __global int * {item.Key},
                                    ";

                }
                else
                {
                    kernelhead += $@" __global int * {item.Key}
                                   )";
                }
                count++;
            }

            return kernelhead;
        }

        static string BuildFeedForwardLayerKernel(Dictionary<string, float[]> floatArrays, Dictionary<string, int[]> intArrays, int nodelayercount)
        {
            var head = BuildKernelHead(floatArrays, intArrays, "FeedForwardLayer");
            var body = $@"
                          {{
                            int i = get_global_id(0);
                          ";
            var arl = 0;
            for (int i = 1; i < nodelayercount; i++)
            {
                if (i == 1)
                {
                    body += $"if (currentlayer[0] == {i})\n{{\nfeedforwardlayer(inNodes, weights1, hiddenNodes1, arrayLengths[{arl}] ,i);\n}}\n";
                    arl += 3;
                }
                else if (i < nodelayercount - 1)
                {
                    body += $"if (currentlayer[0] == {i})\n{{\nfeedforwardlayer(hiddenNodes{i - 1}, weights{i}, hiddenNodes{i}, arrayLengths[{arl}] ,i);\n}}\n";
                    arl += 5;
                }
                else
                {
                    body += $"if (currentlayer[0] == {i})\n{{\nfeedforwardlayer(hiddenNodes{i - 1}, weights{i}, outNodes, arrayLengths[{arl}] ,i);\n}}\n}}";

                }
            }
            return head + body;
        }
        static string BuildBackPropOutputKernel(Dictionary<string, float[]> floatArrays, Dictionary<string, int[]> intArrays, int nodelayercount)
        {
            var head = BuildKernelHead(floatArrays, intArrays, "BackPropOutput");
            var body = $@"
                          {{
                            int i = get_global_id(0);
                          ";
            var inLen = netShape[^2];
            //backpropoutput(float * _error, float * _gamma, float * _in, float * _targets, float * _weightsDelta, float * _out, int inputLength, int globalid )
            body += $"backpropoutput(errorsout, gammaout, hiddenNodes{nodelayercount - 2}, currentTargets, weightsDelta{nodelayercount - 1}, outNodes, {inLen} ,i);\n}}";
            return head + body;
        }

        static string BuildBackPropHiddenKernal(Dictionary<string, float[]> floatArrays, Dictionary<string, int[]> intArrays, int nodelayercount)
        {
            var head = BuildKernelHead(floatArrays, intArrays, "BackPropHidden");
            var body = $@"
                      {{
                         int i = get_global_id(0);
                      ";
            for (int i = nodelayercount - 2; i > 0; i--)
            {
                // i is current layer index for netShape;
                //   backprophidden( float * _out, float * _gamma, float * _gammaForward, float * weightsForward, float * weightsWidth, int gammaForwardLength, int globalid)
                int fwWidth = netShape[i];
                int fgLength = netShape[i + 1];
                body += $"if (currentlayer[0] == {i})\n{{\nbackprophidden(hiddenNodes{i}, gamma{i}, gammaout, weights{i + 1}, {fwWidth}, {fgLength}, i);\n}}\n";
            }
            body += $"}}";
            return head + body;
        }

        static string BuildBackPropHidden2Kernal(Dictionary<string, float[]> floatArrays, Dictionary<string, int[]> intArrays, int nodelayercount)
        {
            var head = BuildKernelHead(floatArrays, intArrays, "BackPropHidden2");
            var body = $@"
                      {{
                         int i = get_global_id(0);
                      ";
            for (int i = nodelayercount - 2; i > 0; i--)
            {
                // i is current layer index for netShape;
                //  backprophidden2(float * _in, float * _gamma, float * _weightsDelta, int numberOfInputs ,int globalid)
                int fwWidth = netShape[i];
                int fgLength = netShape[i - 1];
                if (i == 1)
                {
                    body += $"if (currentlayer[0] == {i})\n{{\nbackprophidden2(inNodes, gamma{i}, weightsDelta{i}, weightsDeltaOLD{i},{fgLength}, i);\n}}\n";

                }
                else
                {
                    body += $"if (currentlayer[0] == {i})\n{{\nbackprophidden2(hiddenNodes{i - 1}, gamma{i}, weightsDelta{i}, weightsDeltaOLD{i},{fgLength}, i);\n}}\n";
                }
            }
            body += $"}}";
            return head + body;
        }

        static string BuildUpdateWeightsKernal(Dictionary<string, float[]> floatArrays, Dictionary<string, int[]> intArrays, int nodelayercount)
        {
            var head = BuildKernelHead(floatArrays, intArrays, "UpdateWeightsLayer");
            var body = $@"
                      {{
                         int i = get_global_id(0);
                      ";
            for (int i = nodelayercount - 1; i > 0; i--)
            {
                // i is current layer index for netShape;
                //  updateWeights(float * _weights, float * _weightsDelta, int numberOfInputs ,int globalid)

                int numberOfInputs = netShape[i - 1];

                body += $"if (currentlayer[0] == {i})\n{{\nupdateWeights(weights{i}, weightsDelta{i}, {numberOfInputs}, i);\n}}\n";

            }
            body += $"}}";

            return head + body;
        }
        static void LoadData(Dictionary<string, float[]> floatArrays = null, Dictionary<string, int[]> intArrays = null)
        {
            if (floatArrays != null)
            {
                foreach (var a in floatArrays)
                {
                    Args_CL.Add(a.Key, new OpenCLTemplate.CLCalc.Program.Variable(a.Value));
                }
            }

            if (intArrays != null)
            {
                foreach (var a in intArrays)
                {
                    Args_CL.Add(a.Key, new OpenCLTemplate.CLCalc.Program.Variable(a.Value));
                }
            }

        }



    }
}

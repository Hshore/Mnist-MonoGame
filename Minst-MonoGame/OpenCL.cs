using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minst_MonoGame
{
    public class OpenCL
    {

        public OpenCLTemplate.CLCalc.Program.Kernel FeedForwardLayer_kernal_CL;
        public OpenCLTemplate.CLCalc.Program.Kernel BackPropOutput_kernal_CL;
        public OpenCLTemplate.CLCalc.Program.Kernel BackPropHidden1_kernal_CL;
        public OpenCLTemplate.CLCalc.Program.Kernel BackPropHidden2_kernal_CL;
        public OpenCLTemplate.CLCalc.Program.Kernel UpdateWeights_kernal_CL;
        public Dictionary<string, OpenCLTemplate.CLCalc.Program.Variable> Args_CL = new Dictionary<string, OpenCLTemplate.CLCalc.Program.Variable>();
        public int[] netShape;
        public OpenCL(int[] net)
        {
            netShape = net;
            InitOpenCL();
        }



        void InitOpenCL()
        {
            OpenCLTemplate.CLCalc.InitCL();
            string kernal1 = KernalBuilder.BuidlKernal(netShape, out Args_CL);

            //Console.WriteLine(kernal1);


            OpenCLTemplate.CLCalc.Program.Compile(new string[] { kernal1 });
            FeedForwardLayer_kernal_CL = new OpenCLTemplate.CLCalc.Program.Kernel("FeedForwardLayer");
            BackPropOutput_kernal_CL = new OpenCLTemplate.CLCalc.Program.Kernel("BackPropOutput");
            BackPropHidden1_kernal_CL = new OpenCLTemplate.CLCalc.Program.Kernel("BackPropHidden");
            BackPropHidden2_kernal_CL = new OpenCLTemplate.CLCalc.Program.Kernel("BackPropHidden2");
            UpdateWeights_kernal_CL = new OpenCLTemplate.CLCalc.Program.Kernel("UpdateWeightsLayer");

        }

        public float[] feedforwardNetwork(float[] inputs)
        {
            Args_CL["inNodes"].WriteToDevice(inputs);
            for (int i = 1; i < netShape.Length; i++)
            {
                Args_CL["currentlayer"].WriteToDevice(new int[] { i });
                FeedForwardLayer_kernal_CL.Execute(Args_CL.Values.ToArray(), new int[] { netShape[i] });
            }
            //float[] outs = new float[Args_CL["outNodes"].OriginalVarLength];
            //Args_CL["outNodes"].ReadFromDeviceTo(outs);
            return null;
        }
        public float[] backpropNetwork(float[] targets)
        {
          //  float[] res = new float[5];
            Args_CL["currentTargets"].WriteToDevice(targets);
            for (int i = netShape.Length - 1; i > 0; i--)
            {
                if (i == netShape.Length - 1)
                {
                    Args_CL["currentlayer"].WriteToDevice(new int[] { i });
                    BackPropOutput_kernal_CL.Execute(Args_CL.Values.ToArray(), new int[] { netShape[i] });
                }
                else
                {
                    Args_CL["currentlayer"].WriteToDevice(new int[] { i });
                    BackPropHidden1_kernal_CL.Execute(Args_CL.Values.ToArray(), new int[] { netShape[i] });
                }
            }

            for (int i = netShape.Length - 2; i > 0; i--)
            {


                Args_CL["currentlayer"].WriteToDevice(new int[] { i });
                BackPropHidden2_kernal_CL.Execute(Args_CL.Values.ToArray(), new int[] { netShape[i] });

            }
           // float[] res = new float[Args_CL["errorsout"].OriginalVarLength];
           // Args_CL["errorsout"].ReadFromDeviceTo(res);
            return null;
        }

        public float[] updateWeights()
        {

            for (int i = netShape.Length - 1; i > 0; i--)
            {


                Args_CL["currentlayer"].WriteToDevice(new int[] { i });
                UpdateWeights_kernal_CL.Execute(Args_CL.Values.ToArray(), new int[] { netShape[i] });

            }


            return null;
        }


    }
}

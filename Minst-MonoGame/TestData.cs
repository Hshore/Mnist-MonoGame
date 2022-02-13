using System;
using System.Collections.Generic;
using System.Text;

namespace Minst_MonoGame
{
    class TestData
    {

        public float[][] inputs;
        public float[][] targets;
        public string[] targetLables;
        public string[] outputLabels;
        public string name;

        public TestData(string _name)
        {
            name = _name;
            switch (_name)
            {
                case "xor":
                    inputs = new float[4][];
                    targets = new float[4][];
                    targetLables = new string[] { "0", "1", "1", "0" };
                    outputLabels = new string[] { "0", "1" };

                    inputs[0] = new float[] { 0, 0, 1};
                    targets[0] = new float[] { 1, 0 };

                    inputs[1] = new float[] { 0, 1, 1};
                    targets[1] = new float[] { 0, 1 };

                    inputs[2] = new float[] { 1, 0, 1};
                    targets[2] = new float[] { 0, 1 };

                    inputs[3] = new float[] { 1, 1, 1 };
                    targets[3] = new float[] { 1, 0 };
                    break;

                case "xnor":
                    inputs = new float[8][];
                    targets = new float[8][];
                    targetLables = new string[] { "0", "1", "0", "1", "0", "1", "1", "0" };
                    outputLabels = new string[] { "0", "1" };

                    inputs[0] = new float[] { 0, 0, 0 };
                    targets[0] = new float[] { 0, 1 };

                    inputs[1] = new float[] { 0, 0, 1 };
                    targets[1] = new float[] { 1, 0 };

                    inputs[2] = new float[] { 0, 1, 0 };
                    targets[2] = new float[] { 1, 0 };

                    inputs[3] = new float[] { 0, 1, 1 };
                    targets[3] = new float[] { 0, 1 };

                    inputs[4] = new float[] { 1, 0, 0 };
                    targets[4] = new float[] { 1, 0 };

                    inputs[5] = new float[] { 1, 0, 1 };
                    targets[5] = new float[] { 0, 1 };

                    inputs[6] = new float[] { 1, 1, 0 };
                    targets[6] = new float[] { 0, 1 };

                    inputs[7] = new float[] { 1, 1, 1 };
                    targets[7] = new float[] { 1, 0 };

                   
                    break;

                case "alpha":
                    inputs = new float[26][];
                    targets = new float[26][];
                    targetLables = new string[] {"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"};
                    outputLabels = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

                    inputs[0] = new float[] { 0,0,0,0,1, 1}; //a
                    inputs[1] = new float[] { 0,0,0,1,0, 1}; //b
                    inputs[2] = new float[] { 0,0,0,1,1, 1}; //c
                    inputs[3] = new float[] { 0,0,1,0,0, 1}; //d
                    inputs[4] = new float[] { 0,0,1,0,1, 1}; //e
                    inputs[5] = new float[] { 0,0,1,1,0, 1}; //f
                    inputs[6] = new float[] { 0,0,1,1,1, 1}; //g
                    inputs[7] = new float[] { 0,1,0,0,0, 1}; //h
                    inputs[8] = new float[] { 0,1,0,0,1, 1}; //i
                    inputs[9] = new float[] { 0,1,0,1,0, 1}; //j
                    inputs[10] = new float[] { 0,1,0,1,1, 1}; //k
                    inputs[11] = new float[] { 0,1,1,0,0, 1}; //l
                    inputs[12] = new float[] { 0,1,1,0,1, 1}; //m
                    inputs[13] = new float[] { 0,1,1,1,0, 1}; //n
                    inputs[14] = new float[] { 0,1,1,1,1, 1}; //o
                    inputs[15] = new float[] { 1,0,0,0,0, 1}; //p
                    inputs[16] = new float[] { 1,0,0,0,1, 1}; //q
                    inputs[17] = new float[] { 1,0,0,1,0, 1}; //r
                    inputs[18] = new float[] { 1,0,0,1,1, 1}; //s
                    inputs[19] = new float[] { 1,0,1,0,0, 1}; //t
                    inputs[20] = new float[] { 1,0,1,0,1, 1}; //u
                    inputs[21] = new float[] { 1,0,1,1,0, 1}; //v
                    inputs[22] = new float[] { 1,0,1,1,1, 1}; //w
                    inputs[23] = new float[] { 1,1,0,0,0, 1}; //x
                    inputs[24] = new float[] { 1,1,0,0,1, 1}; //y
                    inputs[25] = new float[] { 1,1,0,1,0, 1}; //z


                    targets[0] = new float[] {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[1] = new float[] {0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[2] = new float[] {0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[3] = new float[] {0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[4] = new float[] {0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[5] = new float[] {0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[6] = new float[] {0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[7] = new float[] {0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[8] = new float[] {0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[9] = new float[] {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[10] = new float[] {0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[11] = new float[] {0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[12] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[13] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0};
                    targets[14] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0};
                    targets[15] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0};
                    targets[16] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0};
                    targets[17] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0};
                    targets[18] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0};
                    targets[19] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0};
                    targets[20] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0};
                    targets[21] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0};
                    targets[22] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0};
                    targets[23] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0};
                    targets[24] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0};
                    targets[25] = new float[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1};
                    break;

               
            }
        }

        public float[] NextRandomDataSet(out float[] targs, out string label)
        {
            var randSelectorInputs = ThreadSafeRandom.Next(0, inputs.Length);

            var ins = inputs[randSelectorInputs];
            targs = targets[randSelectorInputs];
            label = targetLables[randSelectorInputs];
            return ins;
        }

    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Minst_MonoGame
{
    class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private SpriteFont largefont;
        public static int window_w = 1700;
        public static int window_h = 900;
        public NeuralNet net;
        public TestData testData;
        public Extensions.Image currentMnistImage;
        public Extensions.Image currentMnistOutImage;
        public float[] currentOutputs;
        public string[] netData;
        public float timer = 0.5f;
        public const float TIMER = 0.5f;
        public bool Tick = false;
        public bool runNet = false;
        public string lastoutputstring = "";
        public int lasttargetlabel;
        public int[] lastLabels;
        public string averageConfidance = "";
        public string TimeLabel = "";
        public string debugLabel = "";
        private OpenCL opencl;
        public int currentgen;
        public float runningConfidance = 0;
        public int runningConfidanceCount = 0;

        
        Texture2D whiteRectangle;
        Texture2D mnistImage;
        Texture2D mnistImageOut;
        BackgroundWorker NetWorker;

        private List<Button> _buttonComponents;
        private List<NetworkVisualisation> _netComponents;

        Stopwatch watch = new Stopwatch();
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            NetWorker = new BackgroundWorker();
            NetWorker.WorkerReportsProgress = true;
            NetWorker.WorkerSupportsCancellation = true;
            NetWorker.DoWork += NetWorker_DoWork;
            NetWorker.ProgressChanged += NetWorker_ProgressChanged;
            NetWorker.RunWorkerCompleted += NetWorker_RunWorkerCompleted;
        }

        private void NetWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void NetWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            // NetWorker.CancelAsync();



        }
        private void NetWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //var net = (NeuralNet)e.Argument;
            for (int i = 0; i < 100000; i++)
            {

                if (NetWorker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    var watch = new Stopwatch();
                    watch.Start();
                    //getNextImage
                    currentMnistImage = net.GetRandomTrainingImage();
                    

                    var outputs =  opencl.feedforwardNetwork(currentMnistImage.DataFlat_NetInputs);
                    opencl.backpropNetwork(currentMnistImage.LabelArray);
                    opencl.updateWeights();

                    float currentConfidence = 0;
                    float outTotal = 0;
                    for (int j = 0; j < outputs.Length; j++)
                    {
                        outTotal += outputs[j];
                    }
                    for (int j = 0; j < currentMnistImage.LabelArray.Length; j++)
                    {
                        if (currentMnistImage.LabelArray[j] == 1)
                        {
                            currentConfidence = (outputs[j] / outTotal);
                        }
                    }
                    runningConfidance += currentConfidence;
                    runningConfidanceCount++;
                   
               //     currentOutputs = outputs;
                    //net.Train(1, out currentMnistImage, out currentOutputs);
                    //TimeLabel = watch.Elapsed.TotalSeconds.ToString("0.000") + "s/img";
                    watch.Stop();

                    NetWorker.ReportProgress(i);
                    currentgen++;
                }
            }

            // netData = net.GetNetworkStatus();



        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            netData = net.GetNetworkStatus();


            _graphics.PreferredBackBufferWidth = window_w;
            _graphics.PreferredBackBufferHeight = window_h;
            _graphics.ApplyChanges();
            this.Window.AllowUserResizing = true;



        }

        protected override void LoadContent()
        {
            testData = new TestData("alpha");
            //var img = net.GetRandomTrainingImage();
            net = new NeuralNet(new int[] { 100,300, 50, 300, 100 });
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("defaultFont");
            largefont = Content.Load<SpriteFont>("largeFont");
            var tempimg = net.GetRandomTrainingImage();
            // var tempins = net.GetRandomTrainingImage().LabelArray;
            var inputLength = tempimg.DataFlat_NetInputs.Length;
            var outputLength = tempimg.LabelArray.Length;
          //  currentOutputs = new float[tempins.Length];
            currentOutputs = new float[outputLength];
           // opencl = new OpenCL(new int[] { inputLength, 600,500,400,300,20,300,400,500,600,  tempins.Length });
            opencl = new OpenCL(new int[] { inputLength, 200, 50, outputLength });
            // TODO: use this.Content to load your game content here

            whiteRectangle = new Texture2D(GraphicsDevice, 100, 100);
            int[] arry = new int[whiteRectangle.Width * whiteRectangle.Height];
            for (int i = 0; i < whiteRectangle.Width * whiteRectangle.Height; i++)
            {
                arry[i] = -1;

            }
            whiteRectangle.SetData<int>(arry);

            var w = net.selectedData[0].Data.GetLength(0);
            var h = net.selectedData[0].Data.GetLength(1);
            mnistImage = new Texture2D(GraphicsDevice, w, h);
            mnistImageOut = new Texture2D(GraphicsDevice, w, h);
            currentMnistImage = net.GetRandomTrainingImage();
            currentMnistOutImage = net.GetRandomTrainingImage();
            mnistImage.SetData<byte>(currentMnistImage.DataFlat);
            mnistImageOut.SetData<byte>(currentMnistOutImage.DataFlat);

            var Start_Stop = new Button(whiteRectangle, font);
            Start_Stop.w_offset = 100;
            Start_Stop.h_offset = -100;
            Start_Stop.location = Extensions.Location.BottomLeft;
            Start_Stop.ButtonSize = new Vector2(200, 100);
            Start_Stop.Text = "Run";
            Start_Stop.Click += Start_Stop_Click;

            _buttonComponents = new List<Button>
            {
                Start_Stop
               // singlebutton,              
            };

            var netVisual = new NetworkVisualisation(whiteRectangle, whiteRectangle, net);
            netVisual.pos = new Vector2(450, 20);

            _netComponents = new List<NetworkVisualisation>
            {
                netVisual
               // singlebutton,              
            };
        }

        private void Start_Stop_Click(object sender, Button button)
        {

            runNet = !runNet;
            if (runNet)
            {
                if (NetWorker.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    button.Text = "Stop";
                    NetWorker.RunWorkerAsync(net);
                }
            }
            else
            {
                if (NetWorker.WorkerSupportsCancellation == true)
                {
                    // Cancel the asynchronous operation.
                    button.Text = "Run";
                    NetWorker.CancelAsync();
                }
            }
       
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();

            }

               

            foreach (var button in _buttonComponents)
            {
                button.Update(gameTime, Window);
            }



            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsed;



            if (timer < 0)
            {
                if (NetWorker.IsBusy)
                {
                    NetWorker.CancelAsync();

                    while (NetWorker.IsBusy)
                    {

                    }


                    _netComponents[0].updateVisualisation();
                    netData = net.GetNetworkStatus();
                    mnistImage.SetData<byte>(currentMnistImage.DataFlat);
                    opencl.Args_CL["outNodes"].ReadFromDeviceTo(currentOutputs);
                    byte[] arry = new byte[mnistImageOut.Width*mnistImageOut.Height*4];
                    string outputString = "";
                    float totalouts = 0;
                    if (currentOutputs != null)
                    {
                        // mnistImageOut.SetData<byte>(arry);
                        var c = 0;
                     
                        foreach (var item in currentOutputs)
                        {
                            arry[c] = (byte)(item * 255);
                            arry[c + 1] = (byte)(item * 255);
                            arry[c + 2] = (byte)(item * 255);
                            arry[c + 3] = 255;
                            c += 4;

                            totalouts += item;
                           // outputString += $"{d}:{String.Format("{0:0.00}", item*100)}%\n";
                           
                        }
                        var d = 0;
                        lastLabels = new int[10]; 
                        foreach (var item in currentOutputs)
                        {
                            var v = (item / totalouts) * 100;
                             outputString += $"{d}:{String.Format("{0:0.00}", v)}%\n";
                            lastLabels[d] = (int)((v/100) * 255);
                            d++;
                        }
                        //arry = currentOutputs.Select(f => Convert.ToByte(f)).ToArray();
                        //
                        //currentOutputs.CopyTo(arry, 0);
                        mnistImageOut.SetData<byte>(arry);
                        lasttargetlabel = (int)currentMnistImage.Label;
                        lastoutputstring = outputString;
                        //   NetWorker.RunWorkerAsync();

                    }
                    averageConfidance = $"AvgConfidance:\n{String.Format("{0:0.00}", (runningConfidance / runningConfidanceCount)*100)}";
                    runningConfidance = 0;
                    runningConfidanceCount = 0;
                    NetWorker.RunWorkerAsync();

                }
                timer = TIMER;   //Reset Timer

                //_netComponents[0].updateVisualisation();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20, 255));
            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied);

            foreach (var button in _buttonComponents)
            {
                button.Draw(gameTime, _spriteBatch);
            }

            foreach (var netC in _netComponents)
            {
                netC.Draw(gameTime, _spriteBatch);
            }


            var c = 0;
            if (lastLabels != null)
            {
                foreach (var label in lastLabels)
                {
                    if (c == lasttargetlabel)
                    {
                      _spriteBatch.DrawString(largefont, c.ToString(), new Vector2(window_w / 2-200, window_h / 2-500), new Color(0,255,0,lastLabels[c]));

                    }
                    else
                    {

                      _spriteBatch.DrawString(largefont, c.ToString(), new Vector2(window_w / 2-200, window_h / 2-500), new Color(255,0,0,lastLabels[c]));
                    }
                   
                    c++;
                }
              //  _spriteBatch.DrawString(font, "1", new Vector2(window_w / 2, window_h / 2), new Color(255, 255, 255, lastLabels[1]));
               // _spriteBatch.DrawString(font, "2", new Vector2(window_w / 2, window_h / 2), new Color(255, 255, 255, lastLabels[2]));
                

            }

            //     var s6 = font.MeasureString(netData[6]) / 2;
            //     var s7 = font.MeasureString(netData[7]) / 2;
            //     var s1 = font.MeasureString(netData[1]) / 2;
            //     var s2 = font.MeasureString(netData[2]) / 2;
            //     var s3 = font.MeasureString(netData[3]) / 2;
            //    var s4 = font.MeasureString(netData[4]) / 2;
            //    var s5 = font.MeasureString(netData[5]) / 2;
            //    _spriteBatch.DrawString(font, netData[7], new Vector2(window_w / 2 - s7.X, window_h / 2 -100), Color.Black);
            //    _spriteBatch.DrawString(font, netData[6], new Vector2(window_w / 2 + 300, window_h / 2 +50), Color.Black);
            //   _spriteBatch.DrawString(font, netData[1], new Vector2(window_w/2 - s1.X, window_h/2 ), Color.Black);
            _spriteBatch.DrawString(font, lastoutputstring, new Vector2(window_w / 2 + 430, 50), Color.White);
           // _spriteBatch.DrawString(font, "" + TimeLabel, new Vector2(window_w / 2 + 430, window_h / 2), Color.White);
         //   _spriteBatch.DrawString(font, netData[2], new Vector2(window_w / 2 + 430, window_h / 2 + 50), Color.White);
            _spriteBatch.DrawString(font, "Gen: "+ currentgen, new Vector2(window_w / 2 + 430, window_h / 2 + 220), Color.White);
            _spriteBatch.DrawString(font, averageConfidance, new Vector2(window_w / 2 + 430, window_h / 2 + 270), Color.White);
            // _spriteBatch.DrawString(font, netData[4], new Vector2(window_w / 2 + 430, window_h / 2 + 100), Color.White);
            // _spriteBatch.DrawString(font, netData[6], new Vector2(window_w / 2 + 430, window_h / 2 + 200), Color.White);
            //    _spriteBatch.DrawString(font, netData[3], new Vector2(window_w / 2 - s3.X, window_h / 2 + 100), Color.Black);
            _spriteBatch.Draw(mnistImage, new Rectangle(20, 20, 400, 400), Color.White);
        //    _spriteBatch.Draw(mnistImageOut, new Rectangle(1280, 20, 400, 400), Color.White);
            // TODO: Add your drawing code here
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

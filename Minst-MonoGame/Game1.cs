using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Minst_MonoGame
{
    class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
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
        public bool runNet = false;
        public string lastLabel = "";
        public string TimeLabel = "";

        Texture2D button_texture;
        Texture2D slider_texture;
        Texture2D sliderToggle_texture;
        Texture2D img_input_texture;
        Texture2D img_output_texture;
        Texture2D whiteRectangle;
        Texture2D mnistImage;
        Texture2D mnistImageOut;

        private List<Button> _buttonComponents;
        private List<NetworkVisualisation> _netComponents;

        Stopwatch watch = new Stopwatch();
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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
            net = new NeuralNet(new int[] { 28*28, 400, 10, 400, 28*28 });
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("defaultFont");
            // TODO: use this.Content to load your game content here
            
            whiteRectangle = new Texture2D(GraphicsDevice, 100, 100);
            int[] arry = new int[whiteRectangle.Width*whiteRectangle.Height];
            for (int i = 0; i < whiteRectangle.Width * whiteRectangle.Height; i++)
            {
                arry[i] = -1; 

            }
            whiteRectangle.SetData<int>(arry);

            mnistImage = new Texture2D(GraphicsDevice, 28, 28);
            mnistImageOut = new Texture2D(GraphicsDevice, 28, 28);
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
            netVisual.pos = new Vector2(450,20);

            _netComponents = new List<NetworkVisualisation>
            {
                netVisual
               // singlebutton,              
            };
        }

        private void Start_Stop_Click(object sender, Button button)
        {
            runNet = !runNet;
            if (button.Text == "Run")
            {
                button.Text = "Stop";

            }
            else  
            {
                button.Text = "Run";

            }

            // currentMnistImage = net.GetRandomTrainingImage();
            //  mnistImage.SetData<byte>(currentMnistImage.DataFlat);
           
            // throw new System.NotImplementedException();
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
            if (runNet)
            {
                for (int i = 0; i < 2; i++)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                        net.Train(1, out currentMnistImage, out currentOutputs);
                    TimeLabel = watch.Elapsed.TotalSeconds.ToString();
                    watch.Stop();
                }
                
               // netData = net.GetNetworkStatus();

            }


            
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsed;
           
            if (timer < 0)
            {
                netData = net.GetNetworkStatus();
                mnistImage.SetData<byte>(currentMnistImage.DataFlat);
                byte[] arry = new byte[3136];
                if (currentOutputs != null)
                {
                   // mnistImageOut.SetData<byte>(arry);
                    var c = 0;
                    foreach (var item in currentOutputs)
                    {
                        arry[c] = (byte)(item * 255);
                        arry[c+1] = (byte)(item * 255);
                        arry[c+2] = (byte)(item * 255);
                        arry[c+3] = 255;
                        c += 4;
                    }
                    //arry = currentOutputs.Select(f => Convert.ToByte(f)).ToArray();
                    //
                    //currentOutputs.CopyTo(arry, 0);
                    mnistImageOut.SetData<byte>(arry);
                    lastLabel = currentMnistImage.Label.ToString();

                }
                //Timer expired, execute action
                timer = TIMER;   //Reset Timer
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20,20,20,255));
            _spriteBatch.Begin();

            foreach (var button in _buttonComponents)
            {
                button.Draw(gameTime, _spriteBatch);
            }
            foreach (var netC in _netComponents)
            {
                netC.Draw(gameTime, _spriteBatch);
            }

            /*var s6 = font.MeasureString(netData[6]) / 2;
       //     var s7 = font.MeasureString(netData[7]) / 2;
            var s1 = font.MeasureString(netData[1]) / 2;
            var s2 = font.MeasureString(netData[2]) / 2;
            var s3 = font.MeasureString(netData[3]) / 2;
            var s4 = font.MeasureString(netData[4]) / 2;
            var s5 = font.MeasureString(netData[5]) / 2;
        //    _spriteBatch.DrawString(font, netData[7], new Vector2(window_w / 2 - s7.X, window_h / 2 -100), Color.Black);
            _spriteBatch.DrawString(font, netData[6], new Vector2(window_w / 2 - s6.X, window_h / 2 -50), Color.Black);
            _spriteBatch.DrawString(font, netData[1], new Vector2(window_w/2 - s1.X, window_h/2 ), Color.Black);
            _spriteBatch.DrawString(font, netData[4], new Vector2(window_w / 2 - s4.X, window_h / 2 + 50), Color.Black);
            _spriteBatch.DrawString(font, netData[3], new Vector2(window_w / 2 - s3.X, window_h / 2 + 100), Color.Black);
            _spriteBatch.DrawString(font, netData[2], new Vector2(window_w / 2 - s2.X, window_h / 2 + 150), Color.Black);
            _spriteBatch.DrawString(font, netData[5], new Vector2(window_w / 2 - s5.X, window_h / 2 + 200), Color.Black);
            _spriteBatch.DrawString(font, "Label: " + lastLabel, new Vector2(window_w/2 - s5.X, window_h/2 +250), Color.Black);
            _spriteBatch.DrawString(font, "time: " + TimeLabel, new Vector2(window_w/2 - s5.X, window_h/2 +300), Color.Black);*/
            _spriteBatch.Draw(mnistImage, new Rectangle(20, 20, 400, 400), Color.White);
            _spriteBatch.Draw(mnistImageOut, new Rectangle(1280, 20, 400, 400), Color.White);
            // TODO: Add your drawing code here
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

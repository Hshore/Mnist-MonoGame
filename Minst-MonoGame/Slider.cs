using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Minst_MonoGame
{
    class Slider : Component
    {

        #region Fields
        private SpriteFont _font;
        private Texture2D _texture;
        private Texture2D _toggeltexture;


        #region Properties
        public float togglePos;
        public string _name;
        public event EventHandler<Slider> UpdateValue;
        //EventArgs args = new EventArgs();
        public Color PenColour { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 TogglePosition { get; set; }
        public Vector2 PositionScale { get; set; }

        public Vector2 MinMaxValues { get; set; }

        public int ToggleValue;
        private MouseState _previousMouse;
        private MouseState _currentmouse;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }
        public Rectangle ToggleRectangle
        {
            get
            {
                //var multiple = (_texture.Width) / (MinMaxValues.Y + Math.Abs(MinMaxValues.X));
                //var correctedToggleValue = ToggleValue + Math.Abs(MinMaxValues.X);
                //var pX = (correctedToggleValue * multiple) + Position.X;
                var pX = togglePos;//(togglePos * _texture.Width) + Position.X;
               // Position = new Vector2(Game1.window_w * PositionScale.X, Game1.window_h * PositionScale.Y);
                return new Rectangle((int)pX, (int)Position.Y, _toggeltexture.Width, _toggeltexture.Height);
               // return new Rectangle((int)pX - (_toggeltexture.Width / 2), (int)Position.Y + ((_texture.Height / 2) - (_toggeltexture.Height / 2)), _toggeltexture.Width, _toggeltexture.Height);
            }
        }
        public string Text { get; set; }
        #endregion

        #endregion

        public Slider(Texture2D texture, Texture2D toggelTexture, SpriteFont font, string name, Vector2 _posScale)
        {
            _texture = texture;
            _toggeltexture = toggelTexture;
            _font = font;
            PenColour = Color.LightBlue;
            _name = name;
            PositionScale = _posScale;
            Position = new Vector2(Game1.window_w * _posScale.X, Game1.window_h * _posScale.Y);
            togglePos = ((_texture.Width / 2) - (_toggeltexture.Width/2)) + Position.X;
        }

        public override void Draw(GameTime gameTime, SpriteBatch sprite)
        {
            
            var color = Color.White;
            sprite.Draw(_texture, Rectangle, color);
            sprite.Draw(_toggeltexture, ToggleRectangle, color);
            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                sprite.DrawString(_font, Text, new Vector2(x, y), PenColour);
            }
        }

        public override void Update(GameTime gameTime, GameWindow window)
        {
            //UpdateValue?.Invoke(this, this);
            Text = "" + ToggleValue;
            _previousMouse = _currentmouse;
            _currentmouse = Mouse.GetState();
            //Position = new Vector2(window.ClientBounds.Width * PositionScale.X, window.ClientBounds.Height * PositionScale.Y);
            var mouseRect = new Rectangle(_currentmouse.X, _currentmouse.Y, 1, 1);
            //            togglePos = Position.X + touchPointOnSliderX - (ToggleRectangle.Width/2);


            if (mouseRect.Intersects(Rectangle))
            {


                if (_currentmouse.LeftButton == ButtonState.Pressed)
                {
                    
                   
                      //  multiple = (_texture.Width) / (MinMaxValues.Y + Math.Abs(MinMaxValues.X));
                        int touchPointOnSliderX = _currentmouse.X - Rectangle.X;
                        togglePos = Position.X + touchPointOnSliderX - (ToggleRectangle.Width/2);
                        float sliderNorm = ((float)touchPointOnSliderX / (float)Rectangle.Width);
                        var sliderSteps = Math.Abs(MinMaxValues.X) + MinMaxValues.Y + 1;
                        var sliderValue = (sliderSteps*sliderNorm) + MinMaxValues.X;

                        ToggleValue = (int)sliderValue;
                      //  ToggleValue = (int)(touchPointOnSliderX / multiple);
                        /*  if (ToggleValue > MinMaxValues.Y)
                          {
                              ToggleValue = ToggleValue*-1;
                          }*/


                        UpdateValue?.Invoke(this, this);

                   
                }
            }
            Position = new Vector2(window.ClientBounds.Width * PositionScale.X, window.ClientBounds.Height * PositionScale.Y);
           // togglePos = Position.X + togglePos;
        }
    }
}

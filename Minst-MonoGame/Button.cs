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
    public class Button : Component     
    {
        public enum Location 
        {
            TopLeft,
            TopRight,
            TopMiddle,
            BottomMiddle,
            BottomLeft,
            BottomRight,
        }

        public Location location { get; set; }

        public int w_offset { get; set; }
        public int h_offset { get; set; }

        public int CurrentWindowWidth { get; set; }
        public int CurrentWindowHeight { get; set; }

        public float Scale = 1f;
        public float ScaledTextureWidth
        {
            get
            {
                return _texture.Width * Scale;
            }
        }
        public float ScaledTextureHeight
        {
            get
            {
                return _texture.Height * Scale;
            }
        }

       // public Component_Type type = Component_Type.Button;
        #region Fields
        private MouseState _currentmouse;
        private SpriteFont _font;
        private bool _isHovering;
        private MouseState _previousMouse;
        private Texture2D _texture;
        #endregion

        #region Properties
        public event EventHandler<Button> Click;
        public bool Clicked { get; private set; }

        public Color PenColour { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 PositionScale { get; set; }
        public Rectangle Rectangle
        {


            get
            {

                switch (location)
                {
                    case Location.TopLeft:
                        return new Rectangle(0 + w_offset, 0 + h_offset, (int)ScaledTextureWidth, (int)ScaledTextureHeight);
                        break;
                    case Location.TopRight:
                        return new Rectangle(CurrentWindowWidth - (int)ScaledTextureWidth + w_offset, 0 + h_offset, (int)ScaledTextureWidth, (int)ScaledTextureHeight);
                        break;
                    case Location.TopMiddle:
                        return new Rectangle((CurrentWindowWidth / 2) - ((int)ScaledTextureWidth / 2) + w_offset, 0 + h_offset, (int)ScaledTextureWidth, (int)ScaledTextureHeight);
                        break;
                    case Location.BottomLeft:

                        return new Rectangle(0 + w_offset, CurrentWindowHeight - (int)ScaledTextureHeight + h_offset, (int)ScaledTextureWidth, (int)ScaledTextureHeight);

                        break;
                    case Location.BottomRight:
                        return new Rectangle(CurrentWindowWidth - (int)ScaledTextureWidth + w_offset, CurrentWindowHeight - (int)ScaledTextureHeight + h_offset, (int)ScaledTextureWidth, (int)ScaledTextureHeight);
                        break;
                    case Location.BottomMiddle:
                        return new Rectangle((CurrentWindowWidth / 2) - ((int)ScaledTextureWidth / 2) + w_offset, CurrentWindowHeight - (int)ScaledTextureHeight + h_offset, (int)ScaledTextureWidth, (int)ScaledTextureHeight);
                        break;
                   
                    default:
                        return new Rectangle((CurrentWindowWidth / 2) - ((int)ScaledTextureWidth / 2) + w_offset, CurrentWindowHeight - (int)ScaledTextureHeight + h_offset, (int)ScaledTextureWidth, (int)ScaledTextureHeight);
                        break;
                }


            }
        }
        public string Text { get; set; }
        #endregion

        #region Methods
        public Button(Texture2D texture, SpriteFont font)
        {
            _texture = texture;
            _font = font;
            PenColour = Color.Black;
        }

        

        public override void Draw(GameTime gameTime, SpriteBatch sprite)
        {
            var color = Color.White;

            if (_isHovering)
            {
                color = Color.Gray;
            }

            sprite.Draw(_texture, Rectangle, color);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X/2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y/2);

                sprite.DrawString(_font, Text, new Vector2(x, y), PenColour);
            }
        }

        public override void Update(GameTime gameTime, GameWindow window)
        {

            _previousMouse = _currentmouse;
            _currentmouse = Mouse.GetState();
            CurrentWindowWidth = window.ClientBounds.Width;
            CurrentWindowHeight = window.ClientBounds.Height;
            //Position = new Vector2(window.ClientBounds.Width * PositionScale.X, window.ClientBounds.Height * PositionScale.Y);
            var mouseRect = new Rectangle(_currentmouse.X, _currentmouse.Y, 1, 1);

            _isHovering = false;
            if (mouseRect.Intersects(Rectangle))
            {
                _isHovering = true;

                if (_currentmouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, this);
                }
            }
            
            
        }

        #endregion
    }
}

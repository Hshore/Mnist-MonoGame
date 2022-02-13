using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Minst_MonoGame
{
    public abstract class Component
    {      
        public abstract void Draw(GameTime gameTime, SpriteBatch sprite);
        public abstract void Update(GameTime gameTime, GameWindow window);
    }
}

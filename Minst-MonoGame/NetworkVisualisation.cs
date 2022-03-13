using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minst_MonoGame
{
    class NetworkVisualisation : Component
    {

        public Texture2D nodeTexture;
        public Texture2D weightTexture;
        public NeuralNet netRef;
        public Rectangle backgroundRect 
        {
            get 
            {
                return new Rectangle((int)pos.X, (int)pos.Y, 630, 28*28);   
            } 
             
        }
        public Vector2 pos;
        public Vector2 CurrentNodePos;
        public Vector2 CurrentLayerPos;


        public NetworkVisualisation(Texture2D _nodeTexture, Texture2D _weightTexture, NeuralNet _netRef)
        {
            nodeTexture = _nodeTexture;
            weightTexture = _weightTexture;
            netRef = _netRef;
          //  CurrentLayerPos = new Vector2(pos.X, pos.Y);
          //  CurrentNodePos = new Vector2(CurrentLayerPos.X, CurrentLayerPos.Y);
        }

        public override void Draw(GameTime gameTime, SpriteBatch sprite)
        {
            sprite.Draw(nodeTexture, backgroundRect, Color.Black);

            var layersWidthSpacing = (backgroundRect.Width) / netRef.layers.Length+1;
            var nodePos = pos;
            var padding = 1;
            var layerCount = 0;
            foreach (var layer in netRef.layers)
            {
                if (layerCount == 0)
                {
                    float nodeHeightSpacing = (float)backgroundRect.Height / (float)layer.inputs.Length;
                    foreach (var node in layer.inputs)
                    {
                        int nValue = (int)(node * 255);
                        Color c = new Color(nValue, nValue, nValue, 255);
                        sprite.Draw(nodeTexture, new Rectangle((int)nodePos.X,(int)nodePos.Y,(int)layersWidthSpacing, (int)nodeHeightSpacing), null, c, 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        nodePos.Y += nodeHeightSpacing;
                    }
                    nodePos.Y = pos.Y;
                    nodePos.X += layersWidthSpacing;

                    nodeHeightSpacing = (float)backgroundRect.Height / (float)layer.outputs.Length;
                    foreach (var node in layer.outputs)
                    {
                        int nValue = (int)(node * 255);
                        Color c = new Color(nValue, nValue, nValue, 255);
                        sprite.Draw(nodeTexture, new Rectangle((int)nodePos.X, (int)nodePos.Y, (int)layersWidthSpacing, (int)nodeHeightSpacing), null, c, 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        nodePos.Y += nodeHeightSpacing;
                    }
                    nodePos.Y = pos.Y;
                    nodePos.X += layersWidthSpacing;
                }
                else
                {
                    var nodeHeightSpacing = (float)backgroundRect.Height / (float)layer.outputs.Length;
                    foreach (var node in layer.outputs)
                    {
                        int nValue = (int)(node * 255);
                        Color c = new Color(nValue, nValue, nValue, 255);
                        sprite.Draw(nodeTexture, new Rectangle((int)nodePos.X, (int)nodePos.Y, (int)layersWidthSpacing, (int)nodeHeightSpacing), null, c, 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        nodePos.Y += nodeHeightSpacing;
                    }
                    nodePos.Y = pos.Y;
                    nodePos.X += layersWidthSpacing;
                }


               
                layerCount++;
            }
          //  throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime, GameWindow window)
        {
          // throw new NotImplementedException();
        }
    }
}

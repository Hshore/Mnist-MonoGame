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
                return new Rectangle((int)pos.X, (int)pos.Y, 800, 800);   
            } 
             
        }
        public Vector2 pos;
        public Vector2 CurrentNodePos;
        public Vector2 CurrentLayerPos;
        public List<Rectangle> rects;
        public List<Color> colours;


        public NetworkVisualisation(Texture2D _nodeTexture, Texture2D _weightTexture, NeuralNet _netRef)
        {
            nodeTexture = _nodeTexture;
            weightTexture = _weightTexture;
            netRef = _netRef;
            rects = new List<Rectangle>();
            colours = new List<Color>();
          //  CurrentLayerPos = new Vector2(pos.X, pos.Y);
          //  CurrentNodePos = new Vector2(CurrentLayerPos.X, CurrentLayerPos.Y);
        }

        public void updateVisualisation()
        {
            rects.Clear();
            colours.Clear();
            // throw new NotImplementedException();
            float layersWidthSpacing = (backgroundRect.Width) / (netRef.layers.Length + 1);
            var nodePos = pos;
            //nodePos.X;
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
                        colours.Add( new Color(nValue, nValue, nValue, 255));
                        rects.Add(new Rectangle((int)nodePos.X, (int)nodePos.Y, (int)layersWidthSpacing, ((int)nodeHeightSpacing < 1) ? 1 : (int)nodeHeightSpacing));
                        //sprite.Draw(nodeTexture, new Rectangle((int)nodePos.X,(int)nodePos.Y,(int)layersWidthSpacing, (int)nodeHeightSpacing), null, c, 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        nodePos.Y += nodeHeightSpacing;
                    }
                    nodePos.Y = pos.Y;
                    nodePos.X += layersWidthSpacing;
                    
                    nodeHeightSpacing = (float)backgroundRect.Height / (float)layer.outputs.Length;
                    foreach (var node in layer.outputs)
                    {
                        int nValue = (int)(node * 255);
                        colours.Add( new Color(nValue, nValue, nValue, 255));
                        rects.Add(new Rectangle((int)nodePos.X, (int)nodePos.Y, (int)layersWidthSpacing, ((int)nodeHeightSpacing < 1) ? 1 : (int)nodeHeightSpacing));
                        //sprite.Draw(nodeTexture, new Rectangle((int)nodePos.X, (int)nodePos.Y, (int)layersWidthSpacing, (int)nodeHeightSpacing), null, c, 0, new Vector2(0, 0), SpriteEffects.None, 1);
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
                        colours.Add( new Color(nValue, nValue, nValue, 255));
                        rects.Add(new Rectangle((int)nodePos.X, (int)nodePos.Y, (int)layersWidthSpacing, ((int)nodeHeightSpacing < 1) ? 1 : (int)nodeHeightSpacing));
                        //sprite.Draw(nodeTexture, new Rectangle((int)nodePos.X, (int)nodePos.Y, (int)layersWidthSpacing, (int)nodeHeightSpacing), null, c, 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        nodePos.Y += nodeHeightSpacing;
                    }
                    nodePos.Y = pos.Y;
                    nodePos.X += layersWidthSpacing;
                }



                layerCount++;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch sprite)
        {
            sprite.Draw(nodeTexture, backgroundRect, Color.Black);
            var i = 0;
            foreach (var r in rects)
            {
                sprite.Draw(nodeTexture, r, null, colours[i], 0, new Vector2(0, 0), SpriteEffects.None, 1);
                i++;
            }


            //  throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime, GameWindow window)
        {
            
        }
    }
}

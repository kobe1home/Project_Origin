using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Project_Origin
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FPS : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private double fps = 0, fpsCounter = 0;
        private double intervalTime = 0;
        private const double timeThreshold = 1000; //1 second

        SpriteBatch spriteBatch;
        SpriteFont fpsFont;
        public FPS(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);

            fpsFont = this.Game.Content.Load<SpriteFont>("Fonts\\FPSFont");
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            intervalTime += gameTime.ElapsedGameTime.Milliseconds;
            if (intervalTime < timeThreshold)
            {
                fpsCounter++;
            }
            else
            {
                fps = fpsCounter;
                intervalTime = 0;
                fpsCounter = 0;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            NetworkingClient client = this.Game.Services.GetService(typeof(NetworkingClient)) as NetworkingClient;
            
            //this.drawAllWayPoints();
            MouseState mouse = Mouse.GetState();
            spriteBatch.Begin();
            spriteBatch.DrawString(fpsFont, "Frames Per Second: "+fps, new Vector2(10, 10), Color.White);
            //spriteBatch.DrawString(fpsFont, "Frames Per Second: " + mouse.Y, new Vector2(10, 30), Color.White);
            {
                //use player unique identifier to choose an image
                spriteBatch.DrawString(fpsFont, "" + client.otherPlayerInfo.position + ": ", new Vector2(10, 30), Color.White);
            }
            spriteBatch.End();
            //this.device.RasterizerState = prevRs;
            base.Draw(gameTime);
        }
    }
}

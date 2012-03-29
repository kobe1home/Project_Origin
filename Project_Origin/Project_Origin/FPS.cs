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
        private Shooter shooter;
        private Map map;

        SpriteBatch spriteBatch;
        SpriteFont fpsFont;
        public FPS(Game game)
            : base(game)
        {
            this.shooter = game.Services.GetService(typeof(Shooter)) as Shooter;
            if (this.shooter == null)
            {
                throw new InvalidOperationException("Shooter not found.");
            }
            this.map = game.Services.GetService(typeof(Map)) as Map;
            if (this.map == null)
            {
                throw new InvalidOperationException("Map not found.");
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            
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
            String[] commands ={ "Frames Per Second: " + fps,
                                "Command for Playing the Games:",
                                "W, A, S, D : Move the Screen Position.",
                                "Mouse Wheel to Zoom In/Out.",
                                "Mouse Right Click to Select Waypoint.",
                                "Del: Remove the Last Waypoint.",
                                "C: Clear All Waypoint.",
                                "R : Commit Your Move to the Server.",
                                " ",
                                "Debug Information :",
                                "F10 : ReGenerate Map.",
                                "F12 : Switch Between Random VS Optimized Map.",
                                this.map.InternalMap.calculateRandomMapPercentage(),
                                this.map.InternalMap.calculateOptimizedMapPercentage()}; 
                spriteBatch.Begin();
                int y = 10;
                if (this.shooter.GetGameStatus() == Shooter.GameStatus.Start)
                {
                    foreach (String text in commands)
                    {
                        spriteBatch.DrawString(fpsFont, text, new Vector2(10, y), Color.White);
                        y = y + 15;
                    }
                }
                else
                {
                    spriteBatch.DrawString(fpsFont, commands[0], new Vector2(10, y), Color.White);
                }
                spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}

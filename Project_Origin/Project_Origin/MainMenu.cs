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
    public class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Shooter shooter;
        SpriteBatch spriteBatch;

        Vector2 backgroundPicPos;
        Vector2 backgroundPicCenter;
        Texture2D backgroundPicTexture;

        Vector2 menuPos;
        Vector2 menuCenter;
        Texture2D menuTexture;

        Vector2 backgroundPlayerPicPos;
        Vector2 backgroundPlayerPicCenter;
        Texture2D backgroundPlayerPicTexture;

        Song soundEffect;


        public MainMenu(Game game)
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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundPlayerPicTexture = this.Game.Content.Load<Texture2D>("Models\\backgroundPlayer");
            backgroundPlayerPicPos = new Vector2(-20, this.Game.GraphicsDevice.Viewport.Height / 2- 50);
            backgroundPlayerPicCenter = new Vector2(0, 0);

            backgroundPicTexture = this.Game.Content.Load<Texture2D>("Models\\background");
            backgroundPicPos = new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2, this.Game.GraphicsDevice.Viewport.Height / 2);
            backgroundPicCenter = new Vector2(backgroundPicTexture.Width / 2, backgroundPicTexture.Height / 2);

            menuTexture = this.Game.Content.Load<Texture2D>("Models\\mainMenu");
            menuPos = new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2, this.Game.GraphicsDevice.Viewport.Height / 4 * 3);
            menuCenter = new Vector2(menuTexture.Width / 2, menuTexture.Height / 2);

            soundEffect = this.Game.Content.Load<Song>("Sounds\\bgm");
            this.shooter = this.Game.Services.GetService(typeof(Shooter)) as Shooter;
            if (this.shooter == null)
            {
                throw new InvalidOperationException("Shooter not found.");
            }
            MediaPlayer.Play(soundEffect);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (shooter.GetGameStatus() == Project_Origin.Shooter.GameStatus.MainMenu)
            {
                //soundEffect.Play();
                spriteBatch.Begin();
               

                //Draw background
                spriteBatch.Draw(backgroundPicTexture, backgroundPicPos, null, Color.White,
                    0.0f, backgroundPicCenter, 1.0f, SpriteEffects.None, 0.0f);

                //Draw background player image
                spriteBatch.Draw(backgroundPlayerPicTexture, backgroundPlayerPicPos, null, Color.White,
                    0.0f, backgroundPlayerPicCenter, 0.7f, SpriteEffects.None, 0.0f);

                //Draw main menu
                spriteBatch.Draw(menuTexture, menuPos, null, Color.White,
                    0.0f, menuCenter, 0.7f, SpriteEffects.None, 0.0f);

                spriteBatch.End();
            }
            else
                MediaPlayer.Stop();
        }
    }
}

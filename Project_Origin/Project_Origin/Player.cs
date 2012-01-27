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
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Game game;
        private Model playerGreen, playerRed;
        private float playerAlphaTimer;
        private float playerAlphaSpeed;
        private float playerAlpha;
        private int witdth;
        private int height;
        private DefaultEffect defaultEfft;
        KeyboardState prevKeyboardState = Keyboard.GetState();
        Vector3 playerGreenPosition = new Vector3(20.0f, -20.0f, 110.0f);

        SpriteBatch spriteBatch;
        Vector2 playerPosition = new Vector2(100.0f, 29.0f);

        public Player(Game game, int width, int height)
            : base(game)
        {
            // TODO: Construct any child components here
            this.game = game;
            this.witdth = width;
            this.height = height;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            playerAlphaTimer = 0;
            playerAlphaSpeed = 1;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            playerGreen = game.Content.Load<Model>("Models\\playerGreen");
            playerRed = game.Content.Load<Model>("Models\\playerRed");
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Left))
                playerGreenPosition.X -= 0.1f;
            if (keyboard.IsKeyDown(Keys.Right))
                playerGreenPosition.X += 0.1f;

            prevKeyboardState = keyboard;
            base.Update(gameTime);
        }

        public void DrawGreenPlayer(GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[playerGreen.Bones.Count];
            playerGreen.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix world, scale, rotationZ, translation;
            scale = Matrix.CreateScale(1.0f, 1.0f, 1.0f);
            Vector3 position = playerGreenPosition;
            translation = Matrix.CreateTranslation(position);//Matrix.CreateTranslation(20.0f, -20.0f, 110.0f);
            rotationZ = Matrix.CreateRotationZ(MathHelper.Pi/2);
            world = scale * translation;//* rotationZ;
            foreach (ModelMesh mesh in playerGreen.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 250), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                              game.GraphicsDevice.Viewport.AspectRatio,
                                                                              5.0f,
                                                                              1000.0f);
                    effect.Alpha = playerAlpha;

                    mesh.Draw();
                }
            }
        }

        public void DrawRedPlayer(GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[playerRed.Bones.Count];
            playerRed.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix world, scale, rotationZ, translation;
            scale = Matrix.CreateScale(1.0f, 1.0f, 1.0f);
            translation = Matrix.CreateTranslation(-20.0f, 30.0f, 110.0f);
            rotationZ = Matrix.CreateRotationZ(MathHelper.Pi);
            world = scale * rotationZ * translation;
            foreach (ModelMesh mesh in playerRed.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 250), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                              game.GraphicsDevice.Viewport.AspectRatio,
                                                                              5.0f,
                                                                              1000.0f);
                    effect.Alpha = playerAlpha;

                    mesh.Draw();
                }
            }
        }
        public override void Draw(GameTime gameTime)
        {
            //spriteBatch.Begin();
            //spriteBatch.Draw(playerTexture, playerPosition, null, Color.White,
            //    0.0f, playerCenter, 0.1f, SpriteEffects.None, 0.0f);
            //spriteBatch.End();
            float timeElapse = (float)gameTime.ElapsedGameTime.Milliseconds;
            if (playerAlphaTimer > 1000) //2s
            {
                playerAlphaTimer = 1000;
                playerAlphaSpeed *= -1;
            }
            if (playerAlphaTimer < 0) //2s
            {
                playerAlphaTimer = 0;
                playerAlphaSpeed *= -1;
            }
            playerAlphaTimer += timeElapse * playerAlphaSpeed;
            playerAlpha = (float)playerAlphaTimer / 1000.0f;

            DrawGreenPlayer(gameTime);
            DrawRedPlayer(gameTime);

            base.Draw(gameTime);
        }
    }
}

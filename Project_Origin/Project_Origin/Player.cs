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
        private ICameraService camera;
        KeyboardState prevKeyboardState;
        Vector3 playerGreenPosition = new Vector3(30.0f, -20.0f, 10.0f);
        
        SpriteBatch spriteBatch;
        Vector2 introPosition;
        Vector2 introCenter;

        Texture2D introTexture;
        enum GameStatus
        {
            Intro,
            Start
        }

        GameStatus gameStatus = GameStatus.Intro;

        public Player(Game game, int width, int height)
            : base(game)
        {
            // TODO: Construct any child components here
            this.game = game;
            this.witdth = width;
            this.height = height;
            this.prevKeyboardState = Keyboard.GetState();
            this.camera = this.game.Services.GetService(typeof(ICameraService)) as ICameraService;

            if (this.camera == null)
            {
                throw new InvalidOperationException("ICameraService not found.");
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            
            playerAlphaTimer = 0;
            playerAlphaSpeed = 1;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            playerGreen = game.Content.Load<Model>("Models\\playerGreen");
            playerRed = game.Content.Load<Model>("Models\\playerRed");

            introTexture = game.Content.Load<Texture2D>("Models\\Intro");
            introPosition = new Vector2(game.GraphicsDevice.Viewport.Width/2, game.GraphicsDevice.Viewport.Height/2);
            introCenter = new Vector2(introTexture.Width/2, introTexture.Height/2);

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Left))
                playerGreenPosition.X -= 0.1f;
            if (keyboard.IsKeyDown(Keys.Right))
                playerGreenPosition.X += 0.1f;

            if (keyboard.IsKeyDown(Keys.Enter))
                gameStatus = GameStatus.Start;
            if (keyboard.IsKeyDown(Keys.M))
                gameStatus = GameStatus.Intro;

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
                    effect.View = this.camera.ViewMatrix;
                    effect.Projection = this.camera.ProjectMatrix;
                                                                    
                    effect.Alpha = playerAlpha;

                    mesh.Draw();
                }
            }


            BasicEffect lineEffect = new BasicEffect(this.GraphicsDevice);
            lineEffect.VertexColorEnabled = true;
            lineEffect.View = this.camera.ViewMatrix;
            lineEffect.Projection = this.camera.ProjectMatrix; 
                                                               
                                                               
            lineEffect.Alpha = playerAlpha;
            foreach (EffectPass pass in lineEffect.CurrentTechnique.Passes)
            {
                rotationZ = Matrix.CreateRotationZ(MathHelper.Pi / 8);
                lineEffect.World = rotationZ * world;
                pass.Apply();
                VertexPositionColor[] temp = new VertexPositionColor[2];
                temp[0].Position = new Vector3(0, 0, 0);
                temp[0].Color = Color.Green;
                temp[1].Position = new Vector3(0, 65, 0);
                temp[1].Color = Color.Green;
                this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                            temp, 0, 1,
                                            VertexPositionColor.VertexDeclaration);

                rotationZ = Matrix.CreateRotationZ(-MathHelper.Pi / 8);
                lineEffect.World = rotationZ * world;
                pass.Apply();
                this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                            temp, 0, 1,
                                            VertexPositionColor.VertexDeclaration);
            }
        }

        public void DrawRedPlayer(GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[playerRed.Bones.Count];
            playerRed.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix world, scale, rotationZ, translation;
            scale = Matrix.CreateScale(1.0f, 1.0f, 1.0f);
            translation = Matrix.CreateTranslation(-30.0f, 35.0f, 10.0f);
            rotationZ = Matrix.CreateRotationZ(-MathHelper.Pi / 4 * 3);
            world = scale * rotationZ * translation;
            foreach (ModelMesh mesh in playerRed.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = this.camera.ViewMatrix; 
                    effect.Projection = this.camera.ProjectMatrix; 
                    effect.Alpha = playerAlpha;

                    mesh.Draw();
                }
            }


            BasicEffect lineEffect = new BasicEffect(this.GraphicsDevice);
            lineEffect.VertexColorEnabled = true;
            lineEffect.View = this.camera.ViewMatrix;
            lineEffect.Projection = this.camera.ProjectMatrix; 

            lineEffect.Alpha = playerAlpha;
            foreach (EffectPass pass in lineEffect.CurrentTechnique.Passes)
            {
                rotationZ = Matrix.CreateRotationZ(MathHelper.Pi / 8);
                lineEffect.World = rotationZ * world;
                pass.Apply();
                VertexPositionColor[] temp = new VertexPositionColor[2];
                temp[0].Position = new Vector3(0, 0, 0);
                temp[0].Color = Color.Red;
                temp[1].Position = new Vector3(0, 110, 0);
                temp[1].Color = Color.Red;
                this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                            temp, 0, 1,
                                            VertexPositionColor.VertexDeclaration);

                rotationZ = Matrix.CreateRotationZ(-MathHelper.Pi / 8);
                lineEffect.World = rotationZ * world;
                pass.Apply();
                temp[1].Position = new Vector3(0, 80, 0);
                this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                            temp, 0, 1,
                                            VertexPositionColor.VertexDeclaration);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (gameStatus == GameStatus.Intro)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(introTexture, introPosition, null, Color.White,
                    0.0f, introCenter, 1.0f, SpriteEffects.None, 0.0f);
                spriteBatch.End();
            }
            else
            {
                float timeElapse = (float)gameTime.ElapsedGameTime.Milliseconds;
                if (playerAlphaTimer > 1000) //1s
                {
                    playerAlphaTimer = 1000;
                    playerAlphaSpeed *= -1;
                }
                if (playerAlphaTimer < 0) //1s
                {
                    playerAlphaTimer = 0;
                    playerAlphaSpeed *= -1;
                }
                playerAlphaTimer += timeElapse * playerAlphaSpeed;
                playerAlpha = 0.6f + (float)playerAlphaTimer / 1000.0f;

                DrawGreenPlayer(gameTime);
                DrawRedPlayer(gameTime);
                base.Draw(gameTime);
            }
        }
    }
}

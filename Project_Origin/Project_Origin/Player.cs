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
        private ICameraService camera;
        private Path path;
        private Shooter shooter;
        private NetworkingClient networkingClient;

        #region default properties of two kinds of player

        //Below are embedded properties of two kind of player
        static Vector3 greenPlayerPosition = new Vector3(45.0f, -25.0f, 2.0f);
        static float greenPlayerZRoatation = 0.0f;
        static Vector3 redPlayerPosition = new Vector3(-45.0f, 25.0f, 2.0f);
        static float redPlayerZRoatation = MathHelper.Pi;
        static Color greenPlayerColor = Color.Green;
        static Color redPlayerColor = Color.Red;


        #endregion default properties of two kinds of player

        private Model player, opponent = null;
        private Vector3 playerPosition;
        private float playerZRoatation; //Facing direction
        private Vector3 opponentPosition;
        private float opponentZRoatation; //Facing direction

        private Color playerDiffuseColor = Color.White;
        private float sightDistance = 30;
        private float playerAlphaTimer;
        private float playerAlphaSpeed;
        private float playerAlpha;
        private double moveSpeed;
        private BasicEffect lineEffect;
        KeyboardState prevKeyboardState;



        private List<WayPoint> movingWayPoints; //Store all the way points during moving
        //private Vector3 movingSourcePoint;
        private int movingDestinationPointIndex = 0;
        private Vector3 movingDirection;
        private float movingCurrentDistance;
        private float movingSpeed = 0.02f;
        //private SoundEffect soundEffectWalk;
        private SoundEffect soundEffectShoot;

        SpriteBatch spriteBatch;

        public enum PlayerMode
        {
            Normal,
            Moving,
            Shooting
        }
        PlayerMode playerMode = PlayerMode.Normal; 

        public enum PlayerId
        {
            Green,
            Red
        }
        PlayerId playerId = PlayerId.Green;

        public Player(Game game)
            : base(game)
        {
            this.game = game;
            this.prevKeyboardState = Keyboard.GetState();   
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            //Position and orientation
            playerPosition = new Vector3(0.0f, 0.0f, 2.0f);
            playerZRoatation = 0.0f;

            //Animation
            playerAlphaTimer = 0;
            playerAlphaSpeed = 1;
            playerAlpha = 1.0f;
            lineEffect = new BasicEffect(this.Game.GraphicsDevice);
            lineEffect.VertexColorEnabled = true;

            this.camera = this.game.Services.GetService(typeof(ICameraService)) as ICameraService;
            if (this.camera == null)
            {
                throw new InvalidOperationException("ICameraService not found.");
            }

            this.path = this.game.Services.GetService(typeof(Path)) as Path;
            if (this.path == null)
            {
                throw new InvalidOperationException("Path not found.");
            }
            this.shooter = this.game.Services.GetService(typeof(Shooter)) as Shooter;
            if (this.shooter == null)
            {
                throw new InvalidOperationException("Shooter not found.");
            }
            this.networkingClient = this.game.Services.GetService(typeof(NetworkingClient)) as NetworkingClient;
            if (this.shooter == null)
            {
                throw new InvalidOperationException("Networking not found.");
            }

            //this.playerId = networkingClient.GetPlayerId();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            if(playerId == PlayerId.Green)
                player = game.Content.Load<Model>("Models\\playerGreen");
            else
                player = game.Content.Load<Model>("Models\\playerRed");

            //soundEffectWalk = this.Game.Content.Load<SoundEffect>("Sounds\\move");
            soundEffectShoot = this.Game.Content.Load<SoundEffect>("Sounds\\rifleShoot");

            base.LoadContent();
        }

        public void SetPlayId()
        {
            this.playerId = networkingClient.GetPlayerId();
            if (playerId == PlayerId.Green)
            {
                player = game.Content.Load<Model>("Models\\playerGreen");
                playerPosition = greenPlayerPosition;
                playerZRoatation = greenPlayerZRoatation;
                playerDiffuseColor = greenPlayerColor;

                opponent = game.Content.Load<Model>("Models\\playerRed");
                opponentPosition = redPlayerPosition;
                opponentZRoatation = redPlayerZRoatation;
            }
            else
            {
                player = game.Content.Load<Model>("Models\\playerRed");
                playerPosition = redPlayerPosition;
                playerZRoatation = redPlayerZRoatation;
                playerDiffuseColor = redPlayerColor;

                opponent = game.Content.Load<Model>("Models\\playerGreen");
                opponentPosition = greenPlayerPosition;
                opponentZRoatation = greenPlayerZRoatation;
            }
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            /*
            if (keyboard.IsKeyDown(Keys.Left))
                playerPosition.X -= 0.1f;
            if (keyboard.IsKeyDown(Keys.Right))
                playerPosition.X += 0.1f;
            */
            if (keyboard.IsKeyDown(Keys.C))
            {
                if (playerMode != PlayerMode.Moving)
                {
                    path.CleanWayPoints();
                }
            }
            if (keyboard.IsKeyDown(Keys.R))
            {
                movingWayPoints = path.GetWayPoints();
                if (movingWayPoints.Count > 1)
                {
                    //soundEffectWalk.Play(1.0f, 0.0f, 0.0f);
                    movingDestinationPointIndex = 1;
                    playerMode = PlayerMode.Moving;
                }
            }
            if (keyboard.IsKeyDown(Keys.Delete) && prevKeyboardState.IsKeyUp(Keys.Delete))
            {
                path.removeLastWayPoints();
            }
            

            prevKeyboardState = keyboard;
            base.Update(gameTime);
        }

        public float CalcPlayerRotationFromMovingDirection(Vector3 movingDirection)
        {
            float zRotation = 0.0f;
            if (movingDirection.X > 0 && movingDirection.Y > 0)
            {
                zRotation = 2 * MathHelper.Pi - (float)Math.Atan((movingDirection.X / movingDirection.Y));
            }
            else if(movingDirection.X > 0 && movingDirection.Y < 0)
            {
                zRotation = MathHelper.Pi - (float)Math.Atan((movingDirection.X / movingDirection.Y));
            }
            else if (movingDirection.X < 0 && movingDirection.Y < 0)
            {
                zRotation = MathHelper.Pi + (float)Math.Atan((-movingDirection.X / movingDirection.Y));
            }
            else if (movingDirection.X < 0 && movingDirection.Y > 0)
            {
                zRotation = (float)Math.Atan((-movingDirection.X / movingDirection.Y));
            }

            return zRotation;
        }

        public void UpdatePlayerPosition(GameTime gameTime)
        {
            if (playerMode == PlayerMode.Moving)
            {
                Vector3 movingSourcePoint = movingWayPoints[movingDestinationPointIndex - 1].CenterPos;
                Vector3 movingDestinationPoint = movingWayPoints[movingDestinationPointIndex].CenterPos;
                movingDirection = movingDestinationPoint - movingSourcePoint;
                float d = movingDirection.Length();
                movingDirection.Normalize();
                movingCurrentDistance += (float)gameTime.ElapsedGameTime.Milliseconds * movingSpeed;
                playerZRoatation = CalcPlayerRotationFromMovingDirection(movingDirection);
                
                //Destination = Source + d * Direction
                if (movingCurrentDistance < d)
                {
                    Vector3 position = movingSourcePoint + movingCurrentDistance * movingDirection;
                    playerPosition = new Vector3(position.X, position.Y, playerPosition.Z);
                }
                else
                {
                    playerPosition = new Vector3(movingDestinationPoint.X, movingDestinationPoint.Y, playerPosition.Z);
                    movingDestinationPointIndex++;
                    movingCurrentDistance = 0;
                }

                if (movingDestinationPointIndex >= movingWayPoints.Count)
                {
                    path.CleanWayPoints();
                    this.playerMode = PlayerMode.Normal;
                    MediaPlayer.Stop();
                }
            }
        }

        public bool CheckIfWallBlockExist()
        {
            //Use DDA algorithm to calculate the line from player to enemy

            bool bBlockExist = false;

            float increx, increy, x, y;
            int steps, i;

            if (Math.Abs(opponentPosition.X - playerPosition.X) > Math.Abs(opponentPosition.Y - playerPosition.Y))
            {
                steps = (int)Math.Abs(opponentPosition.X - playerPosition.X);
                increx = 2;
                increy = (float)(opponentPosition.Y - playerPosition.Y) / steps;
            }
            else
            {
                steps = (int)Math.Abs(opponentPosition.Y - playerPosition.Y);
                increx = (float)(opponentPosition.X - playerPosition.X) / steps;
                increy = 2;
            }
            x = playerPosition.X;
            y = playerPosition.Y;

            for (i = 1; i <= steps; ++i)
            {
                //Check if block(x, y) is a wall
                x += increx;
                y += increx;
            }

            return bBlockExist;
        }

        public bool CheckIfEnemyInSight()
        {
            bool bEnemyInSight = false;
            if (playerMode != PlayerMode.Shooting)
            {
                //Check if enemy is in line of sight
                Vector2 posDir = new Vector2(opponentPosition.X, opponentPosition.Y) - new Vector2(playerPosition.X, playerPosition.Y);
                if (posDir.Length() > sightDistance / 4.0 * 3.0)
                    return bEnemyInSight;

                posDir.Normalize();

                Vector2 sightDir = new Vector2(0, 1);
                sightDir = Vector2.Transform(sightDir, Matrix.CreateRotationZ(playerZRoatation));
                sightDir.Normalize();

                float ConeThirtyDegreesDotProduct = (float)Math.Cos(MathHelper.ToRadians(30f / 2f));
                if (Vector2.Dot(posDir, sightDir) > ConeThirtyDegreesDotProduct)
                {
                    //If in line of sight, then check if there are some blocks between player and enemy
                    
                    bEnemyInSight = ! CheckIfWallBlockExist();
                }
                
            }
            return bEnemyInSight;
        }

        public void DrawPlayer(GameTime gameTime)
        {
            UpdatePlayerPosition(gameTime);

            Matrix[] transforms = new Matrix[player.Bones.Count];
            player.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix world, scale, rotationZ, translation;
            scale = Matrix.CreateScale(0.02f, 0.02f, 0.02f);
            Vector3 position = playerPosition;
            translation = Matrix.CreateTranslation(position);//Matrix.CreateTranslation(20.0f, -20.0f, 110.0f);
            rotationZ = Matrix.CreateRotationZ(playerZRoatation);
            world = scale * rotationZ * translation;//* ;
            foreach (ModelMesh mesh in player.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = this.camera.ViewMatrix;
                    effect.Projection = this.camera.ProjectMatrix;
                    effect.DiffuseColor = playerDiffuseColor.ToVector3();
                    effect.Alpha = 1.0f; // playerAlpha;

                    mesh.Draw();
                }
            }

            if (CheckIfEnemyInSight() == true && playerMode == PlayerMode.Moving)
            {
                path.CleanWayPoints();
                this.playerMode = PlayerMode.Normal;
                MediaPlayer.Stop();

                soundEffectShoot.Play(1.0f, 0.0f, 0.0f);
                //playerDiffuseColor = Color.Red;
            }
            
            //Draw line of sight
            lineEffect.View = this.camera.ViewMatrix;
            lineEffect.Projection = this.camera.ProjectMatrix; 
                                                                                                                
            lineEffect.Alpha = playerAlpha;
            foreach (EffectPass pass in lineEffect.CurrentTechnique.Passes)
            {
                translation = Matrix.CreateTranslation(position);
                rotationZ = Matrix.CreateRotationZ(playerZRoatation + MathHelper.Pi / 8);
                lineEffect.World = rotationZ * translation;
                pass.Apply();
                VertexPositionColor[] temp = new VertexPositionColor[2];
                temp[0].Position = new Vector3(0, 5, 0);
                temp[0].Color = Color.Gray;
                temp[1].Position = new Vector3(0, sightDistance, 0);
                temp[1].Color = Color.Gray;

                this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                            temp, 0, 1,
                                            VertexPositionColor.VertexDeclaration);

                rotationZ = Matrix.CreateRotationZ(playerZRoatation  - MathHelper.Pi / 8);
                lineEffect.World = rotationZ * translation;
                pass.Apply();
                this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                            temp, 0, 1,
                                            VertexPositionColor.VertexDeclaration);
            }
        }

        public void DrawOpponentPlayer(GameTime gameTime)
        {
            //Update opponent from networking client
            opponentPosition = networkingClient.otherPlayerInfo.position;
            opponentZRoatation = networkingClient.otherPlayerInfo.orientation;

            Matrix[] transforms = new Matrix[opponent.Bones.Count];
            opponent.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix world, scale, rotationZ, translation;
            scale = Matrix.CreateScale(0.02f, 0.02f, 0.02f);
            Vector3 position = opponentPosition;
            translation = Matrix.CreateTranslation(position);//Matrix.CreateTranslation(20.0f, -20.0f, 110.0f);
            rotationZ = Matrix.CreateRotationZ(opponentZRoatation);
            world = scale * rotationZ * translation;//* ;
            foreach (ModelMesh mesh in opponent.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = this.camera.ViewMatrix;
                    effect.Projection = this.camera.ProjectMatrix;

                    effect.Alpha = 1.0f; // playerAlpha;

                    mesh.Draw();
                }
            }

            //Draw line of sight
            lineEffect.View = this.camera.ViewMatrix;
            lineEffect.Projection = this.camera.ProjectMatrix;

            lineEffect.Alpha = playerAlpha;
            foreach (EffectPass pass in lineEffect.CurrentTechnique.Passes)
            {
                translation = Matrix.CreateTranslation(opponentPosition);
                rotationZ = Matrix.CreateRotationZ(opponentZRoatation + MathHelper.Pi / 8);
                lineEffect.World = rotationZ * translation;
                pass.Apply();
                VertexPositionColor[] temp = new VertexPositionColor[2];
                temp[0].Position = new Vector3(0, 5, 0);
                temp[0].Color = Color.Gray;
                temp[1].Position = new Vector3(0, sightDistance, 0);
                temp[1].Color = Color.Gray;

                this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                            temp, 0, 1,
                                            VertexPositionColor.VertexDeclaration);

                rotationZ = Matrix.CreateRotationZ(opponentZRoatation - MathHelper.Pi / 8);
                lineEffect.World = rotationZ * translation;
                pass.Apply();
                this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                            temp, 0, 1,
                                            VertexPositionColor.VertexDeclaration);
            }
        }
        //public void DrawRedPlayer(GameTime gameTime)
        //{
        //    Matrix[] transforms = new Matrix[playerRed.Bones.Count];
        //    playerRed.CopyAbsoluteBoneTransformsTo(transforms);

        //    Matrix world, scale, rotationZ, translation;
        //    scale = Matrix.CreateScale(0.02f, 0.02f, 0.02f);
        //    translation = Matrix.CreateTranslation(-40.0f, 20.0f, 2.0f);
        //    //rotationZ = Matrix.CreateRotationZ(-MathHelper.Pi / 4 * 3);
        //    world = scale /** rotationZ*/ * translation;
        //    foreach (ModelMesh mesh in playerRed.Meshes)
        //    {
        //        foreach (BasicEffect effect in mesh.Effects)
        //        {
        //            effect.EnableDefaultLighting();
        //            effect.World = transforms[mesh.ParentBone.Index] * world;
        //            effect.View = this.camera.ViewMatrix; 
        //            effect.Projection = this.camera.ProjectMatrix; 
        //            effect.Alpha = 1.0f;
        //            effect.DiffuseColor = playerDiffuseColor.ToVector3();

        //            mesh.Draw();
        //        }
        //    }

        //    /*
        //    lineEffect.View = this.camera.ViewMatrix;
        //    lineEffect.Projection = this.camera.ProjectMatrix; 

        //    lineEffect.Alpha = playerAlpha;
        //    foreach (EffectPass pass in lineEffect.CurrentTechnique.Passes)
        //    {
        //        rotationZ = Matrix.CreateRotationZ(MathHelper.Pi / 8);
        //        lineEffect.World = rotationZ * world;
        //        pass.Apply();
        //        VertexPositionColor[] temp = new VertexPositionColor[2];
        //        temp[0].Position = new Vector3(0, 0, 0);
        //        temp[0].Color = Color.Red;
        //        temp[1].Position = new Vector3(0, 110, 0);
        //        temp[1].Color = Color.Red;
        //        this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
        //                                    temp, 0, 1,
        //                                    VertexPositionColor.VertexDeclaration);

        //        rotationZ = Matrix.CreateRotationZ(-MathHelper.Pi / 8);
        //        lineEffect.World = rotationZ * world;
        //        pass.Apply();
        //        temp[1].Position = new Vector3(0, 80, 0);
        //        this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
        //                                    temp, 0, 1,
        //                                    VertexPositionColor.VertexDeclaration);
        //    }*/
        //}

        public override void Draw(GameTime gameTime)
        {
            if ( shooter.GetGameStatus() == Project_Origin.Shooter.GameStatus.Start)
            {

                /*float timeElapse = (float)gameTime.ElapsedGameTime.Milliseconds;
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
                playerAlpha = 0.6f + (float)playerAlphaTimer / 1000.0f;*/

                DrawPlayer(gameTime);
                DrawOpponentPlayer(gameTime);
                base.Draw(gameTime);
            }
        }

        public Vector3 GetPlayerPosition()
        {
            return playerPosition;
        }

        public float GetPlayerOrientation()
        {
            return playerZRoatation;
        }

        public PlayerMode GetPlayerMode()
        {
            return playerMode;
        }

    }
}

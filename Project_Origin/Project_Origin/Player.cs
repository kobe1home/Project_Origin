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

//Green player uses sniper, red player uses handgun
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
        private Map map;

        BasicEffect basicEffect;
        VertexPositionColor[] vertices;

        #region default properties of two kinds of player

        //Below are embedded properties of two kind of player

        //Green player uses sniper, red player uses handgun
        static float sightDistance = 30;
        static float playerTurnTimerThres = 5000; // 5 seconds
        static float playerTurnTimer = 0;

        static Vector3 greenPlayerPosition = new Vector3(78.0f, -38.0f, 1.0f);
        static float greenPlayerZRoatation = MathHelper.PiOver4;
        static float greenPlayerMovingSpeed = 0.01f;
        static float greenPlayerShootingDistance = 20;
        static float greenPlayerWaitingTimerThres = 1000; //1s

        static Vector3 redPlayerPosition = new Vector3(-78.0f, 38.0f, 1.0f);
        static float redPlayerZRoatation = -MathHelper.PiOver4 * 3;
        static float redPlayerMovingSpeed = 0.02f;
        static float redPlayerShootingDistance = 10;
        static float redPlayerWaitingTimerThres = 500; // 0.5s

        static Color greenPlayerColor = Color.Green;
        static Color redPlayerColor = Color.Red;


        #endregion default properties of two kinds of player

        private Model player, opponent = null;
        private Vector3 playerPosition;
        private float prevRotat;
        private float playerZRoatation; //Facing direction
        private float movingSpeed;
        private float shootingDistance;
        private float playerWaitingTimerThres;
        private float playerWaitingTimer;
        
        private Vector3 opponentPosition;
        private float opponentZRoatation; //Facing direction
        private float opponentMovingSpeed;
        private float opponentShootingDistance;
        Boolean[,] internalBoolMap;

        private Color playerDiffuseColor = Color.White;
        
        private float playerAlphaTimer;
        private float playerAlphaSpeed;
        private float playerAlpha;
        private BasicEffect lineEffect;
        KeyboardState prevKeyboardState;

        Vector2 deadSymPos;
        Vector2 deadSymCenter;
        Texture2D deadSymTexture;

        private List<WayPoint> movingWayPoints; //Store all the way points during moving
        private int movingDestinationPointIndex = 0;
        private Vector3 movingDirection;
        private float movingCurrentDistance;

        private List<WayPoint> opponentWayPoints; //Store all the way points during moving
        private int opponentDestinationPointIndex = 0;
        private Vector3 opponentDirection;
        private float opponentCurrentDistance;

        private bool enemyInSight = true;
        private bool playerInEnemySight = false; 

        //private SoundEffect soundEffectWalk;
        private SoundEffect soundEffectShoot;

        //win symbol
        Vector2 winSymbolPos;
        Vector2 winSymbolCenter;
        Texture2D winSymbolTexture;

        //lose symbol
        Vector2 loseSymbolPos;
        Vector2 loseSymbolCenter;
        Texture2D loseSymbolTexture;

        SpriteBatch spriteBatch;

        public enum PlayerMode
        {
            Normal,
            Moving,
            Shooting,
            PlayerWin,
            EnemyWin
        }
        public PlayerMode playerMode = PlayerMode.Normal; 

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
            basicEffect = new BasicEffect(this.game.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (0, this.game.GraphicsDevice.Viewport.Width,     // left, right
                this.game.GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);                                         // near, far plane

            vertices = new VertexPositionColor[2];


            //Position and orientation
            playerPosition = new Vector3(0.0f, 0.0f, 2.0f);
            playerZRoatation = 0.0f;
            movingSpeed = 0.02f;
            shootingDistance = 20;
            playerWaitingTimerThres = 1000; //1s
            playerWaitingTimer = 0;
            playerTurnTimerThres = 5000; //5s
            playerTurnTimer = 0;

            opponentMovingSpeed = 0.02f;
            opponentShootingDistance = 20;


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
            this.map = this.game.Services.GetService(typeof(Map)) as Map;
            if (this.map == null)
            {
                throw new InvalidOperationException("Map not found.");
            }

            internalBoolMap = map.getCurrentDisplayedMapDetail();

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

            deadSymTexture = this.Game.Content.Load<Texture2D>("Models\\deadSymbol");
            deadSymPos = new Vector2(0, 0);//= new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2, this.Game.GraphicsDevice.Viewport.Height / 4 * 3);
            deadSymCenter = new Vector2(deadSymTexture.Width / 2, deadSymTexture.Height / 2);


            winSymbolTexture = this.Game.Content.Load<Texture2D>("Models\\win");
            winSymbolPos = new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2, this.Game.GraphicsDevice.Viewport.Height / 2);
            winSymbolCenter = new Vector2(winSymbolTexture.Width / 2, winSymbolTexture.Height / 2);

            loseSymbolTexture = this.Game.Content.Load<Texture2D>("Models\\lose");
            loseSymbolPos = new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2, this.Game.GraphicsDevice.Viewport.Height / 2);
            loseSymbolCenter = new Vector2(loseSymbolTexture.Width / 2, loseSymbolTexture.Height / 2);


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
                sightDistance = greenPlayerShootingDistance;
                movingSpeed = greenPlayerMovingSpeed;
                shootingDistance = greenPlayerShootingDistance;
                playerWaitingTimer = greenPlayerWaitingTimerThres;

                opponent = game.Content.Load<Model>("Models\\playerRed");
                opponentPosition = redPlayerPosition;
                opponentZRoatation = redPlayerZRoatation;
                opponentMovingSpeed = redPlayerMovingSpeed;
                opponentShootingDistance = redPlayerShootingDistance;
            }
            else
            {
                player = game.Content.Load<Model>("Models\\playerRed");
                playerPosition = redPlayerPosition;
                playerZRoatation = redPlayerZRoatation;
                playerDiffuseColor = redPlayerColor;
                movingSpeed = redPlayerMovingSpeed;
                shootingDistance = redPlayerShootingDistance;
                playerWaitingTimer = redPlayerWaitingTimerThres;

                opponent = game.Content.Load<Model>("Models\\playerGreen");
                opponentPosition = greenPlayerPosition;
                opponentZRoatation = greenPlayerZRoatation;
                opponentMovingSpeed = greenPlayerMovingSpeed;
                opponentShootingDistance = greenPlayerShootingDistance;
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
                    movingDestinationPointIndex = 1;
                    playerMode = PlayerMode.Moving;
                    this.prevRotat = playerZRoatation;
                }
            }
            if (keyboard.IsKeyDown(Keys.Delete) && prevKeyboardState.IsKeyUp(Keys.Delete))
            {
                path.removeLastWayPoints();
            }


            

            if (this.shooter.GetGameStatus() == Shooter.GameStatus.Simulation)
            {
                UpdatePlayerPosition(gameTime);
                if (playerMode == PlayerMode.Moving)
                {
                    this.IncreaseTimer(gameTime);
                }
                   
            }
            else if (this.shooter.GetGameStatus() == Shooter.GameStatus.Start)
            {
                this.CheckVisibility();
                UpdatePlayerPosition(gameTime);
                UpdateOpponentPosition(gameTime);
                this.IncreaseTimer(gameTime);
            }

            prevKeyboardState = keyboard;
            base.Update(gameTime);
        }

        public void StartToMove()
        {
            this.movingWayPoints = path.GetWayPoints();
            this.opponentWayPoints = path.OpponentWayPoints;
            if (movingWayPoints.Count > 1)
            {
                movingDestinationPointIndex = 1;
                playerMode = PlayerMode.Moving;
            }
            if(opponentWayPoints.Count > 1)
            {
                opponentDestinationPointIndex = 1;
                playerMode = PlayerMode.Moving;
            }
            

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

        public void CheckVisibility()
        {
            //Check if enemy in player's sight
            if (CheckIfEnemyInSight() == true)
            {
                if (CheckIfInShootRange(shootingDistance))
                {
                    if (playerMode == PlayerMode.Moving)
                    {
                        //this.playerMode = PlayerMode.Normal;
                        this.playerMode = PlayerMode.PlayerWin;
                    }
                    MediaPlayer.Stop();
                    soundEffectShoot.Play(1.0f, 0.0f, 0.0f);

                    //Player win
                }
                enemyInSight = true;
            }
            else
            {
                enemyInSight = false;
            }

            //Check if player in enemy's sight
            if (CheckIfPlayerInSight() == true)
            {
                if (CheckIfInShootRange(opponentShootingDistance))
                {
                    if (playerMode == PlayerMode.Moving)
                    {
                        //this.playerMode = PlayerMode.Normal;
                        this.playerMode = PlayerMode.EnemyWin;
                    }
                    MediaPlayer.Stop();
                    soundEffectShoot.Play(1.0f, 0.0f, 0.0f);
                    //Enemy win

                    playerInEnemySight = true;
                }
            }
            else
            {
                playerInEnemySight = false;
            }
        }

        public void UpdatePlayerPosition(GameTime gameTime)
        {

            if (playerMode == PlayerMode.Moving)
            {
                if (movingDestinationPointIndex < movingWayPoints.Count)
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

                }
                else
                {
                    if (this.shooter.GetGameStatus() == Shooter.GameStatus.Start)
                    {
                        path.CleanWayPoints();
                        movingWayPoints.Clear();
                    }
                    else if (this.shooter.GetGameStatus() == Shooter.GameStatus.Simulation)
                    {
                        movingDestinationPointIndex = 1;
                        playerPosition = movingWayPoints[0].CenterPos;
                        playerZRoatation = prevRotat;
                        this.playerMode = PlayerMode.Normal;
                        playerTurnTimer = 0;
                        movingCurrentDistance = 0;
                    }
                }
            }

        }

        public void UpdateOpponentPosition(GameTime gameTime)
        {
            if (playerMode == PlayerMode.Moving)
            {
                if (opponentDestinationPointIndex < opponentWayPoints.Count)
                {
                    Vector3 opponentSourcePoint = opponentWayPoints[opponentDestinationPointIndex - 1].CenterPos;
                    Vector3 opponentDestinationPoint = opponentWayPoints[opponentDestinationPointIndex].CenterPos;
                    opponentDirection = opponentDestinationPoint - opponentSourcePoint;
                    float d = opponentDirection.Length();
                    opponentDirection.Normalize();
                    opponentCurrentDistance += (float)gameTime.ElapsedGameTime.Milliseconds * opponentMovingSpeed;
                    opponentZRoatation = CalcPlayerRotationFromMovingDirection(opponentDirection);

                    //Destination = Source + d * Direction
                    if (opponentCurrentDistance < d)
                    {
                        Vector3 position = opponentSourcePoint + opponentCurrentDistance * opponentDirection;
                        opponentPosition = new Vector3(position.X, position.Y, playerPosition.Z);
                    }
                    else
                    {
                        opponentPosition = new Vector3(opponentDestinationPoint.X, opponentDestinationPoint.Y, playerPosition.Z);
                        opponentDestinationPointIndex++;
                        opponentCurrentDistance = 0;
                    }

                }
                else
                {
                    if (this.shooter.GetGameStatus() == Shooter.GameStatus.Start)
                    {
                        opponentWayPoints.Clear();
                        path.OpponentWayPoints.Clear();
                    }
                    else if (this.shooter.GetGameStatus() == Shooter.GameStatus.Simulation)
                    {
                        opponentDestinationPointIndex = 1;
                        opponentPosition = opponentWayPoints[0].CenterPos;
                    }
                    
                }
            }

        }

        public void IncreaseTimer(GameTime gameTime)
        {
            playerTurnTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (playerTurnTimer > playerTurnTimerThres)
            {
                if (this.shooter.GetGameStatus() == Shooter.GameStatus.Start)
                {
                    path.CleanWayPoints();
                    path.OpponentWayPoints.Clear();
                    movingWayPoints.Clear();
                    opponentWayPoints.Clear();
                }
                else
                {
                    playerPosition = path.GetWayPoints()[0].CenterPos;
                    playerZRoatation = prevRotat;
                }
                this.playerMode = PlayerMode.Normal;
                playerTurnTimer = 0;
                this.shooter.SetGameStatus(Shooter.GameStatus.Simulation);
            }
        }

        #region functions check if player can shoot enemy
        
        public bool CheckIfWallBlockExist()
        {
            //Use DDA algorithm to calculate the line from player to enemy

            bool bBlockExist = false;

            float increx, increy, x, y;
            int steps, i;

            if (Math.Abs(opponentPosition.X - playerPosition.X) > Math.Abs(opponentPosition.Y - playerPosition.Y))
            {
                steps = (int)Math.Abs(opponentPosition.X - playerPosition.X);
                increx = (opponentPosition.X - playerPosition.X) / steps;
                increy = (float)(opponentPosition.Y - playerPosition.Y) / steps;
            }
            else
            {
                steps = (int)Math.Abs(opponentPosition.Y - playerPosition.Y);
                increx = (float)(opponentPosition.X - playerPosition.X) / steps;
                increy = (opponentPosition.Y - playerPosition.Y) / steps; ;
            }
            x = playerPosition.X;
            y = playerPosition.Y;

            //vertices[0].Position = playerPosition;
            //vertices[0].Color = Color.Black;
            //vertices[1].Position = opponentPosition;
            //vertices[1].Color = Color.Black;

            for (i = 1; i <= steps; ++i)
            {
                //Check if block(x, y) is a wall
                int boolMapX = (int)((x + this.map.InternalMap.MapPixelWidth / 2) / 2.0f);
                int boolMapY = (int)-((y - this.map.InternalMap.MapPixelHeight / 2) / 2.0f);
                if (internalBoolMap[boolMapY , boolMapX] == false)
                {
                    bBlockExist = true;
                    return bBlockExist;
                }
                //Console.WriteLine(boolMapX +"  "+ boolMapY);
                x += increx;
                y += increy;
            }

            return bBlockExist;
        }

        public bool CheckIfEnemyInSight()
        {
            bool bEnemyInSight = false;
            if (playerMode != PlayerMode.Shooting && playerMode != PlayerMode.Normal )
            {
                //Step 1: Check the distance between two players
                Vector2 posDir = new Vector2(opponentPosition.X, opponentPosition.Y) - new Vector2(playerPosition.X, playerPosition.Y);
                //if (posDir.Length() > sightDistance / 4.0 * 3.0)
                //    return bEnemyInSight;

                //Step 2: Check if the angle is in the field of view
                posDir.Normalize();

                Vector2 sightDir = new Vector2(0, 1);
                sightDir = Vector2.Transform(sightDir, Matrix.CreateRotationZ(playerZRoatation));
                sightDir.Normalize();

                float ConeThirtyDegreesDotProduct = (float)Math.Cos(MathHelper.ToRadians(30f / 2f));
                if (Vector2.Dot(posDir, sightDir) > ConeThirtyDegreesDotProduct)
                {
                    //Step 3: If in line of sight, then check if there are some blocks between player and enemy
                    bEnemyInSight = ! CheckIfWallBlockExist();
                }
                
            }
            return bEnemyInSight;
        }

        public bool CheckIfInShootRange(float range)
        {
            Vector2 posDir = new Vector2(opponentPosition.X, opponentPosition.Y) - new Vector2(playerPosition.X, playerPosition.Y);
            if (posDir.Length() < range)
                return true;
            return false;
        }
        #endregion


        #region functions check if enemy can shoot player
        public bool CheckIfPlayerInSight()
        {
            bool bPlayerInSight = false;
            if (playerMode != PlayerMode.Shooting && playerMode != PlayerMode.Normal)
            {
                //Step 1: Check the distance between two players
                Vector2 posDir = new Vector2(playerPosition.X, playerPosition.Y) - new Vector2(opponentPosition.X, opponentPosition.Y);
                //if (posDir.Length() > sightDistance / 4.0 * 3.0)
                //    return bEnemyInSight;

                //Step 2: Check if the angle is in the field of view
                posDir.Normalize();

                Vector2 sightDir = new Vector2(0, 1);
                sightDir = Vector2.Transform(sightDir, Matrix.CreateRotationZ(opponentZRoatation));
                sightDir.Normalize();

                float ConeThirtyDegreesDotProduct = (float)Math.Cos(MathHelper.ToRadians(30f / 2f));
                if (Vector2.Dot(posDir, sightDir) > ConeThirtyDegreesDotProduct)
                {
                    //Step 3: If in line of sight, then check if there are some blocks between player and enemy
                    bPlayerInSight = !CheckIfWallBlockExist();
                }

            }
            return bPlayerInSight;
        }

        #endregion

        public void DrawPlayer(GameTime gameTime)
        {
            //UpdatePlayerPosition(gameTime);

            Matrix[] transforms = new Matrix[player.Bones.Count];
            player.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix world, scale, rotationZ, translation;
            if(playerId == PlayerId.Green)
                scale = Matrix.CreateScale(0.017f, 0.017f, 0.017f);
            else
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

            if (playerMode == PlayerMode.EnemyWin)
            {
                //Draw dead symbol if player lose
                Vector3 tempPos = this.GraphicsDevice.Viewport.Project(playerPosition, this.camera.ProjectMatrix, this.camera.ViewMatrix, Matrix.Identity);
                deadSymPos.X = tempPos.X;
                deadSymPos.Y = tempPos.Y;

                spriteBatch.Begin();
                spriteBatch.Draw(deadSymTexture, deadSymPos, null, Color.White,
                    0.0f, deadSymCenter, 0.2f, SpriteEffects.None, 0.0f);
                spriteBatch.End();
            }
        }

        public void DrawOpponentPlayer(GameTime gameTime)
        {
            //Update opponent from networking client
            //opponentPosition = networkingClient.otherPlayerInfo.position;
            //opponentZRoatation = networkingClient.otherPlayerInfo.orientation;

            Matrix[] transforms = new Matrix[opponent.Bones.Count];
            opponent.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix world, scale, rotationZ, translation;
            scale = Matrix.CreateScale(0.02f, 0.02f, 0.02f);
            Vector3 position = opponentPosition;
            translation = Matrix.CreateTranslation(position);//Matrix.CreateTranslation(20.0f, -20.0f, 110.0f);
            rotationZ = Matrix.CreateRotationZ(opponentZRoatation);
            world = scale * rotationZ * translation;

            
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


            if (playerMode == PlayerMode.PlayerWin)
            {
                //Draw dead symbol if opponent lose
                Vector3 tempPos = this.GraphicsDevice.Viewport.Project(opponentPosition, this.camera.ProjectMatrix, this.camera.ViewMatrix, Matrix.Identity);
                deadSymPos.X = tempPos.X;
                deadSymPos.Y = tempPos.Y;

                spriteBatch.Begin();
                spriteBatch.Draw(deadSymTexture, deadSymPos, null, Color.White,
                    0.0f, deadSymCenter, 0.2f, SpriteEffects.None, 0.0f);
                spriteBatch.End();
            }
            
        }

        public override void Draw(GameTime gameTime)
        {
            if (shooter.GetGameStatus() == Project_Origin.Shooter.GameStatus.Start ||
                shooter.GetGameStatus() == Shooter.GameStatus.Simulation)
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

                //If enemy is in player sight or player is shot by enemy, show enemy to let player know
                if (enemyInSight || playerInEnemySight)
                {
                    DrawOpponentPlayer(gameTime);
                }

                if (playerMode == PlayerMode.PlayerWin)
                    DrawWinSymbol(gameTime);
                else if (playerMode == PlayerMode.EnemyWin)
                    DrawLoseSymbol(gameTime);

                base.Draw(gameTime);
            }
        }

        public void DrawWinSymbol(GameTime gameTime)
        {
            //Draw title
            spriteBatch.Begin();
            spriteBatch.Draw(winSymbolTexture, winSymbolPos, null, Color.White,
                0.0f, winSymbolCenter, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }

        public void DrawLoseSymbol(GameTime gameTime)
        {
            //Draw title
            spriteBatch.Begin();
            spriteBatch.Draw(loseSymbolTexture, loseSymbolPos, null, Color.White,
                0.0f, loseSymbolCenter, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.End();
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

        public Path Path
        {
            get { return path; }
            set { path = value; }
        }

    }
}

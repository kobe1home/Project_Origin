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
    public class Wall : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Vector3 position;
        private WallNode wall;

        private Game game;
        private CubePrimitive wallCube;
        private GraphicsDevice gdevice;
        private ICameraService camera;

        private static int WallHeight = 3;


        public Wall(Game game, WallNode wallnode, Vector3 postion)
            : base(game)
        {
            this.wall = wallnode;
            this.game = game;
            this.gdevice = this.game.GraphicsDevice;
            this.camera = this.game.Services.GetService(typeof(ICameraService)) as ICameraService;
            this.wallCube = new CubePrimitive(this.gdevice, InternalMap.GridSize);

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

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            Vector3 Position = new Vector3(this.position.X, this.position.Y, this.position.Z);
            WallNode.WallDirection orientation = this.wall.Orientation;


            Matrix world;
            Matrix view = this.camera.ViewMatrix;
            Matrix project = this.camera.ProjectMatrix;
            float z = Position.Z;


            if (orientation == WallNode.WallDirection.Horizontal)
                Position.Y = Position.Y - InternalMap.GridSize * this.wall.Position;
            else
                Position.X = Position.X + InternalMap.GridSize * this.wall.Position;

            for (int index = 0; index < 5; index++)
            {

                for (int height = 0; height < Wall.WallHeight; height++)
                {
                    Position.Z = Position.Z + InternalMap.GridSize;
                    world = Matrix.CreateTranslation(Position);
                    this.wallCube.Draw(world, view, project, Color.Blue);
                }
                if (orientation == WallNode.WallDirection.Horizontal)
                    Position.X = Position.X + InternalMap.GridSize;
                else
                    Position.Y = Position.Y - InternalMap.GridSize;
                Position.Z = z;
            }
            base.Draw(gameTime);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}

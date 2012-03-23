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
    public class Room : Microsoft.Xna.Framework.DrawableGameComponent
    {

        private Vector3 position;
        private RoomNode room;

        private Game game;
        private GraphicsDevice gdevice;
        private ICameraService camera;
        private CubePrimitive wallcube;

        private static int wallHeight = 3;

        private static Color RoomColor = Color.Blue;

        public Room(Game game, RoomNode roomNode, Vector3 position)
            : base(game)
        {
            this.position = position;
            this.game = game;
            this.room = roomNode;
            this.Initialize();
           
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.gdevice = this.game.GraphicsDevice;
            this.camera = this.game.Services.GetService(typeof(ICameraService)) as ICameraService;
            this.wallcube = new CubePrimitive(this.gdevice, InternalMap.GridSize);
            if (this.camera == null)
            {
                throw new InvalidOperationException("ICameraService not found.");
            }
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            Vector3 Position = new Vector3(this.position.X, this.position.Y, this.position.Z);

            Matrix world;
            Matrix view = this.camera.ViewMatrix;
            Matrix project = this.camera.ProjectMatrix;
            float z = Position.Z;

            for (int index = 0; index < this.room.Width; index++)
            {
                if (this.room.IsDoorIndex(index))
                {
                    Position.X = Position.X + InternalMap.GridSize;
                    continue;
                }

                for (int height = 0; height < Room.wallHeight; height++)
                {
                    world = Matrix.CreateTranslation(Position);
                    wallcube.Draw(world, view, project, Room.RoomColor);
                    Position.Z = Position.Z + InternalMap.GridSize;
                }
                Position.X = Position.X + InternalMap.GridSize;
                Position.Z = z;
            }

            Position.X = Position.X - InternalMap.GridSize;


            for (int index = 1; index < this.room.Height; index++)
            {
                Position.Y = Position.Y - InternalMap.GridSize;
                if (this.room.IsDoorIndex(index))
                {
                    continue;
                }

                for (int height = 0; height < Room.wallHeight; height++)
                {
                    world = Matrix.CreateTranslation(Position);
                    wallcube.Draw(world, view, project, Color.Blue);
                    Position.Z = Position.Z + InternalMap.GridSize;
                }
                
                Position.Z = z;
            }

            for (int index = 1; index < this.room.Width; index++)
            {
                Position.X = Position.X - InternalMap.GridSize;
                if (this.room.IsDoorIndex(index))
                {
                    continue;
                }


                for (int height = 0; height < Room.wallHeight; height++)
                {
                    world = Matrix.CreateTranslation(Position);
                    wallcube.Draw(world, view, project, Color.Blue);
                    Position.Z = Position.Z + InternalMap.GridSize;
                }

                Position.Z = z;
            }

            for (int index = 1; index < this.room.Height; index++)
            {
                Position.Y = Position.Y + InternalMap.GridSize;
                if (this.room.IsDoorIndex(index))
                {
                    continue;
                }

                for (int height = 0; height < Room.wallHeight; height++)
                {
                    world = Matrix.CreateTranslation(Position);
                    wallcube.Draw(world, view, project, Color.Blue);
                    Position.Z = Position.Z + InternalMap.GridSize;
                }

                Position.Z = z;
            }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Origin
{
    public class Room : Node
    {

        private DoorDirection direction;
        private Game game;
        private GraphicsDevice gdevice;
        private ICameraService camera;
        private CubePrimitive wallcube;

        public enum DoorDirection
        {
            North,
            South,
            East,
            West
        }

        public Room(Game game): base()
        {
            Random rand = new Random();
            int num = rand.Next(4);
            if (num == 0)
            {
                this.Initialize(game, DoorDirection.North);
            }
            else if (num == 1)
            {
                this.Initialize(game, DoorDirection.East);

            }
            else if (num == 2)
            {
                this.Initialize(game, DoorDirection.South);
            }
            else
            {
                this.Initialize(game, DoorDirection.West);
            }
        }

        public Room(Game game, int width, int height, DoorDirection direction): base(width, height)
        {
            this.Initialize(game,direction);
        }

        private void Initialize(Game game, DoorDirection direction)
        {
            this.direction = direction;

            this.game = game;
            this.gdevice = game.GraphicsDevice;
            this.camera = this.game.Services.GetService(typeof(ICameraService)) as ICameraService;
            this.wallcube = new CubePrimitive(this.gdevice, Map.GridWidth);
            if (this.camera == null)
            {
                throw new InvalidOperationException("ICameraService not found.");
            }
        }

        public override void Display(Vector3 Position)
        {
            Matrix world;
            Matrix view = this.camera.ViewMatrix;
            Matrix project = this.camera.ProjectMatrix;
            float z = Position.Z;

            for (int index = 0; index < base.Width; index++)
            {
                Position.X = Position.X + Map.GridWidth;
                //if (this.direction == DoorDirection.North)
                //{
                    if (index == 2)
                    {
                        continue;
                    }
                //}
                for (int height = 0; height < 3; height++)
                {
                    Position.Z = Position.Z + Map.GridWidth;
                    world = Matrix.CreateTranslation(Position);
                    wallcube.Draw(world, view, project, Color.Blue);
                }
                Position.Z = z;
            }

            for (int index = 1; index < base.Height; index++)
            {
                Position.Y = Position.Y - Map.GridWidth;
                //if (this.direction == DoorDirection.East)
                //{
                    if (index == 2)
                    {
                        continue;
                    }
                //}
                for (int height = 0; height < 3; height++)
                {
                    Position.Z = Position.Z + Map.GridWidth;
                    world = Matrix.CreateTranslation(Position);
                    wallcube.Draw(world, view, project, Color.Blue);
                }
                Position.Z = z;
            }

            for (int index = 1; index < base.Width; index++)
            {
                Position.X = Position.X - Map.GridWidth;
                //if (this.direction == DoorDirection.South)
                //{
                    if (index == 2)
                    {
                        continue;
                    }
                //}
                
                for (int height = 0; height < 3; height++)
                {
                    Position.Z = Position.Z + Map.GridWidth;
                    world = Matrix.CreateTranslation(Position);
                    wallcube.Draw(world, view, project, Color.Blue);
                }
                Position.Z = z;
            }

            for (int index = 1; index < base.Height; index++)
            {
                Position.Y = Position.Y + Map.GridWidth;
                //if (this.direction == DoorDirection.West)
                //{
                    if (index == 2)
                    {
                        continue;
                    }
                //}
                
                for (int height = 0; height < 3; height++)
                {
                    Position.Z = Position.Z + Map.GridWidth;
                    world = Matrix.CreateTranslation(Position);
                    wallcube.Draw(world, view, project, Color.Blue);
                }
                Position.Z = z;
            }

            base.Display(Position);
        }

  
    }
}

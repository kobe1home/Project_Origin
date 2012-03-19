using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Origin
{
    public class RoomNode : Node
    {

        private DoorDirection direction;
        
        public enum DoorDirection
        {
            North,
            South,
            East,
            West
        }

        public RoomNode(): base()
        {
            Random rand = new Random();
            int num = rand.Next(4);
            if (num == 0)
            {
                this.Initialize(DoorDirection.North);
            }
            else if (num == 1)
            {
                this.Initialize(DoorDirection.East);

            }
            else if (num == 2)
            {
                this.Initialize(DoorDirection.South);
            }
            else
            {
                this.Initialize(DoorDirection.West);
            }
        }

        public RoomNode(int width, int height, DoorDirection direction): base(width, height)
        {
            this.Initialize(direction);
        }

        private void Initialize(DoorDirection direction)
        {
            this.direction = direction;
        }

        /*
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
        */
  
    }
}

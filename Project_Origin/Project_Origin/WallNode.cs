using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Project_Origin
{
    class WallNode : Node
    {
        private int position;
        private WallDirection orientation;
        

        public enum WallDirection
        {
            Horizontal,
            Vertial,
        }

        public WallNode(): base()
        {
            Random rand = new Random();

            if (rand.Next(2) == 0)
            {
                this.Intialize(rand.Next(this.Height), WallDirection.Horizontal);
            }
            else
            {
                this.Intialize(rand.Next(this.Width), WallDirection.Vertial);
            }
        }

        public WallNode(int pos, WallDirection dir)
            : base()
        {
            this.Intialize(pos, dir);
        }

        private void Intialize(int pos, WallDirection dir)
        {
            position = pos;
            orientation = dir;
        }

        public int Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public WallDirection Orientation
        {
            get
            {
                return orientation;
            }

            set
            {
                orientation = value;
            }
        }

        /*
public override void Display(Vector3 Position)
{
    Matrix world;
    Matrix view = this.camera.ViewMatrix;
    Matrix project = this.camera.ProjectMatrix;
    float z = Position.Z;


    if (this.orientation == WallDirection.Horizontal)
        Position.Y = Position.Y - Map.GridWidth * this.position;
    else
        Position.X = Position.X + Map.GridWidth * this.position;

    for (int index = 0; index < 5; index++)
    {
                
        for (int height = 0; height < WallNode.WallHeight; height++)
        {
            Position.Z = Position.Z + Map.GridWidth;
            world = Matrix.CreateTranslation(Position);
            this.wallCube.Draw(world, view, project, Color.Blue);
        }
        if (this.orientation == WallDirection.Horizontal)
            Position.X = Position.X + Map.GridWidth;
        else
            Position.Y = Position.Y - Map.GridWidth;
        Position.Z = z;
    }
    base.Display(Position);
}
*/
    }
}

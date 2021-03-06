﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Project_Origin
{
    [Serializable()]
    public class WallNode : Node
    {
        private int position;
        private WallDirection orientation;
        private Random randomGenerator;

        public enum WallDirection
        {
            Horizontal,
            Vertial,
        }

        public WallNode(Random random): base()
        {
            this.randomGenerator = random;

            if (this.randomGenerator.Next()%2 == 0) // horizontal wall
            {
                this.Intialize(this.randomGenerator.Next(this.Height), WallDirection.Horizontal);
            }
            else // vertical wall
            {
                this.Intialize(this.randomGenerator.Next(this.Width), WallDirection.Vertial);
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
    }
}

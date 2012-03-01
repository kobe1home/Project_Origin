using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Origin
{
    public class Room
    {

        private int width;
        private int length;
        private DoorDirection direction;
        private GraphicsDevice gdevice;

        

        public enum DoorDirection
        {
            North,
            South,
            East,
            West
        }

        public Room(GraphicsDevice graphicsDevice, int width, int length, DoorDirection direction)
        {
            this.width = width;
            this.length = length;
            this.direction = direction;
            this.gdevice = graphicsDevice;

        }

        private void Initialize()
        {
            
        }

         
        private void MakeWallCubes()
        {
            CubePrimitive[] wall = new CubePrimitive[this.width * 3 + this.length * 3];

            for (int index = 0; index < width * 3; index++)
            {
                wall[index] = new CubePrimitive(this.gdevice, 2.0f);
            }
        }
    }
}

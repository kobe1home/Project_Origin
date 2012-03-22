using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Origin
{
    [Serializable()]
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

        public Boolean IsDoorIndex(int index)
        {
            if ((base.Width / 2) == index)
                return true;
            if ((base.Width / 2) - 1 == index)
                return true;
            return false;
        }

        private void Initialize(DoorDirection direction)
        {
            this.direction = direction;
        }
    }
}

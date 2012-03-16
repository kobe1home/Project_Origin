using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
namespace Project_Origin
{
    public class Node
    {
        private int width;
        private int height;

        public Node()
        {
            this.width = 5;
            this.height = 5;
        }

        public Node(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public virtual void Display(Vector3 Position)
        {


        }
    }
}

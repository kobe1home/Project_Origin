using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Origin
{
    public class WayPoint : CubePrimitive
    {
        private Vector3 centerPos;
        private CubePrimitive cube;

        public const float CubeSize = 1.0f;

        public WayPoint(GraphicsDevice graphicDevice, Vector3 centerPos):base(graphicDevice,WayPoint.CubeSize)
        {
            this.centerPos = centerPos;
            this.cube = new CubePrimitive(graphicDevice, WayPoint.CubeSize);
        }

        public Vector3 CenterPos
        {
            get { return centerPos; }
            set { centerPos = value; }
        }

        public void Draw(Matrix view, Matrix projection, Color color)
        {
            Matrix world = Matrix.CreateTranslation(this.centerPos);
            //this.cube.Draw(world, view, projection, color);
            base.Draw(world, view, projection, color);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Origin
{
    class WayPoint
    {
        private Vector3 centerPos;
        private VertexPositionColor[] cubPoints;


        private static float CubeSize = 0.5f;

        public WayPoint(Vector3 centerPos)
        {
            this.centerPos = centerPos;
        }

        public Vector3 CenterPos
        {
            get { return centerPos; }
            set { centerPos = value; }
        }

        private void initializeCubePoints()
        {
            this.cubPoints = new VertexPositionColor[4];

            this.cubPoints[0].Position = new Vector3(this.centerPos.X);

        }
    }
}

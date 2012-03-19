using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
namespace Project_Origin
{
    public class InternalMap
    {
        private int pixelwidth;
        private int pixelheight;
        private int numGridsH; // number of small grids in the map
        private int numGridsV;
        private int numNodesH; // number of node in the map
        private int numNodesV;

        private Node[,] internalMapStruct;
        private Boolean[,] detailedInternalMapStruct;

        public static int GridSize = 2;

        public InternalMap(int pixelWidth, int pixelHeight, int nodeWidth, int nodeHeight)
        {
            this.CheckMapSize(pixelWidth, pixelHeight, nodeWidth, nodeHeight);
            this.pixelwidth = pixelWidth;
            this.pixelheight = pixelHeight;
        }

        private void CheckMapSize(int width, int height, int nodeWidth, int nodeHeight)
        {
            this.numGridsH = width / InternalMap.GridSize;
            this.numGridsV = height / InternalMap.GridSize;

            if (this.numGridsH % nodeWidth != 0 || this.numGridsV % nodeHeight != 0)
            {
                throw new System.ArgumentException("Fix map size");
            }

            this.numNodesH = this.numGridsH / nodeWidth;
            this.numNodesV = this.numGridsV / nodeHeight;
            this.internalMapStruct = new Node[this.numNodesH, this.numGridsV];
            this.detailedInternalMapStruct = new Boolean[this.numGridsH, this.numGridsV];

            Console.WriteLine("NodeH: {0} Node: {1} GridsH: {2} GridsV: {3}", this.numNodesH, this.numNodesV, this.numGridsH, this.numGridsV);
        }

        public void GenerateRandomMap()
        {
            Random randNum = new Random();

            for (int row = 0; row < this.numNodesH; row++)
            {
                for (int col = 0; col < this.numNodesV; col++)
                {
                    int randomType = randNum.Next(3);
                    if (randomType == 0)
                    {
                        this.internalMapStruct[row, col] = new EmptyNode();
                    }
                    else if (randomType == 1)
                    {
                        this.internalMapStruct[row, col] = new RoomNode();
                    }
                    else
                    {
                        this.internalMapStruct[row, col] = new WallNode(); ;
                    }
                }
            }
            this.internalMapStruct[0, 0] = new EmptyNode();
            this.internalMapStruct[this.numNodesH - 1, this.numNodesV - 1] = new EmptyNode();
        }

        private void GenerateDetailMap()
        {
            for (int row = 0; row < this.numNodesH; row++)
            {
                for (int col = 0; col < this.numNodesV; col++)
                {
                    
                }
            }

        }

        public void DisplayMap(Vector3 startPos)
        {
            /*
            Vector3 pos = new Vector3(startPos.X, startPos.Y, startPos.Z);
            for (int row = 0; row < this.numNodesH; row++)
            {
                float y = pos.Y;
                for (int col = 0; col < this.numNodesV; col++)
                {
                    this.internalMapStruct[row, col].Display(pos);
                    pos.Y = pos.Y - (5 * InternalMap.GridSize);
                }
                pos.X = pos.X + (5 * InternalMap.GridSize);
                pos.Y = y;
            }
             * */
        }

        

        public int NumGridsWidth
        {
            get { return numGridsH; }
            set { numGridsH = value; }
        }

        public int NumGridsHeight
        {
            get { return numGridsV; }
            set { numGridsV = value; }
        }

        public int MapPixelWidth
        {
            get { return pixelwidth; }
            set { pixelwidth = value; }
        }

        public int MapPixelHeight
        {
            get { return pixelheight; }
            set { pixelheight = value; }
        }

        public int MapNodeWidth
        {
            get { return numNodesH; }
            set { numNodesH = value; }
        }

        public int MapNodeHeight
        {
            get { return numNodesV; }
            set { numNodesV = value; }
        }

        public Node[,] InternalMapStruct
        {
            get { return internalMapStruct; }
        }

        public Boolean[,] DetailedInternalMapStruct
        {
            get { return detailedInternalMapStruct; }
        }
    }
}

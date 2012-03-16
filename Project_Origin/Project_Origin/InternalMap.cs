using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
namespace Project_Origin
{
    public class InternalMap
    {

        private Game game;
        private int pixelwidth;
        private int pixelheight;
        private int numGridsH; // number of small grids in the map
        private int numGridsV; 
        private int numNodesH; // number of node in the map
        private int numNodesV;

        private Node[,] internalMapStruct;
        private Boolean[,] detailedInternalMapStruct;



        public InternalMap(Game game, int pixelWidth, int pixelHeight, int nodeWidth, int nodeHeight)
        {
            this.game = game;
            this.CheckMapSize(pixelWidth, pixelHeight, nodeWidth, nodeHeight);
            this.pixelwidth = pixelWidth;
            this.pixelheight = pixelHeight;
        }

        private void CheckMapSize(int width, int height, int nodeWidth, int nodeHeight)
        {
            this.numGridsH = width / Map.GridWidth;
            this.numGridsV = height / Map.GridWidth;

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
                        this.internalMapStruct[row, col] = new Empty();
                    }
                    else if (randomType == 1)
                    {
                        this.internalMapStruct[row, col] = new Room(this.game);
                    }
                    else
                    {
                        this.internalMapStruct[row, col] = new Wall(this.game); ;
                    }
                }
            }
            this.internalMapStruct[0, 0] = new Empty();
            this.internalMapStruct[this.numNodesH - 1, this.numNodesV - 1] = new Empty();
        }

        public void DisplayMap(Vector3 startPos)
        {
            Vector3 pos = new Vector3(startPos.X, startPos.Y, startPos.Z);
            for (int row = 0; row < this.numNodesH; row++)
            {
                pos.X = pos.X + (5 * Map.GridWidth);
                float y = pos.Y;
                for (int col = 0; col < this.numNodesV; col++)
                {
                    pos.Y = pos.Y - (5 * Map.GridWidth);
                    this.internalMapStruct[row, col].Display(pos);
                }
                pos.Y = y;
            }
        }

        private void GenerateDetailMap()
        {


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

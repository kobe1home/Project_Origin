using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
namespace Project_Origin
{
    [Serializable()]
    public class InternalMap
    {
        private int pixelwidth;
        private int pixelheight;
        private int numGridsH; // number of small grids in the map
        private int numGridsV;
        private int numNodesH; // number of node in the map
        private int numNodesV;

        private Node[,] internalMapStruct;
        private Node[,] optimizedMapStruct;
        private Boolean[,] detailedInternalMapStruct;
        private int randomSeed;
        //private Random randomGenerator;

        public static int GridSize = 2;

        public InternalMap(int pixelWidth, int pixelHeight, int nodeWidth, int nodeHeight, int randomSeed)
        {
            this.CheckMapSize(pixelWidth, pixelHeight, nodeWidth, nodeHeight);
            this.pixelwidth = pixelWidth;
            this.pixelheight = pixelHeight;
            this.randomSeed = randomSeed;

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
            this.internalMapStruct = new Node[this.numNodesV, this.numGridsH];
            this.detailedInternalMapStruct = new Boolean[this.numGridsV, this.numGridsH];
            this.initializeDetailedMap();
        }

        public void GenerateRandomMap()
        {
            Random randomGenerator = new Random(randomSeed);
            for (int row = 0; row < this.numNodesV; row++)
            {
                for (int col = 0; col < this.numNodesH; col++)
                {
                    int randomType = randomGenerator.Next(3);
                    if (randomType == 0)
                    {
                        this.internalMapStruct[row, col] = new RoomNode(this.randomSeed);
                    }
                    else if (randomType == 1)
                    {
                        this.internalMapStruct[row, col] = new WallNode(this.randomSeed);
                    }
                    else
                    {
                        this.internalMapStruct[row, col] = new EmptyNode(); ;
                    }
                }
            }
            this.internalMapStruct[0, 0] = new EmptyNode();
            this.internalMapStruct[this.numNodesV - 1, this.numNodesH - 1] = new EmptyNode();
            this.GenerateDetailMap();
        }

        public void OptimizeMap()
        {
            int minSize = numNodesH;
            if (numNodesV < numNodesH)
                minSize = numNodesV;
            ArrayList rowList1 = new ArrayList();
            ArrayList colList1 = new ArrayList();
            ArrayList rowList2 = new ArrayList();
            ArrayList colList2 = new ArrayList();

            // find suitable spoitions for placing optimal spots
            for (int row = 0; row < this.numNodesV; row++)
            {
                for (int col = 0; col < this.numNodesH; col++)
                {
                    if (distance(0, 0, row, col) >= 0.75 * minSize)
                    {
                        rowList1.Add(row);
                        colList1.Add(col);
                    }
                    if (distance(this.numNodesV - 1, this.numNodesH - 1, row, col) >= 0.75 * minSize)
                    {
                        rowList2.Add(row);
                        colList2.Add(col);
                    }
                }
            }
            // randomly pick four spots
            Random rand = new Random();
            for (int i = 0; i < 4; i++)
                rand.Next(rowList1.Count);
            for (int i = 0; i < 4; i++)
                rand.Next(rowList1.Count);
        }

        private double distance(int node1row, int node1col, int node2row, int node2col)
        {
            return Math.Sqrt((node1row - node2row) * (node1row - node2row) + (node1col - node2col) * (node1col - node2col));
        }

        private void GenerateDetailMap()
        {
            this.initializeDetailedMap();
            for (int row = 0; row < this.numNodesV; row++)
            {
                for (int col = 0; col < this.numNodesH; col++)
                {
                    Node tempNode = this.internalMapStruct[row, col];
                    int rowIndex = row * tempNode.Height;
                    int colIndex = col * tempNode.Width;

                    if (tempNode is RoomNode)
                    {
                        RoomNode room = (RoomNode)tempNode;
                        this.setRoomInDetailedMap(room, rowIndex, colIndex);
                    }
                    else if (tempNode is WallNode)
                    {
                        WallNode wall = (WallNode)tempNode;
                        this.setWallInDetailedMap(wall, rowIndex, colIndex);
                    }
                    if (tempNode is EmptyNode)
                    {
                        EmptyNode empty = (EmptyNode)tempNode;
                    }
                }
            }
        }

        private void initializeDetailedMap()
        {
            for (int row = 0; row < this.numGridsV; row++)
            {
                for (int col = 0; col < this.numGridsH; col++)
                {
                    this.detailedInternalMapStruct[row, col] = true;
                }
            }
        }

        private void setRoomInDetailedMap(RoomNode room, int rowIndex, int colIndex)
        {
            int row = rowIndex;
            int col = colIndex;
            int index = 0;

            for (; col < colIndex + room.Width; col++)
            {
                if (!room.IsDoorIndex(index))
                {
                    this.detailedInternalMapStruct[row, col] = false;
                    this.detailedInternalMapStruct[row + room.Height - 1, col] = false;
                }
                index++;
            }
            col--;
            index = 0;
            for (; row < rowIndex + room.Height - 1; row++)
            {
                if (!room.IsDoorIndex(index))
                {
                    this.detailedInternalMapStruct[row, col] = false;
                    this.detailedInternalMapStruct[row, col - room.Width + 1] = false;
                }
                index++;
            }
        }

        private void setWallInDetailedMap(WallNode wall, int rowIndex, int colIndex)
        {
            int row = rowIndex;
            int col = colIndex;


            if (wall.Orientation == WallNode.WallDirection.Horizontal)
            {
                row = row + wall.Position;
                for (; col < colIndex + wall.Width; col++)
                {
                    this.detailedInternalMapStruct[row, col] = false;
                }
            }
            else
            {
                col = col + wall.Position;
                for (; row < rowIndex + wall.Height; row++)
                {
                    this.detailedInternalMapStruct[row, col] = false;
                }
            }
        }

        public void printMaps()
        {

            for (int row = 0; row < this.numGridsV; row++)
            {
                for (int col = 0; col < this.numGridsH; col++)
                {
                    if (this.detailedInternalMapStruct[row, col])
                    {
                        Console.Write("  ");
                    }
                    else
                    {
                        Console.Write("* ");
                    }
                    //Console.Write("{0} ", this.detailedInternalMapStruct[row, col]);
                }
                Console.WriteLine("");
            }

            for (int row = 0; row < this.numNodesV; row++)
            {
                for (int col = 0; col < this.numNodesH; col++)
                {
                    Node tempNode = this.internalMapStruct[row, col];

                    if (tempNode is RoomNode)
                    {
                        RoomNode room = (RoomNode)tempNode;
                        Console.Write("R ");
                    }
                    else if (tempNode is WallNode)
                    {
                        WallNode wall = (WallNode)tempNode;
                        Console.Write("W ");
                    }
                    if (tempNode is EmptyNode)
                    {
                        EmptyNode empty = (EmptyNode)tempNode;
                        Console.Write("E ");
                    }
                }
                Console.WriteLine("");
            }
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

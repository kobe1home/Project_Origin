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
        private Boolean[,] detailedOptimizedInternalMapStruct;


        private int randomSeed;
        private Random randomGenerator;

        public static int GridSize = 2;

        public InternalMap(int pixelWidth, int pixelHeight, int nodeWidth, int nodeHeight, int randomSeed)
        {
            this.CheckMapSize(pixelWidth, pixelHeight, nodeWidth, nodeHeight);
            this.pixelwidth = pixelWidth;
            this.pixelheight = pixelHeight;
            this.randomSeed = randomSeed;
            this.randomGenerator = new Random(this.randomSeed);

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
            this.optimizedMapStruct = new Node[this.numNodesV, this.numGridsH];
            this.detailedInternalMapStruct = new Boolean[this.numGridsV, this.numGridsH];
            this.detailedOptimizedInternalMapStruct = new Boolean[this.numGridsV, this.numGridsH];
            this.initializeDetailedMap(this.detailedInternalMapStruct);
            this.initializeDetailedMap(this.detailedOptimizedInternalMapStruct);
        }

        public void GenerateRandomMap()
        {
            //Random randomGenerator = new Random(randomSeed);
            for (int row = 0; row < this.numNodesV; row++)
            {
                for (int col = 0; col < this.numNodesH; col++)
                {
                    int randomType = this.randomGenerator.Next(3);
                    if (randomType == 0)
                    {
                        RoomNode tempRoom = new RoomNode(this.randomGenerator);
                        this.internalMapStruct[row, col] = tempRoom;
                        this.optimizedMapStruct[row, col] = tempRoom;
                    }
                    else if (randomType == 1)
                    {
                        WallNode tempWall = new WallNode(this.randomGenerator);
                        this.internalMapStruct[row, col] = tempWall;
                        this.optimizedMapStruct[row, col] = tempWall;
                    }
                    else
                    {
                        this.internalMapStruct[row, col] = new EmptyNode();
                        this.optimizedMapStruct[row, col] = new EmptyNode();
                    }
                }
            }
            this.internalMapStruct[0, 0] = new EmptyNode();
            this.internalMapStruct[this.numNodesV - 1, this.numNodesH - 1] = new EmptyNode();
            this.optimizedMapStruct[0, 0] = new EmptyNode();
            this.optimizedMapStruct[this.numNodesV - 1, this.numNodesH - 1] = new EmptyNode();
            this.GenerateDetailMap(this.internalMapStruct, this.detailedInternalMapStruct);
            this.GenerateDetailMap(this.optimizedMapStruct, this.detailedOptimizedInternalMapStruct);
        }

        public void OptimizeMap()
        {
            /*
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
            // randomly pick four spots and create "safe" spots
            Random rand = new Random();
            for (int i = 0; i < 4; i++)
            {
                int pos = rand.Next(rowList1.Count);
                for (int j = pos - 1; j <= pos + 1; j++)
                    for (int k = pos - 1; k <= pos + 1; k++)
                        this.optimizedMapStruct[(int)rowList1[j], (int)colList1[k]] = new EmptyNode();
            }
            for (int i = 0; i < 4; i++)
            {
                int pos = rand.Next(rowList2.Count);
                for (int j = pos - 1; j <= pos + 1; j++)
                    for (int k = pos - 1; k <= pos + 1; k++)
                        this.optimizedMapStruct[(int)rowList2[j], (int)colList2[k]] = new EmptyNode();
            }
            */

            /*
            float roomNum = 0, wallNum = 0, emptyNum = 0;
            float totalNum = this.numNodesH * this.numNodesV;
            for (int row = 0; row < this.numNodesV; row++)
            {
                for (int col = 0; col < this.numNodesH; col++)
                {
                    Node tempNode = this.optimizedMapStruct[row, col];
                    if (tempNode is RoomNode)
                    {
                        //Console.WriteLine("Wrong");
                        roomNum++;
                    }
                    else if (tempNode is WallNode)
                    {
                        wallNum++;
                    }
                    if (tempNode is EmptyNode)
                    {
                        emptyNum++;
                    }

                }
            }
            float roomRate = (roomNum / totalNum) * 100;
            float wallRate = (wallNum / totalNum) * 100;
            float emptyRate = (emptyNum / totalNum) * 100;

            while (true)
            {
                Console.WriteLine("{0} {1} {2} ", roomRate, wallRate, emptyRate);
                if (roomRate > 20)
                {
                    int next = this.randomGenerator.Next(2);
                    if (next == 0)
                    {
                        replaceNode(new RoomNode(this.randomGenerator));
                        roomRate = ((roomNum + 1) / totalNum) * 100;
                    }
                    else
                    {
                        replaceNode(new WallNode(this.randomGenerator));
                        wallRate = ((wallNum + 1) / totalNum) * 100;
                    }
                    continue;

                }
                else if (wallNum < 40)
                {
                    int next = this.randomGenerator.Next(2);
                    if (next == 0)
                    {
                        //replaceNode(new RoomNode(this.randomGenerator));
                        //roomRate = ((roomNum + 1) / totalNum) * 100;

                    }
                    else
                    {
                        replaceNode(new EmptyNode());
                        emptyRate = ((emptyNum + 1) / totalNum) * 100;
                    }
                    continue;
                }
                else if (emptyRate < 40)
                {
                    int next = this.randomGenerator.Next(2);
                    if (next == 0)
                    {
                        //replaceNode(new RoomNode(this.randomGenerator));
                        //roomRate = ((roomNum + 1) / totalNum) * 100;
                    }
                    else
                    {
                        replaceNode(new WallNode(this.randomGenerator));
                        wallRate = ((wallNum + 1) / totalNum) * 100;
                    }
                    continue;
                }
                else
                {
                    break;
                }

                
            }
            */

            //this.optimizedMapStruct[0, 0] = new RoomNode(this.randomGenerator);
            this.GenerateDetailMap(this.optimizedMapStruct,this.detailedOptimizedInternalMapStruct);
        }

        private void replaceNode(Node newNode)
        {
            while (true)
            {
                int h = this.randomGenerator.Next(this.numNodesH);
                int v = this.randomGenerator.Next(this.numNodesV);
                Node n = this.optimizedMapStruct[v, h];
                if (n.GetType() == newNode.GetType())
                {
                    this.optimizedMapStruct[v, h] = newNode;
                    break;
                }
            }


        }

        private double distance(int node1row, int node1col, int node2row, int node2col)
        {
            return Math.Sqrt((node1row - node2row) * (node1row - node2row) + (node1col - node2col) * (node1col - node2col));
        }

        private void GenerateDetailMap(Node[,] MapStruct, Boolean[,] detailedMapStruct)
        {
            this.initializeDetailedMap(detailedMapStruct);
            for (int row = 0; row < this.numNodesV; row++)
            {
                for (int col = 0; col < this.numNodesH; col++)
                {
                    Node tempNode = MapStruct[row, col];
                    int rowIndex = row * tempNode.Height;
                    int colIndex = col * tempNode.Width;

                    if (tempNode is RoomNode)
                    {
                        RoomNode room = (RoomNode)tempNode;
                        this.setRoomInDetailedMap(detailedMapStruct, room, rowIndex, colIndex);
                    }
                    else if (tempNode is WallNode)
                    {
                        WallNode wall = (WallNode)tempNode;
                        this.setWallInDetailedMap(detailedMapStruct, wall, rowIndex, colIndex);
                    }
                    if (tempNode is EmptyNode)
                    {
                        EmptyNode empty = (EmptyNode)tempNode;
                    }
                }
            }
        }

        public String calculateRandomMapPercentage()
        {
            return "Random Map -->    " + this.CalculatePercentage(this.internalMapStruct);
        }

        public String calculateOptimizedMapPercentage()
        {

            return "Optimized Map --> " + this.CalculatePercentage(this.optimizedMapStruct);
        }

        private String CalculatePercentage(Node[,] mapStruct)
        {
            if (mapStruct == null)
                Console.WriteLine("Wrong");
            float roomNum = 0, wallNum = 0, emptyNum = 0;
            float totalNum = this.numNodesH * this.numNodesV;
            for (int row = 0; row < this.numNodesV; row++)
            {
                for (int col = 0; col < this.numNodesH; col++)
                {
                    Node tempNode = mapStruct[row, col];
                    if (tempNode is RoomNode)
                    {
                        //Console.WriteLine("Wrong");
                        roomNum++;
                    }
                    else if (tempNode is WallNode)
                    {
                        wallNum++;
                    }
                    if (tempNode is EmptyNode)
                    {
                        emptyNum++;
                    }
                    
                }
            }
            float roomRate = (roomNum / totalNum) * 100;
            float wallRate = (wallNum / totalNum) * 100;
            float emptyRate = (emptyNum / totalNum) * 100;
            //Console.WriteLine(roomNum/totalNum);

            return "R: " + roomRate + "% W: " + wallRate + "% E: " + emptyRate + "%";

        }

        private void initializeDetailedMap(Boolean[,] internalDetailedMapStruct)
        {
            //internalDetailedMapStruct = new Boolean[this.numGridsV, this.numGridsH];
            for (int row = 0; row < this.numGridsV; row++)
            {
                for (int col = 0; col < this.numGridsH; col++)
                {
                    internalDetailedMapStruct[row, col] = true;
                }
            }
        }

        private void setRoomInDetailedMap(Boolean[,] internalDetailedMapStruct, RoomNode room, int rowIndex, int colIndex)
        {
            int row = rowIndex;
            int col = colIndex;
            int index = 0;

            for (; col < colIndex + room.Width; col++)
            {
                if (!room.IsDoorIndex(index))
                {
                    internalDetailedMapStruct[row, col] = false;
                    internalDetailedMapStruct[row + room.Height - 1, col] = false;
                }
                index++;
            }
            col--;
            index = 0;
            for (; row < rowIndex + room.Height - 1; row++)
            {
                if (!room.IsDoorIndex(index))
                {
                    internalDetailedMapStruct[row, col] = false;
                    internalDetailedMapStruct[row, col - room.Width + 1] = false;
                }
                index++;
            }
        }

        private void setWallInDetailedMap(Boolean[,] internalDetailedMapStruct, WallNode wall, int rowIndex, int colIndex)
        {
            int row = rowIndex;
            int col = colIndex;


            if (wall.Orientation == WallNode.WallDirection.Horizontal)
            {
                row = row + wall.Position;
                for (; col < colIndex + wall.Width; col++)
                {
                    internalDetailedMapStruct[row, col] = false;
                }
            }
            else
            {
                col = col + wall.Position;
                for (; row < rowIndex + wall.Height; row++)
                {
                    internalDetailedMapStruct[row, col] = false;
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
        public Node[,] OptimizedMapStruct
        {
            get { return optimizedMapStruct; }
        }

        public Boolean[,] DetailedInternalMapStruct
        {
            get { return detailedInternalMapStruct; }
        }

        public Boolean[,] DetailedOptimizedInternalMapStruct
        {
            get { return detailedOptimizedInternalMapStruct; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Origin
{
    public class Map : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Vector3 start;

        private VertexPositionColor[][] pointMatrics;
        private ICameraService camera;
        private GraphicsDevice device;
        private Game game;
        private Shooter shooter;

        KeyboardState prevKeyboardState;

        private BasicEffect defaultEfft;

        private InternalMap internalMap; // Map got from Server
        private DrawableGameComponent[,] drawableRandomMapNode; // converted from internalMap, so that it can be drawn.
        private DrawableGameComponent[,] drawableOptimizedMapNode; // this is a optimized Map we can display both for comparsion


        private static Boolean displayOptimizedMap = false;
        private static Color DefaultColor = Color.Orange;

        public Map(Game game, Vector3 start, int mapSeed)
            : base(game)
        {
            
            this.start = start;
            this.game = game;
            this.device = this.game.GraphicsDevice;
            this.camera = this.game.Services.GetService(typeof(ICameraService)) as ICameraService;
            this.defaultEfft = new BasicEffect(this.device);
            this.prevKeyboardState = Keyboard.GetState();

            if (this.camera == null)
            {
                throw new InvalidOperationException("ICameraService not found.");
            }

            this.internalMap = new InternalMap(160, 80, 8, 8, mapSeed);//new InternalMap(width, heigh, 8, 8);
            this.internalMap.GenerateRandomMap();
            this.internalMap.OptimizeMap();
            this.drawableRandomMapNode = new DrawableGameComponent[this.internalMap.MapNodeHeight, this.internalMap.MapNodeWidth];
            this.drawableOptimizedMapNode = new DrawableGameComponent[this.internalMap.MapNodeHeight, this.internalMap.MapNodeWidth];
            this.convertMapNodes(this.drawableRandomMapNode, this.internalMap.InternalMapStruct);
            this.convertMapNodes(this.drawableOptimizedMapNode, this.internalMap.OptimizedMapStruct);
            //this.internalMap.printMaps();
            
        }

        public override void Initialize()
        {
            int widthNum;
            int heightNum;
            widthNum = this.internalMap.NumGridsWidth;
            heightNum = this.internalMap.NumGridsHeight;
            
            this.pointMatrics = new VertexPositionColor[heightNum][];
 
            for (int row = 0; row < heightNum; row++)
            {
                VertexPositionColor[] temp = new VertexPositionColor[widthNum * 2 + 2];
                for (int col = 0; col <= widthNum; col++)
                {
                    temp[col * 2].Position = new Vector3(this.start.X + (InternalMap.GridSize * col),
                                                     this.start.Y + (this.start.Y + InternalMap.GridSize * row) ,
                                                     this.start.Z);
                    temp[col * 2 + 1].Position = new Vector3(this.start.X + (InternalMap.GridSize * col),
                                                         this.start.Y + (this.start.Y + InternalMap.GridSize * row) +  InternalMap.GridSize,
                                                         this.start.Z);
                    temp[col * 2].Color = Map.DefaultColor;
                    temp[col * 2 + 1].Color = Map.DefaultColor;
                }
                this.pointMatrics[row] = temp;
            }

            this.shooter = this.Game.Services.GetService(typeof(Shooter)) as Shooter;
            if (this.shooter == null)
            {
                throw new InvalidOperationException("Shooter not found.");
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.F12) && prevKeyboardState.IsKeyUp(Keys.F12))
            {
                Map.displayOptimizedMap = Map.displayOptimizedMap ? false : true;
            }
            this.prevKeyboardState = keyboard;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (shooter.GetGameStatus() == Project_Origin.Shooter.GameStatus.Start)
            {
                /*
                RasterizerState prevRs = this.device.RasterizerState;
                RasterizerState rs = new RasterizerState();
                rs.CullMode = CullMode.None;
                rs.FillMode = FillMode.WireFrame;
                this.device.RasterizerState = rs;
                */
                
                // center the map to (0,0,0) point
                
                Vector3 mapPos = new Vector3(-this.internalMap.MapPixelWidth / 2, 
                                             -this.internalMap.MapPixelHeight / 2, 
                                             0.0f);
                this.defaultEfft.VertexColorEnabled = true;
                this.defaultEfft.World = Matrix.CreateTranslation(mapPos);
                this.defaultEfft.View = this.camera.ViewMatrix;
                this.defaultEfft.Projection = this.camera.ProjectMatrix;

                foreach (EffectPass pass in defaultEfft.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    foreach (VertexPositionColor[] row in this.pointMatrics)
                    {
                        this.device.DrawUserPrimitives(PrimitiveType.TriangleStrip,
                                                    row, 0, row.Length - 2,
                                                    VertexPositionColor.VertexDeclaration);
                    }
                }
                
                if (this.internalMap != null)
                    this.DrawMapWalls(gameTime);
                
                //this.device.RasterizerState = prevRs;
            }
            
            base.Draw(gameTime);
        }

        private void convertMapNodes(DrawableGameComponent [,] mapNodes, Node[,] tempMap)
        {
            //mapNodes = new DrawableGameComponent[this.internalMap.MapNodeHeight, this.internalMap.MapNodeWidth];
            //Node[,] tempMap = this.internalMap.InternalMapStruct;

            Vector3 startPosition = new Vector3(-(this.internalMap.MapPixelWidth / 2) + (InternalMap.GridSize / 2),
                                                 (this.internalMap.MapPixelHeight / 2 - InternalMap.GridSize / 2),
                                                 InternalMap.GridSize / 2);
            //Console.WriteLine(startPosition);
            Node tempNode = tempMap[0, 0];
            for (int row = 0; row < this.internalMap.MapNodeHeight; row++)
            {
                float x = startPosition.X;
                for (int col = 0; col < this.internalMap.MapNodeWidth; col++)
                {
                    tempNode = tempMap[row, col];

                    if (tempNode is RoomNode)
                    {
                        Room room = new Room(this.game, (RoomNode)tempNode, 
                                             new Vector3(startPosition.X, startPosition.Y, startPosition.Z));
                        mapNodes[row, col] = room;
                    }
                    else if (tempNode is WallNode)
                    {
                        Wall wall = new Wall(this.game, (WallNode)tempNode, 
                                             new Vector3(startPosition.X, startPosition.Y, startPosition.Z));
                        mapNodes[row, col] = wall;
                    }
                    if (tempNode is EmptyNode)
                    {
                        EmptySpace empty = new EmptySpace(this.game);
                        mapNodes[row, col] = empty;
                    }
                    startPosition.X = startPosition.X + tempNode.Width * InternalMap.GridSize;
                }
                startPosition.X = x;
                startPosition.Y = startPosition.Y - tempNode.Width * InternalMap.GridSize;
            }
        }

        private void DrawMapWalls(GameTime gameTime)
        {
            DrawableGameComponent[,] tempMaps;
            if (Map.displayOptimizedMap)
            {
                tempMaps = this.drawableOptimizedMapNode;
            }
            else
            {
                tempMaps = this.drawableRandomMapNode;
            }
            if (this.internalMap != null)
            {
                for (int row = 0; row < this.internalMap.MapNodeHeight; row++)
                {
                    for (int col = 0; col < this.internalMap.MapNodeWidth; col++)
                    {
                        tempMaps[row, col].Draw(gameTime);
                    }
                }
            }
        }

        public InternalMap InternalMap
        {
            get { return internalMap; }
            set
            {
                internalMap = value;
                this.convertMapNodes(this.drawableRandomMapNode, this.internalMap.InternalMapStruct);
                this.convertMapNodes(this.drawableOptimizedMapNode, this.internalMap.OptimizedMapStruct);
            }
        }

        public Boolean[,] getCurrentDisplayedMapDetail()
        {
            return Map.displayOptimizedMap ? this.internalMap.DetailedOptimizedInternalMapStruct : this.internalMap.DetailedInternalMapStruct;

        }

        public int Witdth
        {
            get { return internalMap.MapPixelWidth; }
        }
        public int Heigh
        {
            get { return internalMap.MapPixelHeight; }
        }

        public Vector3 Start
        {
            get { return start; }
        }
    }
}

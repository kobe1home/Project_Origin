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
        //private int witdth;
        //private int heigh;
        private Vector3 start;

        

        private VertexPositionColor[][] pointMatrics;
        private ICameraService camera;
        private GraphicsDevice device;
        private Game game;
        private Shooter shooter;

        private BasicEffect defaultEfft;
        private static Color DefaultColor = Color.Orange;


        //private InternalMap internalMapNode;
        private InternalMap internalMap; // Map got from Server
        private DrawableGameComponent[,] drawableMapNode; // converted from internalMap, so that it can be drawn.


        //public static int GridWidth = 2;

        public Map(Game game, Vector3 start, int width, int heigh)
            : base(game)
        {
            
            this.start = start;
            //this.witdth = width;
            //this.heigh = heigh;
            this.game = game;
            this.device = this.game.GraphicsDevice;
            this.camera = this.game.Services.GetService(typeof(ICameraService)) as ICameraService;
            this.defaultEfft = new BasicEffect(this.device);

            if (this.camera == null)
            {
                throw new InvalidOperationException("ICameraService not found.");
            }

            this.internalMap = new InternalMap(width, heigh, 5, 5);
            this.internalMap.GenerateRandomMap();
            this.convertMapNodes();
            this.internalMap.printMaps();
            
        }

        public override void Initialize()
        {

            int widthNum = this.internalMap.NumGridsWidth;//his.witdth / Map.GridWidth;
            int heightNum = this.internalMap.NumGridsHeight;//this.heigh / Map.GridWidth;
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
                
                Vector3 mapPos = new Vector3(-this.internalMap.MapPixelWidth / 2, -this.internalMap.MapPixelHeight / 2, 0.0f);
                //Vector3 mapPos = new Vector3(0, 0, 0.0f);
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
                this.DrawMapWalls(gameTime);
                
                
                
                //this.internalMap.DisplayMap(new Vector3(-(this.internalMap.MapPixelWidth / 2 + 1), (this.internalMap.MapPixelHeight / 2 - 1), 0.0f));
                //this.device.RasterizerState = prevRs;
            }
            
            base.Draw(gameTime);
        }

        private void DrawMapWalls(GameTime gameTime)
        {
            if (this.internalMap != null)
            {
                for (int row = 0; row < this.internalMap.MapNodeHeight; row++)
                {
                    for (int col = 0; col < this.internalMap.MapNodeWidth; col++)
                    {
                        this.drawableMapNode[row, col].Draw(gameTime);
                    }
                }
            }

        }

        private void convertMapNodes()
        {
            this.drawableMapNode = new DrawableGameComponent[this.internalMap.MapNodeHeight, this.internalMap.MapNodeWidth];
            Node[,] tempMap = this.internalMap.InternalMapStruct;

            Vector3 startPosition = new Vector3(-(this.internalMap.MapPixelWidth / 2 + 1), (this.internalMap.MapPixelHeight / 2 - 1), 0.1f);

            for (int row = 0; row < this.internalMap.MapNodeHeight; row++)
            {
                float x = startPosition.X;
                for (int col = 0; col < this.internalMap.MapNodeWidth; col++)
                {
                    Node tempNode = tempMap[row, col];

                    if (tempNode is RoomNode)
                    {
                        Room room = new Room(this.game, (RoomNode)tempNode, new Vector3(startPosition.X, startPosition.Y, startPosition.Z));
                        this.drawableMapNode[row, col] = room;
                    }
                    else if (tempNode is WallNode)
                    {
                        Wall wall = new Wall(this.game, (WallNode)tempNode, new Vector3(startPosition.X, startPosition.Y, startPosition.Z));
                        this.drawableMapNode[row, col] = wall;
                    }
                    if (tempNode is EmptyNode)
                    {
                        EmptySpace empty = new EmptySpace(this.game);
                        this.drawableMapNode[row, col] = empty;
                    }
                    startPosition.X = startPosition.X + 5 * InternalMap.GridSize;
                }
                startPosition.X = x;
                startPosition.Y = startPosition.Y - 5 * InternalMap.GridSize;
            }
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

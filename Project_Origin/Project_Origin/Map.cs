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
        private int witdth;
        private int heigh;
        private Vector3 start;


        private VertexPositionColor[][] pointMatrics;
        private ICameraService camera;
        private GraphicsDevice device;
        private Game game;
        private Shooter shooter;

        //private MouseState previousState;
        BasicEffect defaultEfft;

        private static int GridWidth = 2;
        private static Color DefaultColor = Color.Orange;

        public Map(Game game, Vector3 start, int width, int heigh)
            : base(game)
        {
            
            this.start = start;
            this.witdth = width;
            this.heigh = heigh;
            this.game = game;
            this.device = this.game.GraphicsDevice;
            this.camera = this.game.Services.GetService(typeof(ICameraService)) as ICameraService;
            this.defaultEfft = new BasicEffect(this.device);

            if (this.camera == null)
            {
                throw new InvalidOperationException("ICameraService not found.");
            }

           
        }

        public override void Initialize()
        {

            int widthNum = this.witdth / Map.GridWidth;
            int heightNum = this.heigh / Map.GridWidth;
            this.pointMatrics = new VertexPositionColor[heightNum][];
 
            for (int row = 0; row < heightNum; row++)
            {
                VertexPositionColor[] temp = new VertexPositionColor[widthNum * 2 + 2];
                for (int col = 0; col <= widthNum; col++)
                {
                    temp[col * 2].Position = new Vector3(this.start.X + (Map.GridWidth * col),
                                                     this.start.Y + (this.start.Y + Map.GridWidth * row) ,
                                                     this.start.Z);
                    temp[col * 2 + 1].Position = new Vector3(this.start.X + (Map.GridWidth * col),
                                                         this.start.Y + (this.start.Y + Map.GridWidth * row) +  Map.GridWidth,
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
                this.defaultEfft.VertexColorEnabled = true;
                this.defaultEfft.World = Matrix.CreateTranslation(new Vector3(-this.witdth / 2, -this.heigh / 2, 0.0f));
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
            }
            
            //this.device.RasterizerState = prevRs;
            base.Draw(gameTime);
        }

        //Get methods
        /*
        public int getWitdth()
        {
            return this.witdth;
        }
        public int getHeigh()
        {
            return this.heigh;
        }
        public Vector3 getStart()
        {
            return this.start;
        }
        */
        public int Witdth
        {
            get { return witdth; }
        }
        public int Heigh
        {
            get { return heigh; }
        }

        public Vector3 Start
        {
            get { return start; }
        }
    }
}

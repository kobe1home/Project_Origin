using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Origin
{
    public class Map : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private int witdth;
        private int heigh;
        private Vector3 start;
        private VertexPositionColor[][] pointMatrics;

        private GraphicsDevice device;
        private Game game;
        private ICameraService camera;

        private static int GridWidth = 2;
        private static Color DefaultColor = Color.Black;

        public Map(Game game, Vector3 start, int width, int heigh)
            : base(game)
        {
            
            this.start = start;
            this.witdth = width;
            this.heigh = heigh;
            this.game = game;
            this.device = this.game.GraphicsDevice;
            this.camera = this.game.Services.GetService(typeof(ICameraService)) as ICameraService;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //this.defaultEfft = new DefaultEffect(base.Game.Content.Load<Effect>("Effects/Default"));
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            
            RasterizerState prevRs = this.device.RasterizerState;
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.WireFrame;
            this.device.RasterizerState = rs;
            

            Viewport viewport = this.device.Viewport;
            BasicEffect defaultEfft = new BasicEffect(this.GraphicsDevice);

            defaultEfft.VertexColorEnabled = true;
            defaultEfft.World = Matrix.CreateTranslation(new Vector3(-this.witdth/2, -this.heigh/2, 0.0f));
            defaultEfft.View = this.camera.ViewMatrix;
            defaultEfft.Projection = this.camera.ProjectMatrix;


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

            this.device.RasterizerState = prevRs;
            base.Draw(gameTime);
        }

        
    }
}

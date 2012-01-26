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
        private VertexPositionColor[] pointList;
        private DefaultEffect defaultEfft;
        private GraphicsDevice device;
        private Game game;
        

        public Map(Game game, Vector3 start, int width, int heigh)
            : base(game)
        {
            this.start = start;
            this.witdth = width;
            this.heigh = heigh;
            this.game = game;
            this.device = this.game.GraphicsDevice;
        }

        public override void Initialize()
        {
            this.pointList = new VertexPositionColor[4];
            this.pointList[0].Position = new Vector3(this.start.X, this.start.Y - this.heigh, this.start.Z);
            this.pointList[1].Position = this.start;
            this.pointList[2].Position = new Vector3(this.start.X + this.witdth, this.start.Y - this.heigh, this.start.Z);
            this.pointList[3].Position = new Vector3(this.start.X + this.witdth, this.start.Y, this.start.Z);
            

            this.pointList[0].Color = Color.White;
            this.pointList[1].Color = Color.White;
            this.pointList[2].Color = Color.White;
            this.pointList[3].Color = Color.White;

            this.defaultEfft = new DefaultEffect(base.Game.Content.Load<Effect>("Effects/Default"));
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
            Viewport viewport = this.device.Viewport;
            this.defaultEfft.World = Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, -6.0f));
            this.defaultEfft.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                              viewport.AspectRatio,
                                                                              5.0f,
                                                                              100.0f);

            foreach (EffectPass pass in this.defaultEfft.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 
                                                this.pointList, 0, 2, 
                                                VertexPositionColor.VertexDeclaration);
            }
            base.Draw(gameTime);
        }
    }
}

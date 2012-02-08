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

        private List<WayPoint> points = new List<WayPoint>();
        private List<VertexPositionColor> lines = new List<VertexPositionColor>();

        private MouseState previousState;

        private static int GridWidth = 2;
        private static Color DefaultColor = Color.Blue;

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

            this.previousState = Mouse.GetState();
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
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            CheckMouseClick();

        }

        public override void Draw(GameTime gameTime)
        {
            /*
            RasterizerState prevRs = this.device.RasterizerState;
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.WireFrame;
            this.device.RasterizerState = rs;
            */
            Viewport viewport = this.device.Viewport;
            BasicEffect defaultEfft = new BasicEffect(this.GraphicsDevice);

            defaultEfft.VertexColorEnabled = true;
            defaultEfft.World = Matrix.CreateTranslation(new Vector3(-this.witdth/2, -this.heigh/2, 0.0f));
            defaultEfft.View = this.camera.ViewMatrix;
            defaultEfft.Projection = this.camera.ProjectMatrix;
            //defaultEfft.LightingEnabled = true;
            
            

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


            this.drawAllWayPoints();
            
            //this.device.RasterizerState = prevRs;
            base.Draw(gameTime);
        }

        private void CheckMouseClick()
        {
            MouseState mouseState = Mouse.GetState();

            
            if (mouseState.RightButton == ButtonState.Pressed && this.previousState.RightButton == ButtonState.Released)
            {

                Ray pickRay = GetPickRay(mouseState);

                BoundingBox box = new BoundingBox(this.start - new Vector3(this.witdth / 2, this.heigh / 2, 0.0f), this.start + new Vector3(this.witdth / 2, this.heigh / 2, 0.0f));

                Nullable<float> result = pickRay.Intersects(box);
                if (result.HasValue == true)
                {
                    Vector3 selectedPoint = pickRay.Position + pickRay.Direction * result.Value;
                    WayPoint waypoint = new WayPoint(this.game.GraphicsDevice, selectedPoint + new Vector3(0.0f, 0.0f, WayPoint.CubeSize / 2));
                    this.points.Add(waypoint);
                    this.lines.Add(new VertexPositionColor(waypoint.CenterPos, Color.White));
                }
            }
            this.previousState = mouseState;
        }

        private Ray GetPickRay(MouseState mouseState)
        {
            //MouseState mouseState = Mouse.GetState();

            int mouseX = mouseState.X;
            int mouseY = mouseState.Y;

            Vector3 nearsource = new Vector3((float)mouseX, (float)mouseY, 0f);
            Vector3 farsource = new Vector3((float)mouseX, (float)mouseY, 1f);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource,
                this.camera.ProjectMatrix, this.camera.ViewMatrix, world);

            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource,
                this.camera.ProjectMatrix, this.camera.ViewMatrix, world);

            // Create a ray from the near clip plane to the far clip plane.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray pickRay = new Ray(nearPoint, direction);

            return pickRay;
        }

        private void drawAllWayPoints()
        {
            if (this.points.Count == 1)
            {
                WayPoint pointStart = this.points[0];
                pointStart.Draw(this.camera.ViewMatrix, this.camera.ProjectMatrix, Color.Red);
            }
            if (this.points.Count >= 2)
            {
                for (int index = 0; index < this.points.Count; index++)
                {
                    WayPoint point = this.points[index];
                    point.Draw(this.camera.ViewMatrix, this.camera.ProjectMatrix, Color.Red);
                }
                this.drawAllLines();
            }
        }

        private void drawAllLines()
        {
            BasicEffect defaultEfft = new BasicEffect(this.GraphicsDevice);

            defaultEfft.VertexColorEnabled = true;
            defaultEfft.World = Matrix.Identity;
            defaultEfft.View = this.camera.ViewMatrix;
            defaultEfft.Projection = this.camera.ProjectMatrix;

            foreach (EffectPass pass in defaultEfft.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip,
                                                                    this.lines.ToArray(), 0,
                                                                    this.lines.Count - 1);
            }
  
        }
    }
}

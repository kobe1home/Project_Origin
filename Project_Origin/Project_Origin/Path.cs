using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Project_Origin
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Path : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private ICameraService camera;
        private Map gameMap;

        private MouseState previousState;
        private List<WayPoint> points = new List<WayPoint>();
        private List<VertexPositionColor> lines = new List<VertexPositionColor>();

        public Path(Game game)
            : base(game)
        {
            this.previousState = Mouse.GetState();
            // TODO: Construct any child components here
            this.camera = this.Game.Services.GetService(typeof(ICameraService)) as ICameraService;

            if (this.camera == null)
            {
                throw new InvalidOperationException("ICameraService not found.");
            }

            this.gameMap = this.Game.Services.GetService(typeof(Map)) as Map;

            if (this.gameMap == null)
            {
                throw new InvalidOperationException("Map not found.");
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            CheckMouseClick();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
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

                BoundingBox box = new BoundingBox(this.gameMap.getStart() - new Vector3(this.gameMap.getWitdth() / 2, this.gameMap.getHeigh() / 2, 0.0f), this.gameMap.getStart() + new Vector3(this.gameMap.getWitdth() / 2, this.gameMap.getHeigh() / 2, 0.0f));

                Nullable<float> result = pickRay.Intersects(box);
                if (result.HasValue == true)
                {
                    Vector3 selectedPoint = pickRay.Position + pickRay.Direction * result.Value;
                    WayPoint waypoint = new WayPoint(this.Game.GraphicsDevice, selectedPoint + new Vector3(0.0f, 0.0f, WayPoint.CubeSize / 2));
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
                this.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip,
                                                                    this.lines.ToArray(), 0,
                                                                    this.lines.Count - 1);
            }

        }
    }
}

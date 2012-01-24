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
    /// This is the main type for your game
    /// </summary>
    public class Shooter : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState prevKeyboardState = Keyboard.GetState();



        DefaultEffect effect;
        VertexDeclaration vertexDecl;
        Matrix triangleTransform;
        Matrix rectangleTransform;

        Vector3[] triangleData;
        Vector3[] rectangleData;


        public Shooter()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.Title = "Shooter Game";
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            triangleTransform = Matrix.CreateTranslation(new Vector3(-1.5f, 0.0f, -6.0f));
            rectangleTransform = Matrix.CreateTranslation(new Vector3(1.5f, 0.0f, -6.0f));

            // Initialize the triangle's data
            triangleData = new Vector3[3];
            triangleData[0] = new Vector3(1.0f, -1.0f, 0.0f);
            triangleData[1] = new Vector3(-1.0f, -1.0f, 0.0f);
            triangleData[2] = new Vector3(0.0f, 1.0f, 0.0f);

            // Initialize the Rectangle's data
            rectangleData = new Vector3[4];
            rectangleData[0] = new Vector3(-1.0f, -1.0f, 0.0f);
            rectangleData[1] = new Vector3(-1.0f, 1.0f, 0.0f);
            rectangleData[2] = new Vector3(1.0f, -1.0f, 0.0f);
            rectangleData[3] = new Vector3(1.0f, 1.0f, 0.0f);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            Effect tempEffect = Content.Load<Effect>("Effects/Default");
            effect = new DefaultEffect(tempEffect);
            tempEffect = null;

            ResetProjection();

            vertexDecl = new VertexDeclaration(new VertexElement[] {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
                }
            );
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();

            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboard.IsKeyDown(Keys.F11) && prevKeyboardState.IsKeyUp(Keys.F11))
                this.IsFullScreen = !this.IsFullScreen;

            prevKeyboardState = keyboard;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            effect.World = triangleTransform;
            effect.CurrentTechnique.Passes[0].Apply();

            GraphicsDevice.DrawUserPrimitives<Vector3>(PrimitiveType.TriangleStrip,
                triangleData, 0, 1, vertexDecl);

            effect.World = rectangleTransform;
            effect.CurrentTechnique.Passes[0].Apply();

            GraphicsDevice.DrawUserPrimitives<Vector3>(PrimitiveType.TriangleStrip,
                rectangleData, 0, 2, vertexDecl);
            base.Draw(gameTime);
        }


        protected bool IsFullScreen
        {
            get { return graphics.IsFullScreen; }
            set
            {
                if (value != graphics.IsFullScreen)
                {
                    // Toggle FullScreen, and Mouse Display, then apply the changes
                    // on the DeviceManager
                    graphics.IsFullScreen = !graphics.IsFullScreen;
                    IsMouseVisible = !IsMouseVisible;
                    graphics.ApplyChanges();
                }
            }
        }

        protected void OnClientSizeChanged(object sender, EventArgs e)
        {
            ResetProjection();
        }
        protected void ResetProjection()
        {
            Viewport viewport = graphics.GraphicsDevice.Viewport;

            // Set the Projection Matrix
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                (float)viewport.Width / viewport.Height,
                0.1f,
                100.0f);
        }
    }
}

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
        public bool bIntro { get; set; } 
        //private SpriteBatch spriteBatch;
        KeyboardState prevKeyboardState;


        private Camera camera;
        private Map gameMap;
        private Player gamePlayer;
        private Path path;
        private FPS fps;


        public Shooter()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;

            Window.Title = "Shooter Game";
            Window.AllowUserResizing = true;
            //Window.ClientSizeChanged += OnClientSizeChanged;
            this.prevKeyboardState = Keyboard.GetState();
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.camera = new Camera(this , new Vector3(0,0,100), 
                                        new Vector3(1,0, 0), 
                                        new Vector3(0, 1, 0), 
                                        new Vector3(0,0,0));

            this.Components.Add(this.camera);
            this.Services.AddService(typeof(ICameraService), this.camera);

            this.gameMap = new Map(this, new Vector3(0, 0, 0), 100, 60);
            this.Components.Add(this.gameMap);
            this.Services.AddService(typeof(Map), this.gameMap);

            this.path = new Path(this);
            this.Components.Add(this.path);
            this.Services.AddService(typeof(Path), this.path);

            this.fps = new FPS(this);
            this.Components.Add(fps);
            //this.gamePlayer = new Player(this, 100, 60);
            //this.Components.Add(this.gamePlayer);
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
            base.LoadContent();
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
       
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TruffleSnuffle
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera gameCamera = new Camera();

        // Game Objects
        GameObject player = new GameObject();
        GameObject truffle = new GameObject();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Set up game window
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width / 3;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height / 3;
            graphics.ApplyChanges();

            // Set up GameObjects
            truffle.position.X = 200f;
            truffle.rotation.X = 0.2f;
            truffle.scale = new Vector3(3f);
            truffle.collisionScale = new Vector3(50f, 75f, 50f);
            truffle.collisionOffset = new Vector3(0, 100f, 0);
            truffle.boundingType = GameObject.BoundingType.BOX;
            player.collisionScale = new Vector3(75f, 50f, 75f);
            player.collisionOffset = new Vector3(0, 100f, 0);
            player.boundingType = GameObject.BoundingType.SPHERE;

            // Game camera setup
            gameCamera.offset = new Vector3(0f, 200f, -800f);
            gameCamera.target = player.position;

            // Debug setup
            BoundingRenderer.InitializeGraphics(graphics.GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Load models
            player.LoadModel(Content, "PigN");
            truffle.LoadModel(Content, "TruffleN");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // Make the truffle rotate a bit each frame
            // (really this should be scaled based on gameTime)
            truffle.rotation.Y += 0.1f;

            
            // ------------------------------
            // INPUT
            // ------------------------------

            // Rotate left / right
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                player.rotation.Y += 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                player.rotation.Y -= 0.1f;

            // Move forward
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                player.acceleration.X = (float)Math.Sin(player.rotation.Y) * 1000f;
                player.acceleration.Z = (float)Math.Cos(player.rotation.Y) * 1000f;
            }
            else
            {
                player.acceleration = Vector3.Zero;
            }

            // ------------------------------
            // UPDATE GAME OBJECTS
            // ------------------------------
            player.Update(gameTime);
            truffle.Update(gameTime);

            gameCamera.target = player.position;

            // ------------------------------
            // CHECK FOR COLLISIONS
            // ------------------------------
            // Clear the lists because we'll be adding to them again, clear out anything that isn't colliding anymore
            player.collidingWith.Clear();
            truffle.collidingWith.Clear();
            // Check if collision is active
            player.DetectCollision(truffle);


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // Draw GameObjects
            player.Draw(gameCamera);
            truffle.Draw(gameCamera);

            base.Draw(gameTime);
        }
    }
}

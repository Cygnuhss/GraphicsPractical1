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

namespace GraphicsPractical1
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private InputHelper inputHelper;
        private FrameRateCounter frameRateCounter;

        private BasicEffect effect;
        private Camera camera;

        private Terrain terrain;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.inputHelper = new InputHelper();
            this.frameRateCounter = new FrameRateCounter(this);
            this.Components.Add(this.frameRateCounter);
        }

        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.IsFullScreen = false;
            this.graphics.SynchronizeWithVerticalRetrace = false;
            this.graphics.ApplyChanges();

            this.IsFixedTimeStep = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            this.effect = new BasicEffect(this.GraphicsDevice);
            this.effect.VertexColorEnabled = true;
            this.effect.LightingEnabled = true;
            this.effect.DirectionalLight0.Enabled = true;
            this.effect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
            this.effect.DirectionalLight0.Direction = new Vector3(0, -1, 0);
            this.effect.AmbientLightColor = new Vector3(0.3f);

            this.camera = new Camera(new Vector3(60, 80, -80), new Vector3(0, 0, 0),
                new Vector3(0, 1, 0));

            // Load terrain from a heightmap image.
            Texture2D map = Content.Load<Texture2D>("heightmap");
            //this.terrain = new Terrain(new HeightMap(map), 0.2f, this.GraphicsDevice);

            // Generate terrain with Perlin noise.
            int width = 128;
            int height = 128;
            // Set the smoothness of the terrain.
            // A low octave count will generate a 'spiky' terrain, while a high value will generate a smoother terrain.
            // Octaves between 6 and 14 give the most interesting visual results.
            int octave = 8;
            this.terrain = new Terrain(width, height, octave, 0.2f, this.GraphicsDevice);
        }

        protected override void UnloadContent()
        {
          
        }

        protected override void Update(GameTime gameTime)
        {
            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            this.Window.Title = "Graphics Tutorial | FPS: " + this.frameRateCounter.FrameRate;

            float deltaAngleY = 0;
            float deltaAngleX = 0;
            KeyboardState kbState = Keyboard.GetState();

            // Use the Left and Right arrow keys to rotate the camera in the Y-axis.
            if (kbState.IsKeyDown(Keys.Left))
                deltaAngleY += -3 * timeStep;
            if (kbState.IsKeyDown(Keys.Right))
                deltaAngleY += 3 * timeStep;
            // Use the Up and Down arrow keys to rotate the camera in the X-axis.
            if (kbState.IsKeyDown(Keys.Up))
                deltaAngleX += -3 * timeStep;
            if (kbState.IsKeyDown(Keys.Down))
                deltaAngleX += 3 * timeStep;

            if (deltaAngleY != 0)
            {
                this.camera.Eye = Vector3.Transform(this.camera.Eye, Matrix.CreateRotationY(deltaAngleY));
            }
            if (deltaAngleX != 0)
            {
                this.camera.Eye = Vector3.Transform(this.camera.Eye, Matrix.CreateRotationX(deltaAngleX));
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                FillMode = FillMode.Solid
            };

            GraphicsDevice.Clear(Color.CornflowerBlue);

            this.effect.Projection = this.camera.ProjectionMatrix;
            this.effect.View = this.camera.ViewMatrix;
            Matrix translation = Matrix.CreateTranslation(-0.5f * this.terrain.Width, 0,
                0.5f * this.terrain.Width);
            this.effect.World = translation;


            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            this.terrain.Draw(this.GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}
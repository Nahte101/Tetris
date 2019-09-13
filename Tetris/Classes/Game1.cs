using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tetris
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        bool start;
        TetrisGameField gameField;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameField = new TetrisGameField(spriteBatch, graphics);
            start = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            if (!start)
            { 
                gameField.chooseBlock();
                start = true;
            }
            if (!gameField.gameOver)
            {
                gameField.bottomCollide();
                gameField.update(gameTime);
                gameField.resetField();
                gameField.placeBlock();
                gameField.generateFallCollision();
                gameField.generateLSideCollision();
                gameField.generateRSideCollision();

                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            gameField.drawOutline(spriteBatch);
            gameField.drawPlayField(spriteBatch);
            gameField.drawDebugStats(spriteBatch, font);
            //gameField.drawCollision(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

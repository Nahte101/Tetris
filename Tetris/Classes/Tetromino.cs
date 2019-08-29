using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tetris
{
    class Tetromino
    {
        public static bool[,] I = { { true, false, false, false }, { true, false, false, false }, { true, false, false, false }, { true, false, false, false } };
        public static bool[,] J = { { true, true, false, false }, { true, false, false, false }, { true, false, false, false }, { false, false, false, false } };
        public static bool[,] L = { { true, true, false, false }, { false, true, false, false }, { false, true, false, false }, { false, false, false, false } };
        public static bool[,] O = { { true, true, false, false }, { true, true, false, false }, { false, false, false, false }, { false, false, false, false } };
        public static bool[,] S = { { true, false, false, false }, { true, true, false, false }, { false, true, false, false }, { false, false, false, false } };
        public static bool[,] T = { { true, false, false, false }, { true, true, false, false }, { true, false, false, false }, { false, false, false, false } };
        public static bool[,] Z = { { false, true, false, false }, { true, true, false, false }, { true, false, false, false }, { false, false, false, false } };

        private bool[,] pieces;
        //x and why are indexs inside of the playfield relative to the top left of pieces in this class
        private int x = 5;
        private double y = 0d;
        private double xMovTimer = 0.5d;
        private double yMovTimer = 0.3d;
        private bool isFallen;
        public bool[,] Pieces { get { return pieces; } }
        public int X { get { return x; } }
        public int Y { get { return (int)y; } }
        public double XMovTimer { get { return xMovTimer; } set { value = xMovTimer; } }

        public Tetromino(bool[,] pieces)
        {
            this.pieces = pieces;//Might cause problems since it is a shallow copy
        }
        public bool isBlockFallen()
        {
            return (24 + emptyRowsFromBottom() == y);
        }
        public void update(GameTime gameTime, bool xCollide)
        {
            KeyboardState kState = Keyboard.GetState();
            movX(kState, gameTime, xCollide);
            fall(gameTime);
            yMovTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            speedUp(kState);
        }
        public void movX(KeyboardState kState, GameTime gameTime, bool xCollide)
        {
            if (!xCollide)
            {
                if (x > 0 && kState.IsKeyDown(Keys.Left) && xMovTimer <= 0)
                {
                    x--;
                    xMovTimer = 0.5d;
                }
                else if (x < 6 + emptyColumnsFromRight() && kState.IsKeyDown(Keys.Right) && xMovTimer <= 0)
                {
                    x++;
                    xMovTimer = 0.5d;
                }
                else
                {
                    xMovTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }
        public void speedUp(KeyboardState kState)
        {
            if (kState.IsKeyDown(Keys.Down))
            {
                yMovTimer -= 0.1;
            }
        }
        public int emptyColumnsFromRight()
        {
            int columnCounter = 0;
            for(int c = 3; c >= 0; c--)
            {
                int falseCounter = 0;
                for(int i = 0; i < 4; i++)
                {
                    if(pieces[i,c] == false)
                    { falseCounter++; }
                    if(falseCounter == 4)
                    { columnCounter++; }
                }
            }
            return columnCounter;
        }

        public int emptyRowsFromBottom()
        {
            int rowCounter = 0;
            for(int i = 3; i >= 0  ;i--)
            {
                int falseCounter = 0;
                for(int c = 0; c < 4 ; c++ )
                {
                    if(pieces[i,c] == false)
                    {
                        falseCounter++;
                    }
                    if(falseCounter == 4)
                    {
                        rowCounter++;
                    }
                }
            }
            return rowCounter;
        }

        public void drawPieces(SpriteBatch spriteBatch,int x, int y)
        {
            Texture2D green = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            green.SetData(new Color[] { Color.ForestGreen });

            Texture2D red = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            red.SetData(new Color[] { Color.Red });

            Vector2 pos = new Vector2(600, 50);
            int spacing = 5;
            int size = 10;

            for (int i = 0; i < pieces.GetLength(0); i++)
            {
                for (int c = 0; c < pieces.GetLength(1); c++)
                {
                    if (pieces[i, c] == true)
                    {
                        spriteBatch.Draw(green, new Rectangle((((int)pos.X - 500 + ((c + 1)) * size) + spacing * c),
                             ((int)pos.Y + 200 + ((i + 1) * size) + spacing * i), 10, 10), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(red, new Rectangle((((int)pos.X - 500 + ((c + 1)) * size) + spacing * c),
                             ((int)pos.Y + 200 + ((i + 1) * size) + spacing * i), 10, 10), Color.White);
                    }
                }
            }
        }
        public void fall(GameTime gameTime)
        {
            switch(emptyRowsFromBottom())
            {
                case 0:
                    if ((int)y + 4 < 28 && yMovTimer <=0)
                    {
                        y += 1;
                        yMovTimer = 0.5d;
                    }
                    break;
                case 1:
                    if((int)y + 3 < 28 && yMovTimer <= 0)
                    {
                        y += 1 ;
                        yMovTimer = 0.5d;
                    }
                    break;
                case 2:
                    if ((int)y + 2 < 28 && yMovTimer <= 0)
                    {
                        y += 1;
                        yMovTimer = 0.5d;
                    }
                    break;
                case 3:
                    if ((int)y + 1 < 28 && yMovTimer <= 0)
                    {
                        y += 1;
                        yMovTimer = 0.5d;
                    }
                    break;
            }
            
        }

    }
}

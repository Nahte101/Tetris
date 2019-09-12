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
    public enum Tetro { I, I2, J, J2, J3, J4, L, L2, L3, L4, O, S, S2, T, T2, T3, T4, Z, Z2 };

    class Tetromino
    {
        //Clockwise rotations each numb is one rotation in 90 degrees clockwise
        public static bool[,] I = { { true, false, false, false }, { true, false, false, false }, { true, false, false, false }, { true, false, false, false } };
        public static bool[,] I2 = { { true, true, true, true }, { false, false, false, false }, { false, false, false, false }, { false, false, false, false } };

        public static bool[,] J = { { true, true, false, false }, { true, false, false, false }, { true, false, false, false }, { false, false, false, false } };
        public static bool[,] J2 = { { true, true, true, false }, { false, false, true, false }, { false, false, false, false }, { false, false, false, false } };
        public static bool[,] J3 = { { false, true, false, false }, { false, true, false, false }, { true, true, false, false }, { false, false, false, false } };
        public static bool[,] J4 = { { true, false, false, false }, { true, true, true, false }, { false, false, false, false }, { false, false, false, false } };


        public static bool[,] L = { { true, true, false, false }, { false, true, false, false }, { false, true, false, false }, { false, false, false, false } };
        public static bool[,] L2 = { { false, false, true, false }, { true, true, true, false }, { false, false, false, false }, { false, false, false, false } };
        public static bool[,] L3 = { { true, false, false, false }, { true, false, false, false }, { true, true, false, false }, { false, false, false, false } };
        public static bool[,] L4 = { { true, true, true, false }, { true, false, false, false }, { false, false, false, false }, { false, false, false, false } };

        public static bool[,] O = { { true, true, false, false }, { true, true, false, false }, { false, false, false, false }, { false, false, false, false } };

        public static bool[,] S = { { true, false, false, false }, { true, true, false, false }, { false, true, false, false }, { false, false, false, false } };
        public static bool[,] S2 = { { false, true, true, false }, { true, true, false, false }, { false, false, false, false }, { false, false, false, false } };

        public static bool[,] T = { { true, false, false, false }, { true, true, false, false }, { true, false, false, false }, { false, false, false, false } };
        public static bool[,] T2 = { { true, true, true, false }, { false, true, false, false }, { false, false, false, false }, { false, false, false, false } };
        public static bool[,] T3 = { { false, true, false, false }, { true, true, false, false }, { false, true, false, false }, { false, false, false, false } };
        public static bool[,] T4 = { { false, true, false, false }, { true, true, true, false }, { false, false, false, false }, { false, false, false, false } };

        public static bool[,] Z = { { false, true, false, false }, { true, true, false, false }, { true, false, false, false }, { false, false, false, false } };
        public static bool[,] Z2 = { { true, true, false, false }, { false, true, true, false }, { false, false, false, false }, { false, false, false, false } };

        private bool[,] pieces;

        Color colour;

        //x and why are indexs inside of the playfield relative to the top left of pieces in this class
        private int x = 5;

        private double y = 0d;
        private double xMovTimer = 0.2d;
        private double yMovTimer = 0.3d;
        private double rotateTimer = 0.5d;

        private bool isFallen = false;
        public bool[,] Pieces { get { return pieces; } }
        public int X { get { return x; } }
        public int Y { get { return (int)y; } }
        public double XMovTimer { get { return xMovTimer; }}
        public double RotateTimer { get { return rotateTimer; } set { rotateTimer = value; } }
        public bool IsFallen { get { return isFallen; } set { isFallen = value; } }
        public Tetro Type { get { return type; } set { type = value; } }
        public Color Colour { get { return colour; } }

        private Tetro type;

        public Tetromino(bool[,] pieces, Tetro type)
        {
            this.pieces = pieces;//Might cause problems since it is a shallow copy
            this.type = type;
            Random rand = new Random();
            int r = rand.Next(0, 255);
            int g = rand.Next(0, 255);
            int b = rand.Next(0, 255);
            this.colour = new Color(r,g,b);
        }
        public bool isBlockFallen()
        {
            return (24 + emptyRowsFromBottom() == y);
        }
        public void update(GameTime gameTime, bool leftCollide, bool rightCollide)
        {
            KeyboardState kState = Keyboard.GetState();
            movX(kState, gameTime, leftCollide, rightCollide);
            fall(gameTime);
            yMovTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            rotateTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            speedUp(kState);
            updateBlock();

        }
        public void updateBlockNoRestriction()
        {
            switch (type)
            {
                case Tetro.I:
                    pieces = I;
                    break;
                case Tetro.I2:
                    pieces = I2;
                    break;
                case Tetro.J:
                    pieces = J;
                    break;
                case Tetro.J2:
                    pieces = J2;
                    break;
                case Tetro.J3:
                    pieces = J3;
                    break;
                case Tetro.J4:
                    pieces = J4;
                    break;
                case Tetro.L:
                    pieces = L;
                    break;
                case Tetro.L2:
                    pieces = L2;
                    break;
                case Tetro.L3:
                    pieces = L3;
                    break;
                case Tetro.L4:
                    pieces = L4;
                    break;
                case Tetro.O:
                    pieces = O;
                    break;
                case Tetro.S:
                    pieces = S;
                    break;
                case Tetro.S2:
                    pieces = S2;
                    break;
                case Tetro.T:
                    pieces = T;
                    break;
                case Tetro.T2:
                    pieces = T2;
                    break;
                case Tetro.T3:
                    pieces = T3;
                    break;
                case Tetro.T4:
                    pieces = T4;
                    break;
                case Tetro.Z:
                    pieces = Z;
                    break;
                case Tetro.Z2:
                    pieces = Z2;
                    break;
            }
        }
        public void updateBlock()
        {
            switch (type)
            {
                case Tetro.I:
                    pieces = I;

                    break;
                case Tetro.I2:
                    pieces = I2;

                    break;
                case Tetro.J:
                    pieces = J;

                    break;
                case Tetro.J2:
                    pieces = J2;

                    break;
                case Tetro.J3:
                    pieces = J3;

                    break;
                case Tetro.J4:
                    pieces = J4;

                    break;
                case Tetro.L:
                    pieces = L;

                    break;
                case Tetro.L2:
                    pieces = L2;

                    break;
                case Tetro.L3:
                    pieces = L3;

                    break;
                case Tetro.L4:
                    pieces = L4;

                    break;
                case Tetro.O:
                    pieces = O;
                    break;
                case Tetro.S:
                    pieces = S;
                    break;
                case Tetro.S2:
                    pieces = S2;

                    break;
                case Tetro.T:
                    pieces = T;

                    break;
                case Tetro.T2:
                    pieces = T2;

                    break;
                case Tetro.T3:
                    pieces = T3;

                    break;
                case Tetro.T4:
                    pieces = T4;

                    break;
                case Tetro.Z:
                    pieces = Z;

                    break;
                case Tetro.Z2:
                    pieces = Z2;

                    break;
            }
        }
        public void rotate()
        {
            if (type == Tetro.O)
            {
                //Don't rotate
            }
            else if (type == Tetro.I2 || type == Tetro.Z2 || type == Tetro.S2)
            {
                type = (Tetro)(int)type - 1;
            }
            else if (type == Tetro.J4 || type == Tetro.L4 || type == Tetro.T4)
            {
                type = (Tetro)(int)type - 3;
            }
            else
            {
                type = (Tetro)(int)type + 1;
            }
        }
        public void movX(KeyboardState kState, GameTime gameTime, bool leftCollide, bool rightCollide)
        {
            if (x > 0 && kState.IsKeyDown(Keys.Left) && xMovTimer <= 0 && !leftCollide)
            {
               x--;
               xMovTimer = 0.2d;
            }     
            else if (x < 6 + emptyColumnsFromRight() && kState.IsKeyDown(Keys.Right) && xMovTimer <= 0 && !rightCollide)
            {
                x++;
                xMovTimer = 0.2d;
            }
            else
            {
                xMovTimer -= gameTime.ElapsedGameTime.TotalSeconds;
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
            if (!isFallen)
            {
                switch (emptyRowsFromBottom())
                {
                    case 0:
                        if ((int)y + 4 < 28 && yMovTimer <= 0)
                        {
                            y += 1;
                            yMovTimer = 0.5d;
                        }
                        else if (!((int)y + 4 < 28))
                        {
                            isFallen = true;
                        }
                        break;
                    case 1:
                        if ((int)y + 3 < 28 && yMovTimer <= 0)
                        {
                            y += 1;
                            yMovTimer = 0.5d;
                        }
                        else if (!((int)y + 3 < 28))
                        {
                            isFallen = true;
                        }
                        break;
                    case 2:
                        if ((int)y + 2 < 28 && yMovTimer <= 0)
                        {
                            y += 1;
                            yMovTimer = 0.5d;
                        }
                        else if (!((int)y + 2 < 28))
                        {
                            isFallen = true;
                        }
                        break;
                    case 3:
                        if ((int)y + 1 < 28 && yMovTimer <= 0)
                        {
                            y += 1;
                            yMovTimer = 0.5d;
                        }
                        else if (!((int)y + 1 < 28))
                        {
                            isFallen = true;
                        }
                        break;
                }
            }
        }

    }
}

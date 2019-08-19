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
        private int y = 0;

        public bool[,] Pieces { get { return pieces; } }
        public int X { get { return x; } }
        public int Y { get { return y; } }

        public Tetromino(bool[,] pieces)
        {
            this.pieces = pieces;//Might cause problems since it is a shallow copy
        }
        public void update(GameTime gameTime)
        {
            fall();
        }
        public void fall()
        {
            if (y + 4 < 28)
            {
                y++;
            }
        }

    }
}

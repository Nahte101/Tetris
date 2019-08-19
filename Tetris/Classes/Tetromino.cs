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
        private static bool[,] I = { { true, false, false, false }, { true, false, false, false }, { true, false, false, false }, { true, false, false, false } };
        private static bool[,] J = { { true, true, false, false }, { true, false, false, false }, { true, false, false, false }, { false, false, false, false } };
        private static bool[,] L = { { true, true, false, false }, { false, true, false, false }, { false, true, false, false }, { false, false, false, false } };
        private static bool[,] O = { { true, true, false, false }, { true, true, false, false }, { false, false, false, false }, { false, false, false, false } };
        private static bool[,] S = { { true, false, false, false }, { true, true, false, false }, { false, true, false, false }, { false, false, false, false } };
        private static bool[,] T = { { true, false, false, false }, { true, true, false, false }, { true, false, false, false }, { false, false, false, false } };
        private static bool[,] Z = { { false, true, false, false }, { true, true, false, false }, { true, false, false, false }, { false, false, false, false } };

        private bool[,] pieces;

        public Tetromino(bool[,] pieces)
        {
            this.pieces = pieces;//Might cause problems since it is a shallow copy
        }

    }
}

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
    class TetrisGameField
    {
        private static bool[,] I = { { true, false, false, false }, { true, false, false, false }, { true, false, false, false }, { true, false, false, false } };
        private static bool[,] J = { { true, true, false, false }, { true, false, false, false }, { true, false, false, false }, { false, false, false, false } };
        private static bool[,] L = { { true, true, false, false }, { false, true, false, false }, { false, true, false, false }, { false, false, false, false } };
        private static bool[,] O = { { true, true, false, false }, { true, true, false, false }, { false, false, false, false }, { false, false, false, false } };
        private static bool[,] S = { { true, false, false, false }, { true, true, false, false }, { false, true, false, false }, { false, false, false, false } };
        private static bool[,] T = { { true, false, false, false }, { true, true, false, false }, { true, false, false, false }, { false, false, false, false } };
        private static bool[,] Z = { { false, true, false, false }, { true, true, false, false }, { true, false, false, false }, { false, false, false, false } };

        private Texture2D blankRectSprite;
        
        private GraphicsDeviceManager graphics;
        private int screenWidth;
        private int screenHeight;

        private bool[,] playField = new bool[10, 40];
        private List<Tetromino> fallenPieces = new List<Tetromino>();

        public TetrisGameField(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;//Might be a problem if resolution changes on the fly
            this.screenHeight = graphics.PreferredBackBufferHeight;
            this.screenWidth = graphics.PreferredBackBufferWidth;

            
        }
        public void drawOutline(SpriteBatch spriteBatch)
        {
            blankRectSprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            blankRectSprite.SetData(new Color[] { Color.GhostWhite });

            //Variables for usage in drawing of box (playing field will be 10 by 40 blocks default same as playfield)
            // and each block will be 10 by 10 pixels with a 5 block gap on the x and y

            int widthInBlocks = playField.GetLength(0);
            int heightInBlocks = playField.GetLength(1);
            int screenWidth = graphics.PreferredBackBufferWidth;
            int screenHeight = graphics.PreferredBackBufferHeight;
            int blockSize = 10;
            int gap = 5;

            //Horizontal Top Line
            spriteBatch.Draw(blankRectSprite, new Rectangle( (screenWidth/2 )-( ( widthInBlocks* (blockSize+gap) ) /2)
            ,(screenHeight/16), widthInBlocks * (blockSize + gap), 1 ), Color.White);
            //Horizontal Bottom Line
            spriteBatch.Draw(blankRectSprite, new Rectangle((screenWidth / 2) - ((widthInBlocks * (blockSize + gap)) / 2)
            , screenHeight- (screenHeight / 16), widthInBlocks * (blockSize + gap), 1), Color.White);
            //Vertical Left Line
            spriteBatch.Draw(blankRectSprite, new Rectangle( (screenWidth/2)-( (widthInBlocks * (blockSize + gap) ) /2 ) 
                , (screenHeight / 16), 1,screenHeight-( 2 * (screenHeight/16) ) ), Color.White);
            //Vertical Right Line
            spriteBatch.Draw(blankRectSprite, new Rectangle((screenWidth / 2) + ((widthInBlocks * (blockSize + gap)) / 2)
                , (screenHeight / 16), 1, screenHeight - (2 * (screenHeight / 16))), Color.White);
        }
    }
}

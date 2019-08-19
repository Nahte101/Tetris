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
        
        private Texture2D blankRectSprite;
        
        private GraphicsDeviceManager graphics;

        int widthInBlocks;
        int heightInBlocks;
        int screenWidth;
        int screenHeight;
        int blockSize = 10;
        int gap = 5;
        int startOfFieldX;
        int startOfFieldY;

        private bool[,] playField = new bool[10, 28];

        private Tetromino currentTetromino;
        private List<Tetromino> fallenPieces = new List<Tetromino>();

        public TetrisGameField(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;//Might be a problem if resolution changes on the fly
            this.screenHeight = graphics.PreferredBackBufferHeight;
            this.screenWidth = graphics.PreferredBackBufferWidth;
            this.widthInBlocks = playField.GetLength(0);
            this.heightInBlocks = playField.GetLength(1);

            this.startOfFieldX = (screenWidth / 2) - ((widthInBlocks * (blockSize + gap)) / 2);
            this.startOfFieldX+= 5;
            this.startOfFieldY = (screenHeight / 16);
            this.startOfFieldY+= 5;
        }
        //For now false will be red and true will be green
        public void drawPlayField(SpriteBatch spriteBatch)
        {
            Rectangle baseblock = new Rectangle(0, 0, 10, 10);
            Texture2D redSprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            redSprite.SetData(new Color[] { Color.Red });
            Texture2D greenSprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            greenSprite.SetData(new Color[] { Color.Green });


            for (int i=0;i<playField.GetLength(0);i++)
            {
                for(int c=0;c< playField.GetLength(1); c++)
                {
                    baseblock.Location = new Point(startOfFieldX+(i*(blockSize + gap) )  ,startOfFieldY+( c* (blockSize + gap) ) );
                    if(playField[i,c])
                    {
                        spriteBatch.Draw(greenSprite,baseblock,Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(redSprite, baseblock, Color.White);
                    }
                }
            }
        }
        public void drawOutline(SpriteBatch spriteBatch)
        {
            blankRectSprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            blankRectSprite.SetData(new Color[] { Color.GhostWhite });

            //Variables for usage in drawing of box (playing field will be 10 by 40 blocks default same as playfield)
            // and each block will be 10 by 10 pixels with a 5 block gap on the x and y

            //Add 5 pixels to width and height of the lines here TODO

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

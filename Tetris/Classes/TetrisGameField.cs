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

        int numOfBottomCollisionBlocks = 0;
        int numOfLeftCollisionBlocks = 0;
        int numOfRightCollisionBlocks = 0;

        private bool[,] playField = new bool[10, 28];
        private List<List<int>> bottomCollisionSkinPositions = new List<List<int>>();
        private List<List<int>> lSideCollisionSkinPositions = new List<List<int>>();
        private List<List<int>> rSideCollisionSkinPositions = new List<List<int>>();
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
        public void chooseBlock()
        {
            Random rand = new Random();
            int num = rand.Next(0, 7);
            switch(num)
            {
                case 1:
                    currentTetromino = new Tetromino(Tetromino.I);
                    break;
                case 2:
                    currentTetromino = new Tetromino(Tetromino.J);
                    break;
                case 3:
                    currentTetromino = new Tetromino(Tetromino.L);
                    break;
                case 4:
                    currentTetromino = new Tetromino(Tetromino.O);
                    break;
                case 5:
                    currentTetromino = new Tetromino(Tetromino.S);
                    break;
                case 6:
                    currentTetromino = new Tetromino(Tetromino.T);
                    break;
                default:
                    currentTetromino = new Tetromino(Tetromino.Z);
                    break;
            }
        }
        public void update(GameTime gameTime)
        {
            currentTetromino.update(gameTime, isSideCollide(gameTime));
            if(currentTetromino.isBlockFallen())
            {
                fallenPieces.Add(currentTetromino);   
                chooseBlock();
            }
        }
        public void resetField()//Do before every drawField call
        {
            playField = new bool[10, 28];
            foreach(Tetromino piece in fallenPieces)
            {
                //Might not work due to pass by value
                placeBlock(playField, piece);
            }
        }
        public int[,] findTetroPositionInField(Tetromino tetro)//Checks from top left of Tetro Position to bottom right of playfield for true statements and logs it as such
        {/*Might not work when the block has fallen (so we might need to change it so it only finds the actual tetromino piece and not others)
            possibly through comparing the Tetrominos pieces array with the blocks from the x and y and get the positions based on that*/

            //order of each block positions stored is top left to bottom right (from its x and y)

            
            int[,] TetrominoPiecePositions = new int[4, 2];
            int pieceCounter = 0;


            for (int i = currentTetromino.X; i <= currentTetromino.X+(3-currentTetromino.emptyColumnsFromRight()); i++)
            {
                
                for (int c = currentTetromino.Y; c <= currentTetromino.Y + (3 - currentTetromino.emptyRowsFromBottom()); c++)
                {
                    if (playField[i, c] )
                    {
                        TetrominoPiecePositions[pieceCounter, 0] = i;
                        TetrominoPiecePositions[pieceCounter, 1] = c;
                        pieceCounter++;
                    }
                    
                }
                
            }
            return TetrominoPiecePositions;
        }
        public void generateFallCollision()
        {
            //Resetting current coliision positions
            bottomCollisionSkinPositions = new List<List<int>>();

            int[,] TetrominoPiecePositions = findTetroPositionInField(currentTetromino);
            int collisionCount = 0;

            for (int i = 0; i < TetrominoPiecePositions.GetLength(0); i++)
            {
                int[] BotPos = {TetrominoPiecePositions[i,0] ,TetrominoPiecePositions[i,1]+1 };
                bool makeItABlock = true;
                for (int i2 = 0; i2 < TetrominoPiecePositions.GetLength(0); i2++)
                {
                    if(BotPos[0] == TetrominoPiecePositions[i2,0] && BotPos[1] == TetrominoPiecePositions[i2, 1])
                    {
                        makeItABlock = false;
                    }
                }
                if(makeItABlock)
                {
                    bottomCollisionSkinPositions.Add(new List<int>());
                    bottomCollisionSkinPositions[collisionCount].Add(BotPos[0]);
                    bottomCollisionSkinPositions[collisionCount].Add(BotPos[1]);
                    collisionCount++;
                }
                
            }
            this.numOfBottomCollisionBlocks = collisionCount;
        }
        public void generateLSideCollision()
        {
            //Resetting current coliision positions
            lSideCollisionSkinPositions = new List<List<int>>();

            int[,] TetrominoPiecePositions = findTetroPositionInField(currentTetromino);
            int collisionCount = 0;
            for (int i = 0; i < TetrominoPiecePositions.GetLength(0); i++)
            {
                int[] lefPos = { TetrominoPiecePositions[i, 0] - 1, TetrominoPiecePositions[i, 1] };
                bool makeItABlock = true;
                for (int i2 = 0; i2 < TetrominoPiecePositions.GetLength(0); i2++)
                {
                    if (lefPos[0] == TetrominoPiecePositions[i2, 0] && lefPos[1] == TetrominoPiecePositions[i2, 1])
                    {
                        makeItABlock = false;
                    }
                }
                if (makeItABlock)
                {
                    lSideCollisionSkinPositions.Add(new List<int>());
                    lSideCollisionSkinPositions[collisionCount].Add(lefPos[0]);
                    lSideCollisionSkinPositions[collisionCount].Add(lefPos[1]);
                    collisionCount++;
                }

            }
            this.numOfRightCollisionBlocks = collisionCount;
        }
        public void generateRSideCollision()
        {
            //Resetting current coliision positions
            rSideCollisionSkinPositions = new List<List<int>>();

            int[,] TetrominoPiecePositions = findTetroPositionInField(currentTetromino);
            int collisionCount = 0;
            for (int i = 0; i < TetrominoPiecePositions.GetLength(0); i++)
            {
                int[] rigPos = { TetrominoPiecePositions[i, 0] + 1, TetrominoPiecePositions[i, 1] };
                bool makeItABlock = true;
                for (int i2 = 0; i2 < TetrominoPiecePositions.GetLength(0); i2++)
                {
                    if (rigPos[0] == TetrominoPiecePositions[i2, 0] && rigPos[1] == TetrominoPiecePositions[i2, 1])
                    {
                        makeItABlock = false;
                    }
                }
                if (makeItABlock)
                {
                    rSideCollisionSkinPositions.Add(new List<int>());
                    rSideCollisionSkinPositions[collisionCount].Add(rigPos[0]);
                    rSideCollisionSkinPositions[collisionCount].Add(rigPos[1]);
                    collisionCount++;
                }
                
            }
            this.numOfRightCollisionBlocks = collisionCount;
        }
        
        public bool isSideCollide(GameTime gameTime)
        {
            //Change it not check outside of the bounds
            for (int i=0;i< rSideCollisionSkinPositions.Count;i++)
            {
                if (rSideCollisionSkinPositions[i][0] < 10)
                {
                    if (playField[rSideCollisionSkinPositions[i][0], rSideCollisionSkinPositions[i][1]])
                    {
                        currentTetromino.XMovTimer += gameTime.ElapsedGameTime.TotalSeconds;
                        return true;
                    }
                }
            }
            for (int i = 0; i < lSideCollisionSkinPositions.Count; i++)
            {
                for (int c = 0; c < lSideCollisionSkinPositions[i].Count; c++)
                    if (lSideCollisionSkinPositions[i][0] > 0)
                    {
                        if (playField[lSideCollisionSkinPositions[i][0], lSideCollisionSkinPositions[i][1]])
                        {
                            currentTetromino.XMovTimer += gameTime.ElapsedGameTime.TotalSeconds;
                            return true;
                        }
                    }
            }
            return false;
            
        }
        public void drawCollision(SpriteBatch spriteBatch)
        {
            Texture2D redSprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            redSprite.SetData(new Color[] { Color.MediumVioletRed });
            Texture2D greenSprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            greenSprite.SetData(new Color[] { Color.Green });
            Texture2D blueSprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            blueSprite.SetData(new Color[] { Color.Blue });
            for (int i=0;i < playField.GetLength(0);i++)
            {
                for (int c = 0; c < playField.GetLength(1); c++)
                {
                    Point pointOnScreen = new Point(startOfFieldX + (i * (blockSize + gap)), startOfFieldY + (c * (blockSize + gap)));
                    Point size = new Point(15,15);
                    for(int position = 0; position < numOfBottomCollisionBlocks; position++)
                    {
                        if (i == bottomCollisionSkinPositions[position][0] && c == bottomCollisionSkinPositions[position][1])
                        {
                            spriteBatch.Draw(redSprite, new Rectangle(pointOnScreen,size), Color.White);
                        }
                        /*if (i == lSideCollisionSkinPositions[position][0] && c == lSideCollisionSkinPositions[position][1])
                        {
                            spriteBatch.Draw(greenSprite, new Rectangle(pointOnScreen, size), Color.White);
                        }*/
                        if (i == rSideCollisionSkinPositions[position][0] && c == rSideCollisionSkinPositions[position][1])
                        {
                           spriteBatch.Draw(blueSprite, new Rectangle(pointOnScreen, size), Color.White);
                        }
                    }
                }
            }
        }
        public void placeBlock(bool[,] listToPlaceBlock, Tetromino blockToPlace)
        {
            for (int i = 0; i < blockToPlace.Pieces.GetLength(0) - blockToPlace.emptyRowsFromBottom(); i++)
            {
                for (int c = 0; c < blockToPlace.Pieces.GetLength(1) - blockToPlace.emptyColumnsFromRight(); c++)
                {
                    if(blockToPlace.Pieces[i,c])
                    {
                        listToPlaceBlock[blockToPlace.X + c, blockToPlace.Y + i] = blockToPlace.Pieces[i, c];
                    }
                }
            }
        }
        public void placeBlock()
        {
            //insert the current tetrominos values in pieces into the playfield
            for(int i = 0; i < currentTetromino.Pieces.GetLength(0)-currentTetromino.emptyRowsFromBottom();i++)
            {
                for (int c = 0; c < currentTetromino.Pieces.GetLength(1) - currentTetromino.emptyColumnsFromRight(); c++)
                {
                    if(currentTetromino.Pieces[i, c])
                    {
                        playField[currentTetromino.X + c, currentTetromino.Y + i] = currentTetromino.Pieces[i, c];
                    }
                }
            }
        }
        public void drawDebugStats(SpriteBatch spriteBatch,SpriteFont font)
        {
            spriteBatch.DrawString(font,"Blanks rows From bottom: "+currentTetromino.emptyRowsFromBottom().ToString(),Vector2.Zero,Color.White);
            spriteBatch.DrawString(font, "Blanks columns From Right: " + currentTetromino.emptyColumnsFromRight().ToString(), new Vector2(0, 25), Color.White);
            spriteBatch.DrawString(font, "X move Timer: " + currentTetromino.XMovTimer.ToString(), new Vector2(0,50), Color.White);
            spriteBatch.DrawString(font, "X: " + currentTetromino.X.ToString(), new Vector2(0, 75), Color.White);
            spriteBatch.DrawString(font, "Y: " + currentTetromino.Y.ToString(), new Vector2(0, 100), Color.White);

            currentTetromino.drawPieces(spriteBatch, 100, 100);
            /*
            spriteBatch.DrawString(font, "Num bottom collision blocks: "+ numOfBottomCollisionBlocks.ToString() , new Vector2(550 ,50), Color.White);
            spriteBatch.DrawString(font, "Num left collision blocks: " + numOfLeftCollisionBlocks.ToString(), new Vector2(550, 75), Color.White);
            spriteBatch.DrawString(font, "Num right collision blocks: " + numOfRightCollisionBlocks.ToString(), new Vector2(550, 100), Color.White);
            */
            //Displaying the tetris pieces positions
            //Errors when block overlap should be fixed once collision implemented
            /*
            int[,] TetrominoPiecePositions = findTetroPositionInField(currentTetromino);
            for (int i = 0; i < TetrominoPiecePositions.GetLength(0);i++)
            {
                for(int c= 0; c < TetrominoPiecePositions.GetLength(1); c++)
                {
                    spriteBatch.DrawString(font, TetrominoPiecePositions[i,c].ToString(), new Vector2(505+(30*c),50*i), Color.White);

                }
            }
            */

            //Displaying collision Positions
            int i = 0;
            int c = 0;
            foreach(List<int> list in lSideCollisionSkinPositions)
            {
                foreach(int num in list)
                {
                    spriteBatch.DrawString(font, num.ToString(), new Vector2(505 + (30 * c), 50 * i), Color.White);
                    c++;
                }
                i++;
            }
        }

        //For now false will be red and true will be green
        public void drawPlayField(SpriteBatch spriteBatch)
        {
            Rectangle baseblock = new Rectangle(0, 0, 10, 10);
            Texture2D redSprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            redSprite.SetData(new Color[] { Color.Gray });
            Texture2D greenSprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            greenSprite.SetData(new Color[] { Color.White });

            for (int i=0; i < playField.GetLength(0);i++)
            {
                for(int c = 0; c < playField.GetLength(1); c++)
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
            spriteBatch.Draw(blankRectSprite, new Rectangle((screenWidth/2 )-( ( widthInBlocks* (blockSize+gap) ) /2)
            ,(screenHeight/16), widthInBlocks * (blockSize + gap)+gap, 1 ), Color.White);
            //Horizontal Bottom Line
            spriteBatch.Draw(blankRectSprite, new Rectangle((screenWidth / 2) - ((widthInBlocks * (blockSize + gap)) / 2)
            , screenHeight- (screenHeight / 16)+gap, widthInBlocks * (blockSize + gap)+gap, 1), Color.White);
            //Vertical Left Line
            spriteBatch.Draw(blankRectSprite, new Rectangle( (screenWidth/2)-( (widthInBlocks * (blockSize + gap) ) /2 ) 
                , (screenHeight / 16), 1,screenHeight-( 2 * (screenHeight/16) )+ gap ), Color.White);
            //Vertical Right Line
            spriteBatch.Draw(blankRectSprite, new Rectangle(gap+(screenWidth / 2) + ((widthInBlocks * (blockSize + gap)) / 2)
                , (screenHeight / 16), 1, screenHeight - (2 * (screenHeight / 16))+gap ), Color.White);
        }
    }
}

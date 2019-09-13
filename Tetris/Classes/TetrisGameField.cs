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
        private bool[,] fallenBlocks = new bool[10, 28];
        public bool gameOver = false;

        public bool GameOver { get { return gameOver; } }

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
                    currentTetromino = new Tetromino(Tetromino.I, Tetro.I);
                    break;
                case 2:
                    currentTetromino = new Tetromino(Tetromino.J, Tetro.J);
                    break;
                case 3:
                    currentTetromino = new Tetromino(Tetromino.L, Tetro.L);
                    break;
                case 4:
                    currentTetromino = new Tetromino(Tetromino.O, Tetro.O);
                    break;
                case 5:
                    currentTetromino = new Tetromino(Tetromino.S, Tetro.S);
                    break;
                case 6:
                    currentTetromino = new Tetromino(Tetromino.T, Tetro.T);
                    break;
                default:
                    currentTetromino = new Tetromino(Tetromino.Z, Tetro.Z);
                    break;
            }
        }
        public void update(GameTime gameTime)
        {
            currentTetromino.update(gameTime, isLeftCollide(),isRightCollide());
            if(currentTetromino.isBlockFallen() || currentTetromino.IsFallen)
            {
                fallenPieces.Add(currentTetromino);   
                chooseBlock();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && currentTetromino.RotateTimer <= 0 && canRotate() && currentTetromino.X + (4-currentTetromino.emptyColumnsFromRight()) < 10)
            {
                int[] rotateArray = new int[2];
                rotateArray[0] = 1;
                rotateArray[1] = 1;

                currentTetromino.rotate();
                currentTetromino.updateBlock();
                currentTetromino.RotateTimer = 0.5d;

            }
            List<int> lineInfo = searchForLine();
            if(lineInfo[0] != 0)
            {
                for(int i = 1; i < lineInfo.Count;i++)
                {
                    removeLine(lineInfo[i]);
                }
                for(int c = 1; c < lineInfo.Count;c++)
                { moveFallenBlocksDown(lineInfo[c]); }

            }
            if(!canRotate() && currentTetromino.Y == 0)
            {
                gameOver = true;
            }
        }
        public void resetField()//Do before every drawField call
        {
            playField = new bool[10, 28];
            foreach(Tetromino piece in fallenPieces)
            {
                placeBlock(fallenBlocks, piece);
            }
            fallenPieces.Clear();
            placeList(fallenBlocks);
        }
        public int[,] findTetroPositionInField(Tetromino tetro)//Checks from top left of Tetro Position to bottom right of playfield for true statements and logs it as such
        {/*Might not work when the block has fallen (so we might need to change it so it only finds the actual tetromino piece and not others)
            possibly through comparing the Tetrominos pieces array with the blocks from the x and y and get the positions based on that*/

            //order of each block positions stored is top left to bottom right (from its x and y)

            
            int[,] TetrominoPiecePositions = new int[4, 2];
            int pieceCounter = 0;


            for (int i = currentTetromino.X; i < currentTetromino.X+(4-currentTetromino.emptyColumnsFromRight()); i++)
            {
                
                for (int c = currentTetromino.Y; c < currentTetromino.Y + (4 - currentTetromino.emptyRowsFromBottom()); c++)
                {
                    if (playField[i, c] && currentTetromino.Pieces[c-currentTetromino.Y,i-currentTetromino.X])
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
        
        public bool isRightCollide()
        {
            //Change it not check outside of the bounds
            for (int i=0;i< rSideCollisionSkinPositions.Count;i++)
            {
                if (rSideCollisionSkinPositions[i][0] < 10)
                {
                    if (playField[rSideCollisionSkinPositions[i][0], rSideCollisionSkinPositions[i][1]])
                    {
                        return true;
                    }
                }
            }
            
            return false;
            
        }
        public bool isLeftCollide()
        {
            for (int i = 0; i < lSideCollisionSkinPositions.Count; i++)
            {
                for (int c = 0; c < lSideCollisionSkinPositions[i].Count; c++)
                    if (lSideCollisionSkinPositions[i][0] >= 0)
                    {
                        if (playField[lSideCollisionSkinPositions[i][0], lSideCollisionSkinPositions[i][1]])
                        {
                            return true;
                        }
                    }
            }
            return false;
        }
        public bool bottomCollide()
        {
            for (int i = 0; i < numOfBottomCollisionBlocks; i++)
            {
                if (bottomCollisionSkinPositions[i][0] < playField.GetLength(0))
                {
                    if (playField[bottomCollisionSkinPositions[i][0], bottomCollisionSkinPositions[i][1]])
                    {
                        currentTetromino.IsFallen = true;
                        return true;
                    }
                }
            }
            return false;
        }
        //int List stores the first element is the number of lines found and then after that is the y values of the line
        public List<int> searchForLine()
        {
            //Literally just search through playfield for a line of trues
            List<int> lineInfo = new List<int>();

            int lineCounter = 0;

            List<int> lineNumbers = new List<int>();
            for (int i = 0; i < fallenBlocks.GetLength(1); i++)
            {
                int xBlockCounter = 0;
                for (int c = 0; c < fallenBlocks.GetLength(0); c++)
                {
                    if (fallenBlocks[c, i])
                    {
                        xBlockCounter++;
                    }
                    if (xBlockCounter == 10)
                    {
                        lineCounter++;
                        lineNumbers.Add(i);
                        xBlockCounter = 0;
                    }
                }
            }
            lineInfo.Add(lineCounter);
            lineInfo.AddRange(lineNumbers);
            return lineInfo;
        }
        //index is the y value for playfield
        public void removeLine(int lineIndex)
        {
            for (int i = 0; i < fallenBlocks.GetLength(1); i++)
            {
                if (i == lineIndex)
                {
                    for (int c = 0; c < fallenBlocks.GetLength(0); c++)
                    {
                        fallenBlocks[c, i] = false;
                    }
                }
            }
        }
        public void moveFallenBlocksDown(int lineToStopCopy)
        {
            /*Copy everything from the top of fallenBlocks to the line where the line was removed
             * then remove everything in the whole area where the original of the copy existed in fallenblocks
             * then add the copy into fallenBlocks but one row down more in fallenBlocks
                          
             */
            bool[,] partialCopyOfFallenBlocks = new bool[10, lineToStopCopy];
            //Copying from fallenBlocks
            for (int i = 0; i < lineToStopCopy; i++)
            {
                for(int c = 0; c < fallenBlocks.GetLength(0); c++)
                {
                    partialCopyOfFallenBlocks[c, i] = fallenBlocks[c,i];
                }
            }
            //Removing the copied part
            for (int i = 0; i < lineToStopCopy; i++)
            {
                for(int c = 0; c < fallenBlocks.GetLength(0); c++)
                {
                    fallenBlocks[c, i] = false; ;
                }
            }

            //Adding the copy back in but one down
            for (int i = 0; i < lineToStopCopy; i++)
            {
                for (int c = 0; c < fallenBlocks.GetLength(0); c++)
                {
                    fallenBlocks[c, i+1] = partialCopyOfFallenBlocks[c, i];
                }
            }
        }
        public bool canRotate()
        {
            //Go through all fallenBlocks and cycle through all Rotations and if any of them are inside the fallenBlocks return false
            var rotations = Enum.GetValues(typeof(Tetro));
            Tetro ogType = currentTetromino.Type;
            int[,] currentTetroPos;
            for (int i=0;i < fallenBlocks.GetLength(0);i++)
            {
                for(int c = 0; c < fallenBlocks.GetLength(1);c++)
                {
                    foreach(Tetro rotation in rotations)
                    {
                        currentTetromino.Type = rotation;
                        currentTetromino.updateBlockNoRestriction();
                        try
                        {
                            currentTetroPos = findTetroPositionInField(currentTetromino);
                        }
                        catch
                        {
                            currentTetromino.Type = ogType;
                            currentTetromino.updateBlockNoRestriction();
                            return false;
                        }
                        for(int pos=0; pos < currentTetroPos.GetLength(0); pos++)
                        {
                            if (currentTetroPos[pos,0] == i && fallenBlocks[i,c] && currentTetroPos[pos,1] == c)
                            {
                                currentTetromino.Type = ogType;
                                currentTetromino.updateBlockNoRestriction();
                                return false;
                            }
                        }
                    }
                }
                
            }
            currentTetromino.Type = ogType;
            currentTetromino.updateBlockNoRestriction();
            return true;
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
        //Should be the same size as playfield
        public void placeList(bool[,] listToCopy)
        {
            for (int i = 0; i < listToCopy.GetLength(0); i++)
            {
                for (int c = 0; c < listToCopy.GetLength(1); c++)
                {
                    playField[i,c] = listToCopy[i, c];
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
        public void drawDebugStats(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.DrawString(font, "Blanks rows From bottom: " + currentTetromino.emptyRowsFromBottom().ToString(), Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "Blanks columns From Right: " + currentTetromino.emptyColumnsFromRight().ToString(), new Vector2(0, 25), Color.White);
            spriteBatch.DrawString(font, "X move Timer: " + currentTetromino.XMovTimer.ToString(), new Vector2(0, 50), Color.White);
            spriteBatch.DrawString(font, "X: " + currentTetromino.X.ToString(), new Vector2(0, 75), Color.White);
            spriteBatch.DrawString(font, "Y: " + currentTetromino.Y.ToString(), new Vector2(0, 100), Color.White);

            currentTetromino.drawPieces(spriteBatch, 100, 100);
            spriteBatch.DrawString(font, "Fallen: " + currentTetromino.IsFallen.ToString(), new Vector2(100, 400), Color.White);
            spriteBatch.DrawString(font, "CanRotate: " + canRotate().ToString(), new Vector2(100, 360), Color.White);
            spriteBatch.DrawString(font, "RotateTimer: " + currentTetromino.RotateTimer.ToString(), new Vector2(100, 340), Color.White);

            //Testing Searching for a line
            List<int> lineInfo = searchForLine();
            spriteBatch.DrawString(font, "Line Amount: " + lineInfo[0].ToString(), new Vector2(505, 350), Color.White);
            for (int d = 1; d < lineInfo.Count; d++)
            {
                spriteBatch.DrawString(font, "Y pos: " + lineInfo[d].ToString(), new Vector2(505, 350 + (30 * d)), Color.White);
            }

            //spriteBatch.DrawString(font, "Can rotate?: " + canRotate().ToString(), new Vector2(100, 450), Color.White);
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
            foreach (List<int> list in lSideCollisionSkinPositions)
            {
                foreach (int num in list)
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
            Texture2D greySprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            greySprite.SetData(new Color[] { Color.Black });
            Texture2D whiteSprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            whiteSprite.SetData(new Color[] { currentTetromino.Colour });

            int[,] tetroPos = findTetroPositionInField(currentTetromino);

            for (int i=0; i < playField.GetLength(0);i++)
            {
                for(int c = 0; c < playField.GetLength(1); c++)
                {
                    baseblock.Location = new Point(startOfFieldX+(i*(blockSize + gap) )  ,startOfFieldY+( c* (blockSize + gap) ) );
                    if(playField[i,c])
                    {
                        spriteBatch.Draw(whiteSprite, baseblock, Color.White);
                        
                    }
                    else
                    {
                        spriteBatch.Draw(greySprite, baseblock, Color.White);
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

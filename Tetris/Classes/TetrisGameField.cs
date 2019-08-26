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

        int numOfCollisionBlocks = 0;

        private bool[,] playField = new bool[10, 28];
        private List<List<int>> collisionSkinPositions = new List<List<int>>();//Organised top left to bottom right
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
            currentTetromino.update(gameTime);
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
                for (int c = currentTetromino.Y; c <= currentTetromino.Y + (3-currentTetromino.emptyRowsFromBottom()); c++)
                {
                    if (playField[i, c])
                    {
                        TetrominoPiecePositions[pieceCounter, 0] = i;
                        TetrominoPiecePositions[pieceCounter, 1] = c;
                        pieceCounter++;
                    }
                }
            }
            return TetrominoPiecePositions;
        }
        public void generateCollision()
        {
            //find and store array positions of the current tetrominos squares
            int[,] TetrominoPiecePositions = findTetroPositionInField(currentTetromino);//4 arrays in this array which then has the 2 positions of each block


            //Actually generating the Collision Skin
            int collisionBlockCounter = 0;
            for(int i =0;i < TetrominoPiecePositions.GetLength(0); i++)
            {
                //No assumptions made since rotation could throw a spanner in the works
                //Add a block top, left, down and right and if its the same as any of the positions in TetroPiecePositions delete it
                int[,] TopPos = new int[1, 2];
                int[,] BotPos = new int[1, 2];
                int[,] LefPos = new int[1, 2];
                int[,] RigPos = new int[1, 2];
                

                int x = TetrominoPiecePositions[i, 0];
                int y = TetrominoPiecePositions[i, 1];

                //Above
                TopPos[0, 0] = x;
                TopPos[0, 1] = y + 1;
                //Below
                BotPos[0, 0] = x;
                BotPos[0, 1] = y - 1;
                //Left
                LefPos[0, 0] = x - 1;
                LefPos[0, 1] = y;
                //Right
                RigPos[0, 0] = x + 1;
                RigPos[0, 1] = y;

                    
                for(int c = 0; c < TetrominoPiecePositions.GetLength(0); c++)
                {
                    if(!(LefPos[0,0] == TetrominoPiecePositions[c,0] && LefPos[0,1] == TetrominoPiecePositions[c,1]))
                    {
                        collisionSkinPositions.Add(new List<int>());
                        collisionSkinPositions[collisionBlockCounter].Add(LefPos[0,0]);
                        collisionSkinPositions[collisionBlockCounter].Add(LefPos[0, 1]);
                        collisionBlockCounter++;
                    }
                    if (!(RigPos[0, 0] == TetrominoPiecePositions[c, 0] && RigPos[0, 1] == TetrominoPiecePositions[c, 1]))
                    {
                        collisionSkinPositions.Add(new List<int>());
                        collisionSkinPositions[collisionBlockCounter].Add(RigPos[0, 0]);
                        collisionSkinPositions[collisionBlockCounter].Add(RigPos[0, 1]);
                        collisionBlockCounter++;
                    }
                    if (!(TopPos[0, 0] == TetrominoPiecePositions[c, 0] && TopPos[0, 1] == TetrominoPiecePositions[c, 1]))
                    {
                        collisionSkinPositions.Add(new List<int>());
                        collisionSkinPositions[collisionBlockCounter].Add(TopPos[0, 0]);
                        collisionSkinPositions[collisionBlockCounter].Add(TopPos[0, 1]);
                        collisionBlockCounter++;
                    }
                    if (!(BotPos[0, 0] == TetrominoPiecePositions[c, 0] && BotPos[0, 1] == TetrominoPiecePositions[c, 1]))
                    {
                        collisionSkinPositions.Add(new List<int>());
                        collisionSkinPositions[collisionBlockCounter].Add(BotPos[0, 0]);
                        collisionSkinPositions[collisionBlockCounter].Add(BotPos[0, 1]);
                        collisionBlockCounter++;
                    }
                }
                this.numOfCollisionBlocks = collisionBlockCounter;
            }
        }
        public bool isCollide()
        {
            //Checks the collisions skin for overlap in the fallen blocks
            return true;
        }
        public void drawCollision(SpriteBatch spriteBatch)
        {
            Texture2D redSprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            redSprite.SetData(new Color[] { Color.MediumVioletRed });
            for (int i=0;i < playField.GetLength(0);i++)
            {
                for (int c = 0; c < playField.GetLength(1); c++)
                {

                    Point pointOnScreen = new Point(startOfFieldX + (i * (blockSize + gap)), startOfFieldY + (c * (blockSize + gap)));
                    Point size = new Point(15,15);
                    for(int position = 0; position < numOfCollisionBlocks; position++)
                    {
                        if (i == collisionSkinPositions[position][0] && c == collisionSkinPositions[position][1])
                        {
                            spriteBatch.Draw(redSprite, new Rectangle(pointOnScreen,size), Color.White);
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
            spriteBatch.DrawString(font, "Num collision blocks: "+ numOfCollisionBlocks.ToString() , new Vector2(535 ,50), Color.White);

            //Displaying the tetris pieces positions
            /* Errors when block overlap should be fixed once collision implemented
            int[,] TetrominoPiecePositions = findTetroPositionInField(currentTetromino);
            for (int i = 0; i < TetrominoPiecePositions.GetLength(0);i++)
            {
                for(int c= 0; c < TetrominoPiecePositions.GetLength(1); c++)
                {
                    spriteBatch.DrawString(font, TetrominoPiecePositions[i,c].ToString(), new Vector2(505+(30*c),50*i), Color.White);

                }
            }
            */
            /*
            //Displaying collision Positions
            for(int i = 0; i < numOfCollisionBlocks;i++)
            {
                for(int c = 0; c < collisionSkinPositions[i].Count; c++)
                {
                    spriteBatch.DrawString(font, collisionSkinPositions[i][c].ToString(), new Vector2(505 + (30 * c), 50 * i), Color.White);

                }
            }
            */
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

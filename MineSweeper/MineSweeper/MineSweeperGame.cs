using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    class MineSweeperGame : Game
    {
        const int Rows = 16;//24
        const int Columns = 30;//40

        GraphicsDeviceManager _graphics;
        Texture2D _blockSprite;
        SpriteFont _setoFont;
        Random _random;
        Block[,] _blocks;

        int _totalMines;
        int _totalBlocks;
        int _totalFlags;

        public MineSweeperGame()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = Rows * 40,
                PreferredBackBufferWidth = Columns * 40
            };

        }
        
        protected override void Initialize()
        {
            
            _random = new Random();
            _blocks = CreateBlocks();
            _totalBlocks = Rows * Columns;
            
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (i > 0) _blocks[i, j].Up = _blocks[i - 1, j];
                    if (i < Rows - 1) _blocks[i, j].Down = _blocks[i + 1, j];
                    if (j > 0) _blocks[i, j].Left = _blocks[i, j - 1];
                    if (j < Columns - 1) _blocks[i, j].Right = _blocks[i, j + 1];
                }
            }

            Content.RootDirectory = "Resources";
            IsMouseVisible = true;
            base.Initialize();
        }
        

        Block[,] CreateBlocks()
        {
            Block[,] blocks = new Block[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Block block= new Block(GraphicsDevice, i, j);
                    block.HaveMine = _random.Next(0, 5) == 0;
                    if (block.HaveMine) _totalMines++;
                    block.Clicked += Block_Clicked;
                    block.RClicked += Block_RClicked;
                    block.BothClicked += Block_BothClicked;
                    blocks[i, j] = block;
                    Components.Add(block);
                }
            }

            return blocks;
        }

        private void Block_BothClicked(Button sender, MouseStateEventArgs e)
        {
            Block block = sender as Block;
            if (block.Trigged && !block.Flaged)
            {
                int minenum = block.GetMinesAround();
                int flagnum = block.GetFlagsAround();
                if(minenum== flagnum)
                {
                    block.TriggerAround();
                }
            }
        }

        private void Block_RClicked(Button sender, MouseStateEventArgs e)
        {
            Block block = sender as Block;
            if (!block.Trigged)
            {
                if (!block.Flaged)
                {
                    block.Fixed = true;
                    _totalFlags++;
                    _totalMines--;
                    block.Flaged = true;
                    block.BackGround = Color.LightGreen;
                }
                else
                {
                    block.Fixed = false;
                    _totalFlags--;
                    _totalMines++;
                    block.Flaged = false;
                    block.BackGround = Color.AliceBlue;
                }
            }
        }
        bool losed;
        bool win;
        private void Block_Clicked(Button sender, MouseStateEventArgs e)
        {
            Block block = sender as Block;
            if (!block.Trigged && !block.Flaged)
            {
                if(block.HaveMine)
                {
                    foreach (Block item in _blocks)
                    {
                        item.Fixed = true;
                        item.Trigged = true;
                        item.BackGround = item.HaveMine ? Color.Pink : Color.LightGray;
                    }
                    losed = true;
                }
                else
                {
                    _totalBlocks--;
                    block.Fixed = true;
                    block.Trigged = true;
                    block.BackGround = Color.LightGray;
                    int num = block.GetMinesAround();
                    if (num > 0)
                    {
                        block.Text = num.ToString();
                        switch (num)
                        {
                            case 1:
                                block.ForeGround = Color.Blue;
                                break;
                            case 2:
                                block.ForeGround = Color.Green;
                                break;
                            case 3:
                                block.ForeGround = Color.Red;
                                break;
                            case 4:
                                block.ForeGround = Color.MidnightBlue;
                                break;
                            case 5:
                                block.ForeGround = Color.DarkRed;
                                break;
                            case 6:
                                block.ForeGround = Color.ForestGreen;
                                break;
                            case 7:
                                block.ForeGround = Color.Black;
                                break;
                            case 8:
                                block.ForeGround = Color.DarkGray;
                                break;
                        }
                    }
                    else
                    {
                        block.TriggerAround();
                    }
                }

            }
        }

        protected override void LoadContent()
        {
            _setoFont = Content.Load<SpriteFont>("Fonts\\SetoFont");
            _blockSprite = Content.Load<Texture2D>("Images\\block");
            foreach (Block block in _blocks)
            {
                block.Font = _setoFont;
                block.Sprite = _blockSprite;
            }
        }
        KeyboardState lastKeyboardState;
        protected override void Update(GameTime gameTime)
        {
            if(IsActive)
            {
                KeyboardState curKeyboardState = Keyboard.GetState();
                if (curKeyboardState.IsKeyUp(Keys.F2) && lastKeyboardState.IsKeyDown(Keys.F2))
                {
                    ResetMine();
                }
                if(losed) Window.Title = "MineSweeper - You Lose! Press F2 to restart";
                else if(win) Window.Title = "MineSweeper - You Win! Press F2 to restart";
                else if(_totalBlocks == _totalMines + _totalFlags)
                {
                    win = true;
                    foreach (Block block in _blocks)
                    {
                        block.Trigged = true;
                        block.Fixed = true;
                        if (block.HaveMine) block.BackGround = Color.LightGreen;
                    }
                }
                else
                {
                    Window.Title = $"MineSweeper - {_totalMines} Mines Left";
                }
                
                lastKeyboardState = curKeyboardState;
                base.Update(gameTime);
            }
        }

        private void ResetMine()
        {
            _totalMines = 0;
            _totalBlocks = Rows * Columns;
            _totalFlags = 0;
            losed = false;
            win = false;
            foreach (Block block in _blocks)
            {
                block.BackGround = Color.AliceBlue;
                block.Fixed = false;
                block.Flaged = false;
                block.HaveMine = _random.Next(0, 5) == 0;
                if (block.HaveMine) _totalMines++;
                block.Trigged = false;
                block.Text = "";
            }
        }

    }
}

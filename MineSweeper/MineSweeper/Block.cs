using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    class Block : Button
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public Block Up { get; set; }
        public Block Down { get; set; }
        public Block Left { get; set; }
        public Block Right { get; set; }
        public bool HaveMine { get; set; }
        public bool Trigged { get; set; }
        public bool Flaged { get; set; }

        public Block(GraphicsDevice graphicsDevice,int row,int column) : base(graphicsDevice)
        {
            BackGround = Color.Azure;
            ForeGround = Color.Azure;
            HoveredColor = Color.White;
            PressedColor = Color.LightSkyBlue;
            Row = row;
            Column = column;
            TextPosition.X = 12;
            TextPosition.Y = 4;
        }

        public override void Update(GameTime gameTime)
        {
            Position.X = Column * Width;
            Position.Y = Row * Width;
            base.Update(gameTime);
        }
        public override string ToString()
        {
            return $"Block[{Row},{Column}]";
        }

        public void TriggerAround()
        {
            Up?.Left?.DoClick();
            Up?.DoClick();
            Up?.Right?.DoClick();
            Right?.DoClick();
            Left?.DoClick();
            Down?.Left?.DoClick();
            Down?.DoClick();
            Down?.Right?.DoClick();
        }

        public int GetMinesAround()
        {
            int GetMineInternel(Block item)
            {
                int n = 0;
                if(item.Left?.HaveMine == true) n++;
                if(item?.HaveMine == true) n++;
                if(item.Right?.HaveMine == true) n++;
                return n;
            }
            int num = 0;
            if(Up != null) num += GetMineInternel(Up);
            num += GetMineInternel(this);
            if(Down != null) num += GetMineInternel(Down);
            return num;
        }

        public int GetFlagsAround()
        {
            int GetMineInternel(Block item)
            {
                int n = 0;
                if(item.Left?.Flaged == true) n++;
                if(item?.Flaged == true) n++;
                if(item.Right?.Flaged == true) n++;
                return n;
            }
            int num = 0;
            if(Up != null) num += GetMineInternel(Up);
            num += GetMineInternel(this);
            if(Down != null) num += GetMineInternel(Down);
            return num;
        }

    }
}

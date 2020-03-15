using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    class Program
    {
        public static void Main()
        {
#if DEBUG
            using (var game = new MineSweeperGame())
            {
                game.Run();
            }
#else
                try
                {
                    using (var game = new MineSweeperGame())
                    {
                        game.Run();
                    }
                }
                catch (Exception ex)
                {
                    string error = ex.ToString();
                    MessageBox.Show(error, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    File.WriteAllText("error.log", error);
                    Clipboard.SetText(error);
                }
#endif
        }
    }
}

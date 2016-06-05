//#define CATCHING_ALL_ERRORS

using System;
using System.Windows.Forms;

namespace GodsWill_ASCIIRPG
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if CATCHING_ALL_ERRORS
            try
#endif
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new GameForm());
            }
#if CATCHING_ALL_ERRORS
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
#endif
        }
    }
}

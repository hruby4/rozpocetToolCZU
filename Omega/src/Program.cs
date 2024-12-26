using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Omega
{
    internal static class Program
    {
        /// <summary>
        /// Method <c>MyClosedHandler</c> closes a program.
        /// </summary>
        public static void MyClosedHandler(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.ExitThread();
        }

        public static List<Product> initialize_products = new List<Product>();
        /// <summary>
        /// Hlavní vstupní bod aplikace.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Login());
            }
            catch(Exception e) { 
                MessageBox.Show(e.ToString());
            }
           
            
        }

        

    }
}

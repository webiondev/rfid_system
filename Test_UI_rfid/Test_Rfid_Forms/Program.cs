using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Test_Rfid_Forms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
      //    Application.Run(new AddAssetForm());
      //    Application.Run(new WarehouseForm());
  //Application.Run(new Shelves());
          Application.Run(new rfid_tag());

            
        }
    }
}

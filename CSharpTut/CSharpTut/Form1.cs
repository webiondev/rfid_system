using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace CSharpTut
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void test() { }
        private void button1_Click(object sender, EventArgs e)
        {
            
          

                //Thread button = new Thread(new ThreadStart(test));

               
                //button.Start();
                button1.Text = "button started";
                //Thread.Sleep(1000);
                //button1.Text = "button stopped";
               // button.Abort();
               
                
        }
    }
}

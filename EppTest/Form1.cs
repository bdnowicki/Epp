using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EppParser;
using System.IO;
using EppParser.Classes;

namespace EppTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EppFile epp = new EppFile();
            epp.LoadEpp(new FileStream(@"C:\udost\test.epp", FileMode.Open));
            textBox1.Text = epp.GetString();
            File.WriteAllText(@"C:\udost\test2.epp", epp.GetString(), Encoding.GetEncoding(1250));
        }
    }
}

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
            epp.LoadEpp(new FileStream(@"C:\udost\kontrolne - Vegamarket.epp", FileMode.Open));
            textBox1.Text = epp.GetString();
            File.WriteAllText(@"C:\udost\test2.epp", epp.GetString(), Encoding.GetEncoding(1250));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EppFile epp = new EppFile();
            epp.LoadEpp(new FileStream(@"C:\udost\kontrolne - Vegamarket.epp", FileMode.Open));
            epp.Info.Opis.Clear();
            epp.Info.Opis.AddRangeObjects(new object[]
           {
                "1.05",
                3,
                1250,
                "EppParser",
                "VEGAMARKET",
                "VEGAMARKET",
                "VEGAMARKET Piotr Spyrczak",
                "Wrocław",
                "52-013",
                "ul. Opolska 143a",
                "8991385553",
                "MAG",
                "Główny",
                "Magazyn główny",
                "",
                1,
                DateTime.Now,
                DateTime.Now,
                "Spyrczak Piotr",
                DateTime.Now,
                "Polska",
                "PL",
                "PL8991385553",
                1
            });

            textBox1.Text = epp.GetString();
        }
    }
}

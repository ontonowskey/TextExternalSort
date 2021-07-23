using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TxtSort
{
    public partial class Form1 : Form
    {
        static string path = $@"C:\\TestAntonov\Text.txt";

        public Form1()
        {
            InitializeComponent();
            label1.Visible = false;
            if (System.IO.File.Exists(path))
                label1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TxtFile.Delete();
            TxtFile.Start(path);
            if (System.IO.File.Exists(path))
                label1.Visible = true;
            else label1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Sort.Split(path);
            Sort.SortParts();
            Sort.MergeParts();
        }
    }
}

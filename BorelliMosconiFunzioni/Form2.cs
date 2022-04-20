using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BorelliMosconiFunzioni
{
    public partial class Form2 : Form
    {
        public int range2 { get; set; }
        public double precisione2 { get; set; }

        float precisione = 0;
        int range = 0;

        public Form2()
        {
            InitializeComponent();
            precisione2 = 0.25;
            trackBar1.Value = Convert.ToInt32((precisione2 * 100) + 1);
            label2.Text= $"{precisione2}";

            range2 = 25000;
            trackBar2.Value = range2 / 1000;
            label4.Text = $"{range2}";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label2.Text = Convert.ToString(precisione2);
            precisione = trackBar1.Value;
            precisione2 = ((precisione+1) / 100);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label4.Text = $"{range2}";
            range = trackBar2.Value;
            range2 = (range * 1000)+1000;
        }
    }
}

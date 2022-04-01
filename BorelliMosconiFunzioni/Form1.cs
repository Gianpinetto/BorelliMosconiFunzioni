using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OxyPlot;
using OxyPlot.WindowsForms;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Utilities;
using OxyPlot.Legends;

namespace BorelliMosconiFunzioni
{
    public partial class Form1 : Form
    {
        PlotView pv = new PlotView();
        FunctionSeries fs = new FunctionSeries();
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pv.Location = new Point(0, 0);
            pv.Size = new Size(1000, 500);
            Controls.Add(pv);


            pv.Model = new PlotModel { Title = "CIAO" };
            fs.Points.Add(new DataPoint(0, 4));
            fs.Points.Add(new DataPoint(10, 10));
            pv.Model.Series.Add(fs);

        }
    }
}

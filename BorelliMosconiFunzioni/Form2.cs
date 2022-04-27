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

        public double PuntoInizio { get; set; }

        public double ForzaPalermo { get; set; }

        float precisione = 0;
        int range = 0;
        int volte = 0;

        double[] ValoriPrecisioni = new double[] { 0.010, 0.020, 0.050, 0.10, 0.20, 0.25, 0.50, 1.0 };
        public Form2()
        {
            InitializeComponent();

            textBox1.Enabled = false;
            textBox2.Enabled = false;

            //trackBar1.Enabled = false;

            precisione2 = 0.01;
            trackBar1.Value = 0; //è l'indice del vettore in cui c'è 0.01
            label2.Text = $"{precisione2}";

            range2 = 10000;
            trackBar2.Value = range2 / 1000;
            label4.Text = $"{range2}";

            PuntoInizio = -(range2 / 2) * precisione2;
            textBox1.Text = $"{PuntoInizio}";
            textBox2.Text = $"{Math.Abs(PuntoInizio)}";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            precisione = trackBar1.Value;
            precisione2 = ValoriPrecisioni[Convert.ToInt32(precisione)];
            label2.Text = Convert.ToString(precisione2);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {

            range = trackBar2.Value;
            range2 = (range * 1000);
            label4.Text = $"{range2}";
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            ForzaPalermo = 0;
            e.Cancel = true;
            bool condizione = VerificaString(textBox1.Text, textBox2.Text);
            if (checkBox1.Checked == true && condizione == true)
            {
                ForzaPalermo = 1;
                int RangeMinimo = int.Parse(textBox1.Text) - 1;
                int RangeMassimo = int.Parse(textBox2.Text) + 1;
                double rangePROVVISORIO = (RangeMassimo - RangeMinimo) / precisione2;
                range2 = Convert.ToInt32(rangePROVVISORIO);
                PuntoInizio = RangeMinimo;
                this.Hide();
            }
            else
            {
                if (condizione == false)
                    MessageBox.Show("Inserire un testo valido");
                else
                {
                    trackBar1_Scroll(sender, e);
                    trackBar2_Scroll(sender, e);
                    PuntoInizio = -(range2 / 2) * precisione2;
                    this.Hide();
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (volte % 2 == 0)
            {
                trackBar2.Enabled = false;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
            }
            else
            {
                trackBar2.Enabled = true;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
            volte++;
        }

        public static bool VerificaString(string RangeMinimo, string RangeMaximo) 
        {
            RangeMinimo = RangeMinimo.Replace(" ", "");
            RangeMaximo = RangeMaximo.Replace(" ", "");

            char[] minimo = RangeMinimo.ToCharArray();
            char[] maximo = RangeMaximo.ToCharArray();

            for (int i = 0; i < RangeMinimo.Length; i++)
            {
                if ((int)minimo[i] != 45 && (int)minimo[i] != 43 && ((int)minimo[i] < 48 || (int)minimo[i] > 57)) //45 - 43 + 48 0 57 9
                    return false;
            }
            for (int i = 0; i < RangeMaximo.Length; i++)
            {
                if ((int)maximo[i] != 45 && (int)maximo[i] != 43 && ((int)maximo[i] < 48 || (int)maximo[i] > 57))
                    return false;
            }

            if ((RangeMinimo.Length == 0 || RangeMaximo.Length == 0) || (int.Parse(RangeMinimo) >= int.Parse(RangeMaximo)) || float.Parse(RangeMinimo) < -15000 || float.Parse(RangeMaximo) > 15000)
                return false;
            else
                return true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}

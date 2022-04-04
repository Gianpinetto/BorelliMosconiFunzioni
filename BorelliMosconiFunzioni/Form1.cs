using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text.RegularExpressions; //serve per rimuovere pezzi di stringa
using org.matheval;

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
        int controllo = 0;
        double[,] coordinate = new double[2, 1000];
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (controllo == 1)
            {
                PlotView pv = new PlotView();
                FunctionSeries fs= new FunctionSeries();

                pv.Location = new Point(0, 0);
                pv.Size = new Size(750, 500);
                Controls.Add(pv);


                pv.Model = new PlotModel { Title = "CIAO" };
                for (int i = 0; i < 100; i++)
                {
                    fs.Points.Add(new DataPoint(coordinate[0, i], coordinate[1, i]));
                }

                pv.Model.Series.Add(fs);


                //pv.Model.Series.Add(new FunctionSeries(Math.Sin, -200, 200, 0.1, "Sin(x)"));
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string funzione = textBox2.Text;
            int contatore = 0;
            double x = -50;
            funzione = DenominatoreParentesi(funzione); //aggiungo le tonde al denominatore
            string backup = funzione;
            bool[] condizioni = new bool[100];

            while (contatore < 100)
            {
                funzione = backup;
                try
                {
                    Risoluzione(funzione, coordinate, contatore, ref x, 1);
                }
                catch
                {
                    condizioni[contatore] = true;
                }
                //Console.WriteLine($"CONTATORE: {contatore} X:{x - 1} CONDIZIONE: {condizioni[contatore]} ");
                contatore++;
            }

            x = -50;
            contatore = 0;
            while (contatore < 100)
            {
                funzione = backup;
                if (condizioni[contatore] != true)
                {
                    Risoluzione(funzione, coordinate, contatore, ref x, 0);
                }
                else
                    x += 1;
                contatore++;
            }
            controllo = 1;
            Form1_Load(sender, e);

        }

        //parte funzioni nostre
        public static string DenominatoreParentesi(string Funzione) //aggiungiamo un +0 prima della chiusera di ogni parentesi
        {
            string RisFin = Funzione;
            for (int i = 0; i < Funzione.Length - 1; i++)
            {
                if (Funzione.Substring(i, 1) == "/" && Funzione.Substring(i + 1, 1) != "(") //se c'è denominatore e poi non c'è una tonda
                {
                    RisFin = Funzione.Insert(i + 1, "(");
                    int k = 0;
                    for (k = i + 1; k < Funzione.Length - 1; k++)
                    {
                        Funzione = RisFin;
                        RisFin = Funzione;
                        if (Funzione.Substring(k, 1) == "/" || Funzione.Substring(k, 1) == "+" ||
                            Funzione.Substring(k, 1) == "-" || Funzione.Substring(k, 1) == "*")
                        {
                            RisFin = Funzione.Insert(k, ")");
                            k = Funzione.Length + 1; //esc subito dalla funzione
                        }
                    }
                    if (k != Funzione.Length + 2)
                    {
                        Funzione = RisFin;
                        RisFin = Funzione;
                        RisFin = Funzione.Insert(Funzione.Length, ")");
                    }
                    i += 2;
                    Funzione = RisFin;
                }
            }
            return RisFin;
        }
        public static void Risoluzione(string funzione, double[,] coordinata, int contatore, ref double x, int condizione)
        {
            double y = 0;
            string xStringa = "";
            xStringa = Convert.ToString(x);
            xStringa = Regex.Replace(xStringa, "[,]", "."); //converto la virgola in punto perchè sennò da errore

            for (int i = 0; i < funzione.Length; i++)
            {
                if (funzione.Substring(i, 1).ToUpper() == "X")
                {
                    funzione = funzione.Remove(i, 1); //tolgo la x
                    funzione = funzione.Insert(i, $"*({xStringa})"); //rimpiazzo con "*"+ il numero al momento
                }
            }

            Expression expr = new Expression(funzione);
            x += 1;
            var value = expr.Eval(); //calcolo il valore della nuova espressione
            y = Convert.ToDouble(value); //converto in double

            if (condizione != 1) //se non è la volta in cui entro nel ciclo solo per controllare le condizioni
            {
                coordinata[0, contatore] = x - 1;
                coordinata[1, contatore] = y;
            }
            contatore++;
        }
    }
}
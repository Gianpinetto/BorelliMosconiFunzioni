using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;

using System.Text.RegularExpressions; //serve per rimuovere pezzi di stringa
using org.matheval;

using OxyPlot;
using OxyPlot.WindowsForms;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace BorelliMosconiFunzioni
{

//   System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
//customCulture.NumberFormat.NumberDecimalSeparator = ".";

//System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

    public partial class Form1 : Form
    {

        int controllo = 0;
        int range = 10000;
        double aumentoX = 0.025;
        double[,] coordinate = new double[2, 10000];
        bool[] condizioni = new bool[10000];

        PlotView pv = new PlotView();

        PlotModel model = new PlotModel();
        
        FunctionSeries fs = new FunctionSeries();
        FunctionSeries fx1 = new FunctionSeries();
        FunctionSeries fx2 = new FunctionSeries();
        FunctionSeries fx3 = new FunctionSeries();
        FunctionSeries absf1 = new FunctionSeries();
        FunctionSeries absf2 = new FunctionSeries();
        FunctionSeries absf3 = new FunctionSeries();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;


            if (controllo == 1)
            {

                var yAxis = new OxyPlot.Axes.LinearAxis();
                var xAxis = new OxyPlot.Axes.LinearAxis();
                xAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;

                yAxis.Position = OxyPlot.Axes.AxisPosition.Left; //mio

                pv.Model = model; //pv è il vostro oggetto esistente, assegniamo il model cui sopra

                pv.Location = new Point(0, 0);
                pv.Size = new Size(750, 500);
                Controls.Add(pv);


                pv.Model = new PlotModel { Title = "CIAO" };

                pv.Model.Axes.Add(xAxis);//mio


                pv.Model.Axes.Add(yAxis); //aggiungiamo gli assi
                for (int i = 1; i < range; i++)
                {
                    if (condizioni[i]==false)
                        fs.Points.Add(new DataPoint(coordinate[0, i], coordinate[1, i]));
                    else
                        fs.Points.Add(DataPoint.Undefined);                    
                }

                pv.Model.Series.Add(fs);

                pv.Model.Axes[1].Minimum = -50;  //così setto il valore minimo, nota [1] indica l'asse x perchè sopra è stato inserito per secondo, [0] sarà l'asse y
                pv.Model.Axes[1].Maximum = 50; // e massimo


                pv.Model.Axes[0].Minimum = -50;  //così setto il valore minimo, nota [1] indica l'asse x perchè sopra è stato inserito per secondo, [0] sarà l'asse y
                pv.Model.Axes[0].Maximum = 50; // e massimo



                pv.Model.Axes[1].AbsoluteMinimum = -100000;  //così setto il valore minimo, nota [1] indica l'asse x perchè sopra è stato inserito per secondo, [0] sarà l'asse y
                pv.Model.Axes[1].AbsoluteMaximum = 100000; // e massimo


                pv.Model.Axes[0].AbsoluteMinimum =-range/2;  //così setto il valore minimo, nota [1] indica l'asse x perchè sopra è stato inserito per secondo, [0] sarà l'asse y
                pv.Model.Axes[0].AbsoluteMaximum = range; // e massimo

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string funzione = textBox2.Text;
            int contatore = 0;
            double x = -25;
            funzione = DenominatoreParentesi(funzione); //aggiungo le tonde al denominatore
            string backup = funzione;

            while (contatore < range)
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
                contatore++;
            }

            x = -25;
            contatore = 0;
            while (contatore < range)
            {
                funzione = backup;
                if (condizioni[contatore] != true)
                {
                    Risoluzione(funzione, coordinate, contatore, ref x, 0);
                }
                else
                    x += aumentoX;
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
            double aumentoX = 0.025;
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
            x += aumentoX;
            var value = expr.Eval(); //calcolo il valore della nuova espressione
            y = Convert.ToDouble(value); //converto in double

            if (condizione != 1) //se non è la volta in cui entro nel ciclo solo per controllare le condizioni
            {
                coordinata[0, contatore] = x - aumentoX;
                coordinata[1, contatore] = y;
            }
            contatore++;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pv.Model.Series.Clear();
            pv.Refresh();
        }
    }
}
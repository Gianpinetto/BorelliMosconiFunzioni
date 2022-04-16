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
    public partial class Form1 : Form
    {
        int[] premuto = new int[7] {0,0,0,0,0,0,0};
        int controllo = 0;
        int range = 10000;
        double aumentoX = 0.25, ymin = 0, ymax = 0;
        double[,,] coordinate = new double[7,2, 10000];
        bool[,] condizioni = new bool[7,10000];
        string funzione = "";

        PlotView pv = new PlotView();

        PlotModel model = new PlotModel();

        FunctionSeries fs = new FunctionSeries();
        FunctionSeries x = new FunctionSeries();
        FunctionSeries y = new FunctionSeries();
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
                fs = new FunctionSeries();
                x = new FunctionSeries();
                y = new FunctionSeries();
                fx1 = new FunctionSeries();
                fx2 = new FunctionSeries();
                fx3 = new FunctionSeries();
                absf1 = new FunctionSeries();
                absf2 = new FunctionSeries();
                absf3 = new FunctionSeries();

                var yAxis = new LinearAxis();
                var xAxis = new LinearAxis();

                xAxis.Position = AxisPosition.Bottom;
                yAxis.Position = AxisPosition.Left; //mio

                pv.Model = model; //pv è il vostro oggetto esistente, assegniamo il model cui sopra

                pv.Location = new Point(0, 0);
                pv.Size = new Size(750, 500);
                Controls.Add(pv);
                pv.Model.InvalidatePlot(true);
                pv.Model = new PlotModel { Title = $"GRAFICO DELLA FUNZIONE {textBox2.Text}" };

                pv.Model.Axes.Add(xAxis);//mio
                pv.Model.Axes.Add(yAxis); //aggiungiamo gli assi

                x.Points.Add(new DataPoint(0, -100000));
                x.Points.Add(new DataPoint(0, 100000));
                x.Color = OxyColor.FromArgb(255, 0, 0, 0); ;
                pv.Model.Series.Add(x);

                y.Points.Add(new DataPoint(-100000, 0));
                y.Points.Add(new DataPoint(100000, 0));
                y.Color = OxyColor.FromArgb(255, 0, 0, 0); ;
                pv.Model.Series.Add(y);

                for (int InDiCe = 0; InDiCe < 7; InDiCe++)
                {
                    if (InDiCe==0)
                        DisegnaPunti(fs,pv, InDiCe, range, condizioni, coordinate);
                    else if (InDiCe==1)
                        DisegnaPunti(fx1, pv, InDiCe, range, condizioni, coordinate);
                    else if (InDiCe == 2)
                        DisegnaPunti(fx2, pv, InDiCe, range, condizioni, coordinate);
                    else if (InDiCe == 3)
                        DisegnaPunti(fx3, pv, InDiCe, range, condizioni, coordinate);
                    else if (InDiCe == 4)
                        DisegnaPunti(absf1, pv, InDiCe, range, condizioni, coordinate);
                    else if (InDiCe == 5)
                        DisegnaPunti(absf2, pv, InDiCe, range, condizioni, coordinate);
                    else if (InDiCe == 6)
                        DisegnaPunti(absf3, pv, InDiCe, range, condizioni, coordinate);
                }

                pv.Model.Axes[1].Minimum = -100;  //[1]=Y  [0] = X
                pv.Model.Axes[1].Maximum = 100;
                pv.Model.Axes[0].Minimum = -100;
                pv.Model.Axes[0].Maximum = 100;

                pv.Refresh();

                /*pv.Model.Axes[0].AbsoluteMinimum = coordinate[0,0, 0]-0.5;
                pv.Model.Axes[0].AbsoluteMaximum = coordinate[0, 0, (coordinate.GetLength(2)-1)]+0.5;
                pv.Model.Axes[1].AbsoluteMinimum = ymin-0.5;
                pv.Model.Axes[1].AbsoluteMaximum = ymax+0.5;*/

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int a = 0; a < coordinate.GetLength(0); a++)
            {
                for (int i = 0; i < coordinate.GetLength(1); i++)
                {
                    for (int j = 0; j < coordinate.GetLength(2); j++)
                    {
                        coordinate[a, i, j] = 0;
                        condizioni[a, j] = false;
                    }
                }
            }

            pv.Model.Series.Clear();
            pv.Model.InvalidatePlot(true);
            controllo = 0;
            pv.Refresh();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            funzione = textBox2.Text;
            TrovaPuntiEcondizioni(funzione, range, aumentoX, coordinate, ref ymin, ref ymax, condizioni, 0, ref controllo);
            Form1_Load(sender, e);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            PremiBottoni(fx1, pv, 1, ref premuto, funzione, range, aumentoX, coordinate, ref ymin, ref ymax, condizioni, ref controllo);
            if (premuto[1]-1==0)
                Form1_Load(sender, e);
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            PremiBottoni(fx2, pv, 2, ref premuto, funzione, range, aumentoX, coordinate, ref ymin, ref ymax, condizioni, ref controllo);
            if (premuto[2] - 1 == 0)
                Form1_Load(sender, e);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            PremiBottoni(fx3, pv, 3, ref premuto, funzione, range, aumentoX, coordinate, ref ymin, ref ymax, condizioni, ref controllo);
            if (premuto[3] - 1 == 0)
                Form1_Load(sender, e);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            PremiBottoni(absf1, pv, 4, ref premuto, funzione, range, aumentoX, coordinate, ref ymin, ref ymax, condizioni, ref controllo);
            if (premuto[4] - 1 == 0)
                Form1_Load(sender, e);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            PremiBottoni(absf2, pv, 5, ref premuto, funzione, range, aumentoX, coordinate, ref ymin, ref ymax, condizioni, ref controllo);
            if (premuto[5] - 1 == 0)
                Form1_Load(sender, e);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            PremiBottoni(absf3, pv, 6, ref premuto, funzione, range, aumentoX, coordinate, ref ymin, ref ymax, condizioni, ref controllo);
            if (premuto[6] - 1 == 0)
                Form1_Load(sender, e);
        }

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

        public static void DisegnaPunti(FunctionSeries punti, PlotView pd, int InDiCe, int range, bool[,] condizioni, double[,,] coordinate)
        {
            for (int i = 1; i < range; i++)
            {
                if (condizioni[InDiCe, i] == false) //se non è la condizione di esistenza interrompo linea
                    punti.Points.Add(new DataPoint(coordinate[InDiCe, 0, i], coordinate[InDiCe, 1, i]));
                else
                    punti.Points.Add(DataPoint.Undefined);
            }
            pd.Model.Series.Add(punti);
        }

        public static void Risoluzione(ref string funzione, double[,,] coordinata, int contatore, ref double x, int condizione, ref double ymin, ref double ymax, double aumentoX, int IndiceTrasformazione)
        {
            double y = 0; //1=f(-x); 2= -f(x); 3= -f(-x); 4= f(abs(x)); 5= abs(f(x)); 6=abs(f(abs(x)))
            if (condizione == 1)
            {
                string xStringa = "";
                xStringa = Convert.ToString(x);
                xStringa = Regex.Replace(xStringa, "[,]", "."); //converto la virgola in punto perchè sennò da errore

                for (int i = 0; i < funzione.Length; i++)
                {
                    if (funzione.Substring(i, 1).ToUpper() == "X")
                    {
                        funzione = funzione.Remove(i, 1); //tolgo la x

                        if (IndiceTrasformazione == 0 || IndiceTrasformazione == 2 || IndiceTrasformazione == 5) //se è "tradizionale" oppure gli altri casi aggiungo solo "*"+il numero
                            funzione = funzione.Insert(i, $"*({xStringa})"); //rimpiazzo con "*"+ il numero al momento
                        else if (IndiceTrasformazione == 1 || IndiceTrasformazione == 3)
                            funzione = funzione.Insert(i, $"*((-{xStringa}))");
                        else if (IndiceTrasformazione == 4 || IndiceTrasformazione == 6)
                            funzione = funzione.Insert(i, $"*(abs({xStringa}))");
                    }
                }
            }

            Expression expr = new Expression(funzione);
            x += aumentoX;
            var value = expr.Eval(); //calcolo il valore della nuova espressione
            y = Convert.ToDouble(value); //converto in double

            if (IndiceTrasformazione == 2 || IndiceTrasformazione == 3) //per terminare casi specifici di prima
                y = -y;
            else if (IndiceTrasformazione == 5 || IndiceTrasformazione == 6)
                y = Math.Abs(y);


            if (y < ymin) //per trovare nel grafico
                ymin = y;
            if (y > ymax)
                ymax = y;

            if (condizione == 0) //se non è la volta in cui entro nel ciclo solo per controllare le condizioni
            {
                coordinata[IndiceTrasformazione, 0, contatore] = x - aumentoX;
                coordinata[IndiceTrasformazione, 1, contatore] = y;
            }
            contatore++;
        }

        public static void TrovaPuntiEcondizioni(string funzione, int range, double aumentoX, double[,,] coordinate, ref double ymin, ref double ymax, bool[,] condizioni, int indice, ref int controllo)
        {//questa funione trova sia i punti che le condizioni di esistenza della funzione appena passata 
            int contatore = 0;
            double x = -(range / 2) * aumentoX; //in questo modo con questa formula trovo sempre metà tra positivo e negativo
            double xCiao = x;
            funzione = DenominatoreParentesi(funzione); //aggiungo le tonde al denominatore
            string backup = funzione;

            while (contatore < range) //condizoni di esistenza 
            {
                funzione = backup; //sennò mi ha sostituito la "x" e io non la cambio più
                try
                {
                    Risoluzione(ref funzione, coordinate, contatore, ref x, 1, ref ymin, ref ymax, aumentoX, indice); //provo a risolvere
                }
                catch
                {
                    condizioni[indice, contatore] = true; //se fallisco rendo condizione true (applico "eccezione")
                }

                xCiao = x - aumentoX; //tolgo di nuovo l'aumento perchè sotto se è possibile ricalcolo subito ed essendo ref ha già aumentato da solo
                if (condizioni[indice, contatore] != true) //se la condizione lo permette calcolo subito
                {
                    Risoluzione(ref funzione, coordinate, contatore, ref xCiao, 0, ref ymin, ref ymax, aumentoX, indice);
                    controllo = 1;
                }
                contatore++;
            }
        }

        public static void PremiBottoni(FunctionSeries InsiemeDiCirconferenze, PlotView pci, int indice, ref int[]premuto, string funzione, int range, double aumentoX, double [,,]coordinate, ref double ymin, ref double ymax, bool [,] condizioni, ref int controllo)
        {
            if (premuto[indice] == 0) //se è la prima volta calcolo 
                TrovaPuntiEcondizioni(funzione, range, aumentoX, coordinate, ref ymin, ref ymax, condizioni, indice, ref controllo);
            else if (premuto[indice] % 2 == 1) //se il num è dispari vuol dire che sto disattivando il bottone quindi rendo linea invisibile
            {
                InsiemeDiCirconferenze.LineStyle = LineStyle.None;
                pci.Refresh();
            }
            else //sennò la rendo visibile
            {
                InsiemeDiCirconferenze.LineStyle = LineStyle.Solid;
                pci.Refresh();
            }
            premuto[indice]++; //aumento contatore
        }

    }
}
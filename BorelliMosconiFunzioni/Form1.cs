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
        Form2 Impostasiu = new Form2();

        int[] premuto = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
        int controllo = 0, range = 10000, contatoreRapidissimo = 0;
        double aumentoX = 0.01, PuntoInizio = 0;
        double[,,] coordinate = new double[7, 2, 25000];
        bool[,] condizioni = new bool[7, 25000];

        public struct dimensioniGrafico
        {
            public double ymin;
            public double ymax;
            public double xmin;
            public double xmax;
        }

        dimensioniGrafico dimGraf;

        string funzione = " ";

        int VecchiaLunghezzaCasellaTesto = 0, NuovaLunghezzaCasellaTesto = 0;

        PlotView pv = new PlotView();

        PlotModel model = new PlotModel();

        FunctionSeries fs, x, y, fx1, fx2, fx3, absf1, absf2, absf3 = new FunctionSeries();


        public Form1()
        {
            InitializeComponent();
            button1.Enabled = false;

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.T)) //dove metti funzione
                textBox2.Focus();
            else if (keyData == (Keys.Control | Keys.I)) //impostazioni
                button3.PerformClick();
            else if (keyData == (Keys.Enter)) //calc
                button1.PerformClick();
            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            label1.Text = "";
            label2.Text = "";
            //MessageBox.Show("SONO FUROI");

            if (controllo >= 1)
            {
                //MessageBox.Show("SONO DENTRO 1");
                fs = new FunctionSeries();
                x = new FunctionSeries();
                y = new FunctionSeries();
                fx1 = new FunctionSeries();
                fx1.Color = OxyColor.FromArgb(255, 255, 0, 0); //rosso
                fx2 = new FunctionSeries();
                fx2.Color = OxyColor.FromArgb(255, 255, 128, 0); //arancio
                fx3 = new FunctionSeries();
                fx3.Color = OxyColor.FromArgb(255, 255, 255, 0); //yelloww
                absf1 = new FunctionSeries();
                absf1.Color = OxyColor.FromArgb(255, 0, 255, 255); //azzurro
                absf2 = new FunctionSeries();
                absf2.Color = OxyColor.FromArgb(255, 0, 0, 255); //blu
                absf3 = new FunctionSeries();
                absf3.Color = OxyColor.FromArgb(255, 255, 51, 255); //rosa

                var yAxis = new LinearAxis();
                var xAxis = new LinearAxis();

                xAxis.Position = AxisPosition.Bottom;
                yAxis.Position = AxisPosition.Left; //mio


                pv.Model = model; //pv è il vostro oggetto esistente, assegniamo il model cui sopra

                pv.Location = new Point(0, 0);
                pv.Size = new Size(800, 525);
                Controls.Add(pv);
                pv.Model.InvalidatePlot(true);
                pv.Model = new PlotModel { Title = $"GRAFICO DELLA FUNZIONE: {textBox2.Text}" };

                pv.Model.Axes.Add(xAxis);//mio
                pv.Model.Axes.Add(yAxis); //aggiungiamo gli assi

                x.Points.Add(new DataPoint(0, dimGraf.ymin - 1));
                x.Points.Add(new DataPoint(0, 0));
                x.Points.Add(new DataPoint(0, dimGraf.ymax + 1));
                x.Color = OxyColor.FromArgb(255, 0, 0, 0);
                pv.Model.Series.Add(x);

                y.Points.Add(new DataPoint(dimGraf.xmin, 0));
                y.Points.Add(new DataPoint(0, 0));
                y.Points.Add(new DataPoint(dimGraf.xmax + 1, 0));
                y.Color = OxyColor.FromArgb(255, 0, 0, 0);
                pv.Model.Series.Add(y);

                for (int InDiCe = 0; InDiCe < 7; InDiCe++)
                {
                    if (InDiCe == 0)
                        DisegnaPunti(fs, pv, InDiCe, range, condizioni, coordinate);
                    else if (InDiCe == 1 && (premuto[InDiCe] - 1) % 2 == 0) //-1 perchè guardo quello da cui veniva prima, non quello è appena diventato lo stato attuale
                        DisegnaPunti(fx1, pv, InDiCe, range, condizioni, coordinate);
                    else if (InDiCe == 2 && (premuto[InDiCe] - 1) % 2 == 0)
                        DisegnaPunti(fx2, pv, InDiCe, range, condizioni, coordinate);
                    else if (InDiCe == 3 && (premuto[InDiCe] - 1) % 2 == 0)
                        DisegnaPunti(fx3, pv, InDiCe, range, condizioni, coordinate);
                    else if (InDiCe == 4 && (premuto[InDiCe] - 1) % 2 == 0)
                        DisegnaPunti(absf1, pv, InDiCe, range, condizioni, coordinate);
                    else if (InDiCe == 5 && (premuto[InDiCe] - 1) % 2 == 0)
                        DisegnaPunti(absf2, pv, InDiCe, range, condizioni, coordinate);
                    else if (InDiCe == 6 && (premuto[InDiCe] - 1) % 2 == 0)
                        DisegnaPunti(absf3, pv, InDiCe, range, condizioni, coordinate);
                }

                pv.Model.Axes[1].Minimum = -100;  //[1]=Y  [0] = X
                pv.Model.Axes[1].Maximum = 100;
                pv.Model.Axes[0].Minimum = -100;
                pv.Model.Axes[0].Maximum = 100;

                if (Impostasiu.ForzaPalermo == 0)
                {
                    pv.Model.Axes[0].AbsoluteMinimum = dimGraf.xmin;
                    pv.Model.Axes[0].AbsoluteMaximum = dimGraf.xmax;
                }
                else
                {
                    pv.Model.Axes[0].AbsoluteMinimum = PuntoInizio;
                    pv.Model.Axes[0].AbsoluteMaximum = dimGraf.xmax;
                }
                pv.Model.Axes[1].AbsoluteMinimum = dimGraf.ymin - 1;
                pv.Model.Axes[1].AbsoluteMaximum = dimGraf.ymax + 1;

                label1.Text = "";
                label2.Text = "";

                if (Impostasiu.CondEsBool)
                label1.Text = (CondizioniEsistenza(coordinate, condizioni, contatoreRapidissimo));

                if (Impostasiu.PariDispariBool)
                {
                    if (funzione == PariODispari(funzione, 0))
                        label2.Text = "PARI";
                    else if (PariODispari(funzione, 0) == PariODispari(funzione, 1))
                        label2.Text = "DISPARI";
                    else
                        label2.Text = "NE PARI NE DISPARI";
                }

                //label2.Text = PariDispari(coordinate, funzione, condizioni);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Impostasiu.Show();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            ResetTutto(coordinate, condizioni, premuto, ref controllo, pv, ref dimGraf);
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            checkBox6.Checked = false;

            aumentoX = Impostasiu.precisione2;

            if (aumentoX == 0)
                aumentoX = 0.1;

            range = Impostasiu.range2;

            if (range == 0)
                range = 1000;

            PuntoInizio = Impostasiu.PuntoInizio;
            //MessageBox.Show($"RANGE:{range} \nPRECISIONE:{aumentoX}");
            coordinate = new double[7, 2, range];
            condizioni = new bool[7, range];

            funzione = " ";
            funzione += textBox2.Text;

            bool controllOoOoOo = VerificaFunzione(funzione);
            if (controllOoOoOo == false)
                MessageBox.Show("Inserire una funzione valida");
            else
            {
                funzione = AggiungiSegno(funzione); //aggiunge segno tra xx e x+num
                funzione = DenominatoreParentesi(funzione); //aggiungo le tonde al denominatore e all'esponente
                funzione = AggiungiUno(funzione);
                string espressionee = " +";
                if (funzione.Substring(1, 1) != "-" && funzione.Substring(1, 1) != "+")
                {
                    espressionee += funzione.Substring(1, funzione.Length - 1);
                    funzione = espressionee;
                }
                funzione += "+0";
                //MessageBox.Show(funzione);
                TrovaPuntiEcondizioni(funzione, range, aumentoX, coordinate, condizioni, 0, ref controllo, PuntoInizio, ref dimGraf, ref contatoreRapidissimo);
                Form1_Load(sender, e);
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != String.Empty) //se non è più vuota la casella della funzione
                button1.Enabled = true;
            else
                button1.Enabled = false;
            NuovaLunghezzaCasellaTesto = textBox2.Text.Length;
            if (NuovaLunghezzaCasellaTesto - 1 >= 0 && NuovaLunghezzaCasellaTesto > VecchiaLunghezzaCasellaTesto && textBox2.Text.Substring(NuovaLunghezzaCasellaTesto - 1, 1) == "(")
            {
                textBox2.Text += ")";
                textBox2.Select(NuovaLunghezzaCasellaTesto - 1, 0);
            }
            VecchiaLunghezzaCasellaTesto = NuovaLunghezzaCasellaTesto;

        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (controllo >= 1)
                PremiBottoni(fx1, pv, 1, ref premuto, funzione, range, aumentoX, coordinate, condizioni, ref controllo, PuntoInizio, ref dimGraf, ref contatoreRapidissimo);
            else
                checkBox1.Checked = false;
            Form1_Load(sender, e);
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (controllo >= 1)
                PremiBottoni(fx2, pv, 2, ref premuto, funzione, range, aumentoX, coordinate, condizioni, ref controllo, PuntoInizio, ref dimGraf, ref contatoreRapidissimo);
            else
                checkBox2.Checked = false;
            Form1_Load(sender, e);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (controllo >= 1)
                PremiBottoni(fx3, pv, 3, ref premuto, funzione, range, aumentoX, coordinate, condizioni, ref controllo, PuntoInizio, ref dimGraf, ref contatoreRapidissimo);
            else
                checkBox3.Checked = false;
            Form1_Load(sender, e);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (controllo >= 1)
                PremiBottoni(absf1, pv, 4, ref premuto, funzione, range, aumentoX, coordinate, condizioni, ref controllo, PuntoInizio, ref dimGraf, ref contatoreRapidissimo);
            else
                checkBox4.Checked = false;
            Form1_Load(sender, e);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (controllo >= 1)
                PremiBottoni(absf2, pv, 5, ref premuto, funzione, range, aumentoX, coordinate, condizioni, ref controllo, PuntoInizio, ref dimGraf, ref contatoreRapidissimo);
            else
                checkBox5.Checked = false;
            Form1_Load(sender, e);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (controllo >= 1)
                PremiBottoni(absf3, pv, 6, ref premuto, funzione, range, aumentoX, coordinate, condizioni, ref controllo, PuntoInizio, ref dimGraf, ref contatoreRapidissimo);
            else
                checkBox6.Checked = false;
            Form1_Load(sender, e);
        }

        public static string DenominatoreParentesi(string Funzione) //aggiungiamo un +0 prima della chiusera di ogni parentesi
        {
            string RisFin = Funzione;
            for (int i = 0; i < Funzione.Length - 1; i++)
            {
                if ((Funzione.Substring(i, 1) == "/" || Funzione.Substring(i, 1) == "^") && Funzione.Substring(i + 1, 1) != "(") //se c'è denominatore e poi non c'è una tonda
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

        public static string AggiungiUno(string Funzione)
        {
            for (int i = 0; i < Funzione.Length; i++)
            {
                if (Funzione.Substring(i, 1).ToUpper() == "X")
                {
                    int j = i - 1;// i= pos di x

                    if (Funzione.Substring(j, 1).ToUpper() != "1" && Funzione.Substring(j, 1).ToUpper() != "2" && Funzione.Substring(j, 1).ToUpper() != "3" && Funzione.Substring(j, 1).ToUpper() != "4" && Funzione.Substring(j, 1).ToUpper() != "5" && Funzione.Substring(j, 1).ToUpper() != "6" && Funzione.Substring(j, 1).ToUpper() != "7" && Funzione.Substring(j, 1).ToUpper() != "8" && Funzione.Substring(j, 1).ToUpper() != "9" && Funzione.Substring(j, 1).ToUpper() != "0")
                        Funzione = Funzione.Insert(j + 1, "1");

                    i++;
                }
            }
            return Funzione;
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

        public static void Risoluzione(ref string funzione, double[,,] coordinata, int contatore, ref double x, int condizione, double aumentoX, int IndiceTrasformazione, ref dimensioniGrafico dimGraf)
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
            x = Math.Round(x, 2);//lo arrotondo perchè sennò aggiunge dopo un tot un sacco di zeri a caso
            var value = expr.Eval(); //calcolo il valore della nuova espressione
            y = Convert.ToDouble(value); //converto in double


            if (IndiceTrasformazione == 2 || IndiceTrasformazione == 3) //per terminare casi specifici di prima
                y = -y;
            else if (IndiceTrasformazione == 5 || IndiceTrasformazione == 6)
                y = Math.Abs(y);


            if (y < dimGraf.ymin) //per trovare limiti nel grafico
                dimGraf.ymin = y;
            if (y > dimGraf.ymax)
                dimGraf.ymax = y;

            if (x < dimGraf.xmin)
                dimGraf.xmin = x;
            else if (x > dimGraf.xmax)
                dimGraf.xmax = x;

            coordinata[IndiceTrasformazione, 0, contatore] = x - aumentoX;
            coordinata[IndiceTrasformazione, 1, contatore] = y;

            //MessageBox.Show($"X: '{coordinata[IndiceTrasformazione, 0, contatore]}' Y: '{coordinata[IndiceTrasformazione, 0, contatore]}'");

            contatore++;
        }

        public static void TrovaPuntiEcondizioni(string funzione, int range, double aumentoX, double[,,] coordinate, bool[,] condizioni, int indice, ref int controllo, double PuntoInizio, ref dimensioniGrafico dimGraf, ref int condizioneRapida)
        {//questa funione trova sia i punti che le condizioni di esistenza della funzione appena passata 
            int contatore = condizioneRapida= 0;
            double x = PuntoInizio; //in questo modo con questa formula trovo sempre metà tra positivo e negativo
            double xCiao = x;
            string backup = funzione;

            while (contatore < range) //condizoni di esistenza 
            {
                funzione = backup; //sennò mi ha sostituito la "x" e io non la cambio più
                try //condizioni esistenza + risoluzione contemporaneamente
                {
                    Risoluzione(ref funzione, coordinate, contatore, ref x, 1, aumentoX, indice, ref dimGraf); //provo a risolvere
                }
                catch
                {
                    if (indice == 0)//sarebbe per le condzioni di esistenza, calcolo già qui quante volte non vale la C.E anzichè farlo in un ciclo a parte. Vale solo per 0, la fuznione principale, senza trasformazioni
                        condizioneRapida++;
                    condizioni[indice, contatore] = true; //se fallisco rendo condizione true (applico "eccezione")
                    coordinate[indice, 0, contatore] = x - aumentoX;
                }
                contatore++;
            }
            controllo++;
        }
        public static void PremiBottoni(FunctionSeries InsiemeDiCirconferenze, PlotView pci, int indice, ref int[] premuto, string funzione, int range, double aumentoX, double[,,] coordinate, bool[,] condizioni, ref int controllo, double PuntoInizio, ref dimensioniGrafico dimGraf, ref int contatoreRapidissimo)
        {
            //BUONGIORNO PROFE, SE STA LEGGENDO QUESTA PARTE DI CODICE LA INVIATIAMO CALOROSAMENTE A SALUTARCI, ALLA PROSSIMA LEZIONE, CON L'ESPRESSIONE "ORNITORINCO". GRAZIE E BUONA CORREZIONE ♥

            if (premuto[indice] == 0) //se è la prima volta calcolo 
                TrovaPuntiEcondizioni(funzione, range, aumentoX, coordinate, condizioni, indice, ref controllo, PuntoInizio, ref dimGraf, ref contatoreRapidissimo);
            else if (premuto[indice] % 2 == 1) //se il num è dispari vuol dire che sto disattivando il bottone quindi rendo linea invisibile
                InsiemeDiCirconferenze.LineStyle = LineStyle.None;
            else //sennò la rendo visibile
                InsiemeDiCirconferenze.LineStyle = LineStyle.Solid;

            premuto[indice]++; //aumento contatore
        }
        public static void ResetTutto(double[,,] coordinate, bool[,] condizioni, int[] premuto, ref int controllo, PlotView pr, ref dimensioniGrafico dimGraf)
        {
            if (controllo >= 1)
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

                for (int i = 0; i < premuto.Length; i++)
                    premuto[i] = 0;

                //ymin = ymax = xmin = xmax = 0;
                dimGraf.ymin = dimGraf.xmin = 0;
                dimGraf.ymax = dimGraf.xmax = 1;

                pr.Model.Series.Clear();
                pr.Model.InvalidatePlot(true);
                pr.Refresh();
                controllo = 0;
            }
        }
        public static bool VerificaFunzione(string funzione)
        {
            funzione = funzione.ToUpper();
            string funzioneRidotta = funzione;
            int tondeaperte = 0, tondechiuse = 0;
            //sin, cos, tan, sqrt, abs, cbrt,ln,e, 

            for (int i = 0; i < funzioneRidotta.Length; i++)
            {
                if (funzioneRidotta.Substring(i, 1) == "(")
                    tondeaperte++;
                else if (funzioneRidotta.Substring(i, 1) == ")")
                    tondechiuse++;
            }
            if (tondeaperte != tondechiuse)
                return false;

            string[] caratteri = new string[] { "ABS", "SIN", "COS", "TAN", "SQRT", "LN", "E", "PI", "+", "-", "*", "/", "^", "(", ")", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", " " };

            for (int i = 0; i < caratteri.Length; i++)
            {
                funzioneRidotta = funzioneRidotta.Replace(caratteri[i], "");
                funzione = funzione.Replace(" ", "");
            }

            if (funzione.Substring(funzione.Length - 1, 1) == "+" || funzione.Substring(funzione.Length - 1, 1) == "-" || funzione.Substring(funzione.Length - 1, 1) == "*" || funzione.Substring(funzione.Length - 1, 1) == "/" || funzione.Substring(funzione.Length - 1, 1) == "^")
                return false; //lo metto qui sotto e non sopra perchè così ho già tolto gli spazi

            char[] stringa = funzione.ToCharArray();
            double sommaChar = 0, SommaCharRidotto = 0;
            char[] stringaRidotta = funzioneRidotta.ToCharArray();

            for (int i = 0; i < stringa.Length; i++) //somma della stringa privata solo degli spazi
                sommaChar += (double)stringa[i];

            for (int i = 1; i < stringaRidotta.Length; i++) //somma stringa privata di tutto
                SommaCharRidotto += (double)stringaRidotta[i];

            for (int i = 0; i < stringa.Length - 1; i++)//segni vicini
            {
                if (((int)stringa[i] == 42 || (int)stringa[i] == 43 || (int)stringa[i] == 45 || (int)stringa[i] == 47 || (int)stringa[i] == 94) &&
                    ((int)stringa[i + 1] == 42 || (int)stringa[i + 1] == 43 || (int)stringa[i + 1] == 45 || (int)stringa[i + 1] == 47 || (int)stringa[i + 1] == 94)) //42* 43+ 45- 47/ 94=^
                {
                    return false;
                }
            }

            for (int i = 0; i < stringaRidotta.Length; i++) //se all'interno della stringa ridotta (senza spazi) ci sono altri caratteri oltre alla X
            {
                if (stringaRidotta[i] != 'X')
                    return false;
            }

            if (sommaChar == 0) //se sono solo spazi
                return false;
            else
                return true;
        }
        public static string AggiungiSegno(string funzione)
        {
            funzione = funzione.ToUpper();
            for (int i = 0; i < funzione.Length - 1; i++)
            {
                if ((funzione.Substring(i, 2) == "XX") || (funzione.Substring(i, 1).ToUpper() == "X" && (funzione.Substring(i + 1, 1) == "9" || funzione.Substring(i + 1, 1) == "8" || funzione.Substring(i + 1, 1) == "7" ||
                    funzione.Substring(i + 1, 1) == "6" || funzione.Substring(i + 1, 1) == "5" || funzione.Substring(i + 1, 1) == "4" || funzione.Substring(i + 1, 1) == "3" ||
                    funzione.Substring(i + 1, 1) == "2" || funzione.Substring(i + 1, 1) == "1" || funzione.Substring(i + 1, 1) == "0"))) //se trovo due x attaccate oppure x seguita da un numero ci metto un * in mezzo
                    funzione = funzione.Insert(i + 1, "*");
            }
            return funzione;
        }
        public static string CondizioniEsistenza(double[,,] coordinate, bool[,] condizioni, int CondizioneRapida)
        {
            if (CondizioneRapida == 0)
                return "DOMINIO: R ";

            if (CondizioneRapida == 1)
            {
                int indice = 0;
                while (condizioni[0, indice] == false)
                {
                    indice++;
                }
                return $"DOMINIO: R-[{Math.Round(coordinate[0, 0, indice], 0)}]";
            }

            if (CondizioneRapida == 2)
            {
                int[] indice = new int[2];
                int hello = 0;
                for (int i = 0; i < condizioni.GetLength(1); i++)
                {
                    if (condizioni[0, i] == true)
                    {
                        indice[hello] = i;
                        hello++;
                    }
                }
                return $"DOMINIO: R-[{Math.Round(coordinate[0, 0, indice[0]]),0}, {Math.Round(coordinate[0, 0, indice[1]], 0)}]";
            }

            double[] valori = new double[] { -25000, -25000, -25000, -25000, -25000, -25000, -25000, -25000, -25000, -25000 };
            int helo = 0, GiaDentro = 0;
            string pezzifunzione = "";
            for (int i = 0; i < condizioni.GetLength(1); i++)
            {
                if (condizioni[0, i] == true && GiaDentro == 0)
                {
                    valori[helo] = Math.Round(coordinate[0, 0, i], 0);
                    helo++;
                    GiaDentro++;
                }
                else if (condizioni[0, i] == false && GiaDentro == 1)
                {
                    GiaDentro = 0;
                    valori[helo] = Math.Round(coordinate[0, 0, i], 0);
                    helo++;
                }

            }
            int minore = 0, entrato = 0;
            for (int i = 0; i < valori.Length - 1; i++)
            {
                if (valori[i] == -25000 && valori[i + 1] == -25000) //se si seguono perchè magari il primo può davvero essere -25000 il secondo non può esserlo
                    i = valori.Length;
                else
                {
                    if (valori[i] == valori[i + 1])
                    {
                        pezzifunzione += $" x≠{Math.Round(valori[i], 0)}";
                        entrato = 1;
                    }
                    else if (minore == 0)
                    { //valori[i] != coordinate[0, 0, 0] perchè sennò in alcuni casi mi dice che deve essre maggiore del limite
                        if (valori[i] != coordinate[0, 0, 0] && Math.Round(valori[i], 0) == valori[i]) //se l'arrotondato è uguale al non arrotondato vuol dire che è solo minore sennò è minore o uguale
                            pezzifunzione += $" x≤{Math.Round(valori[i], 0)}";
                        else if (valori[i] != coordinate[0, 0, 0])
                            pezzifunzione += $" x<{Math.Round(valori[i], 0)}";
                        minore++;
                    }
                    else if (minore == 1)
                    {
                        if (Math.Round(valori[i], 0) == valori[i])
                            pezzifunzione += $" x≥{Math.Round(valori[i], 0)}";
                        else
                            pezzifunzione += $" ≥x{Math.Round(valori[i], 0)}";
                        minore = 0;
                    }
                }
                if (entrato == 1)
                    i++;
                entrato = 0;

            }
            //return $"{valori[0]} {valori[1]} {valori[2]} {valori[3]} {valori[4]} {valori[5]} {valori[6]} {valori[7]} {valori[8]} {valori[9]}";
            return pezzifunzione;
        }

        public static string PariODispari(string espressione, int DaDoveVinei)//0=f di meno x 1= meno f di x
        {
            int j = 0;
            bool NonCeX = false, SuperatoPrimoSegno = false, SuperatoSecondoSegno = false;
            Regex rx = new Regex(@"[1-9]");
            string singoloNum;

            for (int i = 0; i < espressione.Length; i++)
            {
                //MessageBox.Show($"{i}");
                singoloNum = "";
                if (espressione.Substring(i, 1) == "X")
                {
                    NonCeX = false;
                    if (i != espressione.Length - 1)//se non è l'ultimo carattere
                    {
                        j = i + 1;//così salto la x
                        if (espressione.Substring(j, 1) == "^")//se è seguito da elevazione
                        {
                            j+=2; //così salto ^ e la parentesi
                            while (j < espressione.Length && rx.IsMatch(espressione.Substring(j, 1))) //finchè resta all'interno della stringa e resta un numero
                            {
                                singoloNum += espressione.Substring(j, 1);
                                j++;
                            }
                        }
                        else
                            singoloNum = "1"; //qui è quando la x è in mezzo all'espressione non alla fine
                    }
                    else
                        singoloNum = "1";//se termina la funzone con X è 1

                    if (int.Parse(singoloNum) % 2 != 0)
                    {
                        espressione = espressione.Insert(j, "*(-1)");
                        i += (j - i) + 4;
                    }
                    //entrato = true;

                }

                if ((espressione.Substring(i, 1) == "+" || espressione.Substring(i, 1) == "-" ||
                    espressione.Substring(i, 1) == "*" || espressione.Substring(i, 1) == "/") && NonCeX && SuperatoSecondoSegno && DaDoveVinei == 1)
                {
                    espressione = espressione.Insert(i, "*(-4)");
                    i += 5;
                }

                if ((espressione.Substring(i, 1) == "+" || espressione.Substring(i, 1) == "-" || espressione.Substring(i, 1) == "*" || espressione.Substring(i, 1) == "/") && !SuperatoPrimoSegno)
                    SuperatoPrimoSegno = true;
                else if (SuperatoPrimoSegno && (espressione.Substring(i, 1) == "+" || espressione.Substring(i, 1) == "-" || espressione.Substring(i, 1) == "*" || espressione.Substring(i, 1) == "/"))
                {
                    NonCeX = true;
                    SuperatoSecondoSegno = true;
                }
            }
            return espressione;
        }
    }
}
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
        int controllo = 0, contatore = 0;
        public struct SuddivisioneFunzione
        {
            public string funzione;
            public int[] PezziFunzione;
            public string[] SingoliPezziFunzione;
            public string[] SingoliPezziFunzioneBackup;
            public int[] coefficenti;
            public int[] esponenti;
            public string[] TondeAperte;
            public string[] TondeChiuse;
            public int[,] InizioFineTonde;
            public string[] TondeAperteChiuse;
            public double[,] coordinata;
        }
        SuddivisioneFunzione FUNZIONE;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (controllo == 1)
            {
                PlotView pv = new PlotView();
                FunctionSeries fs = new FunctionSeries();

                pv.Location = new Point(0, 0);
                pv.Size = new Size(750, 500);
                Controls.Add(pv);


                pv.Model = new PlotModel { Title = "CIAO" };
                for (int i = 0; i < contatore; i++)
                {
                    fs.Points.Add(new DataPoint(FUNZIONE.coordinata[0, i], FUNZIONE.coordinata[1, i]));
                }

                pv.Model.Series.Add(fs);

                pv.Model.Series.Add(new FunctionSeries(Math.Sin, -200, 200, 0.1, "Sin(x)"));
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int indice = 1;
            FUNZIONE.SingoliPezziFunzione = new string[100];
            FUNZIONE.PezziFunzione = new int[100];
            FUNZIONE.esponenti = new int[100];
            FUNZIONE.SingoliPezziFunzioneBackup = new string[100];
            FUNZIONE.coefficenti = new int[100];
            FUNZIONE.esponenti = new int[100];
            FUNZIONE.TondeAperte = new string[100];
            FUNZIONE.TondeChiuse = new string[100];
            FUNZIONE.InizioFineTonde = new int[2, 100];
            FUNZIONE.TondeAperteChiuse = new string[100];
            FUNZIONE.coordinata = new double[2, 100000];

            for (int i = 0; i < FUNZIONE.coefficenti.Length; i++)//pongo tutti i possibili coefficenti pari a 1, quindi come inesistenti
            {
                FUNZIONE.esponenti[i] = 1;
                FUNZIONE.coefficenti[i] = 1;
            }

            FUNZIONE.funzione = " "; //aggiungo lo spazio all'inizio
            FUNZIONE.funzione += textBox2.Text; //prendo in input
            FUNZIONE.funzione = Parentesi(FUNZIONE); //aggiungo il +0 alla fine di qualsiasi tonda

            IndividuazioneOperatori(FUNZIONE, ref indice);

            SuddivisioneSottostringhe(FUNZIONE, indice);

            IndividuazioneCoefficentiEsponenti(FUNZIONE, indice);

            for (int i = 0; i < 100; i++)
            {
                FUNZIONE.SingoliPezziFunzioneBackup[i] = FUNZIONE.SingoliPezziFunzione[i];
            }

            IndividuazioneCoordinate(FUNZIONE, indice, ref contatore);
            controllo = 1;
            Form1_Load(sender, e);

        }

        //parte funzioni nostre
        public static void IndividuazioneOperatori(SuddivisioneFunzione Funzione, ref int Indice)//trovo la posizione degli operatori
        {
            Funzione.PezziFunzione[0] = 0; //assegno al primo pezzo del vettore la posizione 0
            Console.WriteLine("NUOVA POSIZIONE:" + Funzione.funzione.Length);
            for (int i = 0; i < Funzione.funzione.Length; i++) //qui salviamo in un array le posizioni in cui stanno gli operatori
            {
                if (Funzione.funzione.Substring(i, 1) == "+" || Funzione.funzione.Substring(i, 1) == "-" || Funzione.funzione.Substring(i, 1) == "*" || Funzione.funzione.Substring(i, 1) == "/")
                {
                    Funzione.PezziFunzione[Indice] = i; //qui ci sono operatori
                    Indice++; //quantità di operatori nella funzione
                }
            }
            Funzione.PezziFunzione[Indice] = Funzione.funzione.Length;
            //Indice++;
        }

        public static void SuddivisioneSottostringhe(SuddivisioneFunzione Funzione, int Indice)
        {
            //Console.WriteLine("ORA CHE ENTRO QUI È PARI A:"+Funzione.funzione);
            int contatore = 0, aumento = 0, indicatore = 0;
            while (contatore < Indice) //mi salvo in un array di stringe ogni sottostringa in cui ho suddiviso la mia funzione
            {
                if (contatore > 0)
                    aumento = 1;
                Funzione.SingoliPezziFunzione[contatore] = Funzione.funzione.Substring(Funzione.PezziFunzione[contatore] + aumento, (Funzione.PezziFunzione[contatore + 1] - Funzione.PezziFunzione[contatore] - aumento));
                //qui uso la variabile aumento perchè sennò nella sottostringa ci sarebbe acnhe l'operatore
                Funzione.SingoliPezziFunzione[contatore] += " ";

                for (int j = 0; j < Funzione.SingoliPezziFunzione[contatore].Length; j++)
                {
                    if (Funzione.SingoliPezziFunzione[contatore].Substring(j, 1).ToUpper() == "(") //se trova una tonda aperta la salva in un array in cui mi segno con l'indice la poszione in cui si trova la tonda
                    {
                        Funzione.InizioFineTonde[0, indicatore] = contatore;
                        Funzione.TondeAperte[contatore] = "(";
                        Funzione.SingoliPezziFunzione[contatore] = Funzione.SingoliPezziFunzione[contatore].Remove(j, 1);
                    }
                    else if (Funzione.SingoliPezziFunzione[contatore].Substring(j, 1).ToUpper() == ")")
                    {
                        Funzione.InizioFineTonde[1, indicatore] = contatore;
                        Funzione.TondeChiuse[contatore] = ")";
                        Funzione.SingoliPezziFunzione[contatore] = Funzione.SingoliPezziFunzione[contatore].Remove(j, 1);
                        indicatore++;
                    }
                }

                contatore++;
            }
        }

        public static void IndividuazioneCoefficentiEsponenti(SuddivisioneFunzione Funzione, int Indice)//trovo i coefficenti e gli esponenti
        {
            for (int i = 0; i < Indice; i++)
            {
                for (int j = 0; j < Funzione.SingoliPezziFunzione[i].Length - 1; j++) //cerco l'incognita x in ogni pezzo di funzione
                {
                    if (Funzione.SingoliPezziFunzione[i].Substring(j, 2).ToUpper() == "X^")//parte di x^
                    {
                        Funzione.esponenti[i] = int.Parse(Funzione.SingoliPezziFunzione[i].Substring(j + 2, Funzione.SingoliPezziFunzione[i].Length - (j + 2))); //prendiamo l'esponente quindi se la sottostringa è uguale a "x^" sappiamo che dovremo salvarci nell'array l'elezione
                        Funzione.coefficenti[i] = int.Parse(Funzione.SingoliPezziFunzione[i].Substring(0, j));
                        j = Funzione.SingoliPezziFunzione[i].Length; //mettiamo questo così esce da for
                    }
                    else if (Funzione.SingoliPezziFunzione[i].Substring(j, 2).ToUpper() != "X^") //se non è elevato con la "^" allora
                    {
                        Funzione.esponenti[i] = 0; //nel dubbio poniamolo =0
                        if (Funzione.SingoliPezziFunzione[i].Substring(j, 1).ToUpper() == "X") //se invece è presente la x allora cambiamo l'esponete in 
                        {
                            Funzione.esponenti[i] = 1;
                            if (j > 1)
                                Funzione.coefficenti[i] = int.Parse(Funzione.SingoliPezziFunzione[i].Substring(0, j));
                            j = Funzione.SingoliPezziFunzione[i].Length;
                        }
                        else if (Funzione.SingoliPezziFunzione[i].Substring(j, 1) == "^")
                        {
                            //Funzione.coefficenti[i] = int.Parse(Funzione.SingoliPezziFunzione[i].Substring(0, j));
                            Funzione.esponenti[i] = int.Parse(Funzione.SingoliPezziFunzione[i].Substring(j + 1, Funzione.SingoliPezziFunzione[i].Length - (j + 1)));
                            j = Funzione.SingoliPezziFunzione[i].Length;
                        }
                    }
                }
            }
        }

        public static void IndividuazioneCoordinate(SuddivisioneFunzione Funzione, int Indice, ref int contatore)//trovo i coefficenti e gli esponenti
        {
            double x = -250;
            double y;
            contatore = 0;
            string SommaBackup = "";
            while (contatore < 1000)//troviamo le coordinate di alcuni punti appartenenti alla funzione
            {
                SommaBackup = "";
                for (int i = 0; i < 100; i++) //quando lo rifai con la x diversa te la ripristina
                {
                    Funzione.SingoliPezziFunzione[i] = Funzione.SingoliPezziFunzioneBackup[i]; //ripristina i valori originali
                }
                Funzione.coordinata[0, contatore] = x;

                for (int i = 0; i < Indice; i++)
                {
                    for (int j = 0; j < Funzione.SingoliPezziFunzione[i].Length; j++)
                    {

                        if (Funzione.SingoliPezziFunzione[i].Substring(j, 1).ToUpper() == "X")// trova la x
                        {
                            Funzione.SingoliPezziFunzione[i] = Regex.Replace(Funzione.SingoliPezziFunzione[i], "[x,X,^]", string.Empty); //toglie la x e la sostituisce con *(il valore della x) per esponente volte
                            Funzione.SingoliPezziFunzione[i] = Funzione.SingoliPezziFunzione[i].Remove(j, 1);
                            for (int p = 0; p < Funzione.esponenti[i]; p++)
                            {
                                Funzione.SingoliPezziFunzione[i] += "*";
                                Funzione.SingoliPezziFunzione[i] += x;
                            }
                            Funzione.SingoliPezziFunzione[i] = Regex.Replace(Funzione.SingoliPezziFunzione[i], "[,]", ".");//bicos DataTable.Compute vuole il punto e non la virgola
                        }
                        else if (j < Funzione.SingoliPezziFunzione[i].Length - 1 && Funzione.SingoliPezziFunzione[i].Substring(j + 1, 1) == "^" && Funzione.TondeChiuse[i] != ")")
                        {
                            Funzione.SingoliPezziFunzione[i] = Regex.Replace(Funzione.SingoliPezziFunzione[i], "[m,^]", string.Empty);//stesso procedimento di prima per elevazione di una cifra normale
                            Funzione.SingoliPezziFunzione[i] = Funzione.SingoliPezziFunzione[i].Remove(j + 1, 1);
                            string SingoloPezzoFunzione = Funzione.SingoliPezziFunzione[i]; //salviamo in una variabile di backup il numero da continuare a elevare
                            for (int p = 0; p < Funzione.esponenti[i] - 1; p++)
                            {
                                Funzione.SingoliPezziFunzione[i] += "*";
                                Funzione.SingoliPezziFunzione[i] += SingoloPezzoFunzione;
                            }

                        }
                        else if (j < Funzione.SingoliPezziFunzione[i].Length - 1 && Funzione.SingoliPezziFunzione[i].Substring(j + 1, 1) == "^" && Funzione.TondeChiuse[i] == ")")
                        {
                            Funzione.SingoliPezziFunzione[i] = Regex.Replace(Funzione.SingoliPezziFunzione[i], "[m,^]", string.Empty);
                            Funzione.SingoliPezziFunzione[i] = Funzione.SingoliPezziFunzione[i].Remove(j + 1, 1);

                        }
                    }
                }

                ElevazioneTonde(Funzione, Indice);

                for (int i = 0; i < Indice; i++)
                {
                    SommaBackup += Funzione.funzione.Substring(Funzione.PezziFunzione[i], 1) + Funzione.TondeAperte[i] + Funzione.SingoliPezziFunzione[i] + Funzione.TondeChiuse[i] + Funzione.TondeAperteChiuse[i];
                }
                //Console.WriteLine("SOMMA:" + SommaBackup);
                y = Risoluzione(SommaBackup);
                Funzione.coordinata[1, contatore] = y;

                contatore++;
                //MessageBox.Show(Convert.ToString(contatore));
                x += 0.5;
            }
        }

        public static double Risoluzione(string Funzione)//funzione per risolvere una espressione
        {
            DataTable dt = new DataTable();
            var soluzione1 = dt.Compute(Funzione, "");
            double soluzione2 = Convert.ToDouble(soluzione1);
            return soluzione2;
        }

        public static void ElevazioneTonde(SuddivisioneFunzione Funzione, int Indice)
        {
            int f = 0;
            while (f < Indice - 3)
            {
                string parentesi = "("; //inizializo la stringa con "("
                int indiceImportante = 0, controllo = 0, AggiungiSegno = 0;
                while (Funzione.TondeChiuse[f] != ")" && f < Indice - 3) //se non trovo ")" e finchè resto minore delle posizioni occupate
                {
                    if (Funzione.TondeAperte[f] == "(") //prima controllo se trovo la tonda perchè se non la trovo non ha senso inserire nella stringa di salvataggio
                    {
                        controllo = 1;
                    }

                    if (controllo == 1)
                    {
                        if (AggiungiSegno < 1)
                            parentesi += Funzione.SingoliPezziFunzione[f];
                        else
                        {
                            parentesi += Funzione.funzione.Substring(Funzione.PezziFunzione[f], 1); //È SBAGLIATO QUESTO COMANDO E MI HA FATTO DANNARE
                            parentesi += Funzione.SingoliPezziFunzione[f];

                        }
                    }
                    AggiungiSegno++;
                    f++;
                }
                parentesi += Funzione.funzione.Substring(Funzione.PezziFunzione[f], 1); //devo aggiungere ancora il segno
                parentesi += Funzione.SingoliPezziFunzione[f];
                indiceImportante = f;
                f++;
                f++;//
                f++;//
                parentesi += ")";
                Funzione.TondeAperteChiuse[indiceImportante] = "";
                Console.WriteLine("PARENTESI:" + parentesi);
                for (int u = 0; u < Funzione.esponenti[indiceImportante] - 1; u++)
                {
                    Funzione.TondeAperteChiuse[indiceImportante] += "*";
                    Funzione.TondeAperteChiuse[indiceImportante] += parentesi;
                }
            }
        }

        private void plotView2_Click(object sender, EventArgs e)
        {

        }

        public static string Parentesi(SuddivisioneFunzione Funzione)
        {
            string RisFin = Funzione.funzione;
            Console.WriteLine("VECCHIA POSIZIONE:" + Funzione.funzione.Length);
            for (int i = 0; i < Funzione.funzione.Length; i++)
            {
                if (Funzione.funzione.Substring(i, 1) == ")")
                {
                    RisFin = Funzione.funzione.Insert(i, "+0");
                    i += 2;
                    Funzione.funzione = RisFin;
                }
            }
            return RisFin;
        }
    }
}
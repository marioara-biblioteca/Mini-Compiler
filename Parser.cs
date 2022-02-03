using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFT
{
    class Parser
    {
        private AtomLexical[] atomi;
        private int index;
        private AtomLexical atomAux; // folosit pentru a asigura consistenta tipurilor variabilelor in evaluarea expresiilor
        private List<string> erori=new List<string>();
        private List<Variabila> variabile = new List<Variabila>();
        private bool afiseaza = true;
        public IEnumerable<string> Erori => erori;
        public Parser(string text)
        {
            var atomiAuxiliari = new List<AtomLexical>();
            var lexer = new Lexer(text);

            AtomLexical atom;
            try
            {
                do //construim vectorul de atomi lexicali cu care lucreaza parserul
                {
                    atom = lexer.UrmatorulSimbol();
                    if (atom.Tip != TipAtomLexical.Spatiu && atom.Tip != TipAtomLexical.Invalid)
                        atomiAuxiliari.Add(atom);

                } while (atom.Tip != TipAtomLexical.TerminatorSir);
                atomi = atomiAuxiliari.ToArray();
                if (lexer.Erori != null)
                    erori.AddRange(lexer.Erori);
            }
            catch(Exception e)
            {
                erori.Add(e.ToString());
                throw e;   
            }
        }
        private AtomLexical Varf(int avans = 0)
        {
            if (this.index + avans > +atomi.Length)
                return atomi[atomi.Length - 1];
            return atomi[this.index + avans];
        }
        private AtomLexical AtomLexicalCurent => Varf(0);
        private AtomLexical CurentSiInainteaza()
        {
            var curent = AtomLexicalCurent;
            this.index++;
            return curent;
        }
        private ExpresieSintactica ParseazaTermeni()
        {
            var stanga = ParseazaFactori();
            while(AtomLexicalCurent.Tip==TipAtomLexical.Plus || AtomLexicalCurent.Tip == TipAtomLexical.Minus)
            {
                var oper = CurentSiInainteaza();
                var dreapta = ParseazaFactori();
                stanga = new ExpresieSintacticaBinara(stanga, dreapta, oper);
            }
            return stanga;
        }
        private ExpresieSintactica ParseazaFactori()
        {
            var stanga = ParseazaPrimaExpresie();
            while(AtomLexicalCurent.Tip==TipAtomLexical.Inmultit || AtomLexicalCurent.Tip == TipAtomLexical.Impartit)
            {
                var oper= CurentSiInainteaza();
                var dreapta = ParseazaPrimaExpresie();
                stanga = new ExpresieSintacticaBinara(stanga, dreapta, oper);
            }
            return stanga;
        }
        private ExpresieSintactica ParseazaPrimaExpresie() // aici o sa evaluam ce gasim dupa un '='
        {
            if (AtomLexicalCurent.Tip == TipAtomLexical.ParantezaDeschisa)
            {
                var stanga = CurentSiInainteaza();
                var expresie = ParseazaTermeni();
                AtomLexical dreapta;
                if (AtomLexicalCurent.Tip == TipAtomLexical.ParantezaInchisa)
                {
                    dreapta = CurentSiInainteaza();
                }
                else
                {
                    erori.Add($"Parser: Eroare atom lexical invalid {AtomLexicalCurent.Tip} se astepta {TipAtomLexical.ParantezaInchisa}");
                    throw new Exception("Parser: Se astepta paranteza inchisa");
                    //dreapta = new AtomLexical(TipAtomLexical.ParantezaInchisa, AtomLexicalCurent.Index, null, null);
                }
               
                return new ExpresieSintacticaCuParanteze(stanga, expresie, dreapta);
            }
            //daca nu, putem avea un numar int, decimal, un string  sau o variabila
            var valoare = CurentSiInainteaza();
            if (valoare.Tip == TipAtomLexical.Variabila)//inseamna ca ar trb sa avem o asociere a ei;
            {
                var variabila = GasesteVariabila(valoare.Text);
                if (variabila == null)
                    throw new Exception($"Nu exista variabila declarata cu acest nume {AtomLexicalCurent.Text}");
                //altfel cream noi un atom lexical de tipul int, decimal sau string pe baza variabilei               
                switch (variabila.Tip)
                {
                    case "int":
                        atomAux = new AtomLexical(TipAtomLexical.Numar, AtomLexicalCurent.Index, variabila.Nume, variabila.Valoare);
                        break;
                    case "decimal":
                        atomAux = new AtomLexical(TipAtomLexical.Decimal, AtomLexicalCurent.Index, variabila.Nume, variabila.Valoare);
                        break;
                    case "string":
                        atomAux = new AtomLexical(TipAtomLexical.String, AtomLexicalCurent.Index, variabila.Nume, variabila.Valoare);
                        break;
                    default: //dar nu are cum sa ajunga aici
                        atomAux = new AtomLexical(TipAtomLexical.Invalid, AtomLexicalCurent.Index, null, null);
                        break;
                }
                //altfel, inseamna ca avem direct valoarea
                return new ExpresieSintacticaNumerica(atomAux);
            }
            return new ExpresieSintacticaNumerica(valoare);
        }
        private void AsigneazaVariabilaNula(AtomLexical atom,AtomLexical variabila)
        {
            //facem o variabila careia ii asignam 0 (cum ar veni, globala)
            //sau am putea lucra cu valori maxime
            if (atom.Text == "int" || atom.Text == "decimal")
            {
                AdaugaVariabila(atom.Text, variabila.Text, 0);
            }
            else if (atom.Text == "double")
            {
                AdaugaVariabila(atom.Text, variabila.Text, 0.0);
            }
            else if (atom.Text == "string")
            {
                AdaugaVariabila(atom.Text, variabila.Text, "");
            }
        }
        private Variabila GasesteVariabila(string nume)
        {
            return variabile.Find(v => v.Nume == nume);
        }
        private bool AdaugaVariabila(string tip,string nume, object valoare)//adauga o noua variabila sau updateaza o valoare a unei vriabile deja declarate
        {
            foreach(var variabila in variabile)
            {
                if (variabila.Nume == nume)
                {
                    if (variabila.Tip != tip)
                        return false;
                    else
                    {
                        variabila.Valoare = valoare;
                        return true;
                    }
                }
            }
            variabile.Add(new Variabila(tip, nume, valoare));
            return true;
        }
        public void Parseaza()
        {
            try
            {
                while (this.index < atomi.Length)
                {
                    var atom = CurentSiInainteaza();//in atom e atomul curent si am crescut indexul
                    
                    if (atom.Tip == TipAtomLexical.CuvantCheie) //trebuie sa avem neaparat o definitie de variabila dupa
                    {
                        //daca nu e prima declaratie, trebuie sa verificam ; anterior
                        if (this.index -1!= 0)
                        {
                            if(atomi[this.index-2].Tip!=TipAtomLexical.DelimitatorPunctSiVirgula && atomi[this.index-2].Tip!=TipAtomLexical.Comentariu)                               
                            {
                                erori.Add($"Parser: Declararea variabilei nu este facuta corespunzator! {atomi[this.index-4].Text}");
                                throw new Exception($"Parser: Declararea variabilei nu este facuta corespunzator! {atomi[this.index - 4].Text}");
                            }
                        }
                        if (atom.Text == "int" || atom.Text == "decimal" || atom.Text == "string"|| atom.Text=="double")
                        {
                            var variabila = CurentSiInainteaza();
                            var delimitator = CurentSiInainteaza();
                            if (delimitator.Tip == TipAtomLexical.Egal)
                            {
                                //acum in AtomCurent avem ce trebuie parsat dupa egal cu care va incepe ParseazaPrimaExpresie
                                //sau semnul
                                if (AtomLexicalCurent.Tip == TipAtomLexical.Minus)
                                {
                                    //daca gasim un semn minus imediat dupa egal => valoarea de dupa acel minus trb negata
                                    //vom retina valoarea ce trebuie negata, vom evalua expresia, si apoi vom scadea 2*valoare
                                    CurentSiInainteaza();//sarim peste minus
                                    var valoare = AtomLexicalCurent;
                                    var arbore = ParseazaDupaEgal();
                                    var evaluator = new Evaluator(arbore.radacina);
                                    if (atom.Text == "int")
                                    {
                                        AdaugaVariabila(atom.Text, variabila.Text, (int)evaluator.Evaluaeaza(TipAtomLexical.Numar) - 2 * (int)valoare.Valoare);
                                    }
                                    else if (atom.Text == "decimal")
                                    {
                                        AdaugaVariabila(atom.Text, variabila.Text, (decimal)evaluator.Evaluaeaza(TipAtomLexical.Decimal) - 2 * (decimal)valoare.Valoare);
                                    }
                                    else if (atom.Text == "double")
                                    {
                                        AdaugaVariabila(atom.Text, variabila.Text, (double)evaluator.Evaluaeaza(TipAtomLexical.Decimal) - 2 * (double)valoare.Valoare);
                                    }
                                    else if (atom.Text == "string")
                                    {
                                        erori.Add($"Parser: Nu putem avea un string cu valoare negativa! {valoare.Text}");
                                        throw new Exception($"Parser: Nu putem avea un string cu valoare negativa! {valoare.Text}");
                                    }
                                    arbore.AfiseazaArbore(arbore.radacina);
                                }
                                else
                                {
                                    var arbore = ParseazaDupaEgal();
                                    var evaluator = new Evaluator(arbore.radacina);
                                    if (atom.Text == "int")
                                    {
                                        AdaugaVariabila(atom.Text, variabila.Text, evaluator.Evaluaeaza(TipAtomLexical.Numar));
                                    }
                                    else if (atom.Text == "string")
                                    {
                                        AdaugaVariabila(atom.Text, variabila.Text, evaluator.Evaluaeaza(TipAtomLexical.String));
                                    }
                                    else if (atom.Text == "decimal")
                                    {
                                        AdaugaVariabila(atom.Text, variabila.Text, evaluator.Evaluaeaza(TipAtomLexical.Decimal));
                                    }
                                    else if (atom.Text == "double")
                                    {
                                        AdaugaVariabila(atom.Text, variabila.Text, evaluator.Evaluaeaza(TipAtomLexical.Double));
                                    }
                                    arbore.AfiseazaArbore(arbore.radacina);
                                }
                            }
                            else if (delimitator.Tip == TipAtomLexical.DelimitatorPunctSiVirgula || delimitator.Tip == TipAtomLexical.DelimitatorVirgula)
                            {
                                AsigneazaVariabilaNula(atom, variabila);
                            }
                            else
                            {
                                erori.Add("Parser: declarare incorecta de variabila ");
                                throw new Exception("Parser: Declarare incorecta de variabila: dupa numele uneui variabile trebuie sa urmeze un delimitator");
                            }
                        }
                        else
                        {
                            erori.Add("Parser: cuvant cheie necunoscut, nu se va sti tipul variabilei declarate");
                            throw new Exception("Parser: cuvant cheie necunoscut, nu se va sti tipul variabilei declarate");
                        }
                    }
                    else
                    {
                        //daca nu e o declaratie de variabila, e o redefinitie 
                        if (atom.Tip == TipAtomLexical.Variabila)
                        {
                            //trebuie sa existe declarata;
                            var variabila = GasesteVariabila(atom.Text);
                            if (variabila == null)
                            {
                                erori.Add("Parser: Variabila nedeclarata");
                                throw new Exception("Parser: Variabila nedeclarata!!");
                            }

                            //trebuie urmata de un egal, i se atribuie o valoare;
                            var delimitator = CurentSiInainteaza();
                            if (delimitator.Tip != TipAtomLexical.Egal)
                            {
                                erori.Add("Parser: Format gresit al instructiunii");
                                throw new Exception("Parser: Format gresit al instructiunii");
                            }
                            if (AtomLexicalCurent.Tip == TipAtomLexical.Minus)
                            {
                                CurentSiInainteaza();
                                //trebuie sa retinem valoarea variabilei negate
                                var valoare = GasesteVariabila(AtomLexicalCurent.Text);
                                var arbore = ParseazaDupaEgal();
                                var evaluator = new Evaluator(arbore.radacina);
                                if (variabila.Tip == "int" && atomAux.Tip == TipAtomLexical.Numar)
                                {
                                    AdaugaVariabila(variabila.Tip, variabila.Nume, (int)evaluator.Evaluaeaza(TipAtomLexical.Numar)-2*(int)valoare.Valoare);
                                }
                                else if (atom.Text == "decimal" && atomAux.Tip == TipAtomLexical.Decimal)
                                {
                                    AdaugaVariabila(variabila.Tip, variabila.Nume, (decimal)evaluator.Evaluaeaza(TipAtomLexical.Decimal)-2*(decimal)valoare.Valoare);
                                }
                                else if (atom.Text == "double" && atomAux.Tip == TipAtomLexical.Double)
                                {
                                    AdaugaVariabila(variabila.Tip, variabila.Nume, (double)evaluator.Evaluaeaza(TipAtomLexical.Decimal) - 2 * (double)valoare.Valoare);
                                }
                                else if (atom.Text == "string" && atomAux.Tip == TipAtomLexical.String)
                                {
                                    erori.Add("Parser: nu putem avea o valoare negativa pt un string");
                                    throw new Exception("Parser: nu putem avea o valoare negativa pt un string");
                                    //AdaugaVariabila(variabila.Tip, variabila.Nume, evaluator.Evaluaeaza(TipAtomLexical.String));
                                }
                                else
                                {
                                    erori.Add($"Parser: Nu se poate face maparea de tipuri ale variabilelor: {variabila.Tip} <--> {atomAux.Tip}");
                                    throw new Exception($"Parser: Nu se poate face maparea de tipuri ale variabilelor: {variabila.Tip} <--> {atomAux.Tip}");
                                }
                                arbore.AfiseazaArbore(arbore.radacina);
                            }
                            else
                            {
                                var arbore = ParseazaDupaEgal();
                                var evaluator = new Evaluator(arbore.radacina);
                                
                                if (variabila.Tip == "int" && atomAux.Tip == TipAtomLexical.Numar)
                                {
                                    AdaugaVariabila(variabila.Tip, variabila.Nume, evaluator.Evaluaeaza(TipAtomLexical.Numar));
                                }
                                else if (atom.Text == "string" && atomAux.Tip == TipAtomLexical.String)
                                {
                                    AdaugaVariabila(variabila.Tip, variabila.Nume, evaluator.Evaluaeaza(TipAtomLexical.String));
                                }
                                else if (atom.Text == "decimal" && atomAux.Tip == TipAtomLexical.Decimal)
                                {
                                    AdaugaVariabila(variabila.Tip, variabila.Nume, evaluator.Evaluaeaza(TipAtomLexical.Decimal));
                                }
                                else if (atom.Text == "double" && atomAux.Tip == TipAtomLexical.Double)
                                {
                                    AdaugaVariabila(variabila.Tip, variabila.Nume, evaluator.Evaluaeaza(TipAtomLexical.Double));
                                }
                                else
                                {
                                    erori.Add($"Parser: Nu se poate face maparea de tipuri ale variabilelor: {variabila.Tip} <--> {atomAux.Tip}");
                                    throw new Exception($"Parser: Nu se poate face maparea de tipuri ale variabilelor: {variabila.Tip} <--> {atomAux.Tip}");
                                }
                                arbore.AfiseazaArbore(arbore.radacina);
                            }
                        }
                        else if (atom.Tip == TipAtomLexical.Comentariu)
                        {
                            //do nothing
                        }
                        else if (atom.Tip==TipAtomLexical.TerminatorSir)
                        {                         
                            if (atomi[this.index - 2].Tip != TipAtomLexical.DelimitatorPunctSiVirgula &&atomi[this.index-2].Tip!=TipAtomLexical.Comentariu)
                            {
                                erori.Add("Parser: Toate instructiunile trebuie sa se termina cu ';'");
                                throw new Exception("Parser: Toate instructiunile trebuie sa se termina cu ';'");
                            }
                            continue;
                        }
                        else
                        {
                           
                            erori.Add($"Parser: Se astepta o redefinitie, adica un nume de variabila {atom.Text}");
                            throw new Exception($"Parser: Se astepta o redefinitie, adica un nume de variabila {atom.Text}");
                        }
                    }
                }
            }
            catch(Exception e)
            {
                erori.Add(e.ToString());
                throw e;
            }
        }
        private Arbore ParseazaDupaEgal()
        {
            ExpresieSintactica expresie = ParseazaTermeni();
            AtomLexical terminator = (AtomLexicalCurent.Tip==TipAtomLexical.DelimitatorPunctSiVirgula || 
                                    AtomLexicalCurent.Tip==TipAtomLexical.DelimitatorPunctSiVirgula) ?
                                    CurentSiInainteaza() : 
                                    new AtomLexical(AtomLexicalCurent.Tip, AtomLexicalCurent.Index, null, null);
            return new Arbore(expresie, terminator, erori);
        }
        public void AfiseazaVariabile()
        {
            foreach(var variabila in variabile)
            {
                Console.WriteLine(String.Format("Variabila {0} de tip {1} are valoarea {2}",variabila.Nume,variabila.Tip,variabila.Valoare.ToString()));
            }
        }
        public void AfiseazaErori()
        {
            foreach(var eroare in erori)
            {
                Console.WriteLine(eroare);
            }
        }
    }
}
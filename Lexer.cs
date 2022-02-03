using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LFT
{
    class Lexer
    {
        private readonly string text;
        private int index=0;
        private List<string> erori = new List<string>();

        public IEnumerable<string> Erori => erori;
        public Lexer(string text)
        {
            this.text = text;
        }
        private char SimbolCurent
        {
            get
            {
                if (this.index >= this.text.Length)
                {
                    return '\0';
                }
                return this.text[this.index];
            }
        }
        private void Avanseaza()
        {
            this.index++;
        }
        private bool ECuvantCheie(string input)
        {
            string pattern = @"\b(int|float|decimal|string|double)\b";
            Match m = Regex.Match(input, pattern);
            return m.Success;
        }
        private bool EVariabila(string input)
        {
            string pattern = @"^[a-zA-Z_$][a-zA-Z_$0-9]*$";
            Match m = Regex.Match(input, pattern);
            return m.Success;
        }
       
        public AtomLexical UrmatorulSimbol() 
        {
            if (SimbolCurent == '\0')
            {
                return new AtomLexical(TipAtomLexical.TerminatorSir, this.index, "\0", null);
            }
            if (SimbolCurent == '/')//comentariu sau impartire
            {
                if (text[this.index] == '/' && text[this.index] == '/')
                {
                    var start = this.index;
                    while (SimbolCurent != '\n')//comentariu pe o linie
                        Avanseaza();
                    var lungime = this.index - start;
                    var input = this.text.Substring(start, lungime);
                    Avanseaza();
                    return new AtomLexical(TipAtomLexical.Comentariu, start, input, input);
                }
            }
            if (char.IsDigit(SimbolCurent))
            {
                var start = this.index;
                int dot = 0;
                while (char.IsDigit(SimbolCurent) || SimbolCurent == '.')
                {
                    if (SimbolCurent == '.')
                    {
                        if (dot == 0)
                        { 
                            dot++;
                        }
                        else
                        {
                            erori.Add($"Lexer: Acesta nu este un numar decimal valid!");
                            throw new Exception("Lexer: numar decimal inavlid");
                        }
                    }
                    Avanseaza();
                }

                var lungime = this.index - start;
                var input = this.text.Substring(start, lungime);

                if (dot == 1)
                {
                    if (decimal.TryParse(input, out var valoare) == false)
                    {
                        erori.Add($"Lexer: Exceptie: Nu s-a putut realiza conversia la decimal '{text}'");
                        throw new Exception("Lexer: nu s-a putut realiza conversia - numar decimal invalid");
                    }
                    return new AtomLexical(TipAtomLexical.Decimal, start, input, valoare);
                }
                else
                {
                    if (int.TryParse(input, out var valoare) == false)
                    {
                        erori.Add($"Lexer: Exceptie: Nu s-a putut realiza conversia la int '{text}'");
                        throw new Exception("Lexer: nu s-a putut realiza conversia - numar intreg invalid");
                    }
                    return new AtomLexical(TipAtomLexical.Numar, start, input, valoare);
                }
            }

            if (char.IsWhiteSpace(SimbolCurent))
            {
                var start = this.index;

                while (char.IsWhiteSpace(SimbolCurent))
                    Avanseaza();

                var lungime = this.index - start;
                var input = this.text.Substring(start, lungime);
                return new AtomLexical(TipAtomLexical.Spatiu, start, input, null);
            }

            if (char.IsLetter(SimbolCurent)|| SimbolCurent=='_') //keyword sau nume de variabila
            {
                var start = this.index;
                while (char.IsLetterOrDigit(SimbolCurent)|| SimbolCurent=='_')
                    Avanseaza();
                var lungime = this.index - start;
                var input = this.text.Substring(start, lungime);
                if (ECuvantCheie(input))
                {
                    return new AtomLexical(TipAtomLexical.CuvantCheie, start, input, input);
                }
                if (EVariabila(input))
                {
                    return new AtomLexical(TipAtomLexical.Variabila, start, input, input);
                }
                erori.Add($"Lexer: Exceptie: Atom Lexical de tip string invalid '{input}'");
                throw new Exception("Lexer: Atom lexical de tip string nerecunoscut");
                //return new AtomLexical(TipAtomLexical.Invalid, -1, null, null);
            }

            if (SimbolCurent == '"') //string constant
            {
                var start = this.index++;
                while (SimbolCurent != '"' && SimbolCurent!='\0') 
                    Avanseaza();
                if (SimbolCurent == '\0')//am ajuns la finalul inputului si nu s-au inchis ghulimelele
                {
                    erori.Add("Lexer: String-ul constant nu a fost inchis");
                    throw new Exception("Lexer: Ghilimelele deschise nu a fost niciodata inchise");
                }
                Avanseaza();
                var lungime = this.index-1 - (start+1);
                var input = this.text.Substring(start+1, lungime);
                return new AtomLexical(TipAtomLexical.String, start+1, input, input);
            }

            switch (SimbolCurent)
            {
                case '+':
                    return new AtomLexical(TipAtomLexical.Plus, this.index++, "+", null);
                case '-':
                    return new AtomLexical(TipAtomLexical.Minus, this.index++, "-", null);
                case '*':
                    return new AtomLexical(TipAtomLexical.Inmultit, this.index++, "*", null);
                case '/':
                    return new AtomLexical(TipAtomLexical.Impartit, this.index++, "/", null);
                case '(':
                    return new AtomLexical(TipAtomLexical.ParantezaDeschisa, this.index++, "(", null);
                case ')':
                    return new AtomLexical(TipAtomLexical.ParantezaInchisa, this.index++, ")", null);
                case ';':
                    return new AtomLexical(TipAtomLexical.DelimitatorPunctSiVirgula, this.index++, ";", null);
                case ',':
                    return new AtomLexical(TipAtomLexical.DelimitatorVirgula, this.index++, ",", null);
                case '=':
                    return new AtomLexical(TipAtomLexical.Egal, this.index++, "=", null);
                default:
                    erori.Add($"Lexer: Exceptie: Atom Lexical invalid '{SimbolCurent}'");
                    throw new Exception($"Lexer: Atom lexical invalid in contextl dat {SimbolCurent}");
                    //return new AtomLexical(TipAtomLexical.Invalid, -1, null, null);
            }
        }
    }
}

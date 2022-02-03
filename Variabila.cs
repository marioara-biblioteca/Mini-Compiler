using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFT
{
    class Variabila
    {
        public Variabila(string tip,string nume, object valoare)
        {
            Tip = tip;
            Nume = nume;
            Valoare = valoare;
        }

        public string Tip { get; }
        public string Nume { get; }
        public object Valoare { get; set; }
    }
}

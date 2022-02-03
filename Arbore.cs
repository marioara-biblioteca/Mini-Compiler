using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFT
{
    class Arbore
    {
        public ExpresieSintactica radacina { get; }
        public AtomLexical sfarsit { get; }
        public List<string> erori = new List<string>();
        public Arbore(ExpresieSintactica radacina, AtomLexical sfrasit, List<string> eroriParser)
        {
            this.radacina = radacina;
            this.sfarsit = sfarsit;
            this.erori = eroriParser;
        }
        public void AfiseazaArbore(Nod nod, string indentare = "", bool ultimulNod = true)
        {
            var aux = ultimulNod ? "└──" : "├──";
            Console.Write(indentare);
            Console.Write(aux);
            Console.Write(nod.Tip);

            if (nod is AtomLexical t && t.Valoare != null)
            {
                Console.Write(" ");
                Console.Write(t.Valoare);
            }

            Console.WriteLine();

            indentare += ultimulNod ? "    " : "|   ";

            var lastChild = nod.ObtineCopiiNod().LastOrDefault();

            foreach (var copil in nod.ObtineCopiiNod())
            {
                AfiseazaArbore(copil, indentare, copil == lastChild);
            }
        }
    }
}

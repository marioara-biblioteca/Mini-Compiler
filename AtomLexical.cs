using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFT
{
    enum TipAtomLexical
    {
        Numar,
        Decimal,
        Float,
        Double,
        Spatiu,
        Plus,
        Minus,
        Inmultit,
        Impartit,
        ParantezaDeschisa,
        ParantezaInchisa,
        Egal,
        TerminatorSir,
        ExpresieNumerica,
        ExpresieBinara,
        String,
        CuvantCheie,
        Variabila,
        ParantezaExpresieSintactica,
        DelimitatorPunctSiVirgula,
        DelimitatorVirgula,
        Comentariu,
        Invalid
    }
    class AtomLexical:Nod
    {
        public AtomLexical(TipAtomLexical tip, int index, string text, object valoare)
        {
            Tip = tip;
            Index = index;
            Text = text;
            Valoare = valoare;
        }

        public override TipAtomLexical Tip { get; }
        public int Index { get; }
        public string Text { get; set; }
        public object Valoare { get; set; }

        public override IEnumerable<Nod> ObtineCopiiNod()
        {
            return Enumerable.Empty<Nod>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFT
{
    abstract class ExpresieSintactica:Nod
    {
    }
    sealed class ExpresieSintacticaBinara : ExpresieSintactica
    {
        public ExpresieSintacticaBinara(ExpresieSintactica stanga, ExpresieSintactica dreapta, AtomLexical operatorAtom)
        {
            Stanga = stanga;
            Dreapta = dreapta;
            OperatorAtom = operatorAtom;
        }
        public override TipAtomLexical Tip => TipAtomLexical.ExpresieBinara;

        public ExpresieSintactica Stanga { get; }
        public ExpresieSintactica Dreapta { get; }
        public AtomLexical OperatorAtom { get; }

        public override IEnumerable<Nod> ObtineCopiiNod()
        {
            yield return Stanga;
            yield return OperatorAtom;
            yield return Dreapta;
        }
    }
    sealed class ExpresieSintacticaNumerica : ExpresieSintactica
    {
        public AtomLexical NumarAtomLexical { get; }
        public override TipAtomLexical Tip => TipAtomLexical.ExpresieNumerica;
        public ExpresieSintacticaNumerica(AtomLexical numarAtomLexical)
        {
            NumarAtomLexical = numarAtomLexical;
        }

        public override IEnumerable<Nod> ObtineCopiiNod()
        {
            yield return NumarAtomLexical;
        }
    }
    sealed class ExpresieSintacticaCuParanteze : ExpresieSintactica
    {

        public AtomLexical ParantezaDeschisa { get; }
        public ExpresieSintactica Expresie { get; }
        public AtomLexical ParantezaInchisa { get; }
        public ExpresieSintacticaCuParanteze(AtomLexical parantezaDeschisa, ExpresieSintactica expresie, AtomLexical parantezaInchisa)
        {
            ParantezaDeschisa = parantezaDeschisa;
            Expresie = expresie;
            ParantezaInchisa = parantezaInchisa;
        }
        public override TipAtomLexical Tip => TipAtomLexical.ParantezaExpresieSintactica;

        public override IEnumerable<Nod> ObtineCopiiNod()
        {
            yield return ParantezaDeschisa;
            yield return Expresie;
            yield return ParantezaInchisa;
        }
    }


}

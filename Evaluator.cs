using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFT
{
    class Evaluator
    {
        private readonly ExpresieSintactica expresie;
        public Evaluator(ExpresieSintactica expresie)
        {
            this.expresie = expresie;
        }
        int EvaluareExpresieInt(ExpresieSintactica expr)
        {
            if (expr is ExpresieSintacticaNumerica valoare)
            {
                if (valoare.NumarAtomLexical.Tip == TipAtomLexical.Numar)
                    return (int)valoare.NumarAtomLexical.Valoare;
                else
                    throw new Exception("Atom lexical invalid in evaluarea exresiei int");
            }
            else if (expr is ExpresieSintacticaCuParanteze exprParanteze)
            {
                return EvaluareExpresieInt(exprParanteze.Expresie);
            }
            else if (expr is ExpresieSintacticaBinara exprBinara)
            {
                int stanga = EvaluareExpresieInt(exprBinara.Stanga);
                int dreapta = EvaluareExpresieInt(exprBinara.Dreapta);
                switch (exprBinara.OperatorAtom.Tip)
                {
                    case TipAtomLexical.Plus:
                        return (stanga + dreapta);
                    case TipAtomLexical.Minus:
                        return (stanga - dreapta);
                    case TipAtomLexical.Inmultit:
                        return (stanga * dreapta);
                    case TipAtomLexical.Impartit:
                        if (dreapta == 0)
                            throw new Exception("Evaluare expresie int: impartire la 0");
                        return (stanga / dreapta);
                    default:
                        return int.MinValue;
                }
            }
            else
                throw new Exception("Expresie de tip int invalid");           
        }
        decimal EvaluareExpresieDecimala(ExpresieSintactica expr)
        {
            if (expr is ExpresieSintacticaNumerica valoare)
            {
                if (valoare.NumarAtomLexical.Tip == TipAtomLexical.Decimal)
                    return (decimal)valoare.NumarAtomLexical.Valoare;
                else
                    throw new Exception("Atom lexical invalid in evaluarea exresiei decimal");
            }
            else if (expr is ExpresieSintacticaCuParanteze exprParanteze)
            {
                return EvaluareExpresieDecimala(exprParanteze.Expresie);
            }
            else if (expr is ExpresieSintacticaBinara exprBinara)
            {
                decimal stanga = EvaluareExpresieDecimala(exprBinara.Stanga);
                decimal dreapta = EvaluareExpresieDecimala(exprBinara.Dreapta);
                switch (exprBinara.OperatorAtom.Tip)
                {
                    case TipAtomLexical.Plus:
                        return (stanga + dreapta);
                    case TipAtomLexical.Minus:
                        return (stanga - dreapta);
                    case TipAtomLexical.Inmultit:
                        return (stanga * dreapta);
                    case TipAtomLexical.Impartit:
                        if (dreapta == 0)
                            throw new Exception("Evaluare expresie decimala: impartire la 0.");
                        return (stanga / dreapta);
                    default:
                        return decimal.MinValue;
                }
            }
            else
                throw new Exception("Expresie de tip decimal invalid");
        }
        double EvaluareExpresieDouble(ExpresieSintactica expr)
        {
            if (expr is ExpresieSintacticaNumerica valoare)
            {
                if (valoare.NumarAtomLexical.Tip == TipAtomLexical.Double)
                    return (double)valoare.NumarAtomLexical.Valoare;
                else
                    throw new Exception("Atom lexical invalid in evaluarea exresiei double");
            }
            else if (expr is ExpresieSintacticaCuParanteze exprParanteze)
            {
                return EvaluareExpresieDouble(exprParanteze.Expresie);
            }
            else if (expr is ExpresieSintacticaBinara exprBinara)
            {
                double stanga = EvaluareExpresieDouble(exprBinara.Stanga);
                double dreapta = EvaluareExpresieDouble(exprBinara.Dreapta);
                switch (exprBinara.OperatorAtom.Tip)
                {
                    case TipAtomLexical.Plus:
                        return (stanga + dreapta);
                    case TipAtomLexical.Minus:
                        return (stanga - dreapta);
                    case TipAtomLexical.Inmultit:
                        return (stanga * dreapta);
                    case TipAtomLexical.Impartit:
                        if (dreapta == 0)
                            throw new Exception("Evaluare expresie double: impartire la 0.");
                        return (stanga / dreapta);
                    default:
                        return double.MinValue;
                }
            }
            else
                throw new Exception("Expresie de tip double invalid");          
        }
        string EvaluareExprsieString(ExpresieSintactica expr)
        {
            if (expr is ExpresieSintacticaNumerica valoare)
            {
                if (valoare.NumarAtomLexical.Tip == TipAtomLexical.String)
                {
                    return (string)valoare.NumarAtomLexical.Valoare;
                }
                else
                    throw new Exception("Atom lexical invalid in evaluarea expresiei string");
            }
            else if (expr is ExpresieSintacticaCuParanteze exprParanteze)
            {
                return EvaluareExprsieString(exprParanteze.Expresie);
            }
            else if (expr is ExpresieSintacticaBinara exprBin)
            {
                string stanga = EvaluareExprsieString(exprBin.Stanga);
                string dreapta = EvaluareExprsieString(exprBin.Dreapta);
                switch (exprBin.OperatorAtom.Tip)
                {
                    case TipAtomLexical.Plus:
                        return (stanga + dreapta);
                    default:
                        throw new Exception("Operatie invalida pe stringuri");
                }
            }
            else
                throw new Exception("Expresie de tip string invalida");
        }
        public object Evaluaeaza(TipAtomLexical tip)
        {
            try
            {
                switch (tip)
                {
                    case TipAtomLexical.Decimal:
                        return EvaluareExpresieDecimala(this.expresie);
                    case TipAtomLexical.String:
                        return EvaluareExprsieString(this.expresie);
                    case TipAtomLexical.Numar:
                        return EvaluareExpresieInt(this.expresie);
                    default:
                        throw new Exception("Tipul expresiei este necunoscut");
                }
            }
            catch(Exception e)
            {
                throw (e);
            }
        }

    }
}

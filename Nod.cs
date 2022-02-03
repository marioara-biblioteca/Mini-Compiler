using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFT
{
    abstract class Nod
    {
        public abstract TipAtomLexical Tip { get; }
        public abstract IEnumerable<Nod> ObtineCopiiNod();
    }
}

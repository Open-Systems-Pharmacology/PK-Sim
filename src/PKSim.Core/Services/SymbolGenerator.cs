using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Core.Chart;

namespace PKSim.Core.Services
{
   public interface ISymbolGenerator
   {
      Symbols NextSymbol();
   }

   public class SymbolGenerator : ISymbolGenerator
   {
      private int _symbolIndex = 0;
      private readonly List<Symbols> _allSymbols;

      public SymbolGenerator()
      {
         _allSymbols = EnumHelper.AllValuesFor<Symbols>().ToList();
         _allSymbols.Remove(Symbols.None);
      }

      public Symbols NextSymbol()
      {
         return _allSymbols[_symbolIndex++ % _allSymbols.Count];
      }
   }
}
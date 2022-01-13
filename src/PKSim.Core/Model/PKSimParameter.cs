using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public class PKSimParameter : Parameter
   {
      public override double Value
      {
         get => base.Value;
         set
         {
            if (Formula == null || !Formula.IsConstant())
            {
               base.Value = value;
               return;
            }

            var constantFormula = Formula.DowncastTo<ConstantFormula>();

            //To avoid event trigger
            if (constantFormula.Value != value)
               constantFormula.Value = value;

            if (IsFixedValue)
               IsFixedValue = false;
         }
      }
   }

   public class NullParameter : PKSimParameter
   {
   }
}
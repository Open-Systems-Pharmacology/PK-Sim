using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.Engine;

namespace PKSim
{
   public abstract class ContextSpecificationAsync<T> : AbstractContextSpecificationAsync<T>
   {
      public override async Task GlobalContext()
      {
         await base.GlobalContext();
         EngineRegister.InitFormulaParser();
      }
   }

   public abstract class ContextSpecification<T> : AbstractContextSpecification<T>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         EngineRegister.InitFormulaParser();
      }
   }
}
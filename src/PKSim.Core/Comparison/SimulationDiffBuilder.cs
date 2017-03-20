using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class SimulationDiffBuilder : DiffBuilder<Simulation>
   {
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IObjectComparer _comparer;

      public SimulationDiffBuilder(ILazyLoadTask lazyLoadTask, IObjectComparer comparer)
      {
         _lazyLoadTask = lazyLoadTask;
         _comparer = comparer;
      }

      public override void Compare(IComparison<Simulation> comparison)
      {
         _lazyLoadTask.Load(comparison.Object1);
         _lazyLoadTask.Load(comparison.Object2);

         _comparer.Compare(comparison.ChildComparison(x => x.Model));
         _comparer.Compare(comparison.ChildComparison(x => x.UsedBuildingBlocks));
      }
   }
}
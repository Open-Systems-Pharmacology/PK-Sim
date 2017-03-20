using OSPSuite.Utility.Collections;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysesContextMenuFactory : IContextMenuFactory<IPopulationAnalysisField>
   {
   }

   public class PopulationAnalysesContextMenuFactory : ContextMenuFactory<IPopulationAnalysisField>, IPopulationAnalysesContextMenuFactory
   {
      public PopulationAnalysesContextMenuFactory(IRepository<IContextMenuSpecificationFactory<IPopulationAnalysisField>> contextMenuSpecFactoryRepository)
         : base(contextMenuSpecFactoryRepository)
      {
      }
   }
}
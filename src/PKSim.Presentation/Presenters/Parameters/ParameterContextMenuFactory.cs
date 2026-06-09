using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Collections;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IParameterContextMenuFactory : IContextMenuFactory<IParameterDTO>
   {
   }

   public class ParameterContextMenuFactory : ContextMenuFactory<IParameterDTO>, IParameterContextMenuFactory
   {
      public ParameterContextMenuFactory(IRepository<IContextMenuSpecificationFactory<IParameterDTO>> contextMenuSpecFactoryRepository) : base(contextMenuSpecFactoryRepository)
      {
      }
   }
}
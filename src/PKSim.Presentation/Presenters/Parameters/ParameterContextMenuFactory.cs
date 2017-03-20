using OSPSuite.Utility.Collections;

using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters.ContextMenus;

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
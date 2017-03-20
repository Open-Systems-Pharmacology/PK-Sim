using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Core
{
   public interface IObjectBaseDTOFactory
   {
      ObjectBaseDTO CreateFor<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock;
   }

   public class ObjectBaseDTOFactory : IObjectBaseDTOFactory
   {
      private readonly IWorkspace _workspace;

      public ObjectBaseDTOFactory(IWorkspace workspace)
      {
         _workspace = workspace;
      }

      public ObjectBaseDTO CreateFor<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         var dto = new ObjectBaseDTO {ContainerType = PKSimConstants.ObjectTypes.Project};
         dto.AddUsedNames(_workspace.Project.All<TBuildingBlock>().Select(x => x.Name));
         return dto;
      }
   }
}
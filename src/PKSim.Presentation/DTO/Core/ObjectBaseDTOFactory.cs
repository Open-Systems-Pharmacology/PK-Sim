using System.Linq;
using OSPSuite.Presentation.DTO;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.DTO.Core
{
   public interface IObjectBaseDTOFactory
   {
      ObjectBaseDTO CreateFor<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock;
   }

   public class ObjectBaseDTOFactory : IObjectBaseDTOFactory
   {
      private readonly ICoreWorkspace _workspace;
      private readonly IForbiddenCompoundNamesRetriever _forbiddenCompoundNamesRetriever;

      public ObjectBaseDTOFactory(ICoreWorkspace workspace, IForbiddenCompoundNamesRetriever forbiddenCompoundNamesRetriever)
      {
         _workspace = workspace;
         _forbiddenCompoundNamesRetriever = forbiddenCompoundNamesRetriever;
      }

      public ObjectBaseDTO CreateFor<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         var dto = createDTOFor<TBuildingBlock>();
         dto.ContainerType = PKSimConstants.ObjectTypes.Project;
         dto.AddUsedNames(_workspace.Project.All<TBuildingBlock>().Select(x => x.Name));
         return dto;
      }

      private ObjectBaseDTO createDTOFor<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         if (typeof(TBuildingBlock) != typeof(Compound))
            return new ObjectBaseDTO();

         var compoundDTO = new CompoundDTO();
         compoundDTO.AddForbiddenNames(_forbiddenCompoundNamesRetriever.All());
         return compoundDTO;
      }
   }
}

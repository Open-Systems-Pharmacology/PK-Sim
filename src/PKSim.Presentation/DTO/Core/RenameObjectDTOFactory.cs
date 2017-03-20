using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Core
{
   public class RenameObjectDTOFactory : IRenameObjectDTOFactory
   {
      private readonly IWorkspace _workspace;
      private readonly IObjectTypeResolver _objectTypeResolver;

      public RenameObjectDTOFactory(IWorkspace workspace, IObjectTypeResolver objectTypeResolver)
      {
         _workspace = workspace;
         _objectTypeResolver = objectTypeResolver;
      }

      public RenameObjectDTO CreateFor(IWithName objectBase)
      {
         //Entity might be a building block in disguise!
         var buildingBlock = objectBase as IPKSimBuildingBlock;
         if (buildingBlock != null)
            return createFor(buildingBlock);

         var parameterAnalyzable = objectBase as IParameterAnalysable;
         if (parameterAnalyzable != null)
            return createFor(parameterAnalyzable);

         var dto = new RenameObjectDTO(objectBase.Name);

         var entity = objectBase as IEntity;

         if (entity?.ParentContainer == null)
            return dto;

         dto.ContainerType = _objectTypeResolver.TypeFor(entity.ParentContainer);
         dto.AddUsedNames(entity.ParentContainer.AllChildrenNames());

         return dto;
      }

      private RenameObjectDTO createFor(IParameterAnalysable parameterAnalyzable)
      {
         return createRenameInProjectDTO(parameterAnalyzable, _workspace.Project.AllParameterAnalysables.Where(x => x.IsAnImplementationOf(parameterAnalyzable.GetType())));
      }

      private RenameObjectDTO createFor(IPKSimBuildingBlock buildingBlock)
      {
         return createRenameInProjectDTO(buildingBlock, _workspace.Project.All(buildingBlock.BuildingBlockType));
      }

      private RenameObjectDTO createRenameInProjectDTO(IWithName withName, IEnumerable<IWithName> existingObjects)
      {
         var dto = new RenameObjectDTO(withName.Name) {ContainerType = PKSimConstants.ObjectTypes.Project};
         dto.AddUsedNames(existingObjects.AllNames());
         return dto;
      }
   }
}
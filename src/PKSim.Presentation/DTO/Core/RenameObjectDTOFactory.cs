using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.DTO.Core
{
   public class RenameObjectDTOFactory : OSPSuite.Presentation.DTO.RenameObjectDTOFactory
   {
      private readonly IPKSimProjectRetriever _projectRetriever;
      private const PKSimBuildingBlockType _eventOrProtocol = PKSimBuildingBlockType.Event | PKSimBuildingBlockType.Protocol;

      public RenameObjectDTOFactory(
         IPKSimProjectRetriever projectRetriever,
         IObjectTypeResolver objectTypeResolver) : base(projectRetriever, objectTypeResolver)
      {
         _projectRetriever = projectRetriever;
      }

      public override RenameObjectDTO CreateFor(IWithName objectBase)
      {
         if (objectBase is IPKSimBuildingBlock buildingBlock)
            return createFor(buildingBlock);

         return base.CreateFor(objectBase);
      }

      private RenameObjectDTO createFor(IPKSimBuildingBlock buildingBlock)
      {
         var buildingBlockType = buildingBlock.BuildingBlockType.Is(_eventOrProtocol) ? _eventOrProtocol : buildingBlock.BuildingBlockType;
         return CreateRenameInProjectDTO(buildingBlock, _projectRetriever.Current.All(buildingBlockType));
      }
   }
}
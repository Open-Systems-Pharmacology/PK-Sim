using OSPSuite.Assets;
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
         switch (objectBase)
         {
            case Simulation simulation:
               return createFor(simulation);
            case IPKSimBuildingBlock buildingBlock:
               return createFor(buildingBlock);
            default:
               return base.CreateFor(objectBase);
         }
      }

      private RenameObjectDTO createFor(Simulation simulation)
      {
         var renameObjectDTO = new RenamePKSimSimulationDTO(simulation.Name)
         {
            ContainerType = ObjectTypes.Project
         };
         renameObjectDTO.AddUsedNames(_projectRetriever.Current.All(simulation.BuildingBlockType).AllNames());
         renameObjectDTO.AddCompoundNames(simulation.CompoundNames);
         return renameObjectDTO;
      }

      private RenameObjectDTO createFor(IPKSimBuildingBlock buildingBlock)
      {
         var buildingBlockType = buildingBlock.BuildingBlockType.Is(_eventOrProtocol) ? _eventOrProtocol : buildingBlock.BuildingBlockType;
         return CreateRenameInProjectDTO(buildingBlock, _projectRetriever.Current.All(buildingBlockType));
      }
   }
}
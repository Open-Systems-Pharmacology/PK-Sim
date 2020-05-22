using System.Threading.Tasks;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ObserverSetMappingMapper : SnapshotMapperBase<ObserverSetMapping, ObserverSetSelection, PKSimProject, PKSimProject>
   {
      private readonly ILogger _logger;

      public ObserverSetMappingMapper(ILogger logger)
      {
         _logger = logger;
      }

      public override Task<ObserverSetSelection> MapToSnapshot(ObserverSetMapping observerSetMapping, PKSimProject project)
      {
         var observerSetBuildingBlock = project.BuildingBlockById(observerSetMapping.TemplateObserverSetId);
         return SnapshotFrom(observerSetMapping, x => { x.Name = observerSetBuildingBlock.Name; });
      }

      public override Task<ObserverSetMapping> MapToModel(ObserverSetSelection snapshot, PKSimProject project)
      {
         var observerSet = project.BuildingBlockByName<Model.ObserverSet>(snapshot.Name);
         if (observerSet == null)
         {
            _logger.AddError(PKSimConstants.Error.CannotFindObserverSetForMapping(snapshot.Name));
            return null;
         }

         var observerSetMapping = new ObserverSetMapping {TemplateObserverSetId = observerSet.Id ?? string.Empty};
         return Task.FromResult(observerSetMapping);
      }
   }
}
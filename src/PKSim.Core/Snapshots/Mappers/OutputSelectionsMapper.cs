using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Services;
using SnapshotOutputSelections = PKSim.Core.Snapshots.OutputSelections;
using ModelOutputSelections = OSPSuite.Core.Domain.OutputSelections;

namespace PKSim.Core.Snapshots.Mappers
{
   public class OutputSelectionsMapper : SnapshotMapperBase<ModelOutputSelections, SnapshotOutputSelections, Model.Simulation>
   {
      private readonly IEntitiesInContainerRetriever _entitiesInContainerRetriever;
      private readonly ILogger _logger;

      public OutputSelectionsMapper(IEntitiesInContainerRetriever entitiesInContainerRetriever, ILogger logger)
      {
         _entitiesInContainerRetriever = entitiesInContainerRetriever;
         _logger = logger;
      }

      public override async Task<SnapshotOutputSelections> MapToSnapshot(ModelOutputSelections outputSelections)
      {
         if (!outputSelections.HasSelection)
            return null;

         var snapshot = await SnapshotFrom(outputSelections);
         snapshot.AddRange(outputSelections.AllOutputs.Select(x => x.Path));
         return snapshot;
      }

      public override Task<ModelOutputSelections> MapToModel(SnapshotOutputSelections snapshot, Model.Simulation simulation)
      {
         var outputSelections = new ModelOutputSelections();
         if (snapshot == null)
            return Task.FromResult(outputSelections);

         var allQuantities = _entitiesInContainerRetriever.QuantitiesFrom(simulation);

         snapshot.Each(path =>
         {
            var quantity = allQuantities[path];
            if (quantity == null)
               _logger.AddWarning(PKSimConstants.Error.CouldNotFindQuantityWithPath(path));
            else
               outputSelections.AddOutput(new QuantitySelection(path, quantity.QuantityType));
         });

         return Task.FromResult(outputSelections);
      }
   }
}
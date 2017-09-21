using System;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
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

      public OutputSelectionsMapper(IEntitiesInContainerRetriever entitiesInContainerRetriever)
      {
         _entitiesInContainerRetriever = entitiesInContainerRetriever;
      }

      public override async Task<SnapshotOutputSelections> MapToSnapshot(ModelOutputSelections outputSelections)
      {
         var snapshot = await SnapshotFrom(outputSelections);
         snapshot.AddRange(outputSelections.AllOutputs.Select(x => x.Path));
         return snapshot;
      }

      public override Task<ModelOutputSelections> MapToModel(SnapshotOutputSelections snapshot, Model.Simulation simulation)
      {
         var allQuantities = _entitiesInContainerRetriever.QuantitiesFrom(simulation);
         var outputSelections = new ModelOutputSelections();

         snapshot.Each(path =>
         {
            var quantity = allQuantities[path];
            if (quantity == null)
               throw new SnapshotOutdatedException(PKSimConstants.Error.CouldNotFindQuantityWithPath(path));

            outputSelections.AddOutput(new QuantitySelection(path, quantity.QuantityType));
         });

         return Task.FromResult(outputSelections);
      }
   }
}
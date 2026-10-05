using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using ModelSimulation = PKSim.Core.Model.Simulation;
using ModelOverwriteParameterSetSelection = PKSim.Core.Model.OverwriteParameterSetSelection;
using SnapshotOverwriteParameterSetSelection = PKSim.Core.Snapshots.OverwriteParameterSetSelection;

namespace PKSim.Core.Snapshots.Mappers;

public class OverwriteParameterSetSelectionMapper : SnapshotMapperBase<ModelOverwriteParameterSetSelection, SnapshotOverwriteParameterSetSelection, SnapshotContextWithSimulation, PKSimProject>
{
   private readonly IOSPSuiteLogger _logger;

   public OverwriteParameterSetSelectionMapper(IOSPSuiteLogger logger)
   {
      _logger = logger;
   }

   public override Task<SnapshotOverwriteParameterSetSelection> MapToSnapshot(ModelOverwriteParameterSetSelection selection, PKSimProject project)
   {
      return SnapshotFrom(selection, snapshot =>
      {
         snapshot.CompoundName = selection.CompoundName;
         snapshot.OverwriteParameterSetName = selection.OverwriteParameterSet?.Name;
      });
   }

   public override Task<ModelOverwriteParameterSetSelection> MapToModel(SnapshotOverwriteParameterSetSelection snapshot, SnapshotContextWithSimulation snapshotContext)
   {
      var simulation = snapshotContext.Simulation.DowncastTo<ModelSimulation>();
      var compound = simulation.Compounds.FindByName(snapshot.CompoundName);
      if (compound == null)
         return Task.FromResult<ModelOverwriteParameterSetSelection>(null);

      if (string.IsNullOrEmpty(snapshot.OverwriteParameterSetName))
         return Task.FromResult(new ModelOverwriteParameterSetSelection { CompoundName = snapshot.CompoundName });

      var overwriteParameterSet = compound.OverwriteParameterSets.FirstOrDefault(x => x.IsNamed(snapshot.OverwriteParameterSetName));
      if (overwriteParameterSet == null)
      {
         _logger.AddError(PKSimConstants.Error.OverWriteParameterSetNotFoundInCompound(snapshot.OverwriteParameterSetName, snapshot.CompoundName));
         return Task.FromResult<ModelOverwriteParameterSetSelection>(null);
      }

      var selection = new ModelOverwriteParameterSetSelection
      {
         CompoundName = snapshot.CompoundName,
         OverwriteParameterSet = overwriteParameterSet
      };

      return Task.FromResult(selection);
   }
}

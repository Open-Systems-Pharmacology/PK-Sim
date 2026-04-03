using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Assets;
using PKSim.Core.Model;
using ModelCompound = PKSim.Core.Model.Compound;
using ModelOverwriteParameterSetSelection = PKSim.Core.Model.OverwriteParameterSetSelection;
using SnapshotOverwriteParameterSetSelection = PKSim.Core.Snapshots.OverwriteParameterSetSelection;

namespace PKSim.Core.Snapshots.Mappers;

public class OverwriteParameterSetSelectionMapper : SnapshotMapperBase<ModelOverwriteParameterSetSelection, SnapshotOverwriteParameterSetSelection, SnapshotContext, PKSimProject>
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

   public override Task<ModelOverwriteParameterSetSelection> MapToModel(SnapshotOverwriteParameterSetSelection snapshot, SnapshotContext snapshotContext)
   {
      var project = snapshotContext.PKSimProject();
      var compound = project.BuildingBlockByName<ModelCompound>(snapshot.CompoundName);
      if (compound == null)
      {
         _logger.AddWarning(PKSimConstants.Error.SimulationTemplateBuildingBlockNotFoundInProject(snapshot.CompoundName, nameof(ModelCompound)));
         return Task.FromResult<ModelOverwriteParameterSetSelection>(null);
      }

      var overwriteParameterSet = compound.OverwriteParameterSets.FirstOrDefault(x => x.IsNamed(snapshot.OverwriteParameterSetName));
      if (overwriteParameterSet == null)
      {
         _logger.AddWarning(PKSimConstants.Error.OverWriteParameterSetNotFoundInCompound(snapshot.OverwriteParameterSetName, snapshot.CompoundName));
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

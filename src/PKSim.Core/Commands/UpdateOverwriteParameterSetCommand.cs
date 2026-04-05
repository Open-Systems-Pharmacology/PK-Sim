using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   /// <summary>
   ///    Updates an existing <see cref="OverwriteParameterSet"/> by adding/replacing parameter values
   ///    and removing paths that were reset by the user. Stores the previous state for undo.
   /// </summary>
   public class UpdateOverwriteParameterSetCommand : BuildingBlockChangeCommand<Compound>
   {
      private readonly string _overwriteParameterSetId;
      private OverwriteParameterSet _overwriteParameterSet;
      private readonly List<ParameterValue> _newParameterValues;
      private readonly List<string> _pathsToRemove;
      private List<ParameterValue> _previousParameterValues;

      public UpdateOverwriteParameterSetCommand(
         OverwriteParameterSet overwriteParameterSet,
         Compound compound,
         IReadOnlyList<ParameterValue> newParameterValues,
         IReadOnlyList<string> pathsToRemove)
         : base(compound)
      {
         _overwriteParameterSet = overwriteParameterSet;
         _overwriteParameterSetId = overwriteParameterSet.Id;
         _newParameterValues = newParameterValues.ToList();
         _pathsToRemove = pathsToRemove.ToList();
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = $"Update {PKSimConstants.ObjectTypes.OverwriteParameterSet} '{overwriteParameterSet.Name}' in {PKSimConstants.ObjectTypes.Compound} '{compound.Name}'";
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         base.PerformExecuteWith(context);

         //Save previous state for undo
         _previousParameterValues = _overwriteParameterSet.ParameterValues.ToList();

         //Remove reset parameters
         foreach (var path in _pathsToRemove)
         {
            var existing = _overwriteParameterSet.ParameterValueByPath(path);
            if (existing != null)
               _overwriteParameterSet.Remove(existing);
         }

         //Add or replace parameter values
         foreach (var pv in _newParameterValues)
            _overwriteParameterSet.Add(pv);
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RestoreOverwriteParameterSetCommand(_overwriteParameterSet, _buildingBlock, _previousParameterValues).AsInverseFor(this);
      }

      protected override void ClearReferences()
      {
         base.ClearReferences();
         _overwriteParameterSet = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _overwriteParameterSet = _buildingBlock.OverwriteParameterSets.FindById(_overwriteParameterSetId);
      }
   }
}

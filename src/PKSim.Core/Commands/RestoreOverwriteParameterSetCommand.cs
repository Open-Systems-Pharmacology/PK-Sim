using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   /// <summary>
   ///    Restores an <see cref="OverwriteParameterSet"/> to a previous state.
   ///    Used as inverse of <see cref="UpdateOverwriteParameterSetCommand"/>.
   /// </summary>
   public class RestoreOverwriteParameterSetCommand : BuildingBlockChangeCommand<Compound>
   {
      private readonly string _overwriteParameterSetId;
      private OverwriteParameterSet _overwriteParameterSet;
      private readonly List<ParameterValue> _parameterValuesToRestore;
      private List<ParameterValue> _previousParameterValues;

      public RestoreOverwriteParameterSetCommand(
         OverwriteParameterSet overwriteParameterSet,
         Compound compound,
         IReadOnlyList<ParameterValue> parameterValuesToRestore)
         : base(compound)
      {
         _overwriteParameterSet = overwriteParameterSet;
         _overwriteParameterSetId = overwriteParameterSet.Id;
         _parameterValuesToRestore = parameterValuesToRestore.ToList();
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = $"Restore {PKSimConstants.ObjectTypes.OverwriteParameterSet} '{overwriteParameterSet.Name}' in {PKSimConstants.ObjectTypes.Compound} '{compound.Name}'";
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         base.PerformExecuteWith(context);

         //Save current state before restoring
         _previousParameterValues = _overwriteParameterSet.ParameterValues.ToList();

         //Clear and restore
         _overwriteParameterSet.ParameterValues.ToList().Each(pv => _overwriteParameterSet.Remove(pv));
         _parameterValuesToRestore.Each(pv => _overwriteParameterSet.Add(pv));
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

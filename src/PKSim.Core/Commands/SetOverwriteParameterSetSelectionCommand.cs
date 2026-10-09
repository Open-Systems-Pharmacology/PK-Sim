using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   /// <summary>
   ///    Selects the <see cref="OverwriteParameterSet" /> named <c>overwriteParameterSetName</c> of the compound used in the
   ///    simulation for the compound named <c>compoundName</c>. The set is resolved by name when the command executes.
   ///    A <c>null</c> name selects no set. Undo restores the previous selection, or removes the selection entry when the
   ///    compound had none.
   /// </summary>
   public class SetOverwriteParameterSetSelectionCommand : PKSimReversibleCommand
   {
      private readonly string _simulationId;
      private readonly string _compoundName;
      private readonly string _overwriteParameterSetName;
      private readonly bool _removeSelection;
      private Simulation _simulation;
      private bool _hadSelection;
      private string _previousOverwriteParameterSetName;

      public SetOverwriteParameterSetSelectionCommand(Simulation simulation, string compoundName, string overwriteParameterSetName)
         : this(simulation, compoundName, overwriteParameterSetName, removeSelection: false)
      {
      }

      private SetOverwriteParameterSetSelectionCommand(Simulation simulation, string compoundName, string overwriteParameterSetName, bool removeSelection)
      {
         _simulation = simulation;
         _simulationId = simulation.Id;
         _compoundName = compoundName;
         _overwriteParameterSetName = overwriteParameterSetName;
         _removeSelection = removeSelection;
         Visible = false;
         ObjectType = PKSimConstants.ObjectTypes.Simulation;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.SelectOverwriteParameterSetForCompoundInSimulation(overwriteParameterSetName ?? PKSimConstants.UI.None, compoundName, simulation.Name);
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         var selections = _simulation.OverwriteParameterSetSelections;
         _hadSelection = selections.HasSelectionFor(_compoundName);
         _previousOverwriteParameterSetName = selections.SelectedSetFor(_compoundName)?.Name;

         if (_removeSelection)
            selections.RemoveSelectionForCompound(_compoundName);
         else
            _simulation.AddOverwriteParameterSetSelection(_compoundName, overwriteParameterSetToSelect());

         context.PublishEvent(new SimulationStatusChangedEvent(_simulation));
      }

      private OverwriteParameterSet overwriteParameterSetToSelect()
      {
         if (_overwriteParameterSetName == null)
            return null;

         return _simulation.Compounds.FindByName(_compoundName).OverwriteParameterSets.FindByName(_overwriteParameterSetName);
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context) =>
         new SetOverwriteParameterSetSelectionCommand(_simulation, _compoundName, _previousOverwriteParameterSetName, removeSelection: !_hadSelection).AsInverseFor(this);

      protected override void ClearReferences() => _simulation = null;

      public override void RestoreExecutionData(IExecutionContext context) => _simulation = context.Get<Simulation>(_simulationId);
   }
}

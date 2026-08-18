using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   /// <summary>
   ///    Tracks or untracks compound parameter paths in the <see cref="SimulationParameterChangeTracker" /> of a
   ///    simulation. Only paths whose tracking state actually changes are reversed by the inverse command.
   /// </summary>
   public class SetSimulationParameterTrackingCommand : PKSimReversibleCommand
   {
      private readonly bool _tracked;
      private readonly string _simulationId;
      private readonly IReadOnlyList<string> _parameterPaths;
      private IReadOnlyList<string> _changedPaths;
      private Simulation _simulation;

      public SetSimulationParameterTrackingCommand(Simulation simulation, IReadOnlyList<string> parameterPaths, bool tracked)
      {
         _simulation = simulation;
         _simulationId = simulation.Id;
         _parameterPaths = parameterPaths;
         _tracked = tracked;
         Visible = false;
         ObjectType = PKSimConstants.ObjectTypes.Simulation;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.SetSimulationParameterTracking(parameterPaths.Count, tracked, simulation.Name);
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         var tracker = _simulation.ParameterChangeTracker;
         _changedPaths = _parameterPaths.Where(path => tracker.IsTracked(path) != _tracked).ToList();
         _changedPaths.Each(path =>
         {
            if (_tracked)
               tracker.Track(path);
            else
               tracker.Untrack(path);
         });

         context.PublishEvent(new SimulationStatusChangedEvent(_simulation));
      }

      protected override void ClearReferences()
      {
         _simulation = null;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context) =>
         new SetSimulationParameterTrackingCommand(_simulation, _changedPaths, !_tracked).AsInverseFor(this);

      public override void RestoreExecutionData(IExecutionContext context)
      {
         _simulation = context.Get<Simulation>(_simulationId);
      }
   }
}

using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Commands
{
   public class SwapSimulationCommand : SwapBuildingBlockCommand<Simulation>
   {
      public SwapSimulationCommand(Simulation oldSimulation, Simulation newSimulation, IExecutionContext context) : base(oldSimulation, newSimulation, context)
      {
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         context.PublishEvent(new SwapSimulationEvent(_oldBuildingBlock, _newBuildingBlock));
         base.PerformExecuteWith(context);

         var simulationReferenceUpdater = context.Resolve<ISimulationReferenceUpdater>();

         simulationReferenceUpdater.SwapSimulationInParameterAnalysables(_oldBuildingBlock, _newBuildingBlock);

         var simDiffBuilder = context.Resolve<ISimulationCommandDescriptionBuilder>();
         Description = PKSimConstants.Command.ConfigureSimulationDescription;
         ExtendedDescription = simDiffBuilder.BuildDifferenceBetween(_oldBuildingBlock, _newBuildingBlock).ToStringReport();
      }

      protected override RemoveBuildingBlockFromProjectCommand PerformRemoveCommand(IExecutionContext context)
      {
         var oldClassifiable = context.CurrentProject.AllClassifiables.FindById(_oldBuildingBlock.Id);
         var command = base.PerformRemoveCommand(context);

         // Add the new simulation to the project with it's classification set already
         moveNewSimulationUnderOldClassification(context.CurrentProject, oldClassifiable);

         return command;
      }

      private void moveNewSimulationUnderOldClassification(IProject project, IClassifiable oldClassifiable)
      {
         if (oldClassifiable == null) return;

         project.AddClassifiable(new ClassifiableSimulation { Subject = _newBuildingBlock, Parent = oldClassifiable.Parent });
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SwapSimulationCommand(_newBuildingBlock, _oldBuildingBlock, context).AsInverseFor(this);
      }
   }
}
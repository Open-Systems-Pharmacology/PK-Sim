using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using IProtocolTask = PKSim.Presentation.Services.IProtocolTask;

namespace PKSim.Presentation.UICommands
{
   public interface IEditBuildingBlockUICommand<TBuildingBlock> : IObjectUICommand<TBuildingBlock> where TBuildingBlock : class, IPKSimBuildingBlock
   {
   }

   public abstract class EditBuildingBlockUICommand<TBuildingBlock, TBuildingBlockTask> : ObjectUICommand<TBuildingBlock>, IEditBuildingBlockUICommand<TBuildingBlock>
      where TBuildingBlock : class, IPKSimBuildingBlock
      where TBuildingBlockTask : IBuildingBlockTask<TBuildingBlock>
   {
      private readonly IBuildingBlockTask<TBuildingBlock> _buildingBlockTask;

      protected EditBuildingBlockUICommand(TBuildingBlockTask buildingBlockTask)
      {
         _buildingBlockTask = buildingBlockTask;
      }

      protected override void PerformExecute()
      {
         _buildingBlockTask.Edit(Subject);
      }
   }

   public class EditCompoundCommand : EditBuildingBlockUICommand<Compound, ICompoundTask>
   {
      public EditCompoundCommand(ICompoundTask compoundTask) : base(compoundTask)
      {
      }
   }

   public class EditIndividualCommand : EditBuildingBlockUICommand<Individual, IIndividualTask>
   {
      public EditIndividualCommand(IIndividualTask individualTask) : base(individualTask)
      {
      }
   }

   public class EditProtocolCommand : EditBuildingBlockUICommand<Protocol, IProtocolTask>
   {
      public EditProtocolCommand(IProtocolTask protocolTask) : base(protocolTask)
      {
      }
   }

   public class EditSimulationCommand : EditBuildingBlockUICommand<Simulation, ISimulationTask>
   {
      public EditSimulationCommand(ISimulationTask simulationTask) : base(simulationTask)
      {
      }
   }

   public class EditFormulationCommand : EditBuildingBlockUICommand<Formulation, IFormulationTask>
   {
      public EditFormulationCommand(IFormulationTask formulationTask) : base(formulationTask)
      {
      }
   }

   public class EditEventCommand : EditBuildingBlockUICommand<PKSimEvent, IEventTask>
   {
      public EditEventCommand(IEventTask eventTask) : base(eventTask)
      {
      }
   }

   public class EditObserverSetCommand : EditBuildingBlockUICommand<ObserverSet, IObserverSetTask>
   {
      public EditObserverSetCommand(IObserverSetTask observerSetTask) : base(observerSetTask)
      {
      }
   }

   public class EditExpressionProfileCommand : EditBuildingBlockUICommand<ExpressionProfile, IExpressionProfileTask>
   {
      public EditExpressionProfileCommand(IExpressionProfileTask expressionProfileTask) : base(expressionProfileTask)
      {
      }
   }

   public class EditPopulationCommand : EditBuildingBlockUICommand<Population, IPopulationTask>
   {
      public EditPopulationCommand(IPopulationTask populationTask) : base(populationTask)
      {
      }
   }
}
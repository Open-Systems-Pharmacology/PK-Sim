using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using IProtocolTask = PKSim.Presentation.Services.IProtocolTask;

namespace PKSim.Presentation.UICommands
{
   public abstract class AddBuildingBlockUICommand<TBuildingBlock, TBuildingBlockTask> : IUICommand where TBuildingBlock : class, IPKSimBuildingBlock
      where TBuildingBlockTask : IBuildingBlockTask<TBuildingBlock>
   {
      private readonly IBuildingBlockTask<TBuildingBlock> _buildingBlockTask;

      protected AddBuildingBlockUICommand(TBuildingBlockTask buildingBlockTask)
      {
         _buildingBlockTask = buildingBlockTask;
      }

      public virtual void Execute()
      {
         _buildingBlockTask.AddToProject();
      }
   }

   public class AddCompoundUICommand : AddBuildingBlockUICommand<Compound, ICompoundTask>
   {
      public AddCompoundUICommand(ICompoundTask compoundTask) : base(compoundTask)
      {
      }
   }

   public class AddExpressionProfileCommand<TMolecule>:IUICommand where TMolecule : IndividualMolecule
   {
      private readonly IExpressionProfileTask _expressionProfileTask;

      public AddExpressionProfileCommand(IExpressionProfileTask expressionProfileTask) 
      {
         _expressionProfileTask = expressionProfileTask;
      }

      public void Execute()
      {
         _expressionProfileTask.AddForMoleculeToProject<TMolecule>();
      }
   }

   public class AddEventCommand : AddBuildingBlockUICommand<PKSimEvent, IEventTask>
   {
      public AddEventCommand(IEventTask eventTask) : base(eventTask)
      {
      }
   }

   public class AddFormulationCommand : AddBuildingBlockUICommand<Formulation, IFormulationTask>
   {
      public AddFormulationCommand(IFormulationTask formulationTask) : base(formulationTask)
      {
      }
   }

   public class AddIndividualCommand : AddBuildingBlockUICommand<Individual, IIndividualTask>
   {
      public AddIndividualCommand(IIndividualTask individualTask) : base(individualTask)
      {
      }
   }

   public class AddObserverSetCommand : AddBuildingBlockUICommand<ObserverSet, IObserverSetTask>
   {
      public AddObserverSetCommand(IObserverSetTask buildingBlockTask) : base(buildingBlockTask)
      {
      }
   }

   public class AddProtocolCommand : AddBuildingBlockUICommand<Protocol, IProtocolTask>
   {
      public AddProtocolCommand(IProtocolTask buildingBlockTask) : base(buildingBlockTask)
      {
      }
   }

   public class AddRandomPopulationCommand : AddBuildingBlockUICommand<Population, IPopulationTask>
   {
      public AddRandomPopulationCommand(IPopulationTask populationTask) : base(populationTask)
      {
      }
   }

   public class AddSimulationCommand : AddBuildingBlockUICommand<Simulation, ISimulationTask>
   {
      public AddSimulationCommand(ISimulationTask simulationTask) : base(simulationTask)
      {
      }
   }

}
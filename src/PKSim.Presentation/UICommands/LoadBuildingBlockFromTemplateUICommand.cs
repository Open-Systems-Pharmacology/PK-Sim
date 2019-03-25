using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using IProtocolTask = PKSim.Presentation.Services.IProtocolTask;

namespace PKSim.Presentation.UICommands
{
   public abstract class LoadBuildingBlockFromTemplateUICommand<TBuildingBlock, TBuildingBlockTask> : IUICommand
      where TBuildingBlock : IPKSimBuildingBlock
      where TBuildingBlockTask : IBuildingBlockTask<TBuildingBlock>
   {
      private readonly TBuildingBlockTask _buildingBlockTask;

      protected LoadBuildingBlockFromTemplateUICommand(TBuildingBlockTask buildingBlockTask)
      {
         _buildingBlockTask = buildingBlockTask;
      }

      public void Execute()
      {
         _buildingBlockTask.LoadSingleFromTemplate();
      }
   }

   public class LoadCompoundCommand : LoadBuildingBlockFromTemplateUICommand<Compound, ICompoundTask>
   {
      public LoadCompoundCommand(ICompoundTask compoundTask)
         : base(compoundTask)
      {
      }
   }

   public class LoadIndividualCommand : LoadBuildingBlockFromTemplateUICommand<Individual, IIndividualTask>
   {
      public LoadIndividualCommand(IIndividualTask individualTask)
         : base(individualTask)
      {
      }
   }

   public class LoadProtocolCommand : LoadBuildingBlockFromTemplateUICommand<Protocol, IProtocolTask>
   {
      public LoadProtocolCommand(IProtocolTask protocolTask)
         : base(protocolTask)
      {
      }
   }

   public class LoadPopulationCommand : LoadBuildingBlockFromTemplateUICommand<Population, IPopulationTask>
   {
      public LoadPopulationCommand(IPopulationTask populationTask)
         : base(populationTask)
      {
      }
   }

   public class LoadFormulationCommand : LoadBuildingBlockFromTemplateUICommand<Formulation, IFormulationTask>
   {
      public LoadFormulationCommand(IFormulationTask formulationTask)
         : base(formulationTask)
      {
      }
   }

   public class LoadEventCommand : LoadBuildingBlockFromTemplateUICommand<PKSimEvent, IEventTask>
   {
      public LoadEventCommand(IEventTask eventTask) : base(eventTask)
      {
      }
   }

   public class LoadObserverSetCommand : LoadBuildingBlockFromTemplateUICommand<ObserverSet, IObserverSetTask>
   {
      public LoadObserverSetCommand(IObserverSetTask observerSetTask) : base(observerSetTask)
      {
      }
   }
}
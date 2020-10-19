using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RenameMoleculeInSimulationSubjectCommand : RenameEntityCommand
   {
      public RenameMoleculeInSimulationSubjectCommand(IEntity entity, string newName, IExecutionContext context) : base(entity, newName, context)
      {
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         //This will rename the global molecule only. We need to also rename all local containers
         base.PerformExecuteWith(context);

         var individual = context.Get<ISimulationSubject>(BuildingBlockId).Individual;

         var allMoleculeContainers = individual.GetAllChildren<IContainer>(x => x.IsNamed(_oldName));
         allMoleculeContainers.Each(x => x.Name = _newName);
      }
   }
}
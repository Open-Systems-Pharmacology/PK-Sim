using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RenameMoleculeInSimulationSubjectCommand : RenameEntityCommand
   {
      private ISimulationSubject _simulationSubject;
      private IndividualMolecule _individualMolecule;

      public RenameMoleculeInSimulationSubjectCommand(IndividualMolecule individualMolecule, ISimulationSubject simulationSubject, string newName, IExecutionContext context) : 
         base(individualMolecule, newName, context)
      {
         _simulationSubject = simulationSubject;
         _individualMolecule = individualMolecule;
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         var individual = _simulationSubject.Individual;
 
         //This needs to be retrieved before performing base operation to ensure that we capture all containers before they are renamed
         var allMoleculeContainers = individual.GetAllChildren<IContainer>(x => x.IsNamed(_individualMolecule.Name));
 
         //This will rename the global molecule only. We need to also rename all local containers
         base.PerformExecuteWith(context);

         //Now rename all other containers
         allMoleculeContainers.Each(x => x.Name = _newName);

         //We also need to update all object path
         allMoleculeContainers.SelectMany(x=>x.GetAllChildren<IUsingFormula>()).Each(x => x.Formula.ReplaceKeywordInObjectPaths(_oldName, _newName));
      }

      protected override void ClearReferences()
      {
         base.ClearReferences();
         _simulationSubject = null;
         _individualMolecule = null;
      }
   }
}
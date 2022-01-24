using System;
using OSPSuite.Core.Commands.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIIndividualOntogenyTask : IOntogenyTask
   {
      private readonly IExecutionContext _executionContext;

      public CLIIndividualOntogenyTask(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      public ICommand SetOntogenyForMolecule(IndividualMolecule molecule, Ontogeny ontogeny, ISimulationSubject simulationSubject)
      {
         return new SetOntogenyInMoleculeCommand(molecule, ontogeny, simulationSubject, _executionContext).Run(_executionContext);
      }

      public void ShowOntogenyData(Ontogeny ontogeny)
      {
         throw new NotSupportedException();
      }

      public ICommand LoadOntogenyForMolecule(IndividualMolecule molecule, ISimulationSubject simulationSubject)
      {
         throw new NotSupportedException();
      }
   }
}
using System;
using OSPSuite.Core.Commands.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Matlab
{
   public class MatlabIndividualOntogenyTask : IOntogenyTask<Individual>
   {
      private readonly IExecutionContext _executionContext;

      public MatlabIndividualOntogenyTask(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      public ICommand SetOntogenyForMolecule(IndividualMolecule molecule, Ontogeny ontogeny, Individual individual)
      {
         return new SetOntogenyInMoleculeForIndividualCommand(molecule, ontogeny, individual, _executionContext).Run(_executionContext);
      }

      public void ShowOntogenyData(Ontogeny ontogeny)
      {
         throw new NotSupportedException();
      }

      public ICommand LoadOntogenyForMolecule(IndividualMolecule molecule, Individual simulationSubject)
      {
         throw new NotSupportedException();
      }
   }
}
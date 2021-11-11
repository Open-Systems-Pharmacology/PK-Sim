﻿using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class IndividualExpressionTask : ISimulationSubjectExpressionTask<Individual>
   {
      private readonly IExecutionContext _executionContext;

      public IndividualExpressionTask(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      public ICommand RemoveMoleculeFrom(IndividualMolecule molecule, Individual individual)
      {
         return new RemoveMoleculeFromIndividualCommand(molecule, individual, _executionContext).Run(_executionContext);
      }

      public ICommand AddMoleculeTo(IndividualMolecule molecule, Individual individual)
      {
         return new AddMoleculeToIndividualCommand(molecule, individual, _executionContext).Run(_executionContext);
      }

      public IOSPSuiteCommand EditMolecule(IndividualMolecule moleculeToEdit, QueryExpressionResults queryResults, Individual individual)
      {
         return new EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand(moleculeToEdit, queryResults, individual).Run(_executionContext);
      }


      public ICommand RenameMolecule(IndividualMolecule molecule, string newName, Individual simulationSubject)
      {
         return new RenameMoleculeInSimulationSubjectCommand(molecule, simulationSubject, newName, _executionContext).Run(_executionContext);
      }
   }
}
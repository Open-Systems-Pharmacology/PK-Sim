﻿using System.Collections.Generic;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class PopulationExpressionTask : ISimulationSubjectExpressionTask<Population>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IMoleculeParameterVariabilityCreator _moleculeParameterVariabilityCreator;
      private readonly IEntityPathResolver _entityPathResolver;

      public PopulationExpressionTask(IExecutionContext executionContext, IMoleculeParameterVariabilityCreator moleculeParameterVariabilityCreator,
         IEntityPathResolver entityPathResolver)
      {
         _executionContext = executionContext;
         _moleculeParameterVariabilityCreator = moleculeParameterVariabilityCreator;
         _entityPathResolver = entityPathResolver;
      }

      public ICommand RemoveMoleculeFrom(IndividualMolecule molecule, Population population)
      {
         var removeMoleculeCommand = new RemoveMoleculeFromPopulationCommand(molecule, population, _executionContext);
         var command = macroCommandFrom(population,
            removeMoleculeCommand,
            new[]
            {
               removeAdvancedParametersForMolecule(molecule, population),
               removeMoleculeCommand.Run(_executionContext)
            }
         );
         return command;
      }

      private ICommand removeAdvancedParametersForMolecule(IndividualMolecule molecule, Population population)
      {
         var macroCommand = new PKSimMacroCommand
         {
            CommandType = PKSimConstants.Command.CommandTypeDelete,
            Description = PKSimConstants.Command.RemoveAdvancedParametersForMoleculeInPopulation(molecule.Name, population.Name)
         };

         foreach (var parameter in molecule.GetAllChildren<IParameter>())
         {
            var advancedParameter = population.AdvancedParameterFor(_entityPathResolver, parameter);
            if (advancedParameter != null)
               macroCommand.AddCommand(
                  new RemoveAdvancedParameterFromContainerCommand(advancedParameter, population, _executionContext).Run(_executionContext));
         }

         _executionContext.UpdateBuildingBlockPropertiesInCommand(macroCommand, population);
         return macroCommand;
      }

      public ICommand AddMoleculeTo(IndividualMolecule molecule, Population population)
      {
         var baseCommand = new AddMoleculeToPopulationCommand(molecule, population, _executionContext);
         return addMoleculeToPopulation(molecule, population, baseCommand);
      }

      public IOSPSuiteCommand EditMolecule(IndividualMolecule moleculeToEdit,  QueryExpressionResults queryResults, Population population)
      {
         return new EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand(moleculeToEdit,  queryResults, population)
            .Run(_executionContext);
      }


      public ICommand RenameMolecule(IndividualMolecule molecule, string newName, Population simulationSubject)
      {
         return new RenameMoleculeInSimulationSubjectCommand(molecule, simulationSubject, newName, _executionContext).Run(_executionContext);
      }

      private ICommand addMoleculeToPopulation(IndividualMolecule molecule, Population population, ICommand<IExecutionContext> baseCommand)
      {
         return macroCommandFrom(population,
            baseCommand,
            new[]
            {
               baseCommand.Run(_executionContext),
               _moleculeParameterVariabilityCreator.AddMoleculeVariability(molecule, population, usePredefinedMeanVariability: true)
            }
         );
      }

      private ICommand macroCommandFrom(Population population, ICommand baseCommand, IEnumerable<ICommand> commands)
      {
         var macroCommand = new PKSimMacroCommand
         {
            ObjectType = baseCommand.ObjectType,
            Description = baseCommand.Description,
            CommandType = baseCommand.CommandType
         };

         macroCommand.AddRange(commands);
         _executionContext.UpdateBuildingBlockPropertiesInCommand(macroCommand, population);
         return macroCommand;
      }
   }
}
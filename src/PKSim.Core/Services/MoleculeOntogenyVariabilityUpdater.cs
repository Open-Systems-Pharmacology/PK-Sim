using System;
using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Core.Services
{
   public interface IMoleculeOntogenyVariabilityUpdater
   {
      void UpdatePlasmaProteinsOntogenyFor(ISimulationSubject simulationSubject);

      void UpdateMoleculeOntogeny(IndividualMolecule molecule, Ontogeny ontogeny, ISimulationSubject simulationSubject);

      /// <summary>
      ///    Updates the ontogeny factor for all plasma proteins and molecules defined in the population
      /// </summary>
      void UpdateAllOntogenies(Population population);
   }

   public class MoleculeOntogenyVariabilityUpdater : IMoleculeOntogenyVariabilityUpdater
   {
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IFormulaFactory _formulaFactory;

      public MoleculeOntogenyVariabilityUpdater(
         IOntogenyRepository ontogenyRepository,
         IEntityPathResolver entityPathResolver,
         IFormulaFactory formulaFactory)
      {
         _ontogenyRepository = ontogenyRepository;
         _entityPathResolver = entityPathResolver;
         _formulaFactory = formulaFactory;
      }

      public void UpdatePlasmaProteinsOntogenyFor(ISimulationSubject simulationSubject)
      {
         switch (simulationSubject)
         {
            case Individual individual:
               updatePlasmaProteinsOntogenyFor(individual);
               break;
            case Population population:
               updatePlasmaProteinsOntogenyFor(population);
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(simulationSubject));
         }
      }

      public void UpdateMoleculeOntogeny(IndividualMolecule molecule, Ontogeny ontogeny, ISimulationSubject simulationSubject)
      {
         switch (simulationSubject)
         {
            case Individual:
               updateMoleculeOntogeny(molecule, ontogeny);
               break;
            case Population population:
               updateMoleculeOntogenyInPopulation(molecule, ontogeny, population);
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(simulationSubject));
         }
      }

      private void updatePlasmaProteinsOntogenyFor(Individual individual)
      {
         _ontogenyRepository.SupportedProteins.KeyValues.Each(kv => updatePlasmaProteinOntogenyFor(individual, kv.Key, kv.Value));
      }

      private void updatePlasmaProteinOntogenyFor(Individual individual, string tableParameterName, string proteinName)
      {
         var parameter = individual.Organism.Parameter(tableParameterName);
         if (parameter == null) return;
         var tableFormula = _ontogenyRepository.PlasmaProteinOntogenyTableFormula(proteinName, individual.OriginData);

         updateOntogenyParameterTable(parameter, tableFormula);
      }

      private void updatePlasmaProteinsOntogenyFor(Population population)
      {
         updatePlasmaProteinsOntogenyFor(population, allAgesIn(population), allGAsIn(population));
      }

      private void updateMoleculeOntogeny(IndividualMolecule molecule, Ontogeny ontogeny)
      {
         molecule.Ontogeny = ontogeny;
         updateOntogenyParameterTable(molecule.OntogenyFactorGITableParameter, ontogeny, CoreConstants.Groups.ONTOGENY_DUODENUM);
         updateOntogenyParameterTable(molecule.OntogenyFactorTableParameter, ontogeny, CoreConstants.Groups.ONTOGENY_LIVER);
      }

      private void updateMoleculeOntogenyInPopulation(IndividualMolecule molecule, Ontogeny ontogeny, Population population)
      {
         updateMoleculeOntogeny(molecule, ontogeny, population, allAgesIn(population), allGAsIn(population));
      }

      private void clearOntogenyFor(string ontogenyFactorPath, string ontogenyFactorGIPath, Population population)
      {
         population.IndividualValuesCache.Remove(ontogenyFactorPath);
         population.IndividualValuesCache.Remove(ontogenyFactorGIPath);
      }

      private void updateOntogenyParameterTable(IParameter parameter, Ontogeny ontogeny, string containerName)
      {
         var tableFormula = _ontogenyRepository.OntogenyToDistributedTableFormula(ontogeny, containerName);
         updateOntogenyParameterTable(parameter, tableFormula);
      }

      private void updateOntogenyParameterTable(IParameter parameter, TableFormula tableFormula)
      {
         //the formula may be null if no ontogeny was selected. In this case, we simply set a default formula of 1
         parameter.Formula = tableFormula ?? _formulaFactory.ValueFor(CoreConstants.DEFAULT_ONTOGENY_FACTOR, parameter.Dimension);
      }

      private void updateOntogenyParameter(IParameter parameter, double value)
      {
         //this can be the case if the parameter is defined in an expression profile
         if (parameter == null)
            return;

         parameter.DefaultValue = value;
         parameter.Value = parameter.DefaultValue.Value;
      }

      public void UpdateAllOntogenies(Population population)
      {
         if (!population.IsAgeDependent) return;

         var allAges = allAgesIn(population);
         var allGAs = allGAsIn(population);

         updatePlasmaProteinsOntogenyFor(population, allAges, allGAs);

         foreach (var molecule in population.AllMolecules())
         {
            updateMoleculeOntogeny(molecule, molecule.Ontogeny, population, allAges, allGAs);
         }
      }

      private void updateMoleculeOntogeny(IndividualMolecule molecule, Ontogeny ontogeny, Population population, IReadOnlyList<double> allAges, IReadOnlyList<double> allGAs)
      {
         var ontogenyFactorPath = _entityPathResolver.PathFor(molecule.OntogenyFactorParameter);
         var ontogenyFactorGIPath = _entityPathResolver.PathFor(molecule.OntogenyFactorGIParameter);
         molecule.Ontogeny = ontogeny;

         clearOntogenyFor(ontogenyFactorPath, ontogenyFactorGIPath, population);

         if (ontogeny.IsUndefined())
            return;

         var ontogenyFactors = new ParameterValues(ontogenyFactorPath);
         var ontogenyFactorsGI = new ParameterValues(ontogenyFactorGIPath);
         for (int i = 0; i < population.NumberOfItems; i++)
         {
            var age = allAges[i];
            var ga = allGAs[i];
            ontogenyFactors.Add(_ontogenyRepository.OntogenyFactorFor(ontogeny, CoreConstants.Groups.ONTOGENY_LIVER, age, ga, population.RandomGenerator));
            ontogenyFactorsGI.Add(_ontogenyRepository.OntogenyFactorFor(ontogeny, CoreConstants.Groups.ONTOGENY_DUODENUM, age, ga, population.RandomGenerator));
         }

         population.IndividualValuesCache.Add(ontogenyFactors);
         population.IndividualValuesCache.Add(ontogenyFactorsGI);
      }

      private void updatePlasmaProteinsOntogenyFor(Population population, IReadOnlyList<double> allAges, IReadOnlyList<double> allGAs)
      {
         foreach (var supportedProtein in _ontogenyRepository.SupportedProteins.KeyValues)
         {
            updatePlasmaProteinOntogenyFor(population, allAges, allGAs, supportedProtein.Key, supportedProtein.Value);
         }
      }

      private void updatePlasmaProteinOntogenyFor(Population population, IReadOnlyList<double> allAges, IReadOnlyList<double> allGAs, string parameterName, string proteinName)
      {
         var plasmaProteinOntogenyParameter = population.Organism.Parameter(parameterName);
         if (plasmaProteinOntogenyParameter == null) return;
         var ontogenyFactors = new ParameterValues(_entityPathResolver.PathFor(plasmaProteinOntogenyParameter));

         for (int i = 0; i < population.NumberOfItems; i++)
         {
            ontogenyFactors.Add(_ontogenyRepository.PlasmaProteinOntogenyTableFormula(proteinName, allAges[i], allGAs[i], population.Species.Name, population.RandomGenerator));
         }

         population.IndividualValuesCache.Remove(ontogenyFactors.ParameterPath);
         population.IndividualValuesCache.Add(ontogenyFactors);
      }

      private IReadOnlyList<double> allGAsIn(Population population)
      {
         return population.AllOrganismValuesFor(Constants.Parameters.GESTATIONAL_AGE, _entityPathResolver);
      }

      private IReadOnlyList<double> allAgesIn(Population population)
      {
         return population.AllOrganismValuesFor(CoreConstants.Parameters.AGE, _entityPathResolver);
      }
   }
}
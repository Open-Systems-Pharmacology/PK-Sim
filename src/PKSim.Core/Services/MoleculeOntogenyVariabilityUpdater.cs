using System;
using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IMoleculeOntogenyVariabilityUpdater
   {
      void UpdatePlasmaProteinsOntogenyFor(Individual individual);
      void UpdatePlasmaProteinsOntogenyFor(Population population);

      void UpdateMoleculeOntogeny(IndividualMolecule molecule, Ontogeny ontogeny, ISimulationSubject simulationSubject);
      void UpdateMoleculeOntogeny(IndividualMolecule molecule, Ontogeny ontogeny, Individual individual);
      void UpdateMoleculeOntogeny(IndividualMolecule molecule, Ontogeny ontogeny, Population population);

      /// <summary>
      ///    Updates the ontogeny factor for all plasma proteins and molecules defined in the population
      /// </summary>
      void UpdateAllOntogenies(Population population);
   }

   public class MoleculeOntogenyVariabilityUpdater : IMoleculeOntogenyVariabilityUpdater
   {
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly IEntityPathResolver _entityPathResolver;

      public MoleculeOntogenyVariabilityUpdater(IOntogenyRepository ontogenyRepository, IEntityPathResolver entityPathResolver)
      {
         _ontogenyRepository = ontogenyRepository;
         _entityPathResolver = entityPathResolver;
      }

      public void UpdatePlasmaProteinsOntogenyFor(Individual individual)
      {
         foreach (var supportedProtein in _ontogenyRepository.SupportedProteins.KeyValues)
         {
            updatePlasmaProteinOntogenyFor(individual, supportedProtein.Key, supportedProtein.Value);
         }
      }

      private void updatePlasmaProteinOntogenyFor(Individual individual, string parameterName, string proteinNAme)
      {
         var parameter = individual.Organism.Parameter(parameterName);
         if (parameter == null) return;
         updateOntogenyParameter(parameter, _ontogenyRepository.PlasmaProteinOntogenyFactor(proteinNAme, individual.OriginData));
      }

      public void UpdatePlasmaProteinsOntogenyFor(Population population)
      {
         updatePlasmaProteinsOntogenyFor(population, allAgesIn(population), allGAsIn(population));
      }

      public void UpdateMoleculeOntogeny(IndividualMolecule molecule, Ontogeny ontogeny, ISimulationSubject simulationSubject)
      {
         switch (simulationSubject)
         {
            case Individual individual:
               UpdateMoleculeOntogeny(molecule, ontogeny, individual);
               break;
            case Population population:
               UpdateMoleculeOntogeny(molecule, ontogeny, population);
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(simulationSubject));
         }
      }

      public void UpdateMoleculeOntogeny(IndividualMolecule molecule, Ontogeny ontogeny, Individual individual)
      {
         molecule.Ontogeny = ontogeny;
         updateOntogenyParameterTable(molecule.OntogenyFactorGITableParameter, ontogeny, CoreConstants.Groups.ONTOGENY_DUODENUM);
         updateOntogenyParameterTable(molecule.OntogenyFactorTableParameter, ontogeny, CoreConstants.Groups.ONTOGENY_LIVER);
      }

      public void UpdateMoleculeOntogeny(IndividualMolecule molecule, Ontogeny ontogeny, Population population)
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
         parameter.Formula = _ontogenyRepository.OntogenyToDistributedTableFormula(ontogeny, containerName);
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
            ontogenyFactors.Add(_ontogenyRepository.PlasmaProteinOntogenyFactor(proteinName, allAges[i], allGAs[i], population.Species.Name, population.RandomGenerator));
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
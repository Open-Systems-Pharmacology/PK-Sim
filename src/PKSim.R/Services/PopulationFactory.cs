using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using PKSim.R.Domain;

namespace PKSim.R.Services
{
   public interface IPopulationFactory
   {
      IndividualValuesCache CreatePopulation(PopulationCharacteristics populationCharacteristics);
   }

   public class PopulationFactory : IPopulationFactory
   {
      private readonly RandomPopulationSettingsMapper _populationSettingsMapper;
      private readonly IRandomPopulationFactory _randomPopulationFactory;
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly IIndividualEnzymeFactory _individualEnzymeFactory;
      private readonly IMoleculeOntogenyVariabilityUpdater _ontogenyVariabilityUpdater;

      public PopulationFactory(
         RandomPopulationSettingsMapper populationSettingsMapper,
         IRandomPopulationFactory randomPopulationFactory,
         IOntogenyRepository ontogenyRepository,
         IIndividualEnzymeFactory individualEnzymeFactory,
         IMoleculeOntogenyVariabilityUpdater ontogenyVariabilityUpdater)
      {
         _populationSettingsMapper = populationSettingsMapper;
         _randomPopulationFactory = randomPopulationFactory;
         _ontogenyRepository = ontogenyRepository;
         _individualEnzymeFactory = individualEnzymeFactory;
         _ontogenyVariabilityUpdater = ontogenyVariabilityUpdater;
      }

      public IndividualValuesCache CreatePopulation(PopulationCharacteristics populationCharacteristics)
      {
         var populationSettings = _populationSettingsMapper.MapToModel(populationCharacteristics).Result;
         validate(populationSettings);
         var population = _randomPopulationFactory.CreateFor(populationSettings, new CancellationToken()).Result;

         foreach (var moleculeOntogeny in populationCharacteristics.MoleculeOntogenies)
         {
            var allOntogeniesForSpecies = _ontogenyRepository.AllFor(populationCharacteristics.Individual.OriginData.Species);
            if (!allOntogeniesForSpecies.Any())
               continue;

            var ontogeny = allOntogeniesForSpecies.FindByName(moleculeOntogeny.Ontogeny);
            if (ontogeny == null)
               continue;

            var molecule = _individualEnzymeFactory.CreateEmpty().WithName(moleculeOntogeny.Molecule);
            molecule.Ontogeny = ontogeny;

            population.AddMolecule(molecule);
         }

         _ontogenyVariabilityUpdater.UpdateAllOntogenies(population);

         return population.IndividualValuesCache;
      }

      private void validate(RandomPopulationSettings populationSettings)
      {
         var validations = new List<IBusinessRuleSet>();
         foreach (var parameterRange in populationSettings.ParameterRanges)
         {
            validations.Add(parameterRange.Validate());
         }

         var errors = validations.Where(x => !x.IsEmpty).ToList();
         if (!errors.Any())
            return;

         throw new OSPSuiteException(errors.Select(x => x.Message).ToString("\n"));
      }
   }
}
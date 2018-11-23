﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevExpress.Utils.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Matlab.Mappers;

namespace PKSim.Matlab
{
   public interface IMatlabPopulationFactory
   {
      IParameterValueCache CreatePopulation(PopulationSettings matlabPopulationSettings, IEnumerable<MoleculeOntogeny> moleculeOntogenies);
   }

   public class MatlabPopulationFactory : IMatlabPopulationFactory
   {
      private readonly IMatlabPopulationSettingsToPopulationSettingsMapper _populationSettingsMapper;
      private readonly IRandomPopulationFactory _randomPopulationFactory;
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly IMoleculeOntogenyVariabilityUpdater _ontogenyVariabilityUpdater;
      private readonly IIndividualEnzymeFactory _individualEnzymeFactory;

      static MatlabPopulationFactory()
      {
         ApplicationStartup.Initialize();
      }

      public MatlabPopulationFactory() : this(IoC.Resolve<IMatlabPopulationSettingsToPopulationSettingsMapper>(), IoC.Resolve<IRandomPopulationFactory>(),
         IoC.Resolve<IOntogenyRepository>(), IoC.Resolve<IMoleculeOntogenyVariabilityUpdater>(), IoC.Resolve<IIndividualEnzymeFactory>())
      {
      }

      public MatlabPopulationFactory(IMatlabPopulationSettingsToPopulationSettingsMapper populationSettingsMapper, IRandomPopulationFactory randomPopulationFactory,
         IOntogenyRepository ontogenyRepository, IMoleculeOntogenyVariabilityUpdater ontogenyVariabilityUpdater, IIndividualEnzymeFactory individualEnzymeFactory)
      {
         _populationSettingsMapper = populationSettingsMapper;
         _randomPopulationFactory = randomPopulationFactory;
         _ontogenyRepository = ontogenyRepository;
         _ontogenyVariabilityUpdater = ontogenyVariabilityUpdater;
         _individualEnzymeFactory = individualEnzymeFactory;
      }

      public IParameterValueCache CreatePopulation(PopulationSettings matlabPopulationSettings, IEnumerable<MoleculeOntogeny> moleculeOntogenies)
      {
         var populationSettings = _populationSettingsMapper.MapFrom(matlabPopulationSettings);
         var population = _randomPopulationFactory.CreateFor(populationSettings, new CancellationToken()).Result;

         foreach (var moleculeOntogeny in moleculeOntogenies)
         {
            var allOntogeniesForSpecies = _ontogenyRepository.AllFor(matlabPopulationSettings.Individual.OriginData.Species).ToList();
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

         return population.IndividualPropertiesCache;
      }
   }
}
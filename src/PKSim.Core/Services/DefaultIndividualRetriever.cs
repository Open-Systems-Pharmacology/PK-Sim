using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using OSPSuite.Core.Snapshots;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;
using OriginData = OSPSuite.Core.Snapshots.OriginData;

namespace PKSim.Core.Services
{
   public class DefaultIndividualRetriever : IDefaultIndividualRetriever
   {
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IIndividualFactory _individualFactory;
      private readonly OriginDataMapper _originDataMapper;
      private readonly ConcurrentDictionary<SpeciesPopulation, Lazy<Individual>> _individualCacheProSpecies = new ConcurrentDictionary<SpeciesPopulation, Lazy<Individual>>();

      public DefaultIndividualRetriever(
         ISpeciesRepository speciesRepository,
         IIndividualFactory individualFactory,
         OriginDataMapper originDataMapper
      )

      {
         _speciesRepository = speciesRepository;
         _individualFactory = individualFactory;
         _originDataMapper = originDataMapper;
      }

      public Individual DefaultIndividual()
      {
         return DefaultIndividualFor(_speciesRepository.DefaultSpecies);
      }

      public Individual DefaultHuman()
      {
         return DefaultIndividualFor(_speciesRepository.FindByName(CoreConstants.Species.HUMAN));
      }

      public Individual DefaultIndividualFor(Species species)
      {
         return DefaultIndividualFor(species.DefaultPopulation);
      }

      public Gender DefaultGenderFor(SpeciesPopulation speciesPopulation)
      {
         return speciesPopulation.Genders[0];
      }

      public Individual DefaultIndividualFor(SpeciesPopulation speciesPopulation)
      {
         var cachedIndividual = _individualCacheProSpecies.GetOrAdd(speciesPopulation,
            population => new Lazy<Individual>(() => createDefaultIndividualFor(population), LazyThreadSafetyMode.ExecutionAndPublication));

         try
         {
            return cachedIndividual.Value;
         }
         catch
         {
            //a lazy that faulted keeps throwing the same exception, so the entry is dropped to let the next call retry
            _individualCacheProSpecies.TryRemove(new KeyValuePair<SpeciesPopulation, Lazy<Individual>>(speciesPopulation, cachedIndividual));
            throw;
         }
      }

      private Individual createDefaultIndividualFor(SpeciesPopulation speciesPopulation)
      {
         var originDataSnapshot = new OriginData
         {
            Species = speciesPopulation.Species,
            Population = speciesPopulation.Name,
            Gender = DefaultGenderFor(speciesPopulation).Name
         };

         //We do not need to pass any valid snapshot context in this case.
         var originData = _originDataMapper.MapToModel(originDataSnapshot, new SnapshotContext(new PKSimProject(), SnapshotVersions.Current)).Result;
         return _individualFactory.CreateStandardFor(originData);
      }
   }
}

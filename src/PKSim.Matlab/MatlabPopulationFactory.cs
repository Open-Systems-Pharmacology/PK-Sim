using System.Collections.Generic;
using System.Threading;
using OSPSuite.Utility.Container;
using PKSim.Core.Batch.Mapper;
using PKSim.Core.Model;
using PopulationSettings = PKSim.Core.Batch.PopulationSettings;

namespace PKSim.Matlab
{
   public interface IMatlabPopulationFactory
   {
      IParameterValueCache CreatePopulation(PopulationSettings batchPopulationSettings, IEnumerable<string> moleculeNames);
   }

   public class MatlabPopulationFactory : IMatlabPopulationFactory
   {
      private readonly IPopulationSettingsMapper _populationSettingsMapper;
      private readonly IRandomPopulationFactory _randomPopulationFactory;

      static MatlabPopulationFactory()
      {
         ApplicationStartup.Initialize();
      }

      public MatlabPopulationFactory() : this(IoC.Resolve<IPopulationSettingsMapper>(), IoC.Resolve<IRandomPopulationFactory>())
      {
      }

      public MatlabPopulationFactory(IPopulationSettingsMapper populationSettingsMapper, IRandomPopulationFactory randomPopulationFactory)
      {
         _populationSettingsMapper = populationSettingsMapper;
         _randomPopulationFactory = randomPopulationFactory;
      }

      public IParameterValueCache CreatePopulation(PopulationSettings batchPopulationSettings, IEnumerable<string> moleculeNames)
      {
         var populationSettings = _populationSettingsMapper.MapFrom(batchPopulationSettings);
         var population = _randomPopulationFactory.CreateFor(populationSettings, new CancellationToken()).Result;
         return population.IndividualPropertiesCache;
      }
   }
}
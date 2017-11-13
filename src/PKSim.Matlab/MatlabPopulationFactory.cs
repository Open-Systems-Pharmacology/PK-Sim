using System.Collections.Generic;
using System.Threading;
using OSPSuite.Utility.Container;
using PKSim.Core.Batch.Mapper;
using PKSim.Core.Model;
using PKSim.Matlab.Mappers;

namespace PKSim.Matlab
{
   public interface IMatlabPopulationFactory
   {
      IParameterValueCache CreatePopulation(PopulationSettings matlabPopulationSettings, IEnumerable<string> moleculeNames);
   }

   public class MatlabPopulationFactory : IMatlabPopulationFactory
   {
      private readonly IMatlabPopulationSettingsToPopulationSettingsMapper _populationSettingsMapper;
      private readonly IRandomPopulationFactory _randomPopulationFactory;

      static MatlabPopulationFactory()
      {
         ApplicationStartup.Initialize();
      }

      public MatlabPopulationFactory() : this(IoC.Resolve<IMatlabPopulationSettingsToPopulationSettingsMapper>(), IoC.Resolve<IRandomPopulationFactory>())
      {
      }

      public MatlabPopulationFactory(IMatlabPopulationSettingsToPopulationSettingsMapper populationSettingsMapper, IRandomPopulationFactory randomPopulationFactory)
      {
         _populationSettingsMapper = populationSettingsMapper;
         _randomPopulationFactory = randomPopulationFactory;
      }

      public IParameterValueCache CreatePopulation(PopulationSettings matlabPopulationSettings, IEnumerable<string> moleculeNames)
      {
         var populationSettings = _populationSettingsMapper.MapFrom(matlabPopulationSettings);
         var population = _randomPopulationFactory.CreateFor(populationSettings, new CancellationToken()).Result;
         return population.IndividualPropertiesCache;
      }
   }
}
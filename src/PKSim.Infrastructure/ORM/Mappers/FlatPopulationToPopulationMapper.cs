using System.Linq;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatPopulationToPopulationMapper : IMapper<FlatPopulation, SpeciesPopulation>
   {
   }

   public class FlatPopulationToPopulationMapper : IFlatPopulationToPopulationMapper
   {
      private readonly IFlatPopulationGenderRepository _populationGenderRepository;
      private readonly IGenderRepository _genderRepository;

      public FlatPopulationToPopulationMapper(IFlatPopulationGenderRepository populationGenderRepository,
         IGenderRepository genderRepository)
      {
         _populationGenderRepository = populationGenderRepository;
         _genderRepository = genderRepository;
      }

      public SpeciesPopulation MapFrom(FlatPopulation flatPopulation)
      {
         var population = new SpeciesPopulation
         {
            Species = flatPopulation.Species,
            IsAgeDependent = flatPopulation.IsAgeDependent,
            IsHeightDependent = flatPopulation.IsHeightDependent,
            Name = flatPopulation.Id,
            RaceIndex = flatPopulation.RaceIndex,
            Sequence = flatPopulation.Sequence,
         };

         foreach (var popGender in _populationGenderRepository.All().Where(item => item.Population == population.Name).OrderBy(x => x.Sequence))
         {
            population.AddGender(_genderRepository.FindByName(popGender.GenderName));
         }

         return population;
      }
   }
}
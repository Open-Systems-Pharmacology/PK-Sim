using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatSpeciesToSpeciesMapper : IMapper<FlatSpecies, Species>
   {
   }

   public class FlatSpeciesToSpeciesMapper : IFlatSpeciesToSpeciesMapper
   {
      private readonly IPopulationRepository _populationRepository;
      private readonly IFlatParameterValueVersionRepository _flatParameterValueVersionRepository;
      private readonly IFlatPVVListToPVVCategoryListMapper _pvvCategoriesMapper;

      public FlatSpeciesToSpeciesMapper(IPopulationRepository populationRepository,
         IFlatParameterValueVersionRepository flatParameterValueVersionRepository,
         IFlatPVVListToPVVCategoryListMapper pvvCategoriesMapper)
      {
         _populationRepository = populationRepository;
         _flatParameterValueVersionRepository = flatParameterValueVersionRepository;
         _pvvCategoriesMapper = pvvCategoriesMapper;
      }

      public Species MapFrom(FlatSpecies flatSpecies)
      {
         var species = new Species
         {
            Id = flatSpecies.Id,
            Name = flatSpecies.Id,
            Icon = flatSpecies.IconName,
            IsHuman = flatSpecies.IsHuman
         };

         allPopulationsFor(flatSpecies).Each(species.AddPopulation);

         allParameterValueVersionCategoriesFor(flatSpecies).Each(species.AddPVVCategory);

         return species;
      }

      private IEnumerable<ParameterValueVersionCategory> allParameterValueVersionCategoriesFor(FlatSpecies flatSpecies) => _pvvCategoriesMapper.MapFrom(_flatParameterValueVersionRepository.All(), flatSpecies.Id);

      private IEnumerable<SpeciesPopulation> allPopulationsFor(FlatSpecies flatSpecies) => _populationRepository.All().Where(x => x.Species == flatSpecies.Id).OrderBy(x => x.Sequence);
   }
}